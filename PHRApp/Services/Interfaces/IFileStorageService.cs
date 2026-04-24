using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PHRApp.Models.DTOs;

namespace PHRApp.Services.Interfaces
{
    public interface IFileStorageService
    {
        Task<List<StoredFileResult>> SaveFilesAsync(List<string> filePaths);
    }
}