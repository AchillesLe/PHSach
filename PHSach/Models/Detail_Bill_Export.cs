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
    
    public partial class Detail_Bill_Export
    {
        public int id { get; set; }
        public int Bill_Export_id { get; set; }
        public int Book_id { get; set; }
        public int Quantity { get; set; }
        public double Cost { get; set; }
        public double Total { get; set; }
    
        public virtual Bill_Export Bill_Export { get; set; }
        public virtual Book Book { get; set; }
    }
}
