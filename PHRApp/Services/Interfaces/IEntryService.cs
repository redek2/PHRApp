using PHRApp.Models.DTOs;

namespace PHRApp.Services.Interfaces
{
    public interface IEntryService
    {
        Task<int> CreateEntryAsync(CreateEntryDto dto);
        Task<List<EntryListItemDto>> GetEntriesAsync(EntryQueryDto query);
        Task<EntryDetailsDto?> GetEntryByIdAsync(int id);
        Task UpdateEntryAsync(UpdateEntryDto dto);
        Task ArchiveEntryAsync(int id);
        Task UpdateExpiredEntriesAsync();
    }
}
