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
    //Db Nakit Akışı Tablosu
    public class NakitAkisi
    {
        public int Id { get; set; }
        public string ExpenseTitle { get; set; }
        public string ExpenseDetail { get; set; }
        public string ExpencePrice { get; set; }
        public DateTime ExpenseDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool Income { get; set; }
        public bool Expense { get; set; }            
        public int NakitAkisSepetId { get; set; }
        public NakitAkisSepet NakitSepet { get; set; }
    }
    //View e giden nakit akışı modeli
    public class NakitAkisiModel
    {
        public int Id { get; set; }
        public string ExpenseTitle { get; set; }
        public string ExpenseDetail { get; set; }
        public string ExpencePrice { get; set; }
        public DateTime ExpenseDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool Income { get; set; }
        public bool Expense { get; set; }
        public bool isDeleted { get; set; }
        public bool isActive { get; set; }
    }
    //View e giden sepet modeli
    public class NakitAkisGirModel
    {
        public int Id { get; set; }
        public string ExpenseTitle { get; set; }
        public string ExpenseDetail { get; set; }
        public string ExpencePrice { get; set; }
        public DateTime ExpenseDate { get; set; }
        public int Income { get; set; }
        public int Expense { get; set; }
        public string User_Id { get; set; }
    }

    public class NakitAkisSepet
    {
        public int Id { get; set; }
        public string TotalPrice { get; set; }
        public string PortalUserId { get; set; }
        public bool isDelete { get; set; }
        public bool isActive { get; set; }
        public PortalUser User { get; set; }
        public List<NakitAkisi> NakitAkislari { get; set; }       
    }
}