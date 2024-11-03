using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.WebSockets;
using ObsSistem.Models.EntityFramework;

namespace ObsSistem.Controllers
{
    public class EducationController : Controller
    {

        // GET: Education
        public ActionResult Index()
        {
            // Eğitim sayfasının ana görünümünü döndür
            return View();
        }
        // Veritabanı bağlantısı
        SqlConnection connection = new SqlConnection("data source=DESKTOP-IHRMNQ9\\MSSQLSERVER01;initial catalog=ObsSistem;integrated security=True");

        // Ders alma işlemi için sayfa
        public ActionResult DersAlma()
        {
            connection.Open();
            // Ders alma işlemi için stored procedure adı
            string spName = "sp_YariyillaraGoreListelereDerslerAta";

            // Session'dan gelen öğrenci bilgileri
            string ogrenciBolum = Session["ogrenciBolum"].ToString();
            string ogrenciNo = Session["ogrenciNumara"].ToString();
            string ogrenciBolumId = Session["ogrenciBolumId"].ToString();

            // Yarıyıl listesi
            List<dersModel> yariYilList = new List<dersModel>();

            // 1. yarıyıldan 8. yarıyıla kadar döngü
            for (int yariYil = 1; yariYil <= 8; yariYil++)
            {

                SqlCommand cmd = new SqlCommand(spName, connection);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@bolum", ogrenciBolumId);
                cmd.Parameters.AddWithValue("@yariYil", yariYil);
                cmd.Parameters.AddWithValue("@ogrenciNumara", ogrenciNo);

                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    // Ders modeli oluştur
                    dersModel ders = new dersModel
                    {
                        DERS_ID = Convert.ToInt32(dr["DERS_ID"]),
                        DERS_KODU = dr["DERS_KOD"].ToString(),
                        DERS_ADI = dr["DERS_AD"].ToString(),
                        DERSI_VEREN = dr["AKADEMISYEN_AD_SOYAD"].ToString(),
                        KREDI = Convert.ToInt16(dr["DERS_KREDI"]),
                        HARF_NOTU = dr["HARF_NOT"].ToString(),
                        DERS_YARI_YIL = Convert.ToInt32(dr["DERS_YARI_YIL"])
                    };
                    yariYilList.Add(ders);
                }
                dr.Close();
            }
            connection.Close();
            // Ders alma görünümünü döndür
            return View("Ders_alma", yariYilList);
        }

