using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ADS.SaleEvidence.RetailServices.ObjectModel
{
    public partial class SalePoint
    {
        public SalePoint()
        {
            this.Terminals = new HashSet<Terminal>();
            this.Actions = new HashSet<Action>();
            //this.CodeBooks = new HashSet<CodeBook>();
            //this.SellingSummaries = new HashSet<SellingSummary>();
            //this.DailyStoreConditions = new HashSet<DailyStoreCondition>();
        }

        [Key]
        public int Id { get; set; }
        public int MerchantId { get; set; }
        public Nullable<bool> HiddenInventory { get; set; }

        //public virtual Merchant Merchant { get; set; }
        public virtual ICollection<Terminal> Terminals { get; set; }
        public virtual ICollection<Action> Actions { get; set; }
        //public virtual ICollection<CodeBook> CodeBooks { get; set; }
        //public virtual ICollection<SellingSummary> SellingSummaries { get; set; }
        //public virtual ICollection<DailyStoreCondition> DailyStoreConditions { get; set; }
    }
}
