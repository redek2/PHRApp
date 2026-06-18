using PHRApp.Models.DTOs;

namespace PHRApp.Services.Interfaces
{
    public interface IFileStorageService
    {
        Task<List<StoredFileResult>> SaveFilesAsync(List<string> filePaths);
        Task DeleteFilesAsync(List<string> relativePaths);
        string ResolveFullPath(string relativePath);
    }
}