        // AGNO hesaplama işlemi için sayfa
        public ActionResult agnoHesapla()
        {
            // Veritabanı bağlantı bilgileri
            string baglanti = "data source=DESKTOP-IHRMNQ9\\MSSQLSERVER01;initial catalog=ObsSistem;integrated security=True";
            SqlConnection connection = new SqlConnection(baglanti);
            connection.Open();

            // AGNO hesaplama için stored procedure adı
            string spName = "sp_YariyillaraGoreListelereDerslerAta";

            // Session'dan gelen öğrenci bilgileri
            string ogrenciBolum = Session["ogrenciBolum"].ToString();
            string ogrenciNo = Session["ogrenciNumara"].ToString();
            string ogrenciBolumId = Session["ogrenciBolumId"].ToString();

            // Yarıyıl listesi
            List<dersModel> yariYilList = new List<dersModel>();

            // 1. yarıyıldan 8. yarıyıla kadar döngü
            for (int yariYil = 1; yariYil <= 8; yariYil++)
            {

                SqlCommand cmd = new SqlCommand(spName, connection);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@bolum", ogrenciBolumId);
                cmd.Parameters.AddWithValue("@yariYil", yariYil);
                cmd.Parameters.AddWithValue("@ogrenciNumara", ogrenciNo);

                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    // Ders modeli oluştur
                    dersModel ders = new dersModel
                    {
                        DERS_ID = Convert.ToInt32(dr["DERS_ID"]),
                        DERS_KODU = (dr["DERS_KOD"].ToString()),
                        DERS_ADI = dr["DERS_AD"].ToString(),
                        DERSI_VEREN = dr["AKADEMISYEN_AD_SOYAD"].ToString(),
                        KREDI = Convert.ToInt16(dr["DERS_KREDI"]),
                        HARF_NOTU = dr["HARF_NOT"].ToString(),
                        DERS_YARI_YIL = Convert.ToInt32(dr["DERS_YARI_YIL"])
                    };
                    yariYilList.Add(ders);
                }
                dr.Close();
            }
            connection.Close();
            // AGNO hesaplama görünümünü döndür
            return View("Agno_hesaplama", yariYilList);
        }

        // Danışmana onay gönderme işlemi
        public ActionResult danismanaOnayGonder(int dersId, string dersAd, string dersKod, string dersKredi, string dersVeren)
        {
            // Veritabanı bağlantısı
            SqlConnection conn = new SqlConnection("Data Source=DESKTOP-IHRMNQ9\\MSSQLSERVER01;Initial Catalog=ObsSistem;Integrated Security=True");
            conn.Open();
            // Session'dan gelen öğrenci bilgileri
            string ogrenciNo = Session["ogrenciNumara"].ToString();
            string ogrenciAd = Session["OgrenciAdi"].ToString();
            string ogrenciSoyad = Session["OgrenciSoyadi"].ToString();
            string ogrenciDanisman = Session["ogrenciAkademisyen"].ToString();
            string durum = "";

            // Danışmana onay gönderme SQL komutu
            SqlCommand cmd = new SqlCommand("INSERT INTO TBL_DERS_ALMA(OGRENCI_DANISMAN,DURUM,DERS_AD,DERS_ID,OGRENCI_NUMARA,OGRENCI_AD,OGRENCI_SOYAD,KREDI,DERS_KOD,DERS_VEREN) VALUES(@danisman,@dersDurum,@dersAd,@dersId,@ogrenciNo,@ogrenciAd,@ogrenciSoyad,@kredi,@dersKod,@dersVeren)", conn);
            cmd.Parameters.AddWithValue("@dersAd", dersAd);
            cmd.Parameters.AddWithValue("@ogrenciNo", ogrenciNo);
            cmd.Parameters.AddWithValue("@ogrenciAd", ogrenciAd);
            cmd.Parameters.AddWithValue("@ogrenciSoyad", ogrenciSoyad);
            cmd.Parameters.AddWithValue("@kredi", dersKredi);
            cmd.Parameters.AddWithValue("@dersVeren", dersVeren);
            cmd.Parameters.AddWithValue("@dersKod", dersKod);
            cmd.Parameters.AddWithValue("@dersId", dersId);
            cmd.Parameters.AddWithValue("@dersDurum", durum);
            cmd.Parameters.AddWithValue("@danisman", ogrenciDanisman);

            cmd.ExecuteNonQuery();
            conn.Close();

            // Ders alma sayfasına yönlendir
            return RedirectToAction("DersAlma", "Education");
        }

        // Ders alma listesi için sayfa
        [HttpGet]
        public ActionResult dersAlmaListesi()
        {
            connection.Open();

            // Ders alma listesi
            List<dersAlmaModel> dersAlmaListesi = new List<dersAlmaModel>();

            // Session'dan gelen öğrenci bilgileri
            string ogrenciNo = Session["ogrenciNumara"].ToString();
            SqlCommand cmd = new SqlCommand("SELECT * FROM TBL_ALINAN_DERSLER INNER JOIN TBL_DERS ON TBL_DERS.DERS_ID=TBL_ALINAN_DERSLER.DERS_ID INNER JOIN TBL_AKADEMISYEN ON TBL_ALINAN_DERSLER.AKADEMISYEN_ID=TBL_AKADEMISYEN.AKADEMISYEN_ID WHERE OGRENCI_NO=@ogrenciNo", connection);
            cmd.Parameters.AddWithValue("@ogrenciNo", ogrenciNo);
            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                // Ders alma modeli oluştur
                dersAlmaModel ders = new dersAlmaModel
                {
                    DERS_ID = Convert.ToInt32(dr["DERS_ID"]),
                    dersAd = dr["DERS_AD"].ToString(),
                    dersVeren = dr["AKADEMISYEN_AD_SOYAD"].ToString(),
                };
                dersAlmaListesi.Add(ders);
            }
            dr.Close();
            connection.Close();

            // Ders alma listesi görünümünü döndür
            return View("DersAlmaListesi", dersAlmaListesi);
        }
   [HttpGet]
       public ActionResult sinavlar()
        {
            // Sınav model listesi oluşturulması
            List<sınavModel> sınavList = new List<sınavModel>();

            // Öğrenci numarasının alınması
            string ogrenciNo = Session["ogrenciNumara"].ToString();

            // Bağlantının açılması
            connection.Open();

            // SQL sorgusunun hazırlanması ve çalıştırılması
            SqlCommand cmd = new SqlCommand("SELECT * FROM TBL_SINAV INNER JOIN TBL_DERS ON TBL_DERS.DERS_ID=TBL_SINAV.DERS_ID INNER JOIN TBL_OGRENCI ON TBL_OGRENCI.NUMARA=TBL_SINAV.OGRENCI_ID WHERE OGRENCI_ID=@ogrenciId AND OGRENCI_NOT!='' ", connection);
            cmd.Parameters.AddWithValue("@ogrenciId", ogrenciNo);
            SqlDataReader dr = cmd.ExecuteReader();

            // Okunan verilerin modelle eşleştirilmesi ve listeye eklenmesi
            while (dr.Read())
            {
                sınavModel sınav = new sınavModel
                {
                    OGRENCI_ID = dr["NUMARA"].ToString(),
                    OGRENCI_AD = dr["AD"].ToString(),
                    OGRENCI_SOYAD = dr["SOYAD"].ToString(),
                    OGRENCI_NOT = dr["OGRENCI_NOT"].ToString(),
                    DERS_AD = dr["DERS_AD"].ToString(),
                    SINAV_TARIH = dr["SINAV_TARIH"].ToString(),
                    SINAV_TUR = dr["SINAV_TUR"].ToString(),
                };
                sınavList.Add(sınav);
            }

            // DataReader'ı kapatma ve bağlantıyı kapatma
            dr.Close();
            connection.Close();

            // Model listesinin view'e gönderilmesi
            return View("Sınavlar", sınavList);
        }

        [HttpGet]
        public ActionResult notKartlari()
        {
            // Not kartı model listesi oluşturulması
            List<notKartModel> notKartList = new List<notKartModel>();

            // Öğrenci numarasının alınması
            string ogrenciNo = Session["ogrenciNumara"].ToString();

            // Bağlantının açılması
            connection.Open();

            // SQL sorgusunun hazırlanması ve çalıştırılması
            SqlCommand cmd = new SqlCommand("SELECT * FROM TBL_HARF_NOT INNER JOIN TBL_DERS ON TBL_DERS.DERS_ID=TBL_HARF_NOT.HARF_NOT_DERS WHERE OGRENCI_NO=@ogrenciNo and TBL_HARF_NOT.HARF_NOT is not null and TBL_HARF_NOT.HARF_NOT!='' ", connection);
            cmd.Parameters.AddWithValue("@ogrenciNo", ogrenciNo);
            SqlDataReader dr = cmd.ExecuteReader();

            // Okunan verilerin modelle eşleştirilmesi ve listeye eklenmesi
            while (dr.Read())
            {
                notKartModel notKart = new notKartModel
                {
                    DERS_ID = dr["HARF_NOT_DERS"].ToString(),
                    DERS_AD = dr["DERS_AD"].ToString(),
                    HARF_NOT = dr["HARF_NOT"].ToString()
                };
                notKartList.Add(notKart);
            }
            // DataReader'ı kapatma ve bağlantıyı kapatma
            dr.Close();
            connection.Close();

            // Model listesinin view'e gönderilmesi
            return View("NotKartlari", notKartList);
        }


        [HttpPost]
        public ActionResult agnoyuHesapla()
        {
            connection.Open();
            // Toplam not ve kredi değişkenleri
            double toplamOgrenciNot = 0;
            double toplamNot = 0;

            string ogrenciNo = Session["ogrenciNumara"].ToString();

            SqlCommand cmd = new SqlCommand("SELECT * FROM TBL_HARF_NOT INNER JOIN TBL_DERS ON TBL_DERS.DERS_ID=TBL_HARF_NOT.HARF_NOT_DERS WHERE HARF_NOT!='' and HARF_NOT!=null and OGRENCI_NO=@ogrenciNo",connection);
            cmd.Parameters.AddWithValue("@ogrenciNo", ogrenciNo);
            SqlDataReader dr = cmd.ExecuteReader();
            while(dr.Read())
            {
                double harfNot = HarfNotuToSayi(dr["HARF_NOT"].ToString());
                toplamOgrenciNot += Convert.ToDouble(dr["KREDI"])*harfNot;
                toplamNot = Convert.ToDouble(dr["KREDI"].ToString()) * 4;
            }
            double agno = toplamOgrenciNot / toplamNot;

            ViewBag.agno=agno;

            connection.Close();
            return RedirectToAction("agnoHesapla","Education");
        }

        // Harf notunu sayıya çeviren metod
        private double HarfNotuToSayi(string harfNotu)
        {
            // Bağlantıyı açma
            connection.Open();

            // Stored Procedure çağrısı
            SqlCommand cmd = new SqlCommand("SELECT dbo.HarfNotuToSayi(@harfNotu)");
            cmd.Parameters.AddWithValue("@harfNotu", harfNotu);

            // ExecuteScalar ile tek değer döndürme
            object result = cmd.ExecuteScalar();

            // Sonuç double'a çevirme
            double result1 = Convert.ToDouble(result);

            // Bağlantıyı kapatma
            connection.Close();

            // Sonuç döndürme
            return result1;
        }
    }
}
