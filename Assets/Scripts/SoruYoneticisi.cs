using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq; //listeden soru secmemizi saglayan sey
using System.IO;

public class SoruYoneticisi : MonoBehaviour
{
    //singleton yapisi
    public static SoruYoneticisi Instance;

    //butun sorularin saklandigi liste
    public List<Soru> tumSorular = new List<Soru> ();

    //hangi sorularin soruldugunu kaydeden yer
    private static List<int> sorulanlarListesi = new List<int> ();

    private string yedekDosyaAdi = "yedek_sorular";
    private string asilDosyaAdi = "sorular.csv";

    private void Awake()
    {
        Instance = this;
        Application.targetFrameRate = 60;
        SorulariYukle();
    }

    void SorulariYukle()
    {
       
        string dosyaYolu = "";

#if UNITY_ANDROID
        dosyaYolu = Path.Combine(Application.persistentDataPath, asilDosyaAdi);

#else
        dosyaYolu = Path.Combine(Application.streamingAssetsPath, asilDosyaAdi);
#endif



        if (!File.Exists(dosyaYolu))
        {
            Debug.LogWarning("streamingassets yerinde sorular bulunamadý.");

#if !UNITY_ANDROID
            //dosya yoksa dosyayý oluþtur
            if (!Directory.Exists(Application.streamingAssetsPath))
            {
                Directory.CreateDirectory(Application.streamingAssetsPath);
            }
#endif

            TextAsset yedekVeri = Resources.Load<TextAsset>(yedekDosyaAdi);

            if(yedekVeri != null)
            {
                File.WriteAllText(dosyaYolu, yedekVeri.text);
                Debug.Log("sorular resources dosyasýndan yüklendi.");
            }

            else
            {
                Debug.LogWarning("resources dosyasýnda" + yedekDosyaAdi + "bulunamadý");
                return;
            }
        
        }
        
        //adresteki dosyayý kontrol eden kod
        if(File.Exists(dosyaYolu))
        {
            //içindeki bütün yazýlarý okuyup metne çeviren kod
            string icerik = File.ReadAllText(dosyaYolu);

            //metni her enter tuþunun basýldýðý yerden satýr olarak ayýrýr
            string[] satirlar = icerik.Split('\n');

            tumSorular.Clear();

            //okumaya baslayan yer
            for (int i = 0; i < satirlar.Length; i++)
            {
                string satir = satirlar[i];

               //satýr boþ veya hiçbirþey yazmýyorsa orayý atlayan kod 
               if (string.IsNullOrWhiteSpace(satir)) continue;

                //satiri | isaretini gorunce parcalayan kod
                string[] veriler = satir.Split('|');

                //satýrda eksik veri görürse orayý bozuk kabul edip atlayan kod
                if (veriler.Length < 8) continue;

                //oyunun hata vermemesi için kontrolün yapýldýðý yer
                try
                {
                    Soru yeniSoru = new Soru();

                    //int.Parse : yazi olan "1" i sayi olan 1'e cevirir.
                    //trim() : yazinin basindaki ve sonundaki gereksiz bosluklari siler.
                    yeniSoru.id = int.Parse(veriler[0]);
                    yeniSoru.kategori = veriler[1].Trim();
                    yeniSoru.soruMetni = veriler[2];
                    yeniSoru.secenekA = veriler[3];
                    yeniSoru.secenekB = veriler[4];
                    yeniSoru.secenekC = veriler[5];
                    yeniSoru.secenekD = veriler[6];

                    //cevaptaki satir atlama isaretini temizleyen kod
                    yeniSoru.dogruCevap = veriler[7].Trim().Replace("\r", "");

                    //hazirlanan soruyu ana listeye ekleyen kod
                    tumSorular.Add(yeniSoru);

                }

                catch
                {
                    Debug.LogWarning("Satir okunamadý " + i);
                }
            }

            Debug.Log("StreamingAssets üzerinden yüklenen soru sayýsý : " + tumSorular.Count);
        }

        else
        {
            Debug.LogError("HATA! Dosya bulunamadý!!" + dosyaYolu);
        }


    }

    public Soru GetirSoru(string kategoriKodu)
    {
       //kategorisi uyanlari ve sorulanlarlistesinde olmayanlari bulan kod
        var uygunSorular = tumSorular
            .Where(x=> x.kategori == kategoriKodu && !sorulanlarListesi.Contains(x.id))
            .ToList();

        //bütün sorular sorulduysa sorulanlar listesini sýfýrlayan ve tekrar sorularý sormaya devam eden kod
        if(uygunSorular.Count == 0)
        {
            Debug.Log("sorular bitti. liste sýfýrlanýyor.");
            
            sorulanlarListesi.Clear();

            uygunSorular = tumSorular.Where(x => x.kategori == kategoriKodu).ToList();
        }
        
        //daha once sorulmamis soru kalmadiysa
        if (uygunSorular.Count == 0) return null;

        //rastgele soru sececek
        Soru secilen = uygunSorular[Random.Range(0, uygunSorular.Count)];

        //sorunun id'sini not edecek
        sorulanlarListesi.Add(secilen.id);

        return secilen;
    }
}
