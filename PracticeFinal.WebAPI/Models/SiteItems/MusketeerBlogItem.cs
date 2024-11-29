namespace PracticeFinal.WebAPI.Models.SiteItems
{
    /// <summary>
    /// Класс, описывающий запись в блоге на сайте
    /// </summary>
    public class MusketeerBlogItem : MusketeerSiteItem
    {
        public DateTime PublishDate { get; set; }
        public string? Image { get; set; }
        public string? LargeImage { get; set; }
        public string? Header { get; set; }
        public string? Content { get; set; }
    }
}
