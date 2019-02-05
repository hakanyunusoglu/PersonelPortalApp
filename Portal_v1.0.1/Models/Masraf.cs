using Portal_v1._0._1.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Portal_v1._0._1.Models
{
    //Db Masraf Urun tablosu
    public class MasrafUrun
    {
        public int Id { get; set; }
        public string Tedarikci { get; set; }
        public string Aciklama { get; set; }
        public string Tutar { get; set; }
        public DateTime Tarih { get; set; }

        public int MasrafSepetId { get; set; }
        public MasrafSepet Sepet { get; set; }
        
    }

    //Db Masraf Sepet tablosu
    public class MasrafSepet
    {
        public int Id { get; set; }
        public string ToplamTutar { get; set; }
        public bool Onaylandi { get; set; }
        public bool Odendi { get; set; }

        public List<MasrafUrun> Urunler { get; set; }
        public string PortalUserId { get; set; }
        public PortalUser User { get; set; }
    }
    //View e giden Masraf modeli
    public class MasrafGelen
    {
        public string Aciklama { get; set; }
        public string Tutar { get; set; }
        public DateTime Tarih { get; set; }
        public string Tedarikci { get; set; }


    }

    //View e giden sepet modeli
    public class SepetGelen
    {
        public int SepetId { get; set; }
        public string ToplamTutar { get; set; }
        public bool Onaylandi { get; set; }
        public bool Odendi { get; set; }
        public string Tedarikci { get; set; }

        public string Name { get; set; }
        public string LastName { get; set; }

        public string Aciklama { get; set; }
    }
}