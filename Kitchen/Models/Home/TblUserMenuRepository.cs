using System;
using System.Collections.Generic;
using System.Linq;
using MvcStudy.Models.Home;
using MvcStudy.Models.Menu;

namespace MvcStudy.Models
{
    public partial class KitchenRepository : IKitchenRepository
    {
        public IQueryable<MenuSelectionGrouppedByPeople> GetMenuSelection(bool forFutureWeek, out string error)
        {
            error = string.Empty;

            try
            {
                if (DbCtx == null)
                    throw new Exception("Отсутствует подключение к БД");

                var searchDate = DateTime.Now.Date;

                if (forFutureWeek)
                    searchDate = searchDate.AddDays(7).Date;

                var startDateOfWeek = GetStartDateOfWeek(searchDate);
                var endDateOfWeek = startDateOfWeek.AddDays(6);
                return (from selectedMnu in DbCtx.tbl_menu_selection
                    where selectedMnu.tbl_menu.date <= endDateOfWeek && selectedMnu.tbl_menu.date >= startDateOfWeek
                    orderby selectedMnu.tbl_menu.date
                    group selectedMnu by selectedMnu.tbl_people
                    into selectedMnuGroupedByPeople
                    orderby selectedMnuGroupedByPeople.Key.people_name
                    select new MenuSelectionGrouppedByPeople
                    {
                        User = selectedMnuGroupedByPeople.Key,
                        MenuSelectionGroup = selectedMnuGroupedByPeople
                    });
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return null;
            }
        }

        public Dictionary<tbl_menu, int> GetSelectedMealCount(bool forFutureWeek, out string error)
        {
            error = string.Empty;

            try
            {
                if (DbCtx == null)
                    throw new Exception("Отсутствует подключение к БД");

                var searchDate = DateTime.Now.Date;

                if (forFutureWeek)
                    searchDate = searchDate.AddDays(7).Date;

                var startDateOfWeek = GetStartDateOfWeek(searchDate);
                var endDateOfWeek = startDateOfWeek.AddDays(6);
                var selectedMeals = (from selectedMnu in DbCtx.tbl_menu_selection
                    where selectedMnu.tbl_menu.date <= endDateOfWeek && selectedMnu.tbl_menu.date >= startDateOfWeek
                    orderby selectedMnu.tbl_menu.date
                    group selectedMnu by selectedMnu.tbl_menu
                    into selectedMnuGroupedByMeal
                    select new
                    {
                        Menu = selectedMnuGroupedByMeal.Key,
                        TtlMealCount = selectedMnuGroupedByMeal.Sum(x => x.qty ?? 1)
                    });
                var result = new Dictionary<tbl_menu, int>();

                if (selectedMeals == null || !selectedMeals.Any())
                    return null;

                foreach (var menuItm in selectedMeals)
                    result.Add(menuItm.Menu, menuItm.TtlMealCount);

                return result;
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return null;
            }
        }

        public tbl_menu_selection[] GetMenuSelection(tbl_menu_selection selectedMenu, out string error)
        {
            error = string.Empty;
            tbl_menu_selection[] result = null;

            try
            {
                if (DbCtx == null)
                    throw new Exception("Отсутствует подключение к БД");

                if (selectedMenu != null)
                    result =
                        DbCtx.tbl_menu_selection.Where(
                            x => x.id_menu == selectedMenu.id_menu && x.id_people == selectedMenu.id_people)
                            .ToArray();
                else
                    result = DbCtx.tbl_menu_selection.ToArray();
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return null;
            }

            return result;
        }

