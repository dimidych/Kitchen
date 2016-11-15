using System;
using System.Collections.Generic;
using System.Linq;

namespace MvcStudy.Models
{
    public partial class KitchenRepository : IKitchenRepository
    {
        public bool AddMenu(tbl_menu newMenuItem, out string error)
        {
            error = string.Empty;

            try
            {
                if (DbCtx == null)
                    throw new Exception("Отсутствует подключение к БД");

                if (newMenuItem == null)
                    throw new Exception("Пункт меню не определен");

                if (newMenuItem.id_meal < 1)
                    throw new Exception("Блюдо не выбрано");

                if (newMenuItem.id < 1)
                    newMenuItem.id = GetTblMenuNextId();

                newMenuItem.date = newMenuItem.date.Date;
                var existed =
                    DbCtx.tbl_menu.FirstOrDefault(
                        x =>
                            x.id_meal == newMenuItem.id_meal && x.date == newMenuItem.date);

                if (existed != null)
                    throw new Exception("Пункт меню уже существует");

                DbCtx.tbl_menu.Add(newMenuItem);

                if (DbCtx.SaveChanges() <= 0)
                    throw new Exception("Не удалось сохранить пункт меню");

                return true;
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return false;
            }
        }

        public bool DeleteMenu(long? menuId, out string error)
        {
            error = string.Empty;

            try
            {
                if (DbCtx == null)
                    throw new Exception("Отсутствует подключение к БД");

                if (menuId < 1)
                    throw new Exception("Не указан ИД удаляемого пункта меню");

                var selected = DbCtx.tbl_menu.FirstOrDefault(x => x.id == menuId);

                if (selected == null)
                    throw new Exception("Пункт меню не найден");

                DbCtx.tbl_menu.Remove(selected);

                if (DbCtx.SaveChanges() <= 0)
                    throw new Exception("Не удалось удалить пункт меню");

                return true;
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return false;
            }
        }

        public bool UpdateMenu(tbl_menu menuItem, out string error)
        {
            error = string.Empty;

            try
            {
                if (DbCtx == null)
                    throw new Exception("Отсутствует подключение к БД");

                if (menuItem == null)
                    throw new Exception("Пункт меню не определен");

                if (menuItem.id < 1)
                    throw new Exception("Не указан ID пункта меню");

                if (menuItem.id_meal < 1)
                    throw new Exception("Блюдо не выбрано");

                var selected = DbCtx.tbl_menu.FirstOrDefault(x => x.id == menuItem.id);

                if (selected == null)
                    throw new Exception("Пункт меню не найден");

                selected.date = menuItem.date.Date;
                selected.id_meal = menuItem.id_meal;

                if (DbCtx.SaveChanges() <= 0)
                    throw new Exception("Не удалось изменить пункт меню");

                return true;
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return false;
            }
        }

        public tbl_menu[] GetMenus(tbl_menu filterMenuItem, out string error)
        {
            error = string.Empty;
            tbl_menu[] result;

            try
            {
                if (DbCtx == null)
                    throw new Exception("Отсутствует подключение к БД");

                if (filterMenuItem != null && filterMenuItem.id > 0)
                    result = DbCtx.tbl_menu.Where(x => x.id == filterMenuItem.id).ToArray();
                else
                    result = DbCtx.tbl_menu.ToArray();
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return null;
            }

            return result;
        }

        public IEnumerable<tbl_menu> GetMenusFilteredByDate(DateTime filterDate, out string error)
        {
            error = string.Empty;

            try
            {
                if (DbCtx == null)
                    throw new Exception("Отсутствует подключение к БД");

                var startDateOfWeek = GetStartDateOfWeek(filterDate);
                var endDateOfWeek = startDateOfWeek.AddDays(6);
                return (from menuItem in DbCtx.tbl_menu
                    where menuItem.date <= endDateOfWeek && menuItem.date >= startDateOfWeek
                    orderby menuItem.date
                    select menuItem);
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return null;
            }
        }

        public long GetTblMenuNextId()
        {
            if (DbCtx == null)
                throw new Exception("Отсутствует подключение к БД");

            return DbCtx.tbl_menu.Any() ? DbCtx.tbl_menu.Max(x => x.id) + 1 : 1;
        }

        public bool CheckIfMenuExist(DateTime filterDate, out string error)
        {
            error = string.Empty;

            try
            {
                if (DbCtx == null)
                    throw new Exception("Отсутствует подключение к БД");

                var startDateOfWeek = GetStartDateOfWeek(filterDate);
                var endDateOfWeek = startDateOfWeek.AddDays(6);
                return DbCtx.tbl_menu.Any(
                    menuItem => menuItem.date <= endDateOfWeek && menuItem.date >= startDateOfWeek);
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return false;
            }
        }

    }
}