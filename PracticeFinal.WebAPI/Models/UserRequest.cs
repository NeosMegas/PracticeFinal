using System.ComponentModel;

namespace PracticeFinal.WebAPI.Models
{
    /// <summary>
    /// Класс, описывающий заявку пользователя с сайта
    /// </summary>
    public class UserRequest
    {
        public int Id { get; set; }
        /// <summary>
        /// Имя пользователя
        /// </summary>
        public string? Name { get; set; }
        /// <summary>
        /// Электронная почта пользователя
        /// </summary>
        public string? Email { get; set; }
        /// <summary>
        /// Текст сообщения
        /// </summary>
        public string? MessageText { get; set; }
        /// <summary>
        /// Статус обработки заявки
        /// </summary>
        public RequestStatus RequestStatus { get; set; }
        /// <summary>
        /// Дата и время создания заявки
        /// </summary>
        public DateTime? Created { get; set;  }


        public UserRequest()
        {
            //Created = DateTime.Now;
            //RequestStatus = RequestStatus.NewRequest;
        }

    }

    /// <summary>
    /// Перечисление, описывающее возможные статусы заявки:
    /// 1. Получена — начальный статус. Гость заполнил форму и отправил данные, они поступили в систему, но ещё не были обработаны.
    /// 2. В работе.Администратор связался с гостем для уточнения деталей.
    /// 3. Выполнена.Услуга оказана.
    /// 4. Отклонена.Заявка не подходит или сделана ботом.
    /// 5. Отменена.Заказчики передумали или заявка потеряла актуальность.
    /// </summary>
    public enum RequestStatus
    {
        NewRequest,
        InProgress,
        Completed,
        Rejected,
        Cancelled
    }
}
