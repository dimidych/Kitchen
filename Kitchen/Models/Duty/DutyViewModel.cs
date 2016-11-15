using System;
using System.Collections.Generic;
using System.Linq;

namespace MvcStudy.Models.Duty
{
    public class DutyViewModel : BaseViewModel
    {
        public override int RecordsNumber
        {
            get { return DutyPeopleLst == null ? 0 : DutyPeopleLst.Count(); }
        }

        public override string RecordsNumberStr
        {
            get
            {
                var resordNumber = RecordsNumber;
                return resordNumber != 0
                    ? string.Concat("Всего дежурит ", resordNumber, " людей")
                    : "Пока еще никто не дежурит";
            }
        }

        public DateTime StartPunishDate
        {
            get
            {
                if (DbRepository == null)
                    throw new Exception("Соединение с БД потеряно");

                return DbRepository.GetStartDateOfWeek(DateTime.Now).Date;
            }
        }

        public bool ForFutureWeek { get; set; }

        public bool CheckForFutureWeekEaters { get; private set; }

        public bool CheckDutyPeopleAccepted { get; private set; }

        public List<tbl_duty> DutyPeopleLst { get; private set; }

        public DutyViewModel()
        {}

        public DutyViewModel(bool forFutureWeek)
        {}

        public void Initialization(bool forFutureWeek)
        {
            if (DbRepository == null)
                throw new Exception("Соединение с БД потеряно");

            string error;
            IsFutureWeek = ForFutureWeek = forFutureWeek;
            CheckForFutureWeekEaters = DbRepository.CheckForFutureWeekEaters(true, out error);
            var filterDate = (forFutureWeek ? DateTime.Now.AddDays(7) : DateTime.Now).Date;
            WeekDays = GetWeekDays(filterDate);
            DutyPeopleLst = DbRepository.GetDutyRecords(null, out error)
                .Where(x => x.duty_date >= WeekDays.Keys.Min() && x.duty_date <= WeekDays.Keys.Max()).ToList();
            CheckDutyPeopleAccepted = (DutyPeopleLst != null && DutyPeopleLst.Any());

            if (DutyPeopleLst == null || !DutyPeopleLst.Any())
                DutyPeopleLst = DbRepository.CalculateDutyPeople(forFutureWeek, out error);
        }
    }
}