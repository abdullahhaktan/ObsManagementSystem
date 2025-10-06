# ObsManagementSystem

[TR]

**Öğrenci Bilgi Sistemi (OBS) Yönetim Uygulaması**

[![C#](https://img.shields.io/badge/Language-C%23-blue.svg)](https://docs.microsoft.com/en-us/dotnet/csharp/)
[![Database](https://img.shields.io/badge/Database-SQL_Server-CC2927.svg)](https://www.microsoft.com/en-us/sql-server)
[![Platform](https://img.shields.io/badge/Platform-Web%2FDesktop%20App-informational.svg)]()
[![GitHub repo size](https://img.shields.io/github/repo-size/abdullahhaktan/ObsManagementSystem)](https://github.com/abdullahhaktan/ObsManagementSystem)

---

## 💻 Proje Hakkında

Bu proje, bir eğitim kurumunun (okul/üniversite) **Öğrenci Bilgi Sistemi (OBS)** ihtiyaçlarını karşılamak üzere geliştirilmiş bir yönetim uygulamasıdır. Temel amacı, öğrencilerin, derslerin, notların ve öğretim görevlilerinin kayıtlarını dijital ortamda merkezi bir şekilde yönetmektir.

---

## ✨ Temel Özellikler ve Modüller

### Kullanıcı Rolleri ve Yönetimi
* **Çoklu Kullanıcı Desteği:** Sistem, genellikle **Öğrenci, Öğretim Görevlisi ve Yönetici/Personel** olmak üzere üç ana kullanıcı rolüne sahiptir.
* **Kimlik ve Yetkilendirme:** Farklı kullanıcı rollerine özgü erişim yetkileri (Authentication ve Authorization) tanımlanmıştır.

### Akademik Yönetim Modülleri
* **Öğrenci Yönetimi:** Yeni öğrenci kaydı, kişisel bilgileri güncelleme, aktif/pasif durum takibi ve öğrenci transkripti görüntüleme.
* **Ders Yönetimi:** Yeni derslerin sisteme eklenmesi, kredi, dönem ve zorunluluk durumlarının belirlenmesi.
* **Not ve Başarı Takibi:** Öğretim görevlilerinin sınav notlarını girmesi, öğrencilerin notlarını ve genel ortalamalarını (GPA/AGNO) hesaplama ve görüntüleme.
* **Dönem/Sınıf Yönetimi:** Dönemlere göre ders atamaları ve sınıf listelerinin yönetimi.

### Teknik Altyapı
* **Veritabanı Entegrasyonu:** Tüm öğrenci ve akademik verilerin güvenli ve ilişkisel bir yapıda tutulması için **SQL Server** veya benzeri bir veritabanı kullanılır.
* **CRUD Operasyonları:** Tüm temel yönetim modüllerinde **Ekleme, Listeleme, Güncelleme ve Silme** (CRUD) işlemleri tam olarak uygulanmıştır.
* **Geliştirme Teknolojisi:** Proje **C#** dili kullanılarak (.NET Core/Framework ile Web veya Masaüstü uygulaması olarak) geliştirilmiştir.

---

## 🚀 Nasıl Çalıştırılır?

Bu projenin çalıştırılması için gerekli **.NET ortamı** ve **SQL Server** veritabanı erişimi gereklidir.

1.  **Projeyi Klonlama:**
    ```bash
    git clone [https://github.com/abdullahhaktan/ObsManagementSystem](https://github.com/abdullahhaktan/ObsManagementSystem)
    cd ObsManagementSystem
    ```

2.  **Veritabanı Kurulumu:**
    * **SQL Server**'da yeni bir veritabanı oluşturun.
    * Projedeki ilgili tüm tabloları oluşturun

3.  **Bağlantı Dizesini Ayarlama:**
    * Projenin yapılandırma dosyasındaki (`Web.config` veya `appsettings.json`) **veritabanı bağlantı dizesini** kendi yerel SQL Server ayarlarınıza göre güncelleyin.

4.  **Projeyi Başlatma:**
    * **Visual Studio** ile `.sln` (Solution) dosyasını açın.
    * Gerekliyse **NuGet** paketlerini geri yükleyin.
    * Ana projeyi **`Startup Project`** olarak ayarlayın ve **F5** tuşu ile uygulamayı çalıştırın.

---
---

[EN]

# ObsManagementSystem

**Student Information System (SIS) Management Application**

---

## 💻 About the Project

This project is a management application developed to meet the **Student Information System (SIS)** needs of an educational institution (school/university). Its main goal is to centrally manage records for students, courses, grades, and academic staff in a digital environment.

---

## ✨ Core Features and Modules

### User Roles and Management
* **Multi-User Support:** The system typically supports three main user roles: **Student, Instructor, and Administrator/Staff**.
* **Identity and Authorization:** **Authentication** and **Authorization** are implemented to define specific access rights for different user roles.

### Academic Management Modules
* **Student Management:** New student registration, updating personal information, tracking active/inactive status, and viewing student transcripts.
* **Course Management:** Adding new courses, defining credit values, semesters, and mandatory/elective status.
* **Grade and Performance Tracking:** Allowing instructors to input exam scores, calculating and displaying student grades and overall GPA (AGNO).
* **Semester/Class Management:** Managing course assignments per semester and class rosters.

### Technical Foundation
* **Database Integration:** A relational database like **SQL Server** is used to securely store all student and academic data in a structured manner.
* **CRUD Operations:** Full implementation of **Create, Read, Update, and Delete** (CRUD) operations across all core management modules.
* **Development Technology:** The project is developed using the **C#** language (as a Web or Desktop application with .NET Core/Framework).

---

## 🚀 How to Run

Running this project requires the necessary **.NET environment** and **SQL Server** database access.

1.  **Cloning the Project:**
    ```bash
    git clone [https://github.com/abdullahhaktan/ObsManagementSystem](https://github.com/abdullahhaktan/ObsManagementSystem)
    cd ObsManagementSystem
    ```

2.  **Database Setup:**
    * Create a new database in **SQL Server**.
    * Create all relevant tables in the poject

3.  **Configuring the Connection String:**
    * Update the **database connection string** in the project's configuration file (`Web.config` or `appsettings.json`) to match your local SQL Server settings.

4.  **Starting the Project:**
    * Open the **`.sln`** (Solution) file with **Visual Studio**.
    * Restore **NuGet** packages if necessary.
    * Set the main project as the **`Startup Project`** and run the application by pressing **F5**.

---
---

<img width="329" height="157" alt="Ekran görüntüsü 2025-10-06 141426" src="https://github.com/user-attachments/assets/068ea1f3-45d6-45d2-a32e-0f5dc6c09583" />

---

<img width="448" height="212" alt="Ekran görüntüsü 2025-10-06 141446" src="https://github.com/user-attachments/assets/178cdc00-4902-4c60-8cdd-1408746e97ac" />



