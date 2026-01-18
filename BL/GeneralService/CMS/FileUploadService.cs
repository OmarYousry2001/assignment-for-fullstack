using BL.Contracts.GeneralService.CMS;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Resources;
using System.Diagnostics;

namespace BL.GeneralService.CMS
{
    public class FileUploadService : IFileUploadService
    {
        private readonly IWebHostEnvironment _env;
        private readonly long _maxFileSize = 5 * 1024 * 1024; //5MB
        private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif" };
        private readonly string _imagesFolder = "uploads";
        private readonly string _archivedImages = "ArchivedImages";



        public FileUploadService(IWebHostEnvironment env)
        {
            _env = env;
        }
        public async Task ArchiveFileAsync(string relativePath, string entityName)
        {
            if (string.IsNullOrWhiteSpace(relativePath) || string.IsNullOrWhiteSpace(entityName))
                return;

            var sourcePath = Path.Combine(_env.WebRootPath, relativePath.Replace("/", Path.DirectorySeparatorChar.ToString()));
            if (!File.Exists(sourcePath))
                return;

            var fileName = Path.GetFileName(sourcePath);

            var safeEntityName = string.Concat(entityName.Where(c => !Path.GetInvalidFileNameChars().Contains(c))).Trim();

            var archiveFolder = Path.Combine(_env.WebRootPath, _archivedImages, safeEntityName);
            Directory.CreateDirectory(archiveFolder);

            var archivePath = Path.Combine(archiveFolder, fileName);

            if (File.Exists(archivePath))
                File.Delete(archivePath);

            File.Move(sourcePath, archivePath);
        }



        public async Task<byte[]> GetFileBytesAsync(IFormFile file)
        {
            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);
            return memoryStream.ToArray();
        }

        public async Task<byte[]> GetFileBytesAsync(string base64String)
        {
            // Simulate async operation, if needed
            return await Task.Run(() => Convert.FromBase64String(base64String));
        }

        public async Task<string> UploadFileAsync(
           IFormFile file,
           string featureFolder,
           string oldFileName = null,
           bool convertToWebP = true)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException(ValidationResources.Invalidfile);

