//------------------------------------------------------------------------------
// <auto-generated>
//     這個程式碼是由範本產生。
//
//     對這個檔案進行手動變更可能導致您的應用程式產生未預期的行為。
//     如果重新產生程式碼，將會覆寫對這個檔案的手動變更。
// </auto-generated>
//------------------------------------------------------------------------------

namespace ProcCore.Business.DB0
{
    using System;
    using System.Collections.Generic;
    
    using Newtonsoft.Json;
    public partial class ProductStandard : BaseEntityTable
    {
        public int standard_id { get; set; }
        public int product_id { get; set; }
        public string item_no { get; set; }
        public string appearance { get; set; }
        public string viscosity { get; set; }
        public string soften_point { get; set; }
        public string remark { get; set; }
        public Nullable<int> sort { get; set; }
        public bool i_Hide { get; set; }
        public string i_InsertUserID { get; set; }
        public Nullable<int> i_InsertDeptID { get; set; }
        public Nullable<System.DateTime> i_InsertDateTime { get; set; }
        public string i_UpdateUserID { get; set; }
        public Nullable<int> i_UpdateDeptID { get; set; }
        public Nullable<System.DateTime> i_UpdateDateTime { get; set; }
        public string i_Lang { get; set; }
    
    	[JsonIgnore]
        public virtual Product Product { get; set; }
    }
}

