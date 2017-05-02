using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ADS.SaleEvidence.RetailServices.ObjectModel
{
    public partial class Terminal
    {
        public Terminal()
        {
            //this.SIMCards = new HashSet<SIMCard>();
        }

        [Key]
        public int Id { get; set; }
        public int SalePointId { get; set; }
        public string mPOSId { get; set; }
        public string SerialNumber { get; set; }
    
        public virtual SalePoint SalePoint { get; set; }
        //public virtual ICollection<SIMCard> SIMCards { get; set; }
    }
}
