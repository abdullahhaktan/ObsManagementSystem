using ObsSistem.Models.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Migrations.Model;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;

namespace ObsSistem.Controllers
{
    public class AdminController : Controller
    {
        //Bağlantı sınıflarını oluşturdum
        SqlConnection conn = new SqlConnection("Data Source=DESKTOP-IHRMNQ9\\MSSQLSERVER01;Initial Catalog=ObsSistem;Integrated Security=True");
        SqlConnection conn2 = new SqlConnection("Data Source=DESKTOP-IHRMNQ9\\MSSQLSERVER01;Initial Catalog=ObsSistem;Integrated Security=True");
        //Sayfanın varsayılan durumda gelmesi gereken sayfa
        public ActionResult Index()
        {
            return View();
        }
        // HttpGet ile bir şey göndermeden sayfa açıldığında gelmesi gereken durum
        [HttpGet]
        public ActionResult DersEkle()
        {
            //Akademisyen model'den ve bolumModel'de birer liste oluşturdum
            List<AkademisyenModel> akademisyenListe = new List<AkademisyenModel>();
            List<bolumModel> bolumListe = new List<bolumModel>();

            string akademisyenAdSoyad = "";
            string akademisyenID = "";
            string bolumId = "";
            string bolumAd = "";

            //bağlantıyı açtım
            conn.Open();

            // Akademisyenleri çektim
            SqlCommand cmd0 = new SqlCommand("SELECT AKADEMISYEN_ID, AKADEMISYEN_AD_SOYAD FROM TBL_AKADEMISYEN", conn);
            SqlDataReader dr = cmd0.ExecuteReader();
            while (dr.Read())
            {
                //akademisyen tablosunu dolaşarak her bir akademisyeni model ile model listesine atama işlemi
                akademisyenAdSoyad = dr["AKADEMISYEN_AD_SOYAD"].ToString();
                akademisyenID = dr["AKADEMISYEN_ID"].ToString();
                AkademisyenModel akademisyen1 = new AkademisyenModel()
                {
                    AkademisyenAdSoyad = akademisyenAdSoyad,
                    AkademisyenID = akademisyenID,
                };
                akademisyenListe.Add(akademisyen1);
            }
            dr.Close();

            //bölüm tablosunu dolaşarak her bir bölümü model ile model listesine atama işlemi
            SqlCommand cmd1 = new SqlCommand("SELECT BOLUM_ID, BOLUM_AD FROM TBL_BOLUM", conn);
            SqlDataReader dr1 = cmd1.ExecuteReader();
            while (dr1.Read())
            {
                bolumId = dr1["BOLUM_ID"].ToString();
                bolumAd = dr1["BOLUM_AD"].ToString();
                bolumModel bolum = new bolumModel()
                {
                    BOLUM_ID = bolumId,
                    BOLUM_AD = bolumAd
                };
                bolumListe.Add(bolum);
            }
            dr1.Close();
            conn.Close();

            //iki modeli birleştirerek tek bir model şeklinde view'a gönderme
            myViewModel model = new myViewModel
            {
                akademisyenList = akademisyenListe,
                bolumList = bolumListe
            };

            return View("DersEkle", model);
        }

        //ders ekleme işlemi
        [HttpPost]
        public ActionResult dersekle(string dersAd, string derskod, string dersBolum, string dersAkademisyen, string dersYariyil, string dersKredi)
        {

            conn.Open();
            conn2.Open();

            SqlCommand cmd2 = new SqlCommand("INSERT INTO TBL_DERS(DERS_AD,DERS_KOD,DERS_BOLUM,DERS_AKADEMISYEN,DERS_YARI_YIL,DERS_KREDI) VALUES(@dersAd,@dersKod,@dersBolum,@dersAkademisyen,@dersYariyil,@dersKredi)", conn);

            cmd2.Parameters.AddWithValue("@dersAd", dersAd);
            cmd2.Parameters.AddWithValue("@dersKod", derskod);
            cmd2.Parameters.AddWithValue("@dersBolum", dersBolum);
            cmd2.Parameters.AddWithValue("@dersAkademisyen", dersAkademisyen);
            cmd2.Parameters.AddWithValue("@dersYariyil", dersYariyil);
            cmd2.Parameters.AddWithValue("@dersKredi", dersKredi);
            cmd2.ExecuteNonQuery();

            conn.Close();
            conn2.Close();
            return RedirectToAction("DersEkle", "Admin");
        }

