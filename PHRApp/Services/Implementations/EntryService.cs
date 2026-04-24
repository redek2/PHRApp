using Microsoft.EntityFrameworkCore;
using PHRApp.Data;
using PHRApp.Models.DTOs;
using PHRApp.Models.Entities;
using PHRApp.Models.JoinEntities;
using PHRApp.Services.Interfaces;
using PHRApp.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}