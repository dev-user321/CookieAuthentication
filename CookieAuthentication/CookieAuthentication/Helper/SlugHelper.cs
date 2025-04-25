namespace ShopApp.Helper
{
    public static class SlugHelper
    {
        public static string GenerateSlug(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            return "";

        string slug = title.ToLowerInvariant()
            .Replace("ə", "e")
            .Replace("ü", "u")
            .Replace("ö", "o")
            .Replace("ç", "c")
            .Replace("ş", "s")
            .Replace("ı", "i")
            .Replace("ğ", "g");

        slug = System.Text.RegularExpressions.Regex.Replace(slug, @"[^a-z0-9\s-]", ""); // xüsusi simvolları sil
        slug = System.Text.RegularExpressions.Regex.Replace(slug, @"\s+", "-").Trim('-'); // boşluqları `-` ilə əvəzlə
        slug = System.Text.RegularExpressions.Regex.Replace(slug, @"-+", "-"); // çoxlu `-` varsa, birləşdir

        return slug;
    }
    }
}
