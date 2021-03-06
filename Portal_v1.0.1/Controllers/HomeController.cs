using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Portal_v1._0._1.Identity;
using Portal_v1._0._1.Models;
using Portal_v1._0._1.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

//Portal_v1._0._1.Models.NakitAkis nktkş = new NakitAkis(); -- yeni modellerin yollarını bu şekilde veriyoruz
namespace Portal_v1._0._1.Controllers
{
    [Authorize(Roles = "User,Admin")]
    public class HomeController : Controller
    {
        private RoleManager<IdentityRole> roleManager;
        private UserManager<PortalUser> userManager;
        MailController mc = new MailController();
        public HomeController()
        {
            //registry.Schedule<MyJob>().ToRunNow().AndEvery(7).Days();
            //ResmiTatilEkle(DateTime.Now);//Resmi tarihleri her sayfa açıldıgında kontrol ediyor eğer yılın ilk günü ve ayı ise resmi tarihleri güncellliyor
            roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(new IdentityDataContext()));
            var userStore = new UserStore<PortalUser>(new IdentityDataContext());
            userManager = new UserManager<PortalUser>(userStore);
            
        }

        public ActionResult Index()
        {
            var user = userManager.Users.Single(i => i.UserName == HttpContext.User.Identity.Name);
            //user.Yonetici = userManager.Users.FirstOrDefault(i => i.Id == user.YoneticiId);
            return View(user);

        }
      
        public ActionResult Kullanicilar()
        {
            return View(userManager.Users.Where(i => i.CiktiMi == false));
        }

        public ActionResult IzinIste()//Kaç gün izin hakkı kaldığını görsün
        {
            var ToplamIzinHakki = toplamIzinHakki();
            ViewBag.izinListesi = IzinTurRepository.IzinTurListesi();
            var Uyari = "Kullanmak istediğiniz izin günü, izin hakkınızı geçiyor.İzin alırsanız bir dahaki senenin izin hakkından kullanılacak";
            if (ToplamIzinHakki <= 0)
            {
                ViewData["Warning"] = MvcHtmlString.Create("Toplam izin hakkınız : " + ToplamIzinHakki + " gün " + Uyari + "<br>" + "Başlangıç tarihi ile izin sayısını girin yada Başlangıç tarihi ile bitiş tarihini giriniz");
            }
            ViewData["Warning"] = MvcHtmlString.Create("Toplam izin hakkınız : " + ToplamIzinHakki + " gün" + "<br>" + "Başlangıç tarihi ile izin sayısını girin yada Başlangıç tarihi ile bitiş tarihini giriniz");
            return View();
        }

        public class tatiller
        {
            public double tatil_sayisi { get; set; }
            public DateTime tarih { get; set; }
        }

        public tatiller SendTatil(double tatil_sayisi, string tarih)
        {
            tatiller tatil = new tatiller();
            tatil.tarih = Convert.ToDateTime(tarih);
            tatil.tatil_sayisi = tatil_sayisi;

            return tatil;
        }
        //public List<tatiller> ResmiTatilSend()
        //{
        //    WebClient wc = new WebClient();
        //    wc.Encoding = Encoding.UTF8;
        //    string str = wc.DownloadString("http://web.archive.org/web/20180502221624/http://resmitatiller.net:80/");   

        //    var html = new HtmlDocument();
        //    html.LoadHtml(str);
        //    var root = html.DocumentNode;
        //    var get_all_tables = root.Descendants("table");
        //    var related_table = get_all_tables.First();

        //    List<List<string>> table = html.DocumentNode.SelectSingleNode("//table")
        //    .Descendants("tr")
        //    .Skip(1)
        //    .Where(tr => tr.Elements("td").Count() > 1)
        //    .Select(tr => tr.Elements("td").Select(td => td.InnerText.Trim()).ToList())
        //    .ToList();

        //    List<string> all_days = new List<string>();
        //    List<tatiller> all_days_dt = new List<tatiller>();

        //    foreach (var item in table)
        //    {
        //        if (item[2].Contains('\n'))
        //        {
        //            all_days.Add("-" + item[2].Substring(0, item[2].IndexOf('\n')));
        //            all_days.Add(item[2].Substring(item[2].IndexOf('\n') + 1, (item[2].Length - (item[2].IndexOf('\n') + 1))));
        //        }
        //        else
        //        {
        //            if (item[1].Contains("1/2"))
        //            {
        //                all_days.Add("-" + item[2]);
        //            }
        //            else
        //            {
        //                all_days.Add(item[2]);
        //            }
        //        }
        //    }

