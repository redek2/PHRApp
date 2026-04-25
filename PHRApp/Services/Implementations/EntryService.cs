using Microsoft.EntityFrameworkCore;
using PHRApp.Data;
using PHRApp.Models.DTOs;
using PHRApp.Models.Entities;
using PHRApp.Models.Enums;
using PHRApp.Models.JoinEntities;
using PHRApp.Services.Interfaces;

namespace PHRApp.Services.Implementations
{
    public class EntryService : IEntryService
    {
        private readonly AppDbContext _context;
        private readonly IFileStorageService _fileStorageService;

        public EntryService(AppDbContext context, IFileStorageService fileStorageService)
        {
            _context = context;
            _fileStorageService = fileStorageService;
        }

        public async Task<int> CreateEntryAsync(CreateEntryDto dto)
        {
            // Basic validation of DTO
            if (string.IsNullOrWhiteSpace(dto.Title))
            {
                throw new ArgumentException("Title is required.");
            }

            // Validate event date based on status
            if (dto.Status == EntryStatus.Planned && dto.EventDate <= DateTime.UtcNow)
            {
                throw new ArgumentException("Event date must be in the future for planned entries.");
            }

            if (dto.Status == EntryStatus.Completed && dto.EventDate > DateTime.UtcNow)
            {
                throw new ArgumentException("Event date cannot be in the future for completed entries.");
            }

            List<Category> categories = new();

            if (dto.CategoryIds.Any())
            {
                categories = await _context.Categories
                    .Where(c => dto.CategoryIds.Contains(c.Id))
                    .ToListAsync();

                if (categories.Count != dto.CategoryIds.Count)
                    throw new ArgumentException("One or more categories do not exist.");
            }

            // Map DTO to Entry entity
            var entry = new Entry
            {
                Title = dto.Title,
                Description = dto.Description,
                EventDate = dto.EventDate,
                Status = dto.Status,
                IsArchived = false,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                EntryCategories = new List<EntryCategory>()
            };

            foreach (var category in categories)
            {
                entry.EntryCategories.Add(new EntryCategory
                {
                    CategoryId = category.Id,
                });
            }

            // Handle file attachments
            List<StoredFileResult> storedFiles = new();

            if (dto.FilePaths.Any())
            {
                storedFiles = await _fileStorageService.SaveFilesAsync(dto.FilePaths);
            }

            foreach (var file in storedFiles)
            {
                var attachment = new Attachment
                {
                    FileName = file.OriginalFileName,
                    StoredFileName = file.StoredFileName,
                    FilePath = file.RelativePath,
                    ContentType = file.ContentType,
                    FileSize = file.FileSize,
                    CreatedAt = DateTime.UtcNow
                };
                entry.EntryAttachments.Add(new EntryAttachment
                {
                    Attachment = attachment
                });
            }


            _context.Entries.Add(entry);
            await _context.SaveChangesAsync();
            return entry.Id; // Return the ID of the newly created entry
        }

        public async Task<List<EntryListItemDto>> GetEntriesAsync(EntryQueryDto query)
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            if (query.FromDate.HasValue && query.ToDate.HasValue && query.FromDate > query.ToDate)
            {
                throw new ArgumentException("FromDate cannot be later than ToDate.");
            }

            var dbQuery = _context.Entries
                .AsNoTracking()
                .Where(e => !e.IsArchived);

            // Filter by Category
            if (query.CategoryId.HasValue)
            {
                dbQuery = dbQuery.Where(e =>
                    e.EntryCategories.Any(ec => ec.CategoryId == query.CategoryId.Value));
            }

            // Filter by Status
            if (query.Status.HasValue)
            {
                dbQuery = dbQuery.Where(e => e.Status == query.Status.Value);
            }

            // Filter by Date range
            if (query.FromDate.HasValue)
            {
                dbQuery = dbQuery.Where(e => e.EventDate >= query.FromDate.Value);
            }

            if (query.ToDate.HasValue)
            {
                dbQuery = dbQuery.Where(e => e.EventDate <= query.ToDate.Value);
            }

            // Search (Title + Description)
            if (!string.IsNullOrWhiteSpace(query.SearchTerm))
            {
                var term = query.SearchTerm.Trim();

                dbQuery = dbQuery.Where(e =>
                    EF.Functions.Like(e.Title, $"%{term}%") ||
                    (e.Description != null && EF.Functions.Like(e.Description, $"%{term}%")));
            }

            return await dbQuery
                .OrderByDescending(e => e.EventDate)
                .Select(e => new EntryListItemDto
                {
                    Id = e.Id,
                    Title = e.Title,
                    EventDate = e.EventDate,
                    Status = e.Status,
                    CategoryNames = e.EntryCategories
                        .Select(ec => ec.Category.Name)
                        .ToList()
                })
                .ToListAsync();
        }
    }
}