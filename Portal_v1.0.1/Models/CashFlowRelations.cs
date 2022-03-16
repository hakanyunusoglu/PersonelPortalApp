using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Portal_v1._0._1.Models
{
    public class CashFlowRelations
    {
        [Key]
        public int ID { get; set; }
        public string ModifiedUser { get; set; }
        public int CashFlowID { get; set; }
        public int CashFlowItemsID { get; set; }
        public int CashFlowCategoriesID { get; set; }
        public bool isActive { get; set; }
        public bool isDelete { get; set; }
        public CashFlow CashFlows {get;set;}
        public CashFlowItem CashFlowItems { get; set; }
        public CashFlowCategories CashFlowCategoriess { get; set; }


    }
}