        public bool AddMenuSelection(tbl_menu_selection newSelectedMenu, out string error)
        {
            error = string.Empty;

            try
            {
                if (DbCtx == null)
                    throw new Exception("Отсутствует подключение к БД");

                if (newSelectedMenu == null)
                    throw new Exception("Пункт меню не определен");

                if (newSelectedMenu.id_people < 1)
                    throw new Exception("Необходимо указать пользователя");

                if (newSelectedMenu.id < 1)
                    newSelectedMenu.id = GetTblMenuSelectionNextId();

                if (newSelectedMenu.qty == null)
                    newSelectedMenu.qty = 1;

                DbCtx.tbl_menu_selection.Add(newSelectedMenu);

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

        public bool UpdateMenuSelection(tbl_menu_selection selectedMenu, out string error)
        {
            error = string.Empty;

            try
            {
                if (DbCtx == null)
                    throw new Exception("Отсутствует подключение к БД");

                if (selectedMenu == null)
                    throw new Exception("Выбраннй пункт меню не определен");

                if (selectedMenu.id < 1)
                    throw new Exception("Не указан ID выбранного пункта меню");

                var selected = DbCtx.tbl_menu_selection.FirstOrDefault(x => x.id == selectedMenu.id);

                if (selected == null)
                    throw new Exception("Пункт меню не найден");

                selected.id_people = selectedMenu.id_people;
                selected.id_menu = selectedMenu.id_menu;
                selected.qty = selectedMenu.qty ?? 1;

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

        public bool DeleteMenuSelection(long? selectedMenuId, out string error)
        {
            error = string.Empty;

            try
            {
                if (DbCtx == null)
                    throw new Exception("Отсутствует подключение к БД");

                if (selectedMenuId < 1)
                    throw new Exception("Не указан ИД выбранного пункта меню");

                var selected = DbCtx.tbl_menu_selection.FirstOrDefault(x => x.id == selectedMenuId);

                if (selected == null)
                    throw new Exception("Выбранный пункт меню не найден");

                DbCtx.tbl_menu_selection.Remove(selected);

                if (DbCtx.SaveChanges() <= 0)
                    throw new Exception("Не удалось удалить выбранный пункт меню");

                return true;
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return false;
            }
        }

        public long GetTblMenuSelectionNextId()
        {
            if (DbCtx == null)
                throw new Exception("Отсутствует подключение к БД");

            return DbCtx.tbl_menu_selection.Any() ? DbCtx.tbl_menu_selection.Max(x => x.id) + 1 : 1;
        }

        public DateTime GetSelectedMenuMaxDate()
        {
            if (DbCtx == null)
                throw new Exception("Отсутствует подключение к БД");

            var searchDate = DbCtx.tbl_menu_selection.Max(x => x.tbl_menu.date).Date;

            if (searchDate < DateTime.Now)
                searchDate = DateTime.Now.Date;

            return searchDate;
        }

        public List<MenuViewModel> GetNextWeekMenuForUser(long userId, out string error)
        {
            error = string.Empty;

            try
            {
                if (DbCtx == null)
                    throw new Exception("Отсутствует подключение к БД");

                if (userId < 1)
                    throw new Exception("Пользователь не определен");

                var filterDate = DateTime.Now.AddDays(7).Date;

                if (!CheckIfMenuExist(filterDate, out error))
                    throw new Exception(string.Concat("Не заведено меню на следующую неделю. ", error));

                var nextWeekMenu = GetMenusFilteredByDate(filterDate, out error)
                    .Select(x => new MenuViewModel(x)).ToList();
                var startDateOfWeek = GetStartDateOfWeek(filterDate);
                var endDateOfWeek = startDateOfWeek.AddDays(6);
                var selectedNextWeekMenu = (from selectedMnu in DbCtx.tbl_menu_selection
                    where selectedMnu.tbl_menu.date <= endDateOfWeek
                          && selectedMnu.tbl_menu.date >= startDateOfWeek
                          && selectedMnu.id_people == userId
                    select selectedMnu);

                foreach (var menuvm in nextWeekMenu)
                {
                    menuvm.Selected = selectedNextWeekMenu.Any(x => x.id_menu == menuvm.id);

                    if (!menuvm.Selected)
                        continue;

                    var qty = selectedNextWeekMenu.FirstOrDefault(x => x.id_menu == menuvm.id).qty;
                    menuvm.Qty = qty ?? 1;
                }

                return nextWeekMenu;
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return null;
            }
        }

        public List<MenuViewModel> GetNextWeekMenuForUser(long userId, bool useDateFilter, DateTime filterDate,
            out string error)
        {
            error = string.Empty;

            try
            {
                if (DbCtx == null)
                    throw new Exception("Отсутствует подключение к БД");

                if (userId < 1)
                    throw new Exception("Пользователь не определен");

                if (!CheckIfMenuExist(filterDate, out error))
                    throw new Exception(string.Concat("Не заведено меню на неделю. ", error));

                List<MenuViewModel> result = null;
                IQueryable<tbl_menu_selection> selectedNextWeekMenu = null;

                if (useDateFilter)
                {
                    IEnumerable<tbl_menu> tmp = from menuItem in DbCtx.tbl_menu
                        where menuItem.date == filterDate
                        orderby menuItem.date
                        select menuItem;
                    result = tmp.Select(menuItem => new MenuViewModel(menuItem)).ToList();
                    selectedNextWeekMenu = (from selectedMnu in DbCtx.tbl_menu_selection
                        where selectedMnu.tbl_menu.date == filterDate
                              && selectedMnu.id_people == userId
                        select selectedMnu);
                }
                else
                {
                    var startWeekDay = GetStartDateOfWeek(filterDate);
                    var endWeekDay = startWeekDay.AddDays(6);
                    IEnumerable<tbl_menu> tmp = from menuItem in DbCtx.tbl_menu
                        where menuItem.date >= startWeekDay && menuItem.date <= endWeekDay
                        orderby menuItem.date
                        select menuItem;
                    result = tmp.Select(menuItem => new MenuViewModel(menuItem)).ToList();
                    selectedNextWeekMenu = (from selectedMnu in DbCtx.tbl_menu_selection
                        where selectedMnu.tbl_menu.date <= endWeekDay
                              && selectedMnu.tbl_menu.date >= startWeekDay
                              && selectedMnu.id_people == userId
                        select selectedMnu);
                }

                if (selectedNextWeekMenu == null || !selectedNextWeekMenu.Any())
                    return result;

                foreach (var menuvm in result)
                {
                    menuvm.Selected = selectedNextWeekMenu.Any(x => x.id_menu == menuvm.id);

                    if (!menuvm.Selected)
                        continue;

                    var qty = selectedNextWeekMenu.FirstOrDefault(x => x.id_menu == menuvm.id).qty;
                    menuvm.Qty = qty ?? 1;
                }

                return result;
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return null;
            }
        }

        public List<tbl_people> GetUnsignedPeople(DateTime filterDate, out string error)
        {
            error = string.Empty;

            try
            {
                if (DbCtx == null)
                    throw new Exception("Отсутствует подключение к БД");

                var startDate = GetStartDateOfWeek(filterDate).Date;
                var finishDate = startDate.AddDays(6);

                if (DateTime.Now.Date > startDate)
                    startDate = DateTime.Now.Date;

                var signedPeople = (from mnuSelection in DbCtx.tbl_menu_selection
                    where mnuSelection.tbl_menu.date >= startDate && mnuSelection.tbl_menu.date <= finishDate
                    select mnuSelection.id_people).Distinct();
                var result =
                    DbCtx.tbl_people.Where(x => (x.takes_meal ?? false) && !signedPeople.Contains(x.id)).ToList();
                return result;
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return null;
            }
        }
    }
}