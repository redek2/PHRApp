using PHRApp.Models.DTOs;
using PHRApp.Services.Interfaces;
using System.IO;

namespace PHRApp.Services.Implementations
{
    public class FileStorageService : IFileStorageService
    {
        private readonly string _basePath;
        private const string AttachmentsFolder = "attachments";
        private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
        {
            ".pdf",
            ".jpg",
            ".jpeg",
            ".png"
        };

        public FileStorageService()
        {
            var appRootPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "PHRApp");

            _basePath = Path.Combine(appRootPath, AttachmentsFolder);

            Directory.CreateDirectory(_basePath);
        }

        public async Task<List<StoredFileResult>> SaveFilesAsync(List<string> filePaths)
        {
            var results = new List<StoredFileResult>();
            foreach (var path in filePaths)
            {
                if (!File.Exists(path))
                {
                    throw new FileNotFoundException($"File not found: {path}");
                }

                var fileInfo = new FileInfo(path);
                var extension = Path.GetExtension(fileInfo.Name);
                if (!AllowedExtensions.Contains(extension))
                {
                    throw new InvalidOperationException($"File type not allowed: {extension}");
                }

                var storedFileName = $"{Guid.NewGuid()}{extension}";
                var destinationPath = Path.Combine(_basePath, storedFileName);

                using (var sourceStream = new FileStream(path, FileMode.Open, FileAccess.Read))
                using (var destinationStream = new FileStream(destinationPath, FileMode.CreateNew, FileAccess.Write))
                {
                    await sourceStream.CopyToAsync(destinationStream);
                }

                var result = new StoredFileResult
                {
                    OriginalFileName = fileInfo.Name,
                    StoredFileName = storedFileName,
                    RelativePath = Path.Combine(AttachmentsFolder, storedFileName),
                    FileSize = fileInfo.Length,
                    ContentType = GetContentType(extension)
                };

                results.Add(result);
            }
            return results;
        }

        public Task DeleteFilesAsync(List<string> relativePaths)
        {
            foreach (var relativePath in relativePaths)
            {
                var fullPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "PHRApp",
                    relativePath);

                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                }
            }
            return Task.CompletedTask;
        }

        private string GetContentType(string extension)
        {
            return extension.ToLower() switch
            {
                ".pdf" => "application/pdf",
                ".jpg" => "image/jpeg",
                ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                _ => "application/octet-stream"
            };
        }

        public string ResolveFullPath(string relativePath)
        {
            return Path.Combine(_basePath,
                Path.GetFileName(relativePath));
        }
    }
}