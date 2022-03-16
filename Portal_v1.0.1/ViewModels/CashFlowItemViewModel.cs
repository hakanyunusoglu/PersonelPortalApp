using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Portal_v1._0._1.ViewModels
{
    public class CashFlowItemViewModel
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public double Amount { get; set; }
        public double TotalAmount { get; set; }
        public int Unit { get; set; }
        public string ModifiedUser { get; set; }

        public CashFlowViewModel cfvm { get; set; }

    }
}