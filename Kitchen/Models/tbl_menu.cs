//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MvcStudy.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class tbl_menu
    {
        public tbl_menu()
        {
            this.tbl_menu_selection = new HashSet<tbl_menu_selection>();
        }
    
        public long id { get; set; }
        public System.DateTime date { get; set; }
        public long id_meal { get; set; }
    
        public virtual tbl_meal tbl_meal { get; set; }
        public virtual ICollection<tbl_menu_selection> tbl_menu_selection { get; set; }
    }
}