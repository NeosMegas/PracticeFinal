namespace PracticeFinal.WebAPI.Models
{
    /// <summary>
    /// Класс, содержащий данные для отображения в основном интерфейса сайта
    /// </summary>
    public class SiteInfo
    {
        public int Id { get; set; }
        /// <summary>
        /// Строковый идентификатор объекта, например, надписи на кнопке
        /// </summary>
        public required string StringId { get; set; }
        /// <summary>
        /// Содержимое объекта, например, надпись на кнопке
        /// </summary>
        public required string Content { get; set; }
    }
}
