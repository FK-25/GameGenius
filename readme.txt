========================================================================
              GAMEGENIUS - OYUN DÜNYASI BİLGİ YARIŞMASI
                           Sürüm: 1.0.0
           Geliştirici: Fatih Kıtır (Öğrenci No: 250710013)
                         Tarih: 21.12.2025
========================================================================

1. GENEL BİLGİ
------------------------------------------------------------------------
GameGenius, oyun dünyası ve tarihi üzerine kurgulanmış, tek oyunculu bir 
bilgi yarışması oyunudur. Oyun, statik kodlama yerine veri odaklı (Data-Driven) 
bir mimari ile geliştirilmiş olup, tüm sorular harici bir veri kaynağından 
çekilmektedir.

2. KURULUM VE ÇALIŞTIRMA
------------------------------------------------------------------------
Oyun "Taşınabilir" (Portable) yapıdadır. Herhangi bir kuruluma ihtiyaç duymaz.
1. "GameGenius.exe" dosyasına çift tıklayın.
2. Windows güvenlik uyarısı verirse "Yine de çalıştır" seçeneğini seçin.
3. Oyun tam ekran modunda açılacaktır.

3. NASIL OYNANIR?
------------------------------------------------------------------------
- Amaç: Size yöneltilen oyun kültürü sorularını bilerek en yüksek puanı toplamak.
- Kontroller: Oyun tamamen fare ile kontrol edilir.
- Onay Mekanizması: Bir şıkka tıkladığınızda "Emin misin?" paneli açılır. 
  Cevabınızı onaylamak için "EVET", değiştirmek için "HAYIR" butonuna basın.
- Jokerler:
  * %50: Yanlış iki şıkkı eler.
  * Çift Cevap(x2): O soru için cevaplama hakkını ikiye çıkarır.
  * Pas: Soruyu cevaplamadan bir sonrakine geçer.
- Ayarlar: Oyun içindeki butonlar aracılığıyla Müzik ve Ses Efektleri (SFX) 
  kapatılıp açılabilir.

4. TEKNİK DETAYLAR (GELİŞTİRİCİ NOTLARI)
------------------------------------------------------------------------
Bu proje Unity Oyun Motoru kullanılarak geliştirilmiştir. 
- Mimari: Tek sahne üzerinde Panel Yönetimi sistemi kullanılmıştır.
- Veri Yapısı: Sorular "CSV" formatında tutulmakta ve String Parsing yöntemi 
  ile işlenmektedir.
- Kullanılan Teknolojiler: 
  * Kodlama: C# (OyunYoneticisi.cs, SoruYoneticisi.cs, Soru.cs)
  * Arayüz: Unity UI (Legacy Text)
  * Veri Kaynağı: GameGenius_Data\StreamingAssets\sorular.csv

5. VERİ FORMATI (SORU EKLEME)
------------------------------------------------------------------------
Soru havuzu aşağıdaki format yapısına sahiptir. Yeni soru eklenmek istendiğinde 
bu şablon kullanılmalıdır:

ID|Kategori|SoruMetni|SecenekA|SecenekB|SecenekC|SecenekD|DogruCevap

Örnek Satır:
101|K1|Mario'nun kardeşinin adı nedir?|Wario|Luigi|Peach|Bowser|B

6. KREDİLER VE LİSANSLAR
------------------------------------------------------------------------
Bu proje eğitim amaçlı geliştirilmiştir.
- Görsel Varlıklar: Playground AI kullanılarak üretilmiştir.
- Müzik ve Sesler: Suno AI ve Unity Default Assets kullanılmıştır.
- Kod Altyapısı: Tamamen özgün olarak geliştirilmiştir, hazır Asset Store 
  paketi kullanılmamıştır.

========================================================================
                             İyi Eğlenceler!
========================================================================