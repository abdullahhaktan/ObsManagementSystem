using ObsSistem.Models.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ObsSistem.Controllers
{
    public class AkademisyenController : Controller
    {
        // Veritabanı bağlantısı için SqlConnection nesnesi
        SqlConnection conn = new SqlConnection("Data Source=DESKTOP-IHRMNQ9\\MSSQLSERVER01;Initial Catalog=ObsSistem;Integrated Security=True");

        // GET: Akademisyen
        [HttpGet]
        public ActionResult akademisyenGiris()
        {
            // Akademisyen giriş sayfasını görüntüler
            return View("akademisyenGiris");
        }

        [HttpPost]
        public ActionResult akademisyenGiris(string tc, string password)
        {
            // Veritabanı bağlantısını açar
            conn.Open();

            // SQL Server'da tek bir değer döndüren fonksiyon kullanımı
            SqlCommand cmd = new SqlCommand("SELECT dbo.AkademisyenGiris(@tc, @sifre)", conn);
            cmd.Parameters.AddWithValue("@tc", tc);
            cmd.Parameters.AddWithValue("@sifre", password);
            object result = cmd.ExecuteScalar();

            // Eğer fonksiyon bir değer döndürdüyse ve bu değer 1 ise giriş başarılı kabul edilebilir.
            if (result != null && Convert.ToInt32(result) == 1)
            {
                // Diğer Session işlemleri
                SqlCommand cmd2 = new SqlCommand("SELECT * FROM TBL_AKADEMISYEN WHERE AKADEMISYEN_TC=@tc AND AKADEMISYEN_SIFRE=@sifre", conn);
                cmd2.Parameters.AddWithValue("@tc", tc);
                cmd2.Parameters.AddWithValue("@sifre", password);
                SqlDataReader dr = cmd2.ExecuteReader();

                if (dr.Read())
                {
                    // Akademisyen bilgilerini Session değişkenlerine atar
                    Session["AkademisyenID"] = dr["AKADEMISYEN_ID"].ToString();
                    Session["AkademisyenAdSoyad"] = dr["AKADEMISYEN_AD_SOYAD"].ToString();
                    Session["AkademisyenDanismanlikDurumu"] = dr["AKADEMISYEN_DANISMANLIK_DURUMU"].ToString();
                    Session["AkademisyenBolum"] = dr["AKADEMISYEN_BOLUM"].ToString();
                    dr.Close();
                    conn.Close();
                    return RedirectToAction("dersleriGoster", "Akademisyen");
                }
                else
                {
                    // fonksiyonun durumu 1 olmadığına göre giriş hatalı bir şekilde yapıldı bu durumda sayfanın kendisi return edecektir
                    dr.Close();
                    conn.Close();
                    return RedirectToAction("akademisyenGiris", "Akademisyen");
                }
            }
            else
            {
                conn.Close();
                return RedirectToAction("akademisyenGiris", "Akademisyen");
            }
        }

        [HttpGet]
        public ActionResult dersOnaylama()
        {
            // Veritabanı bağlantısını açar
            conn.Open();

            // Akademisyenin danışman olduğu öğrencilerin aldığı dersleri getirir
            string danisman = Session["AkademisyenAdSoyad"].ToString();
            List<dersAlmaModel> dersListe = new List<dersAlmaModel>();
            SqlCommand cmd = new SqlCommand("SELECT * FROM TBL_DERS_ALMA WHERE OGRENCI_DANISMAN=@danisman", conn);
            cmd.Parameters.AddWithValue("@danisman", danisman);
            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                //dersListesine tüm danısmana ait derslerin atanmasının yapılması
                dersAlmaModel ders = new dersAlmaModel()
                {
                    DERS_ALMA_ID = Convert.ToInt32(dr["DERS_ALMA_ID"]),
                    DERS_ID = Convert.ToInt32(dr["DERS_ID"]),
                    dersAd = dr["DERS_AD"].ToString(),
                    ogrenciNumara = Convert.ToInt32(dr["OGRENCI_NUMARA"]),
                    ogrenciAd = dr["OGRENCI_AD"].ToString(),
                    ogrenciSoyad = dr["OGRENCI_SOYAD"].ToString(),
                    durum = Convert.ToInt32(dr["DURUM"]),
                    dersKod = (dr["DERS_KOD"]).ToString(),
                    kredi = Convert.ToInt32(dr["KREDI"]),
                    danisman = dr["OGRENCI_DANISMAN"].ToString()
                };
                dersListe.Add(ders);
            }

            // SqlDataReader ve bağlantıyı kapatır
            dr.Close();
            conn.Close();

            // Ders onaylama sayfasını görüntüler
            return View("DersOnaylamaSayfası", dersListe);
        }

        //ders onaylama post acitonu
        [HttpPost]
        public ActionResult dersOnaylama(int dersId, string ogrenciNo, string dersVeren)
        {
            // Veritabanı bağlantısını açar
            conn.Open();

            // Dersi alan öğrencinin harf notunu kaydetme işlemleri
            SqlCommand cmd3 = new SqlCommand("SELECT DERS_AKADEMISYEN FROM TBL_DERS WHERE DERS_ID=@dersId", conn);
            cmd3.Parameters.AddWithValue("@dersId", dersId);
            SqlDataReader dr = cmd3.ExecuteReader();
            if (dr.Read())
            {
                Session["DersAkademisyenId"] = dr["DERS_AKADEMISYEN"].ToString();
            }
            dr.Close();
            //ders alma listesinden dersin id'sine göre silinme
            SqlCommand cmd = new SqlCommand("DELETE FROM TBL_DERS_ALMA WHERE DERS_ID=@dersId", conn);
            cmd.Parameters.AddWithValue("@dersId", dersId);
            cmd.ExecuteNonQuery();

            string harfNot = "-";
            string dersAkademisyenId = Session["DersAkademisyenId"].ToString();

            //harf not tablosunu güncelleme
            SqlCommand cmd1 = new SqlCommand("UPDATE TBL_HARF_NOT SET HARF_NOT=@harfNot WHERE HARF_NOT_DERS=@dersId AND OGRENCI_NO=@ogrenciNo", conn);
            cmd1.Parameters.AddWithValue("@harfNot", harfNot);
            cmd1.Parameters.AddWithValue("@dersId", dersId);
            cmd1.Parameters.AddWithValue("@ogrenciNo", ogrenciNo);
            cmd1.ExecuteNonQuery();

            //alınan dersler tablosuna ellemeler yapma
            SqlCommand cmd2 = new SqlCommand("INSERT INTO TBL_ALINAN_DERSLER(DERS_ID,OGRENCI_NO,AKADEMISYEN_ID) VALUES(@dersId,@ogrenciNo,@dersVeren)", conn);
            cmd2.Parameters.AddWithValue("@dersId", dersId);
            cmd2.Parameters.AddWithValue("@ogrenciNo", ogrenciNo);
            cmd2.Parameters.AddWithValue("@dersVeren", dersAkademisyenId);
            cmd2.ExecuteNonQuery();
            string sınavTur = "";
            string ogrenciNot = "";
            string sınavTarih = "";

            /// sınavlar tablosuna eklemeler yapma
            SqlCommand cmd5 = new SqlCommand("INSERT INTO TBL_SINAV(SINAV_TUR,DERS_ID,OGRENCI_NOT,SINAV_TARIH,OGRENCI_ID) VALUES(@sınavTur,@dersId,@ogrenciNot,@sinavTarih,@ogrenciId)", conn);
            cmd5.Parameters.AddWithValue("@sınavTur", sınavTur);
            cmd5.Parameters.AddWithValue("@dersId", dersId);
            cmd5.Parameters.AddWithValue("@ogrenciNot", ogrenciNot);
            cmd5.Parameters.AddWithValue("@sinavTarih", sınavTarih);
            cmd5.Parameters.AddWithValue("@ogrenciId", ogrenciNo);
            cmd5.ExecuteNonQuery();

            int durum = 1;

            //ders alma durumunu 1 yapma
            SqlCommand cmd4 = new SqlCommand("UPDATE TBL_DERS_ALMA SET DURUM=@durum ", conn);
            cmd4.Parameters.AddWithValue("@durum", 1);
            cmd4.ExecuteNonQuery();
            conn.Close();

            // Ders onaylama sayfasına yönlendirme
            return RedirectToAction("dersOnaylama", "Akademisyen");
        }

        public ActionResult onayiSil(string dersID)
        {
            conn.Open();
            SqlCommand cmd = new SqlCommand("DELETE FROM TBL_DERS_ALMA WHERE DERS_ID=@dersID", conn);
            cmd.Parameters.AddWithValue("@dersID", dersID);
            cmd.ExecuteNonQuery();
            conn.Close();
            return RedirectToAction("dersOnaylama", "Akademisyen");
        }
        public ActionResult dersleriGoster()
        {
            // Akademisyenin verdiği dersleri görüntüleme
            string akademisyenId = "";
            akademisyenId = Session["AkademisyenID"].ToString();
            conn.Open();
            //model listesi oluşturma
            List<dersModel1> dersListesi = new List<dersModel1>();
            SqlCommand cmd = new SqlCommand("SELECT * FROM TBL_DERS INNER JOIN TBL_AKADEMISYEN ON TBL_AKADEMISYEN.AKADEMISYEN_ID=TBL_DERS.DERS_AKADEMISYEN WHERE DERS_AKADEMISYEN=@akademisyen", conn);
            cmd.Parameters.AddWithValue("@akademisyen", akademisyenId);
            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                //model listesine derslerin ataması
                Session["DersBolumID"] = dr["DERS_BOLUM"].ToString();
                dersModel1 ders = new dersModel1()
                {
                    DERS_ID = Convert.ToInt32(dr["DERS_ID"]),
                    DERS_ADI = dr["DERS_AD"].ToString(),
                    DERS_KODU = (dr["DERS_KOD"]).ToString(),
                    KREDI = Convert.ToInt32(dr["DERS_KREDI"]),
                    BOLUM = dr["DERS_BOLUM"].ToString(),
                    DERS_YARI_YIL = Convert.ToInt32(dr["DERS_YARI_YIL"]),
                    DERSI_VEREN = dr["AKADEMISYEN_AD_SOYAD"].ToString()
                };
                dersListesi.Add(ders);
            }
            Session["dersId"] = "";

            dr.Close();
            conn.Close();

            // Dersler sayfasını görüntüler
            return View("Derslerim", dersListesi);
        }

        [HttpPost]
        public ActionResult notGirme(string dersId)
        {
            // Alınan derslerin ve derse ait öğrencilerin bilgilerini getirme işlemleri
            //sp kullanımı
            string sp_name = "sp_alınanDerslerinVeDerseAitOgrencilerinBilgileriniGetirme";
            Session["dersID"] = dersId;
            conn.Open();
            // alınan  dersden model listesi oluşturulması
            List<alinanDersModel> alinanDersList = new List<alinanDersModel>();
            int akademisyenId = Convert.ToInt32(Session["AkademisyenID"]);
            SqlCommand cmd = new SqlCommand(sp_name, conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@akademisyenId", akademisyenId);
            cmd.Parameters.AddWithValue("@dersId", dersId);
            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                alinanDersModel alinanDers = new alinanDersModel
                {
                    ogrenciNo = Convert.ToInt32(dr["NUMARA"]),
                    ogrenciAd = dr["AD"].ToString(),
                    ogrenciSoyad = dr["SOYAD"].ToString(),
                    dersId = dr["DERS_ID"].ToString(),
                    sınavTur = dr["SINAV_TUR"].ToString(),
                    sınavNot = dr["OGRENCI_NOT"].ToString(),
                    dersAlmaId = Convert.ToInt32(dr["DERS_ALMA_LISTESI_ID"])
                };
                alinanDersList.Add(alinanDers);
            }

            dr.Close();
            conn.Close();

            // Not girme sayfasını görüntüler
            return View("NotGirmeSayfasi", alinanDersList);
        }

        [HttpPost]
        public ActionResult notKaydet(string ogrenciNo, string ogrenciNot, string sinavTur, string sinavTarih)
        {
            // Not kaydetme işlemleri
            conn.Open();
            string dersId = Session["dersID"].ToString();
            Session["OgrenciNoToHarfNot"] = ogrenciNo;
            int akademisyenID = Convert.ToInt32(Session["AkademisyenID"]);

            //sınav tablosuna kaydetme
            SqlCommand cmd = new SqlCommand("INSERT INTO TBL_SINAV(OGRENCI_ID,SINAV_TUR,DERS_ID,OGRENCI_NOT,SINAV_TARIH) VALUES(@ogrenciNo,@sinavTur,@dersId,@ogrenciNot,@sinavTarih)", conn);
            cmd.Parameters.AddWithValue("@sinavTur", sinavTur);
            cmd.Parameters.AddWithValue("@dersId", dersId);
            cmd.Parameters.AddWithValue("@sinavTarih", sinavTarih);
            cmd.Parameters.AddWithValue("@ogrenciNot", ogrenciNot);
            cmd.Parameters.AddWithValue("@ogrenciNo", ogrenciNo);
            cmd.ExecuteNonQuery();
            //bağlantı kapama
            conn.Close();

            // Dersleri göster sayfasına yönlendirme
            return RedirectToAction("dersleriGoster", "Akademisyen");
        }

        public ActionResult ogrencininHarfNotunuVer(string dersId, int ogrenciNo, string harfNot)
        {
            // Öğrencinin harf notunu verme işlemleri
            conn.Open();

            string bolumId = Session["AkademisyenBolum"].ToString();

            //öğrenciye ders atarken harf notuna da null olarak değer atamışdık şimdi öğrenci artık bir harf notuna sahip olduğu için update olması gerek
            SqlCommand cmd = new SqlCommand("UPDATE TBL_HARF_NOT SET HARF_NOT_BOLUM=@bolumId , HARF_NOT=@harfNot WHERE OGRENCI_NO=@ogrenciNo AND HARF_NOT_DERS=@dersId", conn);
            cmd.Parameters.AddWithValue("@harfNot", harfNot);
            cmd.Parameters.AddWithValue("@ogrenciNo", ogrenciNo);
            cmd.Parameters.AddWithValue("@dersId", dersId);
            cmd.Parameters.AddWithValue("@bolumId", bolumId);
            cmd.ExecuteNonQuery();

            //örğenci artık dersi verdiği için alınan derslerden kaldırılması gerek
            SqlCommand cmd1 = new SqlCommand("DELETE FROM TBL_ALINAN_DERSLER WHERE DERS_ID=@dersId", conn);
            cmd1.Parameters.AddWithValue("@dersId", dersId);
            cmd1.ExecuteNonQuery();

            conn.Close();

            // Dersleri göster sayfasına yönlendirme
            return RedirectToAction("dersleriGoster", "Akademisyen");
        }

        public ActionResult alınanDersSilme(string dersId)
        {
            // Alınan dersi silme işlemleri
            //bağlantı açma
            conn.Open();
            SqlCommand cmd = new SqlCommand("DELETE FROM TBL_ALINAN_DERSLER WHERE DERS_ID=@dersId", conn);
            cmd.Parameters.AddWithValue("@dersId", dersId);
            cmd.ExecuteNonQuery();
            //kapama
            conn.Close();

            // Dersleri göster sayfasına yönlendirme
            return RedirectToAction("dersleriGoster", "Akademisyen");
        }
    }
}