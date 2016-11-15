using System.Linq;

namespace MvcStudy.Models.Home
{
    public class MenuSelectionGrouppedByPeople
    {
        public tbl_people User { get; set; }

        public IGrouping<tbl_people, tbl_menu_selection> MenuSelectionGroup { get; set; }

        public MenuSelectionGrouppedByPeople()
        {}

        public MenuSelectionGrouppedByPeople(tbl_people user,
            IGrouping<tbl_people, tbl_menu_selection> menuSelectionGroup)
        {
            User = user;
            MenuSelectionGroup = menuSelectionGroup;
        }
    }
}