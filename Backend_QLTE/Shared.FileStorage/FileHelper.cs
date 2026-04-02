using Microsoft.AspNetCore.Http;
using System.Text.RegularExpressions;
using System.Text;

namespace Shared.FileStorage
{
    // Lưu trữ file , đọc , xóa file
    public static class FileHelper
    {
        // Lưu ảnh vào thư mục và trả về đường dẫn file để lưu vào DB
        public static async Task<string> SaveImageAsync(IFormFile file, string folderPath)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File không hợp lệ");

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var savePath = Path.Combine(folderPath, fileName);

            Directory.CreateDirectory(folderPath);

            // Nếu thư mục chưa có thì tự động tạo.
            using (var stream = new FileStream(savePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return fileName; // trả về tên file để lưu DB
        }

        // Xóa Ảnh theo đường dẫn tương đối
        public static bool DeleteImage(string relativePath) // Đường dẫn tương đối
        {
            if (string.IsNullOrEmpty(relativePath))
                return false;
            
            var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", relativePath);

            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
                return true;
            }
            return false;
        }

        // Lưu Video vào thư mục và trả về đường dẫn file để lưu vào DB
        public static async Task<string> SaveVideoAsync(IFormFile file, string folderPath, string fileNameWithoutExt)
        {
            if (file == null || file.Length == 0) throw new ArgumentException("File không hợp lệ");

            // Đảm bảo tên file không có ký tự lạ
            var safeName = GenerateSafeFileName(fileNameWithoutExt);

            // Lấy extension gốc (mp4, mkv, ...)
            var extension = Path.GetExtension(file.FileName);

            var fileName = $"{safeName}{extension}";
            var savePath = Path.Combine(folderPath, fileName);

            Directory.CreateDirectory(folderPath);

            using (var stream = new FileStream(savePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return fileName;
        }

        // Xóa Video theo đường dẫn tương đối
        public static bool DeleteVideo(string relativePath)
        {
            if (string.IsNullOrEmpty(relativePath))
                return false;

            var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", relativePath);

            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
                return true;
            }
            return false;
        }

        // Helper để tạo tên file an toàn
        private static string GenerateSafeFileName(string input)
        {
            // bỏ dấu, space -> "-"
            string normalized = input.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();
            foreach (var c in normalized)
            {
                var uc = System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c);
                if (uc != System.Globalization.UnicodeCategory.NonSpacingMark)
                    sb.Append(c);
            }
            string noDiacritics = sb.ToString().Normalize(NormalizationForm.FormC);
            string safe = Regex.Replace(noDiacritics.ToLower(), @"\s+", "-");
            safe = Regex.Replace(safe, @"[^a-z0-9-_]", "");
            return safe;
        }

    }
}
