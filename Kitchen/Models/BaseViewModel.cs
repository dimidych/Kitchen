using System;
using System.Collections.Generic;

namespace MvcStudy.Models
{
    public abstract class BaseViewModel
    {
        public IKitchenRepository DbRepository { get; set; }

        public abstract int RecordsNumber { get; }

        public abstract string RecordsNumberStr { get; }

        public bool IsFutureWeek { get; set; }

        public Dictionary<DateTime, string> WeekDays { get; set; }

        protected BaseViewModel()
        {}

        internal Dictionary<DateTime, string> GetWeekDays(DateTime filterDate)
        {
            if(DbRepository==null)
                throw new Exception("Соединение с БД потеряно");

            var result = new Dictionary<DateTime, string>();
            var startDateOfWeek = DbRepository.GetStartDateOfWeek(filterDate);

            for (var i = 0; i < 7; i++)
            {
                string error;
                var weekDayName = DbRepository.GetWeekDays(new tbl_weekday { id = i + 1 }, out error)[0].weekday_name;
                var date = startDateOfWeek.AddDays(i);
                result.Add(date, string.Concat(weekDayName, " - ", date.ToString("dd.MM.yyyy")));
            }

            return result;
        }

    }
}