using System;

namespace FilesStorage.DAO.Entities
{
    /// <summary>
    /// Класс с моделью паспорта из PG SQL
    /// </summary>
    public class Passport
    {
        /// <summary>
        /// Идентификатор в базе
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Серия паспорта
        /// </summary>
        public string Serial { get; set; }
        /// <summary> 
        /// Номер паспорта
        /// </summary>
        public string Number { get; set; }
        /// <summary>
        /// Дата, когда паспорт добавлен
        /// </summary>
        public DateTime? Added { get; set; }
        /// <summary> 
        /// Дата, когда паспорт удалён 
        /// </summary>
        public DateTime? Deleted { get; set; }


        public Passport() {}

        public Passport(string serial, string number)
        {
            Serial = serial;
            Number = number;
        }
    }
}
