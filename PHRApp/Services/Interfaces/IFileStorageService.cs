using PHRApp.Models.DTOs;

namespace PHRApp.Services.Interfaces
{
    public interface IFileStorageService
    {
        Task<List<StoredFileResult>> SaveFilesAsync(List<string> filePaths);
    }
}