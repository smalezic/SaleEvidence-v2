using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ADS.SaleEvidence.RetailServices.ObjectModel
{
    public partial class Action
    {
        [Key]
        public int Id { get; set; }
        public System.DateTime InitTime { get; set; }
        public int SalePointId { get; set; }
        public Nullable<int> CardId { get; set; }
        public Nullable<bool> Calculated { get; set; }

        public virtual SalePoint SalePoint { get; set; }
        //public virtual Card Card { get; set; }

        public virtual Selling Selling { get; set; }
    }
}
