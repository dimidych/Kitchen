using System;
using System.Collections.Generic;
using System.Linq;
using MvcStudy.Models.Menu;

namespace MvcStudy.Models.Home
{
    public class FutureWeekUserMenuViewModel : BaseViewModel
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

        public bool UseDateFilter { get; set; }

        public DateTime DateFilter { get; set; }

        public List<string> ErrorList { get; private set; }

        public List<MenuViewModel> NextWeekMenuForUser { get; set; }
        
        public int MaxColumnCount { get; private set; }

        public FutureWeekUserMenuViewModel()
        {
            MaxColumnCount = 1;
            ErrorList = new List<string>();
        }

        public void Initialization(long userId)
        {
            if (DbRepository == null)
                throw new Exception("Соединение с БД потеряно");

            UserId = userId;
            string error;
            NextWeekMenuForUser = DbRepository.GetNextWeekMenuForUser(userId, UseDateFilter,
                DateFilter, out error);

            if (!string.IsNullOrEmpty(error.Trim()))
                ErrorList.Add(error);

            if (UseDateFilter)
                return;

            WeekDays = GetWeekDays(DateTime.Now.AddDays(7).Date);
            var grouppedWeeklyMenu = NextWeekMenuForUser.GroupBy(x => x.date);

            foreach (var grp in grouppedWeeklyMenu)
                MaxColumnCount = Math.Max(MaxColumnCount*2, grp.Count());
        }
    }
}