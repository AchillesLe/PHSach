//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace PHSach.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Inventory_Agency
    {
        public int id { get; set; }
        public int Agency_id { get; set; }
        public int Book_id { get; set; }
        public Nullable<int> deliver_quantity { get; set; }
        public Nullable<int> repay_quantity { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
    
        public virtual Agency Agency { get; set; }
        public virtual Book Book { get; set; }
    }
}
