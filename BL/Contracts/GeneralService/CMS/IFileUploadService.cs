using Microsoft.AspNetCore.Http;

namespace BL.Contracts.GeneralService.CMS
{
    public interface IFileUploadService
    {
        public  Task<List<string>> AddImagesAsync(IFormFileCollection files, string featureFolder, string nameEntity, List<string>? oldFileNames = null);
        public Task ArchiveFileAsync(string relativePath, string entityName);
        Task<byte[]> GetFileBytesAsync(IFormFile file);
        Task<byte[]> GetFileBytesAsync(string base64String);

        public void DeleteFiles(IEnumerable<string> relativePaths);
        public void DeleteFile(string relativePath);

        public  Task<string> UploadFileAsync(  IFormFile file,string featureFolder,string oldFileName = null, bool convertToWebP = true);
        Task<string> UploadFileAsync(byte[] fileBytes, string folderName, string? oldFileName = null);
        public Task<string> UploadVideoAsync(IFormFile file, string featureFolder, string oldFileName = null);

        bool IsValidFile(IFormFile file);
        bool IsValidFile(string base64File, string fileName);
        (bool isValid, string errorMessage) ValidateFile(IFormFile file);
        (bool isValid, string errorMessage) ValidateFile(string base64String);
    }
}
