using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Portal_v1._0._1.Models
{
    public class CashFlowItem
    {
        [Key]
        public int ID { get; set; }
        public string ItemTitle { get; set; }
        public string ItemContent { get; set; }
        public int Unit { get; set; }
        public double Amount { get; set; }
        public double TotalAmount { get; set; }
        public virtual List<CashFlowRelations> CashFlowRelations { get; set; }
    }
}