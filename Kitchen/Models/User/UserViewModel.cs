using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace MvcStudy.Models.User
{
    public class UserViewModel : tbl_people
    {
        public IKitchenRepository DbRepository { get; set; }
        public bool ForProfile { get; set; }

        public string ConfirmPassword { get; set; }

        public bool UserTakesMeal
        {
            get { return takes_meal ?? false; }
            set { takes_meal = value; }
        }

        public bool UserExceptsDuty
        {
            get { return excepts_duty ?? false; }
            set { excepts_duty = value; }
        }

        public string GroupName
        {
            get { return tbl_group.group_name; }
        }

        public string UserTakesMealAsStr
        {
            get { return (takes_meal ?? false) ? "Питается в офисе ; " : "Не питается в офисе ; "; }
        }

        public string UserExceptsDutyAsStr
        {
            get { return (excepts_duty ?? false) ? "Не учавствует в дежурствах. " : "Учавствует в дежурствах. "; }
        }

        public IEnumerable<SelectListItem> GroupList
        {
            get
            {
                if (DbRepository == null)
                    throw new Exception("Соединение с БД потеряно");

                string error;
                var allGroups = DbRepository.GetGroups(null, out error);

                if (allGroups == null || allGroups.Length == 0)
                    yield break;

                foreach (var group in allGroups)
                    yield return new SelectListItem
                    {
                        Text = group.group_name,
                        Value = group.id.ToString(),
                        Selected = (group.id == id_group)
                    };
            }
        }

        public UserViewModel()
        {}

        public UserViewModel(tbl_people user)
        {
            id = user.id;
            id_group = user.id_group;
            people_name = user.people_name;
            email = user.email;
            hased_pwd = user.hased_pwd;
            takes_meal = user.takes_meal;
            excepts_duty = user.excepts_duty;
            tbl_group = user.tbl_group;
        }

        public tbl_people CreateClone()
        {
            var result = new tbl_people
            {
                id = this.id,
                people_name = this.people_name,
                email = this.email,
                excepts_duty = this.excepts_duty,
                hased_pwd = this.hased_pwd,
                id_group = this.id_group,
                takes_meal = this.takes_meal,
                tbl_group = this.tbl_group
            };
            return result;
        }
    }
}