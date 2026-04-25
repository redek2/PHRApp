using PHRApp.Models.DTOs;

namespace PHRApp.Services.Interfaces
{
    public interface IEntryService
    {
        Task<int> CreateEntryAsync(CreateEntryDto dto);
        Task<List<EntryListItemDto>> GetEntriesAsync(EntryQueryDto query);
    }
}