            if (file.Length > _maxFileSize)
                throw new ArgumentException(string.Format(ValidationResources.FileSizeLimit, _maxFileSize / 1024 / 1024));

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!_allowedExtensions.Contains(extension))
                throw new ArgumentException($"{ValidationResources.InvalidFormat} {string.Join(", ", _allowedExtensions)}");

            /// <summary>
            /// Cleans a file name by removing invalid characters and trimming extra spaces.
            /// </summary>
            /// <example>
            /// Example:
            /// <code>
            /// string fileName = CleanFileName("my:file?.txt");  
            /// // Result: "myfile.txt"
            /// </code>
            /// </example>
            string CleanFileName(string input)
            {
                var invalidChars = Path.GetInvalidFileNameChars(); //[\, /, :, *, ?, ", <, >, |]
                return new string(input.Where(ch => !invalidChars.Contains(ch)).ToArray()).Trim();
            }

            featureFolder = CleanFileName(featureFolder);

            // Create Path
            var uploadsFolder = Path.Combine(_env.WebRootPath, _imagesFolder, featureFolder);
            Directory.CreateDirectory(uploadsFolder);

            // Delete old file if exists    
            if (!string.IsNullOrEmpty(oldFileName))
            {
                var oldFileNameOnly = Path.GetFileName(oldFileName); 
                var oldFilePath = Path.Combine(uploadsFolder, oldFileNameOnly);
                if (File.Exists(oldFilePath))
                {
                    File.Delete(oldFilePath);
                }
            }

            // convert to byte[]
            var fileBytes = await GetFileBytesAsync(file);

            string uniqueFileName;
            string filePath;

            if (convertToWebP)
            {
                // convert to WebP
                var imageProcessor = new ImageProcessingService();
                var processedImage = imageProcessor.ConvertToWebP(fileBytes, quality: 100);

                uniqueFileName = $"{Guid.NewGuid()}.webp";
                filePath = Path.Combine(uploadsFolder, uniqueFileName);

                await File.WriteAllBytesAsync(filePath, processedImage);
            }
            else
            {
                // Save the image in its original format
                uniqueFileName = $"{Guid.NewGuid()}{extension}";
                filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }
            }

            // return relative path
            var relativePath = Path.Combine(_imagesFolder, featureFolder, uniqueFileName)
                                  .Replace("\\", "/");

            return relativePath;
        }


    
        public async Task<string> UploadVideoAsync( IFormFile file, string featureFolder, string oldFileName = null)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException(ValidationResources.Invalidfile);

            // Increase the maximum allowed video size (e.g., 100MB)
            // Check if the uploaded file exceeds the maximum allowed size

            var maxVideoSize = 100 * 1024 * 1024;
            if (file.Length > maxVideoSize)
                throw new ArgumentException($"File size exceeds the limit of {maxVideoSize / 1024 / 1024} MB.");

            // Validate the video file extension
            var allowedVideoExtensions = new[] { ".mp4", ".mov", ".avi", ".mkv" };
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!allowedVideoExtensions.Contains(extension))
                throw new ArgumentException($"Invalid video format. Allowed: {string.Join(", ", allowedVideoExtensions)}");

            // Clean the folder name
            string CleanFileName(string input)
            {
                var invalidChars = Path.GetInvalidFileNameChars();
                return new string(input.Where(ch => !invalidChars.Contains(ch)).ToArray()).Trim();
            }

            featureFolder = CleanFileName(featureFolder);

            // Create the path
            var uploadsFolder = Path.Combine(_env.WebRootPath, _imagesFolder, featureFolder);
            Directory.CreateDirectory(uploadsFolder);

            // Delete the old video if it exists
            if (!string.IsNullOrEmpty(oldFileName))
            {
                var oldFilePath = Path.Combine(uploadsFolder, Path.GetFileName(oldFileName));
                if (File.Exists(oldFilePath))
                {
                    File.Delete(oldFilePath);
                }
            }

            // Generate a unique file name
            var uniqueFileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            // Save the video
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            // Return the relative path
            var relativePath = Path.Combine(_imagesFolder, featureFolder, uniqueFileName)
                                  .Replace("\\", "/");

            return relativePath;
        }


        public async Task<string> UploadFileAsync(byte[] fileBytes, string folderName, string? oldFileName = null)
        {
            // Create the uploads folder if it doesn't exist
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", folderName);
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            if (!string.IsNullOrEmpty(oldFileName))
            {
                var oldFilePath = Path.Combine(uploadsFolder, oldFileName);
                if (File.Exists(oldFilePath))
                {
                    File.Delete(oldFilePath);
                }
            }

            //   WebP  100%
            var imageProcessor = new ImageProcessingService();
            var processedImage = imageProcessor.ConvertToWebP(fileBytes, quality: 100);

            // Generate a new GUID and append the original file extension
            var uniqueFileName = $"{Guid.NewGuid()}.webp";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);
            int index = filePath.IndexOf("uploads");
            string relativePath = filePath.Substring(index);

            // Write the file bytes to the specified path
            await File.WriteAllBytesAsync(filePath, processedImage);

            return uniqueFileName;
        }
        public async Task<List<string>> AddImagesAsync(IFormFileCollection files, string featureFolder, string nameEntity, List<string>? oldFileNames = null)
        {
            string CleanFileName(string input)
            {
                var invalidChars = Path.GetInvalidFileNameChars();
                return new string(input.Where(ch => !invalidChars.Contains(ch)).ToArray()).Trim();
            }

            featureFolder = CleanFileName(featureFolder);
            nameEntity = CleanFileName(nameEntity);

            var savedImagePaths = new List<string>();
            var uploadsFolder = Path.Combine(_env.WebRootPath, _imagesFolder, featureFolder, nameEntity);
            Directory.CreateDirectory(uploadsFolder);

            var imageProcessor = new ImageProcessingService();

            if (oldFileNames != null )
            {
                foreach (var oldFile in oldFileNames)
                {
                    var oldFileNameOnly = Path.GetFileName(oldFile);
                    var oldPath = Path.Combine(uploadsFolder, oldFileNameOnly);
                    if (File.Exists(oldPath))
                        File.Delete(oldPath);
                }
            }

            //foreach (var file in files)
            //{
            //    if (file.Length > 0)
            //    {
            //        var fileBytes = await GetFileBytesAsync(file);
            //        var processedImage = imageProcessor.ConvertToWebP(fileBytes, quality: 100);
            //        var uniqueFileName = $"{Guid.NewGuid()}.webp";
            //        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            //        await File.WriteAllBytesAsync(filePath, processedImage);

            //        var relativePath = Path.Combine(_imagesFolder, featureFolder, nameEntity, uniqueFileName)
            //                            .Replace("\\", "/");
            //        savedImagePaths.Add(relativePath);
            //    }
            //}


            var tasks = files.Select(async file => {
                var fileBytes = await GetFileBytesAsync(file);
                var processedImage = imageProcessor.ConvertToWebP(fileBytes, quality: 80);
                var uniqueFileName = $"{Guid.NewGuid()}.webp";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                await File.WriteAllBytesAsync(filePath, processedImage);
                return Path.Combine(_imagesFolder, featureFolder, nameEntity, uniqueFileName).Replace("\\", "/");
            });

            var savedImagePathsEnd = await Task.WhenAll(tasks);




            return savedImagePathsEnd.ToList();
        }


        public bool IsValidFile(IFormFile file)
        {
            return ValidateFile(file).isValid;
        }

        public bool IsValidFile(string base64File, string fileName)
        {
            // Implementation for base64 string validation
            throw new NotImplementedException();
        }

        public (bool isValid, string errorMessage) ValidateFile(IFormFile file)
        {
            if (file == null)
                return (false, "File is null.");

            if (!file.ContentType.StartsWith("image/"))
                return (false, "Invalid file type. Only images are allowed.");

            return (true, string.Empty);
        }

        public (bool isValid, string errorMessage) ValidateFile(string base64String)
        {
            if (string.IsNullOrEmpty(base64String)) return (false, "File is null.");

            Span<byte> buffer = new Span<byte>(new byte[base64String.Length]);
            return (Convert.TryFromBase64String(base64String, buffer, out _), "File is not valid");
        }

     

        
            public void DeleteFile(string relativePath)
            {
                if (string.IsNullOrWhiteSpace(relativePath)) return;

                var fullPath = Path.Combine(_env.WebRootPath, relativePath.Replace("/", Path.DirectorySeparatorChar.ToString()));
                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                }
            }

            public void DeleteFiles(IEnumerable<string> relativePaths)
            {
                if (relativePaths == null) return;

                foreach (var path in relativePaths)
                {
                    DeleteFile(path);
                }
            }
        }



}
