using System;
using System.Linq;
using MvcStudy.Utils;

namespace MvcStudy.Models
{
    public partial class KitchenRepository : IKitchenRepository
    {
        public bool AddUser(tbl_people newUser, out string error)
        {
            error = string.Empty;

            try
            {
                if (DbCtx == null)
                    throw new Exception("Отсутствует подключение к БД");

                if (newUser == null)
                    throw new Exception("Пользователь не определен");

                if (string.IsNullOrEmpty(newUser.people_name.Trim()))
                    throw new Exception("Имя пользователя не может быть пустым");

                var selected =
                    DbCtx.tbl_people.FirstOrDefault(
                        x =>
                            String.Equals(x.people_name.Trim().ToLower(), newUser.people_name.Trim().ToLower()));

                if (selected != null)
                    throw new Exception("Пользователь с таким именем уже существует");

                selected =
                    DbCtx.tbl_people.FirstOrDefault(
                        x =>
                            String.Equals(x.email.Trim().ToLower(), newUser.email.Trim().ToLower()));

                if (selected != null)
                    throw new Exception("Пользователь с таким e-mail уже существует");

                if (newUser.id < 1)
                    newUser.id = GetTblPeopleNextId();

                newUser.hased_pwd = Tools.CreateCryptedStr(newUser.hased_pwd);
                DbCtx.tbl_people.Add(newUser);

                if (DbCtx.SaveChanges() <= 0)
                    throw new Exception("Не удалось сохранить пользователя");

                return true;
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return false;
            }
        }

        public bool DeleteUser(long userId, out string error)
        {
            error = string.Empty;

            try
            {
                if (DbCtx == null)
                    throw new Exception("Отсутствует подключение к БД");

                if (userId < 1)
                    throw new Exception("Не указан ИД пользователя");

                var selected = DbCtx.tbl_people.FirstOrDefault(x => x.id == userId);

                if (selected == null)
                    throw new Exception("Пользователь не найден");

                using (var transaction = DbCtx.Database.BeginTransaction())
                {
                    var dutyRecs = DbCtx.tbl_duty.Where(x => x.id_people == userId);

                    if (dutyRecs.Any())
                        DbCtx.tbl_duty.RemoveRange(dutyRecs);

                    var mnuSelectionRecs = DbCtx.tbl_menu_selection.Where(x => x.id_people == userId);

                    if (mnuSelectionRecs.Any())
                        DbCtx.tbl_menu_selection.RemoveRange(mnuSelectionRecs);

                    DbCtx.tbl_people.Remove(selected);

                    if (DbCtx.SaveChanges() <= 0)
                        throw new Exception("Не удалось удалить пользователя");

                    transaction.Commit();
                    return true;
                }
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return false;
            }
        }

        public bool UpdateUser(tbl_people user, bool forProfile, out string error)
        {
            error = string.Empty;

            try
            {
                if (DbCtx == null)
                    throw new Exception("Отсутствует подключение к БД");

                if (user == null)
                    throw new Exception("Пользователь не определен");

                if (user.id < 1)
                    throw new Exception("Не указан ИД пользователя");

                if (string.IsNullOrEmpty(user.people_name.Trim()))
                    throw new Exception("Имя пользователя не может быть пустым");

                var selected = DbCtx.tbl_people.FirstOrDefault(x => x.id == user.id);

                if (selected == null)
                    throw new Exception("Пользователь не найден");

                selected.people_name = user.people_name;
                selected.email = user.email;

                if (!string.IsNullOrEmpty(user.hased_pwd))
                    selected.hased_pwd = Tools.CreateCryptedStr(user.hased_pwd);

                if (!forProfile)
                {
                    selected.id_group = user.id_group;
                    selected.takes_meal = user.takes_meal;
                    selected.excepts_duty = user.excepts_duty;
                }

                if (DbCtx.SaveChanges() <= 0)
                    throw new Exception("Не удалось изменить пользователя");

                return true;
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return false;
            }
        }

        public tbl_people[] GetUsers(tbl_people filterUser, out string error)
        {
            tbl_people[] result = null;
            error = string.Empty;

            try
            {
                if (DbCtx == null)
                    throw new Exception("Отсутствует подключение к БД");

                if (filterUser != null && filterUser.id > 0)
                    result = DbCtx.tbl_people.Where(x => x.id == filterUser.id).ToArray();
                else
                    result = DbCtx.tbl_people.ToArray();
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return null;
            }

            return result;
        }

        public long GetTblPeopleNextId()
        {
            if (DbCtx == null)
                throw new Exception("Отсутствует подключение к БД");

            return DbCtx.tbl_people.Any() ? DbCtx.tbl_people.Max(x => x.id) + 1 : 1;
        }
    }
}