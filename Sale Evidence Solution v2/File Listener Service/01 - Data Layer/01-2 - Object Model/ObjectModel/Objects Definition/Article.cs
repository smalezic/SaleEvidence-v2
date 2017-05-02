using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ADS.SaleEvidence.RetailServices.ObjectModel
{
    public partial class Article
    {
        public Article()
        {
            this.Sellings = new HashSet<Selling>();
            //this.Storings = new HashSet<Storing>();
            //this.CodeBooks = new HashSet<CodeBook>();
            //this.Inventories = new HashSet<Inventory>();
            //this.DailyStoreConditions = new HashSet<DailyStoreCondition>();
        }

        [Key]
        public int Id { get; set; }
        public string Barcode { get; set; }
        public string Name { get; set; }
        public Nullable<int> ArticleCategoryId { get; set; }
        public Nullable<decimal> PDV { get; set; }

        public virtual ICollection<Selling> Sellings { get; set; }
        //public virtual ICollection<Storing> Storings { get; set; }
        //public virtual ICollection<CodeBook> CodeBooks { get; set; }
        //public virtual ArticleCategory ArticleCategory { get; set; }
        //public virtual ICollection<Inventory> Inventories { get; set; }
        //public virtual ICollection<DailyStoreCondition> DailyStoreConditions { get; set; }
    }
}
