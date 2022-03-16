using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Portal_v1._0._1.ViewModels
{
    public class CashFlowGeneralVM
    {
        public int ID { get; set; }
        public string Title { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public string Date { get; set; }
        public string Content { get; set; }
        public double Amount { get; set; }
        public double TotalAmount { get; set; }
        public string ModifiedUser { get; set; }
        public int Unit { get; set; }
        public string CategoryName { get; set; }
        public int CategoryID { get; set; }
        public bool isActive { get; set; }
        public bool isDelete { get; set; }
        public string EventColor { get; set; }
    }
}