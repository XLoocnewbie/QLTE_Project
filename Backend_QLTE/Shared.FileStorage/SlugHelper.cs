using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;

namespace Shared.FileStorage
{
    public static class SlugHelper
    {
        public static string GenerateSlug(string phrase)
        {
            if (string.IsNullOrWhiteSpace(phrase))
                return string.Empty;

            // 1. Chuyển sang chữ thường
            string str = phrase.ToLowerInvariant();

            // 2. Chuẩn hóa Unicode để loại bỏ dấu tiếng Việt
            str = RemoveDiacritics(str);

            // 3. Xóa ký tự không hợp lệ
            str = Regex.Replace(str, @"[^a-z0-9\s-]", "");

            // 4. Thay khoảng trắng bằng dấu gạch ngang
            str = Regex.Replace(str, @"\s+", "-").Trim('-');

            // 5. Gộp nhiều dấu - liên tiếp
            str = Regex.Replace(str, @"-+", "-");

            return str;
        }

        public static string RemoveDiacritics(string text)
        {
            var normalized = text.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();

            foreach (var c in normalized)
            {
                var uc = CharUnicodeInfo.GetUnicodeCategory(c);
                if (uc != UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(c);
                }
            }

            return sb.ToString().Normalize(NormalizationForm.FormC);
        }

        public static string NormalizeSearchText(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return string.Empty;
            return RemoveDiacritics(text).Trim().ToLowerInvariant();
        }
    }
}
