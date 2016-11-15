using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace MvcStudy.Models.Duty
{
    public class ChangingDutyManViewModel : tbl_duty
    {
        public IKitchenRepository DbRepository { get; set; }

        public IEnumerable<SelectListItem> DutySupposedPeople
        {
            get
            {
                if (DbRepository == null)
                    throw new Exception("Соединение с БД потеряно");

                string error;
                var startDateOfWeek = DbRepository.GetStartDateOfWeek(duty_date);
                var endDayOfWeek = startDateOfWeek.AddDays(6);
                var dutyPeople = (from selMenuItm in DbRepository.GetMenuSelection(null, out error)
                    where selMenuItm.tbl_menu.date >= startDateOfWeek && selMenuItm.tbl_menu.date <= endDayOfWeek
                          && selMenuItm.id_people != id_people && !(selMenuItm.tbl_people.excepts_duty ?? false)
                    select selMenuItm.tbl_people);

                var dutySupposedPeople = new List<tbl_people>();

                foreach (var people in dutyPeople)
                    if (dutySupposedPeople.Any())
                    {
                        if (dutySupposedPeople.FirstOrDefault(x => x.id == people.id) == null)
                            dutySupposedPeople.Add(people);
                    }
                    else
                        dutySupposedPeople.Add(people);

                foreach (var dutySupposeMan in dutySupposedPeople)
                    yield return new SelectListItem
                    {
                        Text = dutySupposeMan.people_name,
                        Selected = (dutySupposeMan.id == id_people),
                        Value = dutySupposeMan.id.ToString()
                    };
            }
        }

        public ChangingDutyManViewModel()
        {

        }

        public ChangingDutyManViewModel(tbl_duty baseObj)
        {
            id = baseObj.id;
            id_people = baseObj.id_people;
            duty_date = baseObj.duty_date;
        }

        public tbl_duty CreateClone()
        {
            return new tbl_duty
            {
                id = this.id,
                id_people = this.id_people,
                duty_date = this.duty_date
            };
        }
    }
}