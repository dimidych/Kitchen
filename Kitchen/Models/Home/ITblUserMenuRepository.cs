using System;
using System.Collections.Generic;
using System.Linq;
using MvcStudy.Models.Home;
using MvcStudy.Models.Menu;

namespace MvcStudy.Models
{
    public partial interface IKitchenRepository : IBaseRepository
    {
        IQueryable<MenuSelectionGrouppedByPeople> GetMenuSelection(bool forFutureWeek, out string error);
        Dictionary<tbl_menu, int> GetSelectedMealCount(bool forFutureWeek, out string error);
        tbl_menu_selection[] GetMenuSelection(tbl_menu_selection selectedMenu, out string error);
        bool AddMenuSelection(tbl_menu_selection newSelectedMenu, out string error);
        bool UpdateMenuSelection(tbl_menu_selection selectedMenu, out string error);
        bool DeleteMenuSelection(long? selectedMenuId, out string error);
        long GetTblMenuSelectionNextId();
        DateTime GetSelectedMenuMaxDate();
        List<MenuViewModel> GetNextWeekMenuForUser(long userId, out string error);
        List<MenuViewModel> GetNextWeekMenuForUser(long userId, bool useDateFilter, DateTime filterDate,
            out string error);
        List<tbl_people> GetUnsignedPeople(DateTime filterDate, out string error);
    }
}