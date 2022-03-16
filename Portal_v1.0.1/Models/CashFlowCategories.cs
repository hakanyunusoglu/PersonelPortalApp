using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Portal_v1._0._1.Models
{
    public class CashFlowCategories
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public virtual List<CashFlowRelations> CashFlowRelations { get; set; }
    }
}