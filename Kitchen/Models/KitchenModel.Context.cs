﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class kitchenEntities : DbContext
    {
        public kitchenEntities()
            : base("name=kitchenEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public DbSet<sysdiagram> sysdiagrams { get; set; }
        public DbSet<tbl_group> tbl_group { get; set; }
        public DbSet<tbl_meal> tbl_meal { get; set; }
        public DbSet<tbl_menu_selection> tbl_menu_selection { get; set; }
        public DbSet<tbl_weekday> tbl_weekday { get; set; }
        public DbSet<tbl_duty> tbl_duty { get; set; }
        public DbSet<tbl_menu> tbl_menu { get; set; }
        public DbSet<tbl_people> tbl_people { get; set; }
        public DbSet<tbl_settings> tbl_settings { get; set; }
    }
}