        //    for (int i = 0; i < all_days.Count; i++)
        //    {
        //        var item = all_days[i];
        //        if (item.Contains("OCAK"))
        //        {
        //            all_days[i] = item.Replace("OCAK", "01");
        //        }
        //        else if (item.Contains("ŞUBAT"))
        //        {
        //            all_days[i] = item.Replace("ŞUBAT", "02");
        //        }
        //        else if (item.Contains("MART"))
        //        {
        //            all_days[i] = item.Replace("MART", "03");
        //        }
        //        else if (item.Contains("NİSAN"))
        //        {
        //            all_days[i] = item.Replace("NİSAN", "04");
        //        }
        //        else if (item.Contains("MAYIS"))
        //        {
        //            all_days[i] = item.Replace("MAYIS", "05");
        //        }
        //        else if (item.Contains("HAZİRAN"))
        //        {
        //            all_days[i] = item.Replace("HAZİRAN", "06");
        //        }
        //        else if (item.Contains("TEMMUZ"))
        //        {
        //            all_days[i] = item.Replace("TEMMUZ", "07");
        //        }
        //        else if (item.Contains("AĞUSTOS"))
        //        {
        //            all_days[i] = item.Replace("AĞUSTOS", "08");
        //            var x = all_days[i];
        //        }
        //        else if (item.Contains("EYLÜL"))
        //        {
        //            all_days[i] = item.Replace("EYLÜL", "09");
        //        }
        //        else if (item.Contains("EKİM"))
        //        {
        //            all_days[i] = item.Replace("EKİM", "10");
        //        }
        //        else if (item.Contains("KASIM"))
        //        {
        //            all_days[i] = item.Replace("KASIM", "11");
        //        }
        //        else if (item.Contains("ARALIK"))
        //        {
        //            all_days[i] = item.Replace("ARALIK", "12");
        //        }
        //        all_days[i] = all_days[i].Replace(' ', '/').Replace("  ", string.Empty);

        //        if (all_days[i].Contains('-'))
        //        {
        //            all_days_dt.Add(SendTatil(0.5, all_days[i].Substring(1)));
        //        }
        //        else
        //        {
        //            all_days_dt.Add(SendTatil(1, all_days[i]));
        //        }


        //    }
        //    return all_days_dt;
        //}
        public int ekstraIzin(int Calismagün)
        {
            if (Calismagün > 5 && Calismagün < 15)
                return 21;
            else if (Calismagün >= 15)
                return 26;
            else
                return 14;
        }


        public double GunKontrol(DateTime dt)
        {
            var model = db.ResmiTatiller.ToList();
            foreach (var item in model)
            {
                if (dt == item.tarih)
                    return (1.0 - item.tatil_sayisi);
            }
            return 1.0;
        }

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

