using Portal_v1._0._1.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Portal_v1._0._1.Models
{
    public class BilgilendirmeModel
    {
        public int Id { get; set; }
        public PortalUser User { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public string PortalUserId { get; set; }
    }

    public class BilgilendirmeGelen
    {
        public int Id { get; set; }
        public string BilgilendirmeAciklama { get; set; }
        public DateTime Tarih { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
    }
}