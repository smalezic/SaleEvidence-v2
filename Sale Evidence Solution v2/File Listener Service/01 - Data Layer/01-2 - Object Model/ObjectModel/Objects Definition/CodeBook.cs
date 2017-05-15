using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ADS.SaleEvidence.RetailServices.ObjectModel
{
    public partial class CodeBook
    {
        [Key]
        public int Id { get; set; }
        public string Code { get; set; }
        public int SalePointId { get; set; }
        public int ArticleId { get; set; }
        public Nullable<decimal> SellingPrice { get; set; }
        public string Name { get; set; }
        public Nullable<int> StoreAmount { get; set; }
        public Nullable<int> MinTreshold { get; set; }
        public Nullable<int> SupplierId { get; set; }
        public Nullable<decimal> InputPrice { get; set; }

        public virtual SalePoint SalePoint { get; set; }
        public virtual Article Article { get; set; }
        //public virtual Supplier Supplier { get; set; }
    }
}