        public double toplamIzinHakki()
        {
            var user = db.Users.Single(i => i.UserName == HttpContext.User.Identity.Name);
            DateTime Calismadate = user.IseGiris;
            
            TimeSpan subt = DateTime.Now.Subtract(Calismadate);
            int Calismagün = Convert.ToInt32(subt.TotalDays);
            int count = calismaYilHesapla(Calismagün);//toplam çalıştığı yılı hesaplıyor
            var izinler = db.Izinler.Where(a => a.PortalUserId == user.Id && a.IzinTuru=="Yıllık izin").Select(i => new IzinGelen()
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
        public JsonResult checkDatetime(DateTime selectedDate, int num)
        {
            DateTime a = Convert.ToDateTime(selectedDate);
            a = a.AddHours(-3);
            double resmiCheck;
            int i = 0;
            while (true)
            {


                string gun = string.Empty;
                gun = a.ToString("dddd");
                if (gun != "Cumartesi" && gun != "Pazar")
                {
                    resmiCheck = GunKontrol(a);
                    if (resmiCheck == 1 || resmiCheck == 0.5)
                    {
                        a = a.AddDays(1);
                        i++;
                    }
                    else
                    {
                        a = a.AddDays(1);
                    }
                }
                else
                {
                    a = a.AddDays(1);
                }
                if (i >= num)
                {
                    a = a.AddDays(-1);
                    break;
                }

            }


            return Json("" + Convert.ToDateTime(a).ToString("yyyy-MM-dd"), JsonRequestBehavior.AllowGet);
        }
        //public void ResmiTatilEkle(DateTime BitisTarihi)
        //{
        //    bool status = false;
        //    DateTime LastItem = DateTime.Now.AddYears(-1);
        //    var model = db.ResmiTatiller.ToList();
        //    try
        //    {
        //        if (model.Count == 0)
        //            status = true;
        //        foreach (var x in model)
        //        {
        //            if (LastItem <= x.tarih)
        //            {
        //                LastItem = x.tarih;
        //            }

        //        }
        //        if (LastItem.Year != DateTime.Now.Year)
        //            status = true;
        //        if (BitisTarihi.Year != DateTime.Now.Year)
        //            status = true;
        //    }
        //    catch (Exception)
        //    {
        //        status = true;
        //    }


        //    if (status == true)
        //    {
        //        List<tatiller> resmi_tatiller = ResmiTatilSend();
        //        ResmiTatiller context = new ResmiTatiller();
        //        bool isList = false;
        //        foreach (var item in resmi_tatiller)
        //        {
        //            isList = false;
        //            context.tarih = item.tarih;
        //            context.tatil_sayisi = item.tatil_sayisi;
        //            foreach (var x in model)
        //            {
        //                if (context.tarih == x.tarih)
        //                {
        //                    isList = true;
        //                }
        //            }
        //            if (isList == false)
        //            {
        //                db.ResmiTatiller.Add(context);
        //                db.SaveChanges();
        //            }
        //        }
        //    }
        //}
        IdentityDataContext db = new IdentityDataContext();
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IzinIste(IzinModel izin)
        {
            
            //var userN = userManager.Users.Single(i => i.UserName == HttpContext.User.Identity.Name);
            //var userRole = userN.Roles.FirstOrDefault();
            //var roleName = db.Roles.Find(userRole.RoleId).Name;
            ViewBag.izinListesi = IzinTurRepository.IzinTurListesi();
            if (ModelState.IsValid)
            {
                var user = db.Users.Single(i => i.UserName == HttpContext.User.Identity.Name);
                var yeni = new IzinModel();
                yeni.IzinTuru = izin.IzinTuru; 
                yeni.IzinAciklama = izin.IzinAciklama;
                yeni.BaslangicTarihi = izin.BaslangicTarihi;
                yeni.BitisTarihi = izin.BitisTarihi;
                //ResmiTatilEkle(izin.BitisTarihi);//son yılı kontrol ediyor
                DateTime geciciTarih = izin.BaslangicTarihi;
                double gunSayi = 0;
                string gun = string.Empty;

                while (geciciTarih <= izin.BitisTarihi)//haftasonlarını geçiyor
                {
                    gun = geciciTarih.ToString("dddd");
                    if (gun != "Cumartesi" && gun != "Pazar")
                    {
                        gunSayi = gunSayi + GunKontrol(geciciTarih);
                    }
                    geciciTarih = geciciTarih.AddDays(1);
                }

         
                DateTime Calismadate = user.IseGiris;
                TimeSpan subt = DateTime.Now.Subtract(Calismadate);
                int Calismagün = Convert.ToInt32(subt.TotalDays);
                int count = calismaYilHesapla(Calismagün);//toplam çalıştığı yılı hesaplıyor


                var izinler = db.Izinler.Where(a => a.PortalUserId == user.Id).Select(i => new IzinGelen()
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
                kullanilanIzin = kullanilanIzin + gunSayi;
                if (yeni.IzinTuru == "Doğum izni")
                {
                    if (gunSayi > 5)
                    {
                        ViewData["Warning"] = "Doğum izninde 5 günden fazla izin kullanılmaz";
                        return View();
                    }
                    yeni.IzinGun = gunSayi.ToString();
                    yeni.Onaylandi = false;
                    yeni.PortalUserId = user.Id;
                    db.Izinler.Add(yeni);
                    db.SaveChanges();
                    ViewData["Success"] = "Kullandıgınız Doğum izni: " + gunSayi + " gün";
                    var mesaj = String.Format("{0} kullanıcısı {3} nedeni ile {1} ve {2} tarihleri arasında doğum izni talep etmektedir.", user.Name + " " + user.LastName, izin.BaslangicTarihi.ToLongDateString(), izin.BitisTarihi.ToLongDateString(), izin.IzinAciklama);
                    mc.MailGonderAsync(mesaj, "izin");
                }
                else if (yeni.IzinTuru == "Ölüm izni")
                {
                    if (gunSayi > 3)
                    {
                        ViewData["Warning"] = "Ölüm izninde 3 günden fazla kullanılmaz";
                        return View();
                    }
                    yeni.IzinGun = gunSayi.ToString();
                    yeni.Onaylandi = false;
                    yeni.PortalUserId = user.Id;
                    db.Izinler.Add(yeni);
                    db.SaveChanges();
                    ViewData["Success"] = "Kullandıgınız Ölüm izni: " + gunSayi + " gün";
                    var mesaj = String.Format("{0} kullanıcısı {3} nedeni ile {1} ve {2} tarihleri arasında ölüm izni talep etmektedir.", user.Name + " " + user.LastName, izin.BaslangicTarihi.ToLongDateString(), izin.BitisTarihi.ToLongDateString(), izin.IzinAciklama);
                    mc.MailGonderAsync(mesaj, "izin");
                }
                else if (yeni.IzinTuru == "Ücretli izin")
                {
                    yeni.IzinGun = gunSayi.ToString();
                    yeni.Onaylandi = false;
                    yeni.PortalUserId = user.Id;
                    db.Izinler.Add(yeni);
                    db.SaveChanges();
                    ViewData["Success"] = "Kullandıgınız Ücretli izni: " + gunSayi + " gün";
                    var mesaj = String.Format("{0} kullanıcısı {3} nedeni ile {1} ve {2} tarihleri arasında ücretli izin talep etmektedir.", user.Name + " " + user.LastName, izin.BaslangicTarihi.ToLongDateString(), izin.BitisTarihi.ToLongDateString(), izin.IzinAciklama);
                    mc.MailGonderAsync(mesaj, "izin");
                }
                else if (yeni.IzinTuru == "Evlilik izni")
                {
                    if (gunSayi > 3)
                    {
                        ViewData["Warning"] = "Evlilik izninde 3 günden fazla kullanılmaz";
                        return View();
                    }
                    yeni.IzinGun = gunSayi.ToString();
                    yeni.Onaylandi = false;
                    yeni.PortalUserId = user.Id;
                    db.Izinler.Add(yeni);
                    db.SaveChanges();
                    ViewData["Success"] = "Kullandıgınız Evlilik izni: " + gunSayi + " gün";
                    var mesaj = String.Format("{0} kullanıcısı {3} nedeni ile {1} ve {2} tarihleri arasında evlilik izni talep etmektedir.", user.Name + " " + user.LastName, izin.BaslangicTarihi.ToLongDateString(), izin.BitisTarihi.ToLongDateString(), izin.IzinAciklama);
                    mc.MailGonderAsync(mesaj, "izin");
                }
                else//Yıllık izin
                {
                    if (kullanilanIzin > toplamIzınHakkı)
                    {
                        if (kullanilanIzin > toplamIzınHakkı + ekstraIzin(count))
                        {
                            kullanilanIzin = kullanilanIzin - gunSayi;
                            ViewData["Warning"] = MvcHtmlString.Create("Izin hakkınızı geçiyor." + "<br>" + "Kullanılan izin Hakkı : " + kullanilanIzin + " gün" +"<br>" + "Almak istediginiz izin : " + gunSayi + " gün" +"<br>" + "Kalan izin hakkınız : " + (Convert.ToDouble(toplamIzınHakkı) - kullanilanIzin) + " gün");
                            return View();
                        }
                        ViewData["Warning"] = "Kullanmak istediğiniz izin günü, izin hakkınızı geçti. Bir dahaki senenin izin hakkından kullanılacak";
                        yeni.IzinGun = gunSayi.ToString();
                        yeni.Onaylandi = false;
                        yeni.PortalUserId = user.Id;
                        db.Izinler.Add(yeni);
                        db.SaveChanges();
                        kullanilanIzin = kullanilanIzin - gunSayi;
                        ViewData["Success"] = MvcHtmlString.Create("İzin başarıyla kaydedildi" + "<br>" + "Aldıgınız izin : " + gunSayi + " gün" + "<br>" + "Toplam kullanılan izinler : " + kullanilanIzin + "+" + gunSayi + " gün" + "<br>" + "Kalan izin hakkınız : " + (Convert.ToDouble(toplamIzınHakkı) - (kullanilanIzin + gunSayi)) + " gün");
                        var mesaj = String.Format("{0} kullanıcısı {3} nedeni ile {1} ve {2} tarihleri arasında yıllık izin talep etmektedir.", user.Name + " " + user.LastName, izin.BaslangicTarihi.ToLongDateString(), izin.BitisTarihi.ToLongDateString(), izin.IzinAciklama);
                        mc.MailGonderAsync(mesaj, "izin");
                    }
                    else
                    {
                        yeni.IzinGun = gunSayi.ToString();
                        yeni.Onaylandi = false;
                        yeni.PortalUserId = user.Id;
                        db.Izinler.Add(yeni);
                        db.SaveChanges();
                        kullanilanIzin = kullanilanIzin - gunSayi;
                        ViewData["Success"] = MvcHtmlString.Create("İzin başarıyla kaydedildi" + "<br>" + "Aldıgınız izin : " + gunSayi + " gün" +"<br>" + "Toplam kullanılan izinler : " + kullanilanIzin + "+" + gunSayi + " gün" + "<br>" + "Kalan izin hakkınız : " + (Convert.ToDouble(toplamIzınHakkı) - (kullanilanIzin + gunSayi)) + " gün");
                        var mesaj = String.Format("{0} kullanıcısı {3} nedeni ile {1} ve {2} tarihleri arasında yıllık izin talep etmektedir.", user.Name + " " + user.LastName, izin.BaslangicTarihi.ToLongDateString(), izin.BitisTarihi.ToLongDateString(), izin.IzinAciklama);
                        mc.MailGonderAsync(mesaj, "izin");
                    }
                }
                return View();
            }
            else
            {
                return View(izin);
            }
        }

        public ActionResult IzinGoruntule()
        {
            var user = userManager.Users.Single(i => i.UserName == HttpContext.User.Identity.Name);
            var izinler = db.Izinler.Where(a => a.PortalUserId == user.Id).Select(i => new IzinGelen()
            {
                Id = i.Id,
                Name = i.User.Name,
                BaslangicTarihi = i.BaslangicTarihi,
                BitisTarihi = i.BitisTarihi,
                LastName = i.User.LastName,
                IzinAciklama = i.IzinAciklama,
                IzinGun = i.IzinGun,
                IzinTuru = i.IzinTuru,
                Onaylandi = i.Onaylandi,
            });

            return View(izinler);
        }
        public ActionResult RaporGir()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RaporGir(RaporModel rapor, HttpPostedFileBase file)
        {
            var userN = userManager.Users.Single(i => i.UserName == HttpContext.User.Identity.Name);
            var userRole = userN.Roles.FirstOrDefault();
            var roleName = db.Roles.Find(userRole.RoleId).Name;
            if (ModelState.IsValid)
            {
                if (file != null && file.ContentLength > 0)
                {
                    var extension = Path.GetExtension(file.FileName);
                    if (extension == ".pdf" || extension == ".docx" || extension == ".doc" || extension == ".jpg" || extension == ".jpeg" || extension == ".png")
                    {
                        var folder = Server.MapPath("~/uploads/RaporPicture");
                        var randomfilename = Path.GetFileNameWithoutExtension(file.FileName) + "_" + Path.GetRandomFileName();
                        var filename = Path.ChangeExtension(randomfilename, extension);
                        var path = Path.Combine(folder, filename);
                        var user = userManager.Users.Single(i => i.UserName == HttpContext.User.Identity.Name);
                        var yeni = new RaporModel();
                        yeni.RaporAciklama = rapor.RaporAciklama;
                        yeni.BaslangicTarihi = rapor.BaslangicTarihi;
                        yeni.BitisTarihi = rapor.BitisTarihi;
                        TimeSpan gunFarki = rapor.BitisTarihi.Subtract(rapor.BaslangicTarihi);
                        yeni.RaporGun = gunFarki.ToString();
                        yeni.Onaylandi = false;
                        yeni.Resim = filename;
                        yeni.PortalUserId = user.Id;
                        db.Raporlar.Add(yeni);
                        db.SaveChanges();
                        file.SaveAs(path);

                        TempData["Success"] = "Rapor başarıyla kaydedildi";

                        var mesaj = String.Format("{0} kullanıcısı {3} nedeni ile {1} ve {2} tarihleri arasında rapor girdi", user.Name + " " + user.LastName, rapor.BaslangicTarihi.ToLongDateString(), rapor.BitisTarihi.ToLongDateString(), rapor.RaporAciklama);
                        mc.MailGonderAsync(mesaj, "izin");
                    }
                    else
                    {
                        TempData["message"] = "Gönderdiğiniz dosyanın uzantısı .pdf, .docx, .jpg, .jpeg, .png veya .doc olmalıdır";
                    }
                }
                else
                {
                    TempData["message"] = "Bir dosya seçiniz";
                }
                if (roleName == "Admin")
                {
                    return RedirectToAction("Raporlar", "Admin");
                }
                else
                {
                    return View();
                }
            }
            else
            {
                return View(rapor);
            }
        }

        public ActionResult RaporGoruntule()
        {
            var user = userManager.Users.Single(i => i.UserName == HttpContext.User.Identity.Name);
            var raporlar = db.Raporlar.Where(i => i.PortalUserId == user.Id).Select(i => new RaporGelen()
            {
                Name = i.User.Name,
                LastName = i.User.LastName,
                RaporAciklama = i.RaporAciklama,
                RaporGun = i.RaporGun,
                BaslangicTarihi = i.BaslangicTarihi,
                BitisTarihi = i.BitisTarihi,
                Onaylandi = i.Onaylandi,
                Id = i.Id
            });
            return View(raporlar);
        }

        public ActionResult UserUpdate(string id)
        {
            if (String.IsNullOrEmpty(id))
            {
                TempData["Hata"] = "Gösterilecek Kullanıcı Bulunamadı !";
                return Redirect("/Error/Page404");
            }
            var user = userManager.Users.FirstOrDefault(i => i.Id == id);
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UserUpdate(PortalUser gelen)
        {
            var user = db.Users.FirstOrDefault(i => i.Id == gelen.Id);
            if (ModelState.IsValid)
            {
                user.UserName = gelen.UserName;
                user.Name = gelen.Name;
                user.LastName = gelen.LastName;
                user.Title = gelen.Title;
                user.Email = gelen.Email;
                user.PhoneNumber = gelen.PhoneNumber;
                user.Address = gelen.Address;
                db.SaveChanges();

                TempData["Success"] = "Değişiklikler kaydedildi";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["message"] = "Bir hata oluştu";
                return View(gelen);
            }
        }

        public ActionResult MasrafGir()
        {
            var user = userManager.Users.FirstOrDefault(i => i.UserName == HttpContext.User.Identity.Name);
            var userRole = user.Roles.FirstOrDefault();
            var roleName = db.Roles.Find(userRole.RoleId).Name;
            ViewBag.UserRoleName = roleName;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MasrafSil(int? id)
        {
            
            if (id == null)
            {
                TempData["Hata"] = "Gösterilecek Kullanıcı Bulunamadı !";
                return Redirect("/Error/Page404");
            }
            var sepet = db.MasrafSepetler.FirstOrDefault(i => i.Id == id);
            if (sepet.Onaylandi==true)
            {
                return RedirectToAction("SepetGoruntule");
            }
            db.MasrafSepetler.Remove(sepet);
            db.SaveChanges();
            TempData["Success"] = "Masraf Silindi !";
            return RedirectToAction("SepetGoruntule");
        }

        [HttpPost]
        public ActionResult MasrafGir(List<MasrafGelen> sepet, string toplam)
        {
            var user = userManager.Users.FirstOrDefault(i => i.UserName == HttpContext.User.Identity.Name);
            var userRole = user.Roles.FirstOrDefault();
            var roleName = db.Roles.Find(userRole.RoleId).Name;
            ViewBag.UserRoleName = roleName;
            
            MasrafSepet yenisepet = new MasrafSepet();
            yenisepet.PortalUserId = db.Users.FirstOrDefault(i => i.UserName == user.UserName).Id;
            yenisepet.ToplamTutar = toplam;
            yenisepet.Onaylandi = false;
            db.MasrafSepetler.Add(yenisepet);
            foreach (var item in sepet)
            {
                MasrafUrun yeniurun = new MasrafUrun();
                yeniurun.Aciklama = item.Aciklama;
                yeniurun.Tarih = item.Tarih;
                yeniurun.Tutar = item.Tutar;
                yeniurun.MasrafSepetId = yenisepet.Id;
                yeniurun.Tedarikci = item.Tedarikci;
                db.MasrafUrunler.Add(yeniurun);
            }
            db.SaveChanges();
            
            var mesaj = String.Format("{0} kullanıcısı toplam {1} ₺ tutarında masraf girdi. Açıklama : {2}", user.Name + " " + user.LastName, toplam, sepet[0].Aciklama);
            mc.MailGonderAsync(mesaj, "masraf");
            return View();
        }

        public ActionResult SepetGoruntule()
        {
            var user = userManager.Users.Single(i => i.UserName == HttpContext.User.Identity.Name);
            var sepet = db.MasrafSepetler.Where(a => a.PortalUserId == user.Id).Select(i => new SepetGelen()
            {
                ToplamTutar = i.ToplamTutar,
                Onaylandi = i.Onaylandi,
                Name = i.User.Name,
                Odendi = i.Odendi,
                LastName = i.User.LastName,
                Aciklama = db.MasrafUrunler.FirstOrDefault(x => x.MasrafSepetId == i.Id).Aciklama,
                SepetId = i.Id
            }).OrderByDescending(i => i.SepetId);
            return View(sepet);
        }

        public ActionResult MasrafGoruntule(int? id)
        {
            if (id == null)
            {
                TempData["Hata"] = "Gösterilecek Kullanıcı Bulunamadı !";
                return Redirect("/Error/Page404");
            }
            var masraflar = db.MasrafUrunler.Where(i => i.MasrafSepetId == id);

            return View(masraflar);
        }

        //public ActionResult NakitAkisGir()
        //{
        //    var user = userManager.Users.FirstOrDefault(i => i.UserName == HttpContext.User.Identity.Name);
        //    var userRole = user.Roles.FirstOrDefault();
        //    var roleName = db.Roles.Find(userRole.RoleId).Name;
        //    ViewBag.UserRoleName = roleName;
        //    return View();
        //}
        //[HttpPost]
        //public ActionResult NakitAkisGir(List<NakitAkisGirModel> sepet, string toplam)
        //{
        //    var user = userManager.Users.FirstOrDefault(i => i.UserName == HttpContext.User.Identity.Name);
        //    var userRole = user.Roles.FirstOrDefault();
        //    var roleName = db.Roles.Find(userRole.RoleId).Name;
        //    ViewBag.UserRoleName = roleName;

        //    NakitAkisSepet yenisepet = new NakitAkisSepet();
        //    yenisepet.PortalUserId = db.Users.FirstOrDefault(i => i.UserName == user.UserName).Id;
        //    yenisepet.TotalPrice = toplam;
        //    yenisepet.isDelete = false;
        //    yenisepet.isActive = true;
        //    db.NakitAkisSepeti.Add(yenisepet);
        //    foreach (var item in sepet)
        //    {
        //        NakitAkisi yeniurun = new NakitAkisi();
        //        if(item.Income == 0 && item.Expense == 1)
        //        {
        //            yeniurun.Income = false;
        //            yeniurun.Expense = true;
        //        }
        //        else if(item.Income == 1 && item.Expense == 0)
        //        {
        //            yeniurun.Income = true;
        //            yeniurun.Expense = false;
        //        }
        //        yeniurun.ExpenseTitle = item.ExpenseTitle;
        //        yeniurun.ExpenseDetail = item.ExpenseDetail;
        //        yeniurun.ExpencePrice = item.ExpencePrice;
        //        yeniurun.ExpenseDate = item.ExpenseDate;
        //        yeniurun.CreatedDate = DateTime.Today;
        //        db.CashFlowStatement.Add(yeniurun);
        //    }
        //    db.SaveChanges();

        //    //var mesaj = String.Format("{0} kullanıcısı toplam {1} ₺ tutarında masraf girdi. Açıklama : {2}", user.Name + " " + user.LastName, toplam, sepet[0].Aciklama);
        //    //mc.MailGonderAsync(mesaj, "masraf");
        //    return View();
        //}
        public ActionResult BilgilendirmeEkle()
        {
            return View();
        }

        [HttpPost]
        public ActionResult BilgilendirmeEkle(BilgilendirmeModel model)
        {
            var user = userManager.Users.Single(i => i.UserName == HttpContext.User.Identity.Name);
            BilgilendirmeModel yeni = new BilgilendirmeModel();
            yeni.Description = model.Description;
            yeni.Date = model.Date;
            yeni.PortalUserId = user.Id;

            BilgilendirmeModelGenel yeniGenel = new BilgilendirmeModelGenel();
            yeniGenel.Description = model.Description;
            yeniGenel.Date = model.Date;
            yeniGenel.PortalUserId = user.Id;
            db.Bilgilendirme.Add(yeni);
            db.BilgilendirmeGenel.Add(yeniGenel);
            db.SaveChanges();
            TempData["Success"] = "Bilgilendirme kaydedildi";
            var mesaj = String.Format("{0} isimli kullanıcı {1} tarihinde {2} açıklamalı bilgilendirme eklemiştir.", user.Name + user.LastName, model.Date, model.Description);
            mc.MailGonderAsync(mesaj, "bilgilendirme");
            return View();
        }

        public ActionResult BilgilendirmeGoruntule()
        {
            var user = userManager.Users.Single(i => i.UserName == HttpContext.User.Identity.Name);
            var bilgilendirmeler = db.Bilgilendirme.Where(i => i.PortalUserId == user.Id).Select(i => new BilgilendirmeGelen()
            {
                Name = i.User.Name,
                LastName = i.User.LastName,
                BilgilendirmeAciklama = i.Description,
                Tarih = i.Date,
                Id = i.Id
            });
            return View(bilgilendirmeler);
        }


        public ActionResult CVEkle()
        {
            var user = userManager.Users.Single(i => i.UserName == HttpContext.User.Identity.Name);
            var cvler = db.CVler.Where(i => i.PortalUserId == user.Id);
            return View(cvler);
        }



        [HttpPost]
        public ActionResult CVEkle(HttpPostedFileBase file)
        {
            var user = userManager.Users.Single(i => i.UserName == HttpContext.User.Identity.Name);
            if (file != null && file.ContentLength > 0)
            {
                var extension = Path.GetExtension(file.FileName);
                if (extension == ".pdf" || extension == ".docx" || extension == ".doc")
                {
                    var folder = Server.MapPath("~/uploads/UploadCV");
                    var randomfilename = Path.GetFileNameWithoutExtension(file.FileName) + "_" + Path.GetRandomFileName();
                    var filename = Path.ChangeExtension(randomfilename, extension);
                    var path = Path.Combine(folder, filename);


                    CVModel yeni = new CVModel();
                    yeni.FileName = filename;
                    yeni.FilePath = path;
                    yeni.PortalUserId = user.Id;

                    db.CVler.Add(yeni);
                    db.SaveChanges();
                    file.SaveAs(path);

                    TempData["Success"] = "CV kaydedildi";
                }
                else
                {
                    TempData["message"] = "Gönderdiğiniz dosyanın uzantısı .pdf,.docx veya .doc olmalıdır";
                }
            }
            else
            {
                TempData["message"] = "Bir dosya seçiniz";
            }
            return RedirectToAction("CVEkle");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteCV(int? id)
        {
            if (id == null)
            {
                TempData["Hata"] = "Gösterilecek Kullanıcı Bulunamadı !";
                return Redirect("/Error/Page404");
            }

            var silinecek = db.CVler.FirstOrDefault(i => i.Id == id);
            string fullPath = Request.MapPath("~/uploads/UploadCv/" + silinecek.FileName);
            if (System.IO.File.Exists(fullPath))
            {
                System.IO.File.Delete(fullPath);
            }


            db.CVler.Remove(silinecek);
            db.SaveChanges();

            TempData["Success"] = "CV silindi !";
            return RedirectToAction("CVEkle");

        }

        public ActionResult CashFlowSchedule()
        {
            //List<CashFlowViewModel> data = new List<CashFlowViewModel>();
            //List<CashFlowItemViewModel> data2 = new List<CashFlowItemViewModel>();
            //List<CashFlowGeneralVM> cfgvm = new List<CashFlowGeneralVM>();

            List<CashFlowGeneralVM> cfgvm = new List<CashFlowGeneralVM>();

            AddEvent(cfgvm);

            var cfl = cfgvm.ToList();


            return View(cfl.OrderByDescending(x=>x.Date));
        }
        public JsonResult CashFlowEvent()
        {
            List<CashFlowGeneralVM> cfgvm = new List<CashFlowGeneralVM>();

            IdentityDataContext dbt = new IdentityDataContext();

            AddEvent(cfgvm);

            var events = cfgvm.ToList();

            return new JsonResult { Data = events, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

        }

        public void AddEvent( List<CashFlowGeneralVM> addevents)
        {
            IdentityDataContext db = new IdentityDataContext();

            List<CashFlowGeneralVM> cfgvm = new List<CashFlowGeneralVM>();

           
            foreach (var cf in db.CashFlow.ToList())
            {
                var a = cf;
                var cashFlowItem = db.CashFlowRelations.Where(x => x.CashFlows.ID == a.ID).ToList();
              
                foreach (var cfi in cashFlowItem)
                {
                    var property = db.CashFlowItem.ToList().Where(y => y.ID == cfi.CashFlowItems.ID).FirstOrDefault();
                    var getcategoryname = db.CashFlowCategories.ToList().Where(z => z.ID == cfi.CashFlowCategoriess.ID).FirstOrDefault();
                    CashFlowGeneralVM cfvm = new CashFlowGeneralVM();
                    cfvm.Date = cf.Date.ToString("MM/dd/yyyy");
                    cfvm.Title = property.ItemTitle;
                    cfvm.Content = property.ItemContent;
                    cfvm.Amount = property.Amount;
                    cfvm.TotalAmount = property.Amount * property.Unit;
                    cfvm.ID = cfi.ID;
                    cfvm.CategoryName = getcategoryname.Name;
                    cfvm.isActive = cfi.isActive;
                    cfvm.isDelete = cfi.isDelete;
                    cfvm.Unit = property.Unit;
                    if(cfi.isActive == true && cfi.isDelete == false)
                    {
                        cfvm.EventColor = "#F7F464";
                    }
                    else if(cfi.isDelete == true && cfi.isActive == true)
                    {
                        cfvm.EventColor = "#C4C4C4";
                    }
                    else if(cfi.isActive == false && cfi.isDelete == false)
                    {
                        cfvm.EventColor = "#FED7D7";
                    }
                    else if(cfi.isActive == false && cfi.isDelete == true)
                    {
                        cfvm.EventColor = "#F25536";
                    }
                    
                    cfgvm.Add(cfvm);

                    addevents.Add(cfvm);
                }                
                
            }
        }

        public JsonResult CashFlowDayEvents (string checkedDate)
        {
            IdentityDataContext dbt = new IdentityDataContext();
            List<CashFlowGeneralVM> cfgvm = new List<CashFlowGeneralVM>();
            DateTime getdate;
            getdate = DateTime.Parse(checkedDate);
            
                string chckdate = getdate.ToString("yyyy-MM-dd", DateTimeFormatInfo.InvariantInfo);
                DateTime chcksdate = DateTime.ParseExact(chckdate, "yyyy-MM-dd", CultureInfo.InvariantCulture);

                var selectedDate = db.CashFlow.Where(x => x.Date == chcksdate).FirstOrDefault();
                if (selectedDate != null)
                {
                    var cashFlowItem = db.CashFlowRelations.Where(x => x.CashFlows.ID == selectedDate.ID).ToList();

                    foreach (var cfi in cashFlowItem)
                    {
                    var property = db.CashFlowItem.ToList().Where(y => y.ID == cfi.CashFlowItems.ID).FirstOrDefault();
                    var getcategoryname = db.CashFlowCategories.ToList().Where(z => z.ID == cfi.CashFlowCategoriess.ID).FirstOrDefault();
                    CashFlowGeneralVM cfvm = new CashFlowGeneralVM();
                    cfvm.Date = getdate.ToString("MM/dd/yyyy");
                    cfvm.Title = property.ItemTitle;
                    cfvm.Content = property.ItemContent;
                    cfvm.Amount = property.Amount;
                    cfvm.TotalAmount = property.Amount * property.Unit;
                    cfvm.ID = cfi.ID;
                    cfvm.CategoryName = getcategoryname.Name;
                    cfvm.isActive = cfi.isActive;
                    cfvm.isDelete = cfi.isDelete;
                    cfvm.Unit = property.Unit;
                    cfgvm.Add(cfvm);
                    }
                }
            var data = cfgvm.ToList();

            return new JsonResult { Data = data.OrderByDescending(x => x.CategoryName), JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public ActionResult ShowCashFlowUpdateModal(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            IdentityDataContext dbt = new IdentityDataContext();
            List<CashFlowGeneralVM> cfvm = new List<CashFlowGeneralVM>();
            CashFlowGeneralVM objitem = new CashFlowGeneralVM();
            var selectedCF = dbt.CashFlowRelations.Where(x => x.ID == id).ToList();


            if (selectedCF == null)
            {
                return HttpNotFound();
            }
            else
            {
                foreach (var item in selectedCF)
                {
                    var itemprop = dbt.CashFlowItem.ToList().Where(y => y.ID == item.CashFlowItems.ID).FirstOrDefault();
                    var dateprop = dbt.CashFlow.ToList().Where(x => x.ID == item.CashFlows.ID).FirstOrDefault();
                    var catprop = dbt.CashFlowCategories.ToList().Where(z => z.ID == item.CashFlowCategoriesID).FirstOrDefault();
                    objitem.Title = itemprop.ItemTitle;
                    objitem.Content = itemprop.ItemContent;
                    objitem.Amount = itemprop.Amount;
                    objitem.Date = dateprop.Date.ToShortDateString();
                    objitem.isActive = item.isActive;
                    objitem.isDelete = item.isDelete;
                    objitem.ID = item.ID;
                    objitem.TotalAmount = itemprop.TotalAmount;
                    objitem.CategoryName = catprop.Name;
                    objitem.CategoryID = catprop.ID;
                    objitem.Unit = itemprop.Unit;

                    
                }
            }

            return PartialView("_ShowCashFlowDetails", objitem);
        }

    }
}
