using System;
using System.Collections.Generic;
using System.Linq;

namespace MvcStudy.Models.Menu
{
    public class MenuCollectionViewModel : BaseViewModel
    {
        private DateTime _filterDate;

        public string FilterDate { get; set; }

        public bool CanEditFutureMenu { get; private set; }

        public IEnumerable<tbl_menu> MenusGrouppedByDate { get; private set; }

        public DateTime StartDateOfWeek { get; private set; }

        public override int RecordsNumber
        {
            get { return MenusGrouppedByDate == null ? 0 : MenusGrouppedByDate.Count(); }
        }

        public override string RecordsNumberStr
        {
            get
            {
                var recordNumber = RecordsNumber;
                return recordNumber != 0
                    ? string.Concat("Всего записей - ", recordNumber)
                    : "Меню не заполнено за выбранную неделю";
            }
        }

        public MenuCollectionViewModel()
        {}

        public MenuCollectionViewModel(string filterDate)
        {}

        public void Initialization(string filterDate)
        {
            if (DbRepository == null)
                throw new Exception("Соединение с БД потеряно");

            FilterDate = filterDate;

            if (!DateTime.TryParse(filterDate, out _filterDate))
                _filterDate = DateTime.Now.Date;

            StartDateOfWeek = DbRepository.GetStartDateOfWeek(_filterDate);
            MenusGrouppedByDate = GetMenusGrouppedByDate();
            WeekDays = GetWeekDays(_filterDate);
            CanEditFutureMenu = CheckIfCanEditFutureMenu();
        }

        private IEnumerable<tbl_menu> GetMenusGrouppedByDate()
        {
            if (DbRepository == null)
                throw new Exception("Соединение с БД потеряно");

            string error;
            var result = DbRepository.GetMenusFilteredByDate(_filterDate, out error);
            return result;
        }

        private bool CheckIfCanEditFutureMenu()
        {
            if (DbRepository == null)
                throw new Exception("Соединение с БД потеряно");

            var startDateOfCurrentWeek = DbRepository.GetStartDateOfWeek(DateTime.Now);
            return startDateOfCurrentWeek < StartDateOfWeek &&
                   startDateOfCurrentWeek.AddDays(6) < StartDateOfWeek.AddDays(6);
        }
    }
}