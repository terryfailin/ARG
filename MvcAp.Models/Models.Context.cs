﻿//------------------------------------------------------------------------------
// <auto-generated>
//     這個程式碼是由範本產生。
//
//     對這個檔案進行手動變更可能導致您的應用程式產生未預期的行為。
//     如果重新產生程式碼，將會覆寫對這個檔案的手動變更。
// </auto-generated>
//------------------------------------------------------------------------------

namespace MvcAp.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class Entities : DbContext
    {
        public Entities()
            : base("name=Entities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<BUCatalogList> BUCatalogList { get; set; }
        public virtual DbSet<ColumnType> ColumnType { get; set; }
        public virtual DbSet<DbConfig> DbConfig { get; set; }
        public virtual DbSet<Department> Department { get; set; }
        public virtual DbSet<Event> Event { get; set; }
        public virtual DbSet<EventDropList> EventDropList { get; set; }
        public virtual DbSet<EventRegist> EventRegist { get; set; }
        public virtual DbSet<EventSign> EventSign { get; set; }
        public virtual DbSet<MenuCategory> MenuCategory { get; set; }
        public virtual DbSet<PostalData> PostalData { get; set; }
        public virtual DbSet<SystemManuFunctions> SystemManuFunctions { get; set; }
        public virtual DbSet<SystemMenu> SystemMenu { get; set; }
        public virtual DbSet<SystemRole> SystemRole { get; set; }
        public virtual DbSet<SystemRoleMenus> SystemRoleMenus { get; set; }
        public virtual DbSet<SystemUser> SystemUser { get; set; }
        public virtual DbSet<SystemUserRoles> SystemUserRoles { get; set; }
        public virtual DbSet<Agreement> Agreement { get; set; }
    }
}
