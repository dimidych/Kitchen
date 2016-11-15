using System;
using System.Collections.Generic;
using System.EnterpriseServices;
using System.Linq;
using System.Web.Mvc;
using MvcStudy.Models.Menu;


namespace MvcStudy.Models.Home
{
    public class MenuForUnsignedUserViewModel : BaseViewModel
    {
        public override int RecordsNumber
        {
            get { return -1; }
        }

        public override string RecordsNumberStr
        {
            get { return string.Empty; }
        }

        public long UserId { get; set; }

        public DateTime DateFilter { get; set; }

        public List<string> ErrorList { get; private set; }

        public IEnumerable<SelectListItem> UnsignedPeople
        {
            get
            {
                if (DbRepository == null)
                    throw new Exception("Соединение с БД потеряно");

                string error;
                var unsignedPeople = DbRepository.GetUnsignedPeople(DateFilter, out error);

                if (!string.IsNullOrEmpty(error.Trim()))
                {
                    ErrorList.Add(error);
                    yield break;
                }

                if (unsignedPeople == null || !unsignedPeople.Any())
                    yield break;

                foreach (var unsigned in unsignedPeople)
                    yield return new SelectListItem
                    {
                        Text = unsigned.people_name,
                        Value = unsigned.id.ToString()
                    };
            }
        }

        public List<MenuViewModel> MenuLst { get; set; }

        public int MaxColumnCount { get; private set; }

        public MenuForUnsignedUserViewModel()
        {
            MaxColumnCount = 1;
            ErrorList = new List<string>();
        }

        public void Initialization()
        {
            if (DbRepository == null)
                throw new Exception("Соединение с БД потеряно");

            WeekDays = GetWeekDays(DateFilter);
            string error;
            MenuLst =
                DbRepository.GetMenusFilteredByDate(DateFilter, out error).Select(x => new MenuViewModel(x)).ToList();

            if (!string.IsNullOrEmpty(error.Trim()))
                ErrorList.Add(error);

            MaxColumnCount = (from menu in MenuLst
                group menu by menu.date
                into mnuGrp
                select mnuGrp.Count()).Max()*2;
        }
    }
}