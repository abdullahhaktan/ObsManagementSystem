using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Management;
using System.Web.Mvc;

namespace ObsSistem.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        //öğrenci ve admin girişi
        [HttpPost]
        public ActionResult Login(string tc, string password)
        {
            SqlConnection connection = new SqlConnection("Data Source=DESKTOP-IHRMNQ9\\MSSQLSERVER01;Initial Catalog=ObsSistem;Integrated Security=True");
            connection.Open();

            // Admin Girişi Kontrolü
            SqlCommand cmdAdmin = new SqlCommand("SELECT dbo.AdminGiris(@tc, @sifre)", connection);
            cmdAdmin.Parameters.AddWithValue("@tc", tc);
            cmdAdmin.Parameters.AddWithValue("@sifre", password);
            object resultAdmin = cmdAdmin.ExecuteScalar();

            // Eğer Admin girişi başarılı ise
            if (resultAdmin != null && Convert.ToInt32(resultAdmin) == 1)
            {
                // Diğer Session işlemleri
                return RedirectToAction("Index", "Admin");
            }
            // Öğrenci Girişi Kontrolü
            else
            {
                string sp_name = "sp_ogrenciGirisYapma";
                SqlCommand cmdOgrenci = new SqlCommand(sp_name, connection);
                cmdOgrenci.CommandType = CommandType.StoredProcedure;
                cmdOgrenci.Parameters.AddWithValue("@tc_numara", tc);
                cmdOgrenci.Parameters.AddWithValue("@sifre", password);
                SqlDataReader dr = cmdOgrenci.ExecuteReader();

                // Eğer Öğrenci girişi başarılı ise
                if (dr.Read())
                {
                    Session["OgrenciAdi"] = dr["AD"].ToString();
                    Session["OgrenciSoyadi"] = dr["SOYAD"].ToString();
                    Session["ogrenciNumara"] = dr["NUMARA"].ToString();
                    Session["ogrenciBolum"] = dr["BOLUM_AD"].ToString();
                    Session["ogrenciBolumId"] = dr["BOLUM_ID"].ToString();
                    Session["ogrenciAkademisyen"] = dr["DANISMAN"].ToString();
                    ViewBag.title = "Giriş yapıldı";
                    connection.Close();
                    dr.Close();
                    return View("FirstPage");
                }
                // Eğer Admin ve Öğrenci girişi başarısız ise
                else
                {
                    ViewBag.title = "Giriş yapılamadı";
                    connection.Close();
                    dr.Close();
                    return View("index");
                }
            }
        }

        //çıkış action'u
        public ActionResult cikis()
        {
            return View("index");
        }

    }
}