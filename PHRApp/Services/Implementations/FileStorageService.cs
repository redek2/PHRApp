using PHRApp.Models.DTOs;
using PHRApp.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PHRApp.Services.Implementations
{
    public class FileStorageService : IFileStorageService
    {
        private readonly string _basePath;
        private const string AttachmentsFolder = "attachments";

        public FileStorageService()
        {
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            _basePath = Path.Combine(appData, "PHRApp", AttachmentsFolder);

            if (!Directory.Exists(_basePath))
            {
                Directory.CreateDirectory(_basePath);
            }
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
    }
}