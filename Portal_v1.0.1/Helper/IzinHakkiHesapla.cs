using System;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity;
using Portal_v1._0._1.Identity;
using Portal_v1._0._1.Models;

namespace Portal_v1._0._1.Helper
{
    public class IzinHakkiHesapla
    {
        private IdentityDataContext db = new IdentityDataContext();


        public int calismaYilHesapla(int Calismagün)
        {
            int count = 0;
            while (true)
            {
                if (Calismagün < 365)
                    break;
                else
                {
                    Calismagün = Calismagün - 365;
                    count = count + 1;
                }
            }
            return count;
        }
        public int izinHakkiHesapla(int count)
        {
            int toplamIzınHakkı = 0;
            int c1 = 0;
            while (true)
            {
                if (count == 0)
                    break;
                if (count > 5 && count < 15)
                {
                    toplamIzınHakkı = toplamIzınHakkı + (20);
                    count--;
                }
                else if (count >= 15)
                {
                    toplamIzınHakkı = toplamIzınHakkı + (26);
                    count--;
                }
                else
                {
                    toplamIzınHakkı = toplamIzınHakkı + (14);
                    count--;
                }
                if (c1 == 4)//Son 5 yıl
                    break;

                c1++;
            }
            return toplamIzınHakkı;
        }



        public double toplamIzinHakki(string id)
        {
            var user = db.Users.Single(i => i.Id == id);
            DateTime Calismadate = user.IseGiris;
            TimeSpan subt = DateTime.Now.Subtract(Calismadate);
            int Calismagün = Convert.ToInt32(subt.TotalDays);
            int count = calismaYilHesapla(Calismagün);//toplam çalıştığı yılı hesaplıyor
            var izinler = db.Izinler.Where(a => a.PortalUserId == user.Id && a.IzinTuru == "Yıllık izin").Select(i => new IzinGelen()
            {
                Id = i.Id,
                Name = i.User.Name,
                BaslangicTarihi = i.BaslangicTarihi,
                BitisTarihi = i.BitisTarihi,        //bir fonksiyon içinde hesaplama yapılabilir burdan sadece değişken gönderilir bu sayede fonksiyon if içinde ve dışında çağırılır
                LastName = i.User.LastName,
                IzinAciklama = i.IzinAciklama,
                IzinGun = i.IzinGun,
                IzinTuru = i.IzinTuru,
                Onaylandi = i.Onaylandi,
            });
            double kullanilanIzin = 0;
            int toplamIzınHakkı = izinHakkiHesapla(count);
            foreach (var item in izinler)
            {
                if (DateTime.Now.AddYears(-5) < item.BaslangicTarihi)
                {
                    kullanilanIzin = Convert.ToDouble(item.IzinGun) + kullanilanIzin; //veritabanından kullanıcının kullandığı izinlerin günlerini değişkene aktarıyor
                }
            }

            var kalan = Convert.ToDouble(toplamIzınHakkı) - kullanilanIzin;
            //(Convert.ToDouble(toplamIzınHakkı) - (kullanilanIzin + gunSayi))
            return kalan;
        }
    }
}