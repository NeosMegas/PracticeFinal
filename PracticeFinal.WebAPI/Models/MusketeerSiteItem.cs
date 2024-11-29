namespace PracticeFinal.WebAPI.Models
{
    /// <summary>
    /// Абстрактный класс, описывающий общие для всех элементов сайта свойства
    /// </summary>
    public abstract class MusketeerSiteItem
    {
        public int Id { get; set; }
        /// <summary>
        /// Наименование элемента
        /// </summary>
        public string? Name { get; set; }
        /// <summary>
        /// Описание элемента
        /// </summary>
        public string? Description { get; set; }
        /// <summary>
        /// Позиция элемента в списке на сайте
        /// </summary>
        public int Position { get; set; }

    }
}
