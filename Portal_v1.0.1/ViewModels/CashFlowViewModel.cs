using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Portal_v1._0._1.ViewModels
{
    public class CashFlowViewModel
    {
        public string Date { get; set; }
        public string ModifiedUser { get; set; }
        public CashFlowItemViewModel Cfivm { get; set; }
    }
}