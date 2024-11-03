using ObsSistem.Models.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ObsSistem.Controllers
{
    public class Personal_informationController : Controller
    {
        // GET: Personal_information
        public ActionResult Index()
        {
            return View();
        }
        SqlConnection conn = new SqlConnection("Data Source=DESKTOP-IHRMNQ9\\MSSQLSERVER01;Initial Catalog=ObsSistem;Integrated Security=True");

        [HttpGet]
        public ActionResult SifreDegistirme()
        {
            // Şifre değiştirme sayfasını görüntüleme
            return View();
        }

        [HttpPost]
        public ActionResult SifreDegistirme(string eskiSifre, string yeniSifre, string yeniSifreTekrar)
        {
            if (yeniSifre == yeniSifreTekrar)
            {
                SqlConnection connection = new SqlConnection("Data Source=DESKTOP-IHRMNQ9\\MSSQLSERVER01;Initial Catalog=ObsSistem;Integrated Security=True");
                connection.Open();

                // SQL sorgusuyla önceki şifreyi kontrol etme
                SqlCommand cmd = new SqlCommand("UPDATE TBL_OGRENCI SET SIFRE=@yeniSifre where SIFRE=@eskiSifre", connection);
                cmd.Parameters.AddWithValue("@yeniSifre", yeniSifre);
                cmd.Parameters.AddWithValue("@eskiSifre", eskiSifre);
                cmd.ExecuteNonQuery();
                connection.Close();

                // Şifre değiştirildikten sonra ilgili sayfaya yönlendirme
                return View("SifreDegistirme");
            }
            else
            {
                // Şifreler uyuşmuyorsa anasayfaya yönlendirme
                return View("Index");
            }
        }

        public ActionResult ozlukBilgileriGetir()
        {
            SqlConnection connection = new SqlConnection("Data Source=DESKTOP-IHRMNQ9\\MSSQLSERVER01;Initial Catalog=ObsSistem;Integrated Security=True");
            connection.Open();
            string ogrenciNumara = Session["ogrenciNumara"].ToString();
            List<ogrenciModel> ozlukBilgileriList = new List<ogrenciModel>();

            // Öğrenci bilgilerini getiren SQL sorgusu
            SqlCommand cmd = new SqlCommand("SELECT * FROM TBL_OGRENCI INNER JOIN TBL_BOLUM ON TBL_BOLUM.BOLUM_ID=TBL_OGRENCI.BOLUM_ID WHERE NUMARA=@numara", connection);
            cmd.Parameters.AddWithValue("@numara", ogrenciNumara);
            SqlDataReader dr = cmd.ExecuteReader();
            if (dr.Read())
            {
                // Öğrenci bilgilerini alıp modele ekleme
                ogrenciModel ogrenci = new ogrenciModel
                {
                    TC = dr["TC"].ToString(),
                    NUMARA = Convert.ToInt32(dr["NUMARA"]),
                    AD = dr["AD"].ToString(),
                    SOYAD = dr["SOYAD"].ToString(),
                    CINSIYET = dr["CINSIYET"].ToString(),
                    DOGUM_TARIHI = dr["DOGUM_TARIHI"].ToString(),
                    E_POSTA = dr["E_POSTA"].ToString(),
                    TELEFON_NO = dr["TELEFON_NO"].ToString(),
                    SIFRE = dr["SIFRE"].ToString(),
                    ADRES = dr["ADRES"].ToString(),
                    BASLANGIC_YILI = dr["BASLANGIC_YILI"].ToString(),
                    BOLUM = dr["BOLUM_AD"].ToString(),
                    DANISMAN = dr["DANISMAN"].ToString()
                };
                ozlukBilgileriList.Add(ogrenci);
                dr.Close();
            }
            connection.Close();
            dr.Close();

            // Öğrenci bilgilerini görüntüleme sayfasına yönlendirme
            return View("ozlukBilgileriGetir", ozlukBilgileriList);
        }

        // İş Deneyimleri Getirme
        public ActionResult isDeneyimleri()
        {
            SqlConnection connection = new SqlConnection("Data Source=DESKTOP-IHRMNQ9\\MSSQLSERVER01;Initial Catalog=ObsSistem;Integrated Security=True");
            connection.Open();

            string ogrenciNumara = Session["OgrenciNumara"].ToString();

            string SirketAdi1 = "";
            string pozisyon1 = "";
            string baslangicTarihi1 = "";
            string bitisTarihi1 = "";
            string isTanimi1 = "";

            List<Deneyim> isDeneyimleri = new List<Deneyim>();

            SqlCommand cmd = new SqlCommand("Select * from TBL_IS_DENEYIMLERI where ogrenciNumara=@numara", connection);
            cmd.Parameters.AddWithValue("@numara", ogrenciNumara);
            SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                SirketAdi1 = dr["SirketAdi"].ToString();
                pozisyon1 = dr["pozisyon"].ToString();
                baslangicTarihi1 = dr["baslangicTarihi"].ToString();
                bitisTarihi1 = dr["bitisTarihi"].ToString();
                isTanimi1 = dr["isTanimi"].ToString();

                Deneyim isDeneyimi = new Deneyim()
                {
                    SirketAdi = SirketAdi1,
                    Pozisyon = pozisyon1,
                    BaslangicTarihi = baslangicTarihi1,
                    BitisTarihi = bitisTarihi1,
                    IsTanimi = isTanimi1
                };
                isDeneyimleri.Add(isDeneyimi);
            }
            dr.Close();

            connection.Close();
            return View("isDeneyimleri", isDeneyimleri);
        }

        // İş Deneyimi Ekleme
        [HttpGet]
        public ActionResult isDeneyimiEkle()
        {
            return View();
        }

        [HttpPost]
        public ActionResult isDeneyimiEkle(string companyName, string position, string startDate, string endDate, string jobDescription)
        {
            SqlConnection connection = new SqlConnection("Data Source=DESKTOP-IHRMNQ9\\MSSQLSERVER01;Initial Catalog=ObsSistem;Integrated Security=True");
            connection.Open();
            string ogrenciNumara = "";
            ogrenciNumara = Session["ogrenciNumara"].ToString();
            SqlCommand cmd = new SqlCommand("INSERT INTO TBL_IS_DENEYIMLERI(SirketAdi,Pozisyon,baslangicTarihi,bitisTarihi,isTanimi,ogrenciNumara) VALUES (@sirketAdi,@pozisyon,@baslangicTarihi,@bitisTarihi,@isTanimi,@ogrenciNumara) ", connection);
            cmd.Parameters.AddWithValue("@sirketAdi", companyName);
            cmd.Parameters.AddWithValue("@pozisyon", position);
            cmd.Parameters.AddWithValue("@baslangicTarihi", startDate);
            cmd.Parameters.AddWithValue("@bitisTarihi", endDate);
            cmd.Parameters.AddWithValue("@isTanimi", jobDescription);
            cmd.Parameters.AddWithValue("@ogrenciNumara", ogrenciNumara);
            cmd.ExecuteNonQuery();
            connection.Close();

            return View("isDeneyimiEkle");
        }

        // Ders Alma Listesi
        [HttpGet]
        public ActionResult dersAlmaListesi()
        {
            string ogrenciNumara = "";
            ogrenciNumara = Session["ogrenciNumara"].ToString();
            List<dersAlmaModel> dersAlmaList = new List<dersAlmaModel>();
            conn.Open();
            SqlCommand cmd = new SqlCommand("SELECT * FROM TBL_DERS_ALMA WHERE OGRENCI_NUMARA=@ogrenciNo AND DURUM=@durum", conn);
            cmd.Parameters.AddWithValue("@durum", 1);
            cmd.Parameters.AddWithValue("@ogrenciNo", ogrenciNumara);
            SqlDataReader dr = cmd.ExecuteReader();
            if (dr.Read())
            {
                dersAlmaModel dersAlma = new dersAlmaModel()
                {
                    DERS_ID = Convert.ToInt32(dr["DERS_ID"]),
                    dersAd = dr["DERS_AD"].ToString(),
                    dersVeren = dr["DERS_VEREN"].ToString()
                };
                dersAlmaList.Add(dersAlma);
                dr.Close();
            }
            conn.Close();
            return View("dersAlmaListesi", dersAlmaList);
        }

        // Talep Oluşturma Sayfası
        [HttpGet]
        public ActionResult talepOlusturma()
        {
            return View("TalepOlusturmaSayfasi");
        }

        [HttpPost]
        public ActionResult talepOlusturma(string mesaj, string konu)
        {
            string gonderenId = Session["ogrenciNumara"].ToString();
            int durum = 0;
            conn.Open();
            SqlCommand cmd = new SqlCommand("INSERT INTO TBL_TALEP(TALEP_KONU,TALEP_MESAJ,TALEP_DURUM,GONDEREN_ID) VALUES(@konu,@mesaj,@durum,@gonderenId)", conn);
            cmd.Parameters.AddWithValue("@konu", konu);
            cmd.Parameters.AddWithValue("@mesaj", mesaj);
            cmd.Parameters.AddWithValue("@gonderenId", gonderenId);
            cmd.Parameters.AddWithValue("@durum", durum);
            cmd.ExecuteNonQuery();
            conn.Close();
            return RedirectToAction("talepOlusturma", "Personal_information");
        }

        // Talepleri Görüntüleme
        [HttpGet]
        public ActionResult talepleriGoruntule()
        {
            conn.Open();
            List<talepModel> talepList = new List<talepModel>();
            string ogrenciNo = Session["ogrenciNumara"].ToString();
            SqlCommand cmd = new SqlCommand("SELECT * FROM TBL_TALEP WHERE GONDEREN_ID=@id", conn);
            cmd.Parameters.AddWithValue("@id", ogrenciNo);
            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                talepModel talep = new talepModel()
                {
                    talepId = dr["TALEP_ID"].ToString(),
                    gonderenId = dr["GONDEREN_ID"].ToString(),
                    mesaj = dr["TALEP_MESAJ"].ToString(),
                    konu = dr["TALEP_KONU"].ToString(),
                    mesajYanit = dr["TALEP_YANIT"].ToString(),
                    durum = Convert.ToInt32(dr["TALEP_DURUM"])
                };
                talepList.Add(talep);
            }
            dr.Close();
            conn.Close();
            return View("TalepleriGoruntule", talepList);
        }

        // Kimlik Kart Talep Etme
        [HttpGet]
        public ActionResult kimlikKartTalepEt()
        {
            string ogrenciNo = Session["ogrenciNumara"].ToString();

            KimlikKartTalepEtModel kimlik = new KimlikKartTalepEtModel
            {
                OgrenciNo = ogrenciNo
            };

            return View("KimlikKartTalepEt", kimlik);
        }

        [HttpPost]
        public ActionResult kimlikKartTalepEt(string ogrenciNo)
        {
            conn.Open();
            SqlCommand cmd = new SqlCommand("INSERT INTO TBL_KIMLIK_KART_TALEP(OGRENCI_NO) VALUES(@ogrenciNo)", conn);
            cmd.Parameters.AddWithValue("@ogrenciNo", ogrenciNo);
            cmd.ExecuteNonQuery();
            return RedirectToAction("kimlikKartTalepEt");
        }

        // Belge Talep Etme get actionu
        [HttpGet]
        public ActionResult belgeTalepEt()
        {
            return View("BelgeTalepSayfası");
        }

        //belge talep post action'u
        [HttpPost]
        public ActionResult belgeTalepEt(string belgeTuru)
        {
            conn.Open();
            string ogrenciNo = Session["ogrenciNumara"].ToString();
            SqlCommand cmd = new SqlCommand("INSERT INTO TBL_BELGE_TALEP(BELGE_TUR,BELGE_OGRENCI_NO) VALUES(@belgeTur,@belgeOgrenciNo)", conn);
            cmd.Parameters.AddWithValue("@belgeTur", belgeTuru);
            cmd.Parameters.AddWithValue("@belgeOgrenciNo", ogrenciNo);
            cmd.ExecuteNonQuery();
            conn.Close();
            return View("BelgeTalepSayfası");
        }
    }
}