using System;
using System.Collections.Generic;
using System.Linq;

namespace MvcStudy.Models.Home
{
    public class SelectedMenuCollectionViewModel : BaseViewModel
    {
        public IQueryable<MenuSelectionGrouppedByPeople> MenuSelectionForWeek { get; private set; }

        public Dictionary<tbl_menu, int> SelectedMealCountForWeek { get; private set; }

        public Dictionary<DateTime, bool> CheckWhetherMenuExistByDate { get; private set; }

        public bool CheckIfFutureMenuExist { get; set; }

        public bool ForFutureMenu { get; set; }

        public int TotalDinnerPeople
        {
            get
            {
                if (DbRepository == null)
                    throw new Exception("Соединение с БД потеряно");

                string error;
                return DbRepository.GetUsers(null, out error).Count(x => x.takes_meal ?? false);
            }
        }

        public override int RecordsNumber
        {
            get { return MenuSelectionForWeek == null ? 0 : MenuSelectionForWeek.Count(); }
        }

        public override string RecordsNumberStr
        {
            get
            {
                var resordNumber = RecordsNumber;
                return resordNumber != 0
                    ? string.Concat("Всего записалось ", resordNumber, " человек из ", TotalDinnerPeople)
                    : "Пока еще никто не записался";
            }
        }

        public SelectedMenuCollectionViewModel()
        {}

        public SelectedMenuCollectionViewModel(bool forFutureMenu)
        {}

        public void Initialization(bool forFutureMenu)
        {
            if (DbRepository == null)
                throw new Exception("Соединение с БД потеряно");

            string error;
            CheckIfFutureMenuExist = DbRepository.CheckIfMenuExist(DateTime.Now.AddDays(7), out error);
            ForFutureMenu = forFutureMenu && CheckIfFutureMenuExist;
            var filterDate = (forFutureMenu ? DateTime.Now.AddDays(7) : DateTime.Now).Date;
            WeekDays = GetWeekDays(filterDate);
            MenuSelectionForWeek = DbRepository.GetMenuSelection(forFutureMenu, out error);
            SelectedMealCountForWeek = DbRepository.GetSelectedMealCount(forFutureMenu, out error);
            CheckWhetherMenuExistByDate = new Dictionary<DateTime, bool>();
            var menuItmsByDate = DbRepository.GetMenusFilteredByDate(filterDate, out error).GroupBy(x => x.date);

            foreach (var date in WeekDays.Keys)
                CheckWhetherMenuExistByDate.Add(date, menuItmsByDate.Any(x => x.Key == date));
        }
    }
}