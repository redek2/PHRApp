using Microsoft.EntityFrameworkCore;
using Moq;
using PHRApp.Models.DTOs;
using PHRApp.Models.Entities;
using PHRApp.Models.Enums;
using PHRApp.Services.Implementations;
using PHRApp.Services.Interfaces;
using PHRApp.Tests.TestHelpers;

namespace PHRApp.Tests.Services
{
    public class EntryServiceTests : IDisposable
    {
        private readonly TestDbContextFactory _db;
        private readonly Mock<IFileStorageService> _fileStorageMock;
        private readonly EntryService _sut;

        public EntryServiceTests()
        {
            _db = new TestDbContextFactory();
            _fileStorageMock = new Mock<IFileStorageService>();

            _fileStorageMock
                .Setup(s => s.SaveFilesAsync(It.IsAny<List<string>>()))
                .ReturnsAsync((List<string> paths) => paths.Select(p => new StoredFileResult
                {
                    OriginalFileName = Path.GetFileName(p),
                    StoredFileName = $"{Guid.NewGuid()}{Path.GetExtension(p)}",
                    RelativePath = $"attachments/{Guid.NewGuid()}{Path.GetExtension(p)}",
                    FileSize = 1024,
                    ContentType = "application/octet-stream"
                }).ToList());

            _sut = new EntryService(_db.Context, _fileStorageMock.Object);
        }

        private async Task<int> SeedCategoryAsync(string name)
        {
            var category = new Category { Name = name };
            _db.Context.Categories.Add(category);
            await _db.Context.SaveChangesAsync();
            return category.Id;
        }

        private static CreateEntryDto ValidCreateDto() => new()
        {
            Title = "Wizyta kontrolna",
            Description = "Opis wizyty",
            EventDate = DateTime.Now.AddDays(5),
            Status = EntryStatus.Planned,
            CategoryIds = new List<int>(),
            FilePaths = new List<string>(),
        };

        [Fact]
        public async Task CreateEntryAsync_WithEmptyTitle_ThrowsArgumentException()
        {
            var dto = ValidCreateDto();
            dto.Title = "";

            await Assert.ThrowsAsync<ArgumentException>(() => _sut.CreateEntryAsync(dto));
        }

        [Fact]
        public async Task CreateEntryAsync_PlannedWithPastDate_ThrowsArgumentException()
        {
            var dto = ValidCreateDto();
            dto.Status = EntryStatus.Planned;
            dto.EventDate = DateTime.Now.AddDays(-1);

            await Assert.ThrowsAsync<ArgumentException>(() => _sut.CreateEntryAsync(dto));
        }

        [Fact]
        public async Task CreateEntryAsync_CompletedWithFutureDate_ThrowsArgumentException()
        {
            var dto = ValidCreateDto();
            dto.Status = EntryStatus.Completed;
            dto.EventDate = DateTime.Now.AddDays(5);

            await Assert.ThrowsAsync<ArgumentException>(() => _sut.CreateEntryAsync(dto));
        }

        [Fact]
        public async Task CreateEntryAsync_WithNonExistentCategory_ThrowsArgumentException()
        {
            var dto = ValidCreateDto();
            dto.CategoryIds = new List<int> { 9999 };

            await Assert.ThrowsAsync<ArgumentException>(() => _sut.CreateEntryAsync(dto));
        }

        [Fact]
        public async Task CreateEntryAsync_WithValidData_ReturnsPositiveIdAndPersistsEntry()
        {
            var categoryId = await SeedCategoryAsync("Badania");
            var dto = ValidCreateDto();
            dto.CategoryIds = new List<int> { categoryId };

            var id = await _sut.CreateEntryAsync(dto);

            Assert.True(id > 0);

            var saved = await _db.Context.Entries
                .Include(e => e.EntryCategories)
                .FirstAsync(e => e.Id == id);

            Assert.Equal("Wizyta kontrolna", saved.Title);
            Assert.Single(saved.EntryCategories);
        }

        [Fact]
        public async Task CreateEntryAsync_WithFiles_CallsFileStorageServiceAndCreatesAttachments()
        {
            var dto = ValidCreateDto();
            var filePath = @"C:\temp\badanie.pdf";
            dto.FilePaths = new List<string> { filePath };

            var id = await _sut.CreateEntryAsync(dto);

            _fileStorageMock.Verify(s => s.SaveFilesAsync(
                It.Is<List<string>>(paths => paths.Count == 1 && paths[0] == filePath)),
                Times.Once);

            var saved = await _db.Context.Entries
                .Include(e => e.EntryAttachments)
                .FirstAsync(e => e.Id == id);

            Assert.Single(saved.EntryAttachments);
        }

        [Fact]
        public async Task GetEntriesAsync_ExcludesArchivedEntries()
        {
            var activeId = await _sut.CreateEntryAsync(ValidCreateDto());
            var archivedId = await _sut.CreateEntryAsync(ValidCreateDto());
            await _sut.ArchiveEntryAsync(archivedId);

            var result = await _sut.GetEntriesAsync(new EntryQueryDto());

            Assert.Contains(result, e => e.Id == activeId);
            Assert.DoesNotContain(result, e => e.Id == archivedId);
        }

        [Fact]
        public async Task GetEntriesAsync_FiltersByStatus()
        {
            var plannedDto = ValidCreateDto();
            plannedDto.Status = EntryStatus.Planned;
            var plannedId = await _sut.CreateEntryAsync(plannedDto);

            var cancelledDto = ValidCreateDto();
            cancelledDto.Status = EntryStatus.Cancelled;
            var cancelledId = await _sut.CreateEntryAsync(cancelledDto);

            var result = await _sut.GetEntriesAsync(new EntryQueryDto { Status = EntryStatus.Cancelled });

            Assert.Single(result);
            Assert.Equal(cancelledId, result[0].Id);
        }

        [Fact]
        public async Task GetEntriesAsync_FiltersByCategory()
        {
            var categoryId = await SeedCategoryAsync("Szczepienia");

            var withCategoryDto = ValidCreateDto();
            withCategoryDto.CategoryIds = new List<int> { categoryId };
            var withCategoryId = await _sut.CreateEntryAsync(withCategoryDto);

            await _sut.CreateEntryAsync(ValidCreateDto());

            var result = await _sut.GetEntriesAsync(new EntryQueryDto { CategoryId = categoryId });

            Assert.Single(result);
            Assert.Equal(withCategoryId, result[0].Id);
        }

        [Fact]
        public async Task GetEntriesAsync_FiltersBySearchTerm()
        {
            var matchingDto = ValidCreateDto();
            matchingDto.Title = "Badanie krwi";
            var matchingId = await _sut.CreateEntryAsync(matchingDto);

            var otherDto = ValidCreateDto();
            otherDto.Title = "Wizyta stomatologiczna";
            await _sut.CreateEntryAsync(otherDto);

            var result = await _sut.GetEntriesAsync(new EntryQueryDto { SearchTerm = "krwi" });

            Assert.Single(result);
            Assert.Equal(matchingId, result[0].Id);
        }

        [Fact]
        public async Task GetEntriesAsync_FromDateLaterThanToDate_ThrowsArgumentException()
        {
            var query = new EntryQueryDto
            {
                FromDate = DateTime.Now,
                ToDate = DateTime.Now.AddDays(-1)
            };

            await Assert.ThrowsAsync<ArgumentException>(() => _sut.GetEntriesAsync(query));
        }

        [Fact]
        public async Task GetEntryByIdAsync_NonExistentId_ReturnsNull()
        {
            var result = await _sut.GetEntryByIdAsync(9999);

            Assert.Null(result);
        }

        [Fact]
        public async Task GetEntryByIdAsync_ArchivedEntry_ReturnsNull()
        {
            var id = await _sut.CreateEntryAsync(ValidCreateDto());
            await _sut.ArchiveEntryAsync(id);

            var result = await _sut.GetEntryByIdAsync(id);

            Assert.Null(result);
        }

        [Fact]
        public async Task GetEntryByIdAsync_ExistingEntry_ReturnsFullDetails()
        {
            var categoryId = await SeedCategoryAsync("Konsultacje");
            var dto = ValidCreateDto();
            dto.CategoryIds = new List<int> { categoryId };
            dto.FilePaths = new List<string> { @"C:\temp\wynik.pdf" };

            var id = await _sut.CreateEntryAsync(dto);

            var result = await _sut.GetEntryByIdAsync(id);

            Assert.NotNull(result);
            Assert.Equal("Wizyta kontrolna", result!.Title);
            Assert.Single(result.CategoryNames);
            Assert.Equal("Konsultacje", result.CategoryNames[0]);
            Assert.Single(result.Attachments);
        }

        [Fact]
        public async Task UpdateEntryAsync_NonExistentEntry_ThrowsException()
        {
            var dto = new UpdateEntryDto
            {
                Id = 9999,
                Title = "Coś",
                EventDate = DateTime.Now.AddDays(1),
                Status = EntryStatus.Planned
            };

            await Assert.ThrowsAsync<Exception>(() => _sut.UpdateEntryAsync(dto));
        }

        [Fact]
        public async Task UpdateEntryAsync_RemovesAttachment_CallsDeleteFilesAsync()
        {
            var createDto = ValidCreateDto();
            createDto.FilePaths = new List<string> { @"C:\temp\stary.pdf" };
            var entryId = await _sut.CreateEntryAsync(createDto);

            var details = await _sut.GetEntryByIdAsync(entryId);
            var attachmentId = details!.Attachments[0].Id;
            var attachmentPath = details.Attachments[0].FilePath;

            var updateDto = new UpdateEntryDto
            {
                Id = entryId,
                Title = details.Title,
                EventDate = details.EventDate,
                Status = details.Status,
                CategoryIds = new List<int>(),
                AttachmentIdsToRemove = new List<int> { attachmentId },
                NewFilePaths = new List<string>()
            };

            await _sut.UpdateEntryAsync(updateDto);

            _fileStorageMock.Verify(s => s.DeleteFilesAsync(
                It.Is<List<string>>(paths => paths.Contains(attachmentPath))),
                Times.Once);

            var afterUpdate = await _sut.GetEntryByIdAsync(entryId);
            Assert.Empty(afterUpdate!.Attachments);
        }

        [Fact]
        public async Task UpdateEntryAsync_AddsNewAttachment_IncreasesAttachmentCount()
        {
            var entryId = await _sut.CreateEntryAsync(ValidCreateDto());
            var details = await _sut.GetEntryByIdAsync(entryId);

            var updateDto = new UpdateEntryDto
            {
                Id = entryId,
                Title = details!.Title,
                EventDate = details.EventDate,
                Status = details.Status,
                CategoryIds = new List<int>(),
                AttachmentIdsToRemove = new List<int>(),
                NewFilePaths = new List<string> { @"C:\temp\nowy.jpg" }
            };

            await _sut.UpdateEntryAsync(updateDto);

            var afterUpdate = await _sut.GetEntryByIdAsync(entryId);
            Assert.Single(afterUpdate!.Attachments);
        }

        [Fact]
        public async Task ArchiveEntryAsync_ExistingEntry_SetsIsArchivedTrue()
        {
            var id = await _sut.CreateEntryAsync(ValidCreateDto());

            await _sut.ArchiveEntryAsync(id);

            var entry = await _db.Context.Entries.IgnoreQueryFilters().FirstAsync(e => e.Id == id);
            Assert.True(entry.IsArchived);
        }

        [Fact]
        public async Task ArchiveEntryAsync_NonExistentEntry_ThrowsException()
        {
            await Assert.ThrowsAsync<Exception>(() => _sut.ArchiveEntryAsync(9999));
        }

        [Fact]
        public async Task UpdateExpiredEntriesAsync_PlannedEntryInPast_BecomesCompleted()
        {
            var id = await _sut.CreateEntryAsync(ValidCreateDto());
            var entry = await _db.Context.Entries.FirstAsync(e => e.Id == id);
            entry.EventDate = DateTime.Now.AddDays(-1);
            await _db.Context.SaveChangesAsync();

            await _sut.UpdateExpiredEntriesAsync();

            var updated = await _db.Context.Entries.FirstAsync(e => e.Id == id);
            Assert.Equal(EntryStatus.Completed, updated.Status);
        }

        [Fact]
        public async Task UpdateExpiredEntriesAsync_FuturePlannedEntry_RemainsPlanned()
        {
            var id = await _sut.CreateEntryAsync(ValidCreateDto());

            await _sut.UpdateExpiredEntriesAsync();

            var entry = await _db.Context.Entries.FirstAsync(e => e.Id == id);
            Assert.Equal(EntryStatus.Planned, entry.Status);
        }

        public void Dispose() => _db.Dispose();
    }
}
