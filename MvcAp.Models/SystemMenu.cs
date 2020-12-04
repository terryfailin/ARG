//------------------------------------------------------------------------------
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
    using System.Collections.Generic;
    
    public partial class SystemMenu
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public SystemMenu()
        {
            this.SystemManuFunctions = new HashSet<SystemManuFunctions>();
            this.SystemRoleMenus = new HashSet<SystemRoleMenus>();
        }
    
        public int ID { get; set; }
        public string Name { get; set; }
        public bool IsEnable { get; set; }
        public int Sort { get; set; }
        public int CategoryID { get; set; }
        public string IconName { get; set; }
    
        public virtual MenuCategory MenuCategory { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SystemManuFunctions> SystemManuFunctions { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SystemRoleMenus> SystemRoleMenus { get; set; }
    }
}