        //HttpGet action'u oluşturma
        [HttpGet]
        public ActionResult dersSil()
        {
            //dersModel1 ile model list oluşturma
            List<dersModel1> dersListesi = new List<dersModel1>();
            conn.Open();
            //burada tanımladığım sp'yi çekiyorum
            string sp_name = "sp_DersleriVeIlgiliSutunlariniCekme";
            SqlCommand cmd = new SqlCommand(sp_name, conn);
            //sp şeklinde çekmek için CommandType'i tanımladım
            cmd.CommandType = CommandType.StoredProcedure;
            SqlDataReader dr = cmd.ExecuteReader();
            //read ile her bir dersi dolaşarak bunları model liste atama işlemi
            while (dr.Read())
            {
                dersModel1 model = new dersModel1()
                {
                    DERS_ID = Convert.ToInt32(dr["DERS_ID"]),
                    DERS_ADI = dr["DERS_AD"].ToString(),
                    DERS_KODU = dr["DERS_KOD"].ToString(),
                    DERSI_VEREN = dr["AKADEMISYEN_AD_SOYAD"].ToString(),
                    DERS_YARI_YIL = Convert.ToInt32(dr["DERS_YARI_YIL"]),
                    BOLUM = dr["BOLUM_AD"].ToString(),
                    KREDI = Convert.ToInt32(dr["DERS_KREDI"])
                };
                dersListesi.Add(model);
            }
            conn.Close();
            dr.Close();
            return View("DersSil", dersListesi);
        }

        [HttpPost]
        public ActionResult dersSil(string ders)
        {
            conn.Open();

            //Harf notları tablosu ile ders tablosu ilişkili olduğu için eğer ders tablosundan silinirse hata verecektir 
            //bu yüzden harf not tablosundan da silmek lazım
            SqlCommand cmd2 = new SqlCommand("DELETE FROM TBL_HARF_NOT WHERE HARF_NOT_DERS=@ders_id1", conn); ;
            cmd2.Parameters.AddWithValue("@ders_id1", ders);
            cmd2.ExecuteNonQuery();

            //ders tablosundan silme işlemi
            SqlCommand cmd1 = new SqlCommand("DELETE FROM TBL_DERS WHERE DERS_ID=@ders_id", conn);
            cmd1.Parameters.AddWithValue("@ders_id", ders);
            cmd1.ExecuteNonQuery();

            conn.Close();
            return RedirectToAction("dersSil", "Admin");//aynı controller içinde dersSil metodunda yönlendirme
        }

        [HttpGet]
        public ActionResult ogrenciEkle(string ogrenciAd)
        {
            //bolumModel'den bolum listesi oluşturma
            List<bolumModel> bolumListesi = new List<bolumModel>();
            conn.Open();
            SqlCommand cmd = new SqlCommand("SELECT BOLUM_ID,BOLUM_AD FROM TBL_BOLUM", conn);
            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                //her bir bölümü read ile dolaşarak model listesine atama işlemi
                bolumModel model = new bolumModel()
                {
                    BOLUM_AD = dr["BOLUM_AD"].ToString(),
                    BOLUM_ID = dr["BOLUM_ID"].ToString()
                };
                bolumListesi.Add(model);
            }
            return View("OgrenciEkle", bolumListesi);
        }
        [HttpPost]
        public ActionResult ogrenciEkle(string ogrenciDanisman,string ogrenciAd, string ogrenciSoyad, string ogrenciTc, string ogrenciBolum, string ogrenciNumara, string ogrenciCinsiyet, string dogumTarihi, string e_posta, string telefon_no, string ogrenciSifre, string ogrenciAdres, string baslangicYili)
        {
            // iki farklı bağlantı açtım çünkü bunları kullanırken iki farklı bağlantı lazım bir bağlantı açıkken diğeri kullanılamıyor
            conn.Open();
            conn2.Open();

            SqlCommand cmd1 = new SqlCommand("INSERT INTO TBL_OGRENCI(DANISMAN,AD,SOYAD,TC,CINSIYET,DOGUM_TARIHI,E_POSTA,TELEFON_NO,SIFRE,ADRES,BASLANGIC_YILI,BOLUM_ID,NUMARA) VALUES(@ogrenciDanisman,@ogrenci_ad,@ogrenci_soyad,@tc,@cinsiyet,@dogum_tarihi,@e_posta,@telefon_no,@sifre,@adres,@baslangic_yili,@bolum_id,@ogrenci_numara)", conn);
            cmd1.Parameters.AddWithValue("@ogrenci_ad", ogrenciAd);
            cmd1.Parameters.AddWithValue("@ogrenci_soyad", ogrenciSoyad);
            cmd1.Parameters.AddWithValue("@tc", ogrenciTc);
            cmd1.Parameters.AddWithValue("@cinsiyet", ogrenciCinsiyet);
            cmd1.Parameters.AddWithValue("@dogum_tarihi", dogumTarihi);
            cmd1.Parameters.AddWithValue("@e_posta", e_posta);
            cmd1.Parameters.AddWithValue("@telefon_no", telefon_no);
            cmd1.Parameters.AddWithValue("@sifre", ogrenciSifre);
            cmd1.Parameters.AddWithValue("@baslangic_yili", baslangicYili);
            cmd1.Parameters.AddWithValue("@bolum_id", ogrenciBolum);
            cmd1.Parameters.AddWithValue("@adres", ogrenciAdres);
            cmd1.Parameters.AddWithValue("@ogrenci_numara", ogrenciNumara);
            cmd1.Parameters.AddWithValue("@ogrenciDanisman", ogrenciDanisman);
            cmd1.ExecuteNonQuery();

            SqlCommand cmd2 = new SqlCommand("SELECT * FROM TBL_DERS WHERE DERS_BOLUM=@bolumId", conn);
            cmd2.Parameters.AddWithValue("@bolumId", ogrenciBolum);
            SqlDataReader dr = cmd2.ExecuteReader();
            while (dr.Read())
            {
                /*ders tablosunu dolaşarak her birini çekiyorum zira bir öğrenci eklendiğinde örneğin
                 bilgisayar mühendisliğine bir öğrenci ekleyelim bilgisayar mühendisliğindeki tüm
                derslerin öğrenciye tanımlanması gerek ders tablosuyla harf not tablosu arasında inner join
                olduğu için harf not tablosuna öğrenciye eklenen derslerin ve öğrencinin numarasının eklenmesi gerek*/
                string harfNotDers = dr["DERS_ID"].ToString();
                string harfNot = "";
                SqlCommand cmd4 = new SqlCommand("INSERT INTO TBL_HARF_NOT(OGRENCI_NO,HARF_NOT,HARF_NOT_DERS,HARF_NOT_BOLUM) VALUES (@ogrenciNo,@harfNot,@harfNotDers,@harfNotBolum)", conn2);
                cmd4.Parameters.AddWithValue("@ogrenciNo", ogrenciNumara);
                cmd4.Parameters.AddWithValue("@harfNot", harfNot);
                cmd4.Parameters.AddWithValue("@harfNotDers", harfNotDers);
                cmd4.Parameters.AddWithValue("@harfNotBolum", ogrenciBolum);
                cmd4.ExecuteNonQuery();
            }
            dr.Close();

            conn.Close();
            conn2.Close();
            // get actionuna geri dönme
            return RedirectToAction("ogrenciEkle", "Admin");
        }
        [HttpGet]
        public ActionResult ogrenciSil()
        {
            //ogrenciModel modelinden bir mdoel list oluşturma
            List<ogrenciModel> ogrenciList = new List<ogrenciModel>();
            //bağlantının açılması
            conn.Open();
            //sp tanımlama
            string sp_name = "sp_OgrenciListesiÇekme";
            // sp yi çekme
            SqlCommand cmd = new SqlCommand(sp_name, conn);
            cmd.CommandType = CommandType.StoredProcedure;
            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                ogrenciModel model = new ogrenciModel()
                {
                    BOLUM = dr["BOLUM_AD"].ToString(),
                    NUMARA = Convert.ToInt32(dr["NUMARA"]),
                    TC = dr["TC"].ToString(),
                    AD = dr["AD"].ToString(),
                    SOYAD = dr["SOYAD"].ToString(),
                    CINSIYET = dr["CINSIYET"].ToString(),
                    E_POSTA = dr["E_POSTA"].ToString(),
                    TELEFON_NO = dr["TELEFON_NO"].ToString(),
                    SIFRE = dr["SIFRE"].ToString(),
                    ADRES = dr["ADRES"].ToString(),
                };
                ogrenciList.Add(model);
            }
            conn.Close();
            dr.Close();
            return View("OgrenciSil", ogrenciList);
        }

        // öğrenci sil post işlemi
        [HttpPost]
        public ActionResult ogrenciSil(string ogrenciNumara)
        {
            //bağlantının açılması
            conn.Open();
            //öğrenci tablosu ile sınav tablosu ilişikili olduğundan öğrenci silindiğinde sınav tablosundan da silinme işlemi gerçekleşecek
            SqlCommand cmd1 = new SqlCommand("DELETE FROM TBL_SINAV WHERE OGRENCI_ID=@ogrenciNo", conn);
            cmd1.Parameters.AddWithValue("@ogrenciNo", ogrenciNumara);
            cmd1.ExecuteNonQuery();
            //öğrenci tablosundan silme işlem
            SqlCommand cmd = new SqlCommand("DELETE FROM TBL_OGRENCI WHERE NUMARA=@ogrenci_numara", conn);
            cmd.Parameters.AddWithValue("@ogrenci_numara", ogrenciNumara);
            cmd.ExecuteNonQuery();
            conn.Close();
            return RedirectToAction("OgrenciSil");
        }

        //Akademisyen ekleme get işlemi
        [HttpGet]
        public ActionResult akademisyenEkle()
        {
            //bağlantının açılması
            conn.Open();
            //bolumModel'den model listesi oluşturma
            List<bolumModel> bolumListesi = new List<bolumModel>();
            SqlCommand cmd1 = new SqlCommand("SELECT BOLUM_ID,BOLUM_AD FROM TBL_BOLUM", conn);
            SqlDataReader dr = cmd1.ExecuteReader();
            while (dr.Read())
            {
                //her bir bölümü model listesine atama
                bolumModel model = new bolumModel()
                {
                    BOLUM_AD = dr["BOLUM_AD"].ToString(),
                    BOLUM_ID = dr["BOLUM_ID"].ToString()
                };
                bolumListesi.Add(model);
            }
            dr.Close();
            conn.Close();
            return View("akademisyenEkle", bolumListesi);
        }

        //akademisyen post ekleme işlemi
        [HttpPost]
        public ActionResult akademisyenEkle(string AkademisyenAdSoyad, string AkademisyenTC, string AkademisyenSifre, int AkademisyenDanismanlikDurumu, string AkademisyenBolum)
        {
            //bağlantının açılması
            conn.Open();
            //akademisyen tablosuna verilerin eklenmesi
            SqlCommand cmd = new SqlCommand("INSERT INTO TBL_AKADEMISYEN(AKADEMISYEN_DANISMANLIK_DURUMU,AKADEMISYEN_AD_SOYAD,AKADEMISYEN_TC,AKADEMISYEN_SIFRE) VALUES(@akademisyenDanismanlik,@akademisyen_ad_soyad,@akademisyen_tc,@akademisyen_sifre)", conn);
            cmd.Parameters.AddWithValue("@akademisyen_ad_soyad", AkademisyenAdSoyad);
            cmd.Parameters.AddWithValue("@akademisyen_sifre", AkademisyenSifre);
            cmd.Parameters.AddWithValue("@akademisyen_TC", AkademisyenTC);
            cmd.Parameters.AddWithValue("@akademisyenDanismanlik", AkademisyenDanismanlikDurumu);
            cmd.ExecuteNonQuery();
            //bağlantıyı kapama
            conn.Close();
            return RedirectToAction("akademisyenEkle", "Admin");
        }

        [HttpGet]
        public ActionResult akademisyenSil()
        {
            //bağlanntı açma
            conn.Open();
            List<AkademisyenModel> akademisyenList = new List<AkademisyenModel>();
            SqlCommand cmd = new SqlCommand("SELECT * FROM TBL_AKADEMISYEN", conn);
            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                //akdemisyen model listesine akamdesiyenlerin eklenmesi
                AkademisyenModel akademisyenModel = new AkademisyenModel()
                {
                    AkademisyenID = dr["AKADEMISYEN_ID"].ToString(),
                    AkademisyenAdSoyad = dr["AKADEMISYEN_AD_SOYAD"].ToString(),
                    AkademisyenTC = dr["AKADEMISYEN_TC"].ToString(),
                    AkademisyenSifre = dr["AKADEMISYEN_SIFRE"].ToString(),
                    AkademisyenDanismanlikDurumu = Convert.ToInt32(dr["AKADEMISYEN_DANISMANLIK_DURUMU"])
                };
                akademisyenList.Add(akademisyenModel);
            }
            conn.Close();
            return View("AkademisyenSil", akademisyenList);
        }
        [HttpPost]
        public ActionResult akademisyenSil(string akademisyenID)
        {
            conn.Open();
            conn2.Open();
            // TBL_DERS tablosundaki kayıtları al
            SqlCommand cmd3 = new SqlCommand("SELECT DERS_ID FROM TBL_DERS WHERE DERS_AKADEMISYEN=@dersAkademisyen", conn);
            cmd3.Parameters.AddWithValue("@dersAkademisyen", akademisyenID);
            SqlDataReader dr = cmd3.ExecuteReader();

            // TBL_HARF_NOT tablosundan ilgili kayıtları sil
            while (dr.Read())
            {
                string ders_id = dr["DERS_ID"].ToString();
                SqlCommand cmd2 = new SqlCommand("DELETE FROM TBL_HARF_NOT WHERE HARF_NOT_DERS=@ders_id1", conn2);
                cmd2.Parameters.AddWithValue("@ders_id1", ders_id);
                cmd2.ExecuteNonQuery();
            }
            dr.Close();

            // TBL_DERS ve TBL_AKADEMISYEN tablolarındaki kayıtları sil
            SqlCommand cmd1 = new SqlCommand("DELETE FROM TBL_DERS WHERE DERS_AKADEMISYEN = @akademisyenID", conn);
            cmd1.Parameters.AddWithValue("@akademisyenID", akademisyenID);
            cmd1.ExecuteNonQuery();

            SqlCommand cmd = new SqlCommand("DELETE FROM TBL_AKADEMISYEN WHERE AKADEMISYEN_ID=@akademisyenID", conn);
            cmd.Parameters.AddWithValue("@akademisyenID", akademisyenID);
            cmd.ExecuteNonQuery();

            conn.Close();
            conn2.Close();
            return RedirectToAction("AkademisyenSil");
        }

        //bölüm ekeleme için get action
        [HttpGet]
        public ActionResult bolumEkle()
        {
            return View("BolumEkle");
        }

        //bölüm ekeleme için post action
        [HttpPost]
        public ActionResult bolumEkle(string bolumAd)
        {
            //bağlantı açma
            conn.Open();
            SqlCommand cmd = new SqlCommand("INSERT INTO TBL_BOLUM(BOLUM_AD) VALUES(@bolumAD)", conn);
            cmd.Parameters.AddWithValue("@bolumAD", bolumAd);
            cmd.ExecuteNonQuery();
            //bağlantı kapama
            conn.Close();
            return RedirectToAction("bolumEkle");
        }

        //bolum silme için get action
        [HttpGet]
        public ActionResult bolumSil()
        {
            //bolumModel ile liste oluşturma
            List<bolumModel> bolumList = new List<bolumModel>();
            conn.Open();
            SqlCommand cmd = new SqlCommand("SELECT * FROM TBL_BOLUM", conn);
            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                //her bir bölümü listeye atama işlemi
                bolumModel bolum = new bolumModel
                {
                    BOLUM_ID = dr["BOLUM_ID"].ToString(),
                    BOLUM_AD = dr["BOLUM_AD"].ToString()
                };
                bolumList.Add(bolum);
            }
            dr.Close();
            return View("BolumSil", bolumList);
        }

        //bölüm silme post işlemi
        [HttpPost]
        public ActionResult bolumSil(string bolumID)
        {
            //bolum model'inden bir liste oluşturma
            List<bolumModel> bolumList = new List<bolumModel>();
            //bağlantıyı açma 
            conn.Open();
            SqlCommand cmd = new SqlCommand("SELECT * FROM TBL_BOLUM", conn);
            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                //her bir bolümü listeye atama işlemi
                bolumModel bolum = new bolumModel
                {
                    BOLUM_ID = dr["BOLUM_ID"].ToString(),
                    BOLUM_AD = dr["BOLUM_AD"].ToString()
                };
                bolumList.Add(bolum);
            }
            dr.Close();

            //tablolar arasında ilişkiler olduğu için silmemiz öncelikle farklı tablolarda silmemiz gereken veriler var
            SqlCommand cmd2 = new SqlCommand("DELETE FROM TBL_DERS WHERE DERS_BOLUM=@bolumId", conn);
            cmd2.Parameters.AddWithValue("@bolumId", bolumID);
            cmd2.ExecuteNonQuery();

            SqlCommand cmd1 = new SqlCommand("DELETE FROM TBL_BOLUM WHERE BOLUM_ID=@bolumID", conn);
            cmd1.Parameters.AddWithValue("@bolumID", bolumID);
            cmd1.ExecuteNonQuery();

            SqlCommand cmd3 = new SqlCommand("DELETE FROM TBL_OGRENCI WHERE BOLUM_ID=@bolumID", conn);
            cmd3.Parameters.AddWithValue("@bolumID", bolumID);
            cmd3.ExecuteNonQuery();

            conn.Close();
            return RedirectToAction("BolumSil", bolumList);
        }

        //talepleri görme get işlemi
        [HttpGet]
        public ActionResult talepleriGor()
        {
            //bağlantı açma
            conn.Open();
            //talep model listesi oluşturma
            List<talepModel> talepList = new List<talepModel>();
            //view çağırma işlemi
            SqlCommand cmd = new SqlCommand("SELECT * FROM vw_OgrenciNoVeDersIdSineGoreEslesecekSekildeGetir", conn);
            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                //eğer talebin durumu 0 ise talepleri her bir talebi alma işlemi
                if (dr["TALEP_DURUM"].ToString() == "0")
                {
                    talepModel talep = new talepModel
                    {
                        gonderenId = dr["NUMARA"].ToString(),
                        gonderenAd = dr["AD"].ToString(),
                        gonderenSoyad = dr["SOYAD"].ToString(),
                        durum = Convert.ToInt32(dr["TALEP_DURUM"]),
                        konu = dr["TALEP_KONU"].ToString(),
                        mesaj = dr["TALEP_MESAJ"].ToString(),
                        talepId = dr["TALEP_ID"].ToString()
                    };
                    talepList.Add(talep);
                }
            }
            dr.Close();
            conn.Close();
            return View("AdminTalepler", talepList);
        }

        //talepleri görme post işlemi
        [HttpPost]
        public ActionResult talepleriGor(string yanit,string talepId)
        {
            int durum = 1;
            conn.Open();
            SqlCommand cmd = new SqlCommand("UPDATE TBL_TALEP SET TALEP_YANIT=@yanit , TALEP_DURUM=@durum WHERE TALEP_ID=@talepId", conn);
            cmd.Parameters.AddWithValue("@yanit", yanit);
            cmd.Parameters.AddWithValue("@talepId", talepId);
            cmd.Parameters.AddWithValue("@durum", durum);
            cmd.ExecuteNonQuery();
            conn.Close();

            return RedirectToAction("talepleriGor", "Admin");
        }


        [HttpGet]
        public ActionResult kimlikKartTalepleriniGor()
        {
            // Veritabanı bağlantısı açılıyor.
            conn.Open();

            // Kimlik kartı taleplerini tutmak için boş bir liste oluşturuluyor.
            List<KimlikKartTalepEtModel> kimlikKartTalepList = new List<KimlikKartTalepEtModel>();

            // SQL sorgusu ile TBL_KIMLIK_KART_TALEP tablosundan veriler okunuyor.
            SqlCommand cmd = new SqlCommand("SELECT * FROM TBL_KIMLIK_KART_TALEP", conn);

            // SqlDataReader nesnesi ile veriler okunuyor.
            SqlDataReader dr = cmd.ExecuteReader();

            // Veriler döngü içinde okunarak KimlikKartTalepEtModel nesneleri oluşturulup listeye ekleniyor.
            while (dr.Read())
            {
                KimlikKartTalepEtModel kimlik = new KimlikKartTalepEtModel
                {
                    talepId = dr["KIMLIK_KART_TALEP_ID"].ToString(),
                    OgrenciNo = dr["OGRENCI_NO"].ToString()
                };
                kimlikKartTalepList.Add(kimlik);
            }

            // SqlDataReader ve bağlantı kapatılıyor.
            dr.Close();
            conn.Close();

            // Oluşturulan kimlik kartı talepleri listesi view'e gönderilerek ilgili sayfa görüntüleniyor.
            return View("KimlikKartTalepleri", kimlikKartTalepList);
        }

        [HttpPost]
        public ActionResult kimlikKartTalepleriniGor(string ogrenciNo)
        {
            // Veritabanı bağlantısı açılıyor.
            conn.Open();

            // SQL sorgusu ile TBL_KIMLIK_KART_TALEP tablosundan belirli bir öğrenciye ait talepler siliniyor.
            SqlCommand cmd = new SqlCommand("DELETE FROM TBL_KIMLIK_KART_TALEP WHERE OGRENCI_NO=@ogrenciNo", conn);
            cmd.Parameters.AddWithValue("@ogrenciNo", ogrenciNo);
            cmd.ExecuteNonQuery();

            // Bağlantı kapatılıyor.
            conn.Close();

            // İşlem tamamlandıktan sonra ilgili sayfaya yönlendiriliyor.
            return RedirectToAction("kimlikKartTalepleriniGor", "Admin");
        }

        [HttpGet]
        public ActionResult belgeTalepleriGor()
        {
            // Belge taleplerini tutmak için boş bir liste oluşturuluyor.
            List<belgeTalepModel> belgeTalepList = new List<belgeTalepModel>();

            // Veritabanı bağlantısı açılıyor.
            conn.Open();

            // SQL sorgusu ile TBL_BELGE_TALEP tablosundan veriler okunuyor.
            SqlCommand cmd = new SqlCommand("SELECT * FROM TBL_BELGE_TALEP", conn);

            // SqlDataReader nesnesi ile veriler okunuyor.
            SqlDataReader dr = cmd.ExecuteReader();

            // Veriler döngü içinde okunarak belgeTalepModel nesneleri oluşturulup listeye ekleniyor.
            while (dr.Read())
            {
                belgeTalepModel belgeTalep = new belgeTalepModel()
                {
                    belgeTalepId = Convert.ToInt32(dr["BELGE_TALEP_ID"]),
                    belgeTalepOgrenciNo = dr["BELGE_OGRENCI_NO"].ToString(),
                    belgeTalepTur = dr["BELGE_TUR"].ToString()
                };
                belgeTalepList.Add(belgeTalep);
            }

            // SqlDataReader ve bağlantı kapatılıyor.
            conn.Close();
            dr.Close();

            // Oluşturulan belge talepleri listesi view'e gönderilerek ilgili sayfa görüntüleniyor.
            return View("BelgeTalepleriGor", belgeTalepList);
        }

        [HttpPost]
        public ActionResult belgeTalepleriGor(string ogrenciNo)
        {
            // Veritabanı bağlantısı açılıyor.
            conn.Open();

            // SQL sorgusu ile TBL_BELGE_TALEP tablosundan belirli bir öğrenciye ait talepler siliniyor.
            SqlCommand cmd = new SqlCommand("DELETE FROM TBL_BELGE_TALEP WHERE BELGE_OGRENCI_NO=@ogrenciNo", conn);
            cmd.Parameters.AddWithValue("@ogrenciNo", ogrenciNo);
            cmd.ExecuteNonQuery();

            // Bağlantı kapatılıyor.
            conn.Close();

            // İşlem tamamlandıktan sonra ilgili sayfaya yönlendiriliyor.
            return RedirectToAction("belgeTalepleriGor", "Admin");
        }
    }
}

