using System;
using System.Collections.Generic;

namespace MvcStudy.Models
{
    public partial interface IKitchenRepository
    {
        tbl_menu[] GetMenus(tbl_menu filterMenuItem, out string error);
        IEnumerable<tbl_menu> GetMenusFilteredByDate(DateTime filterDate, out string error);
        bool AddMenu(tbl_menu newMenuItem, out string error);
        bool UpdateMenu(tbl_menu menuItem, out string error);
        bool DeleteMenu(long? menuId, out string error);
        long GetTblMenuNextId();
        bool CheckIfMenuExist(DateTime filterDate, out string error);
    }
}