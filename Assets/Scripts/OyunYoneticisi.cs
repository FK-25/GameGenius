using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OyunYoneticisi : MonoBehaviour
{
    [Header("------ PANELLER ------")]
    public GameObject girisPaneli;
    public GameObject gamePanel;
    public GameObject bitisPaneli;
    public GameObject dogruCevapPaneli;
    public GameObject nasilOynanirPaneli;
    public GameObject eminMisinPaneli;
    public GameObject bolumGecisPaneli;
    public GameObject pausePaneli;

    [Header("--- ARKA PLAN VE GÖRSELLER ---")]
    public Image BackgroundImage;
    public Sprite[] bolumResimleri;
    public GameObject[] yildizlar;

    [Header("--- YAZILAR(Text) ---")]
    public Text soruMetni;
    public Text bolumAdiText;
    public Text sureText;
    public Text puanText;
    public Text highScoreText;
    public Text bitisPuanText;
    public Text bitisBaslikText;
    public Text geriSayimText;
    public Text gecisMesajText;
    public Text soruSayacText;

    [Header("--- ÞIK BUTONLARI ---")]
    public Button butonA;
    public Button butonB;
    public Button butonC;
    public Button butonD;

    public Text textA;
    public Text textB;
    public Text textC;
    public Text textD;

    [Header("--- JOKER VE DÝÐER BUTONLAR ---")]
    public Button joker50Btn;
    public Button jokerX2Btn;
    public Button jokerSkipBtn;
    public GameObject devamEtButonu;
    public Image muzikButonResmi;
    public Image sfxButonResmi;
    public Image pauseMuzikButonResmi;
    public Image pauseSfxButonResmi;
    public GameObject finalMenuButonu;
    public GameObject finalCikisButonu;

    [Header("--- SES SÝSTEMÝ ---")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

    public AudioClip[] bolumMuzikleri;
    public AudioClip sesDogru;
    public AudioClip sesYanlis;
    public AudioClip sesClick;
    public AudioClip sesBolumBitisi;
    public AudioClip sesJoker;

    // -- GÝZLÝ DEÐÝÞKENLER --
    private Soru suankiSoru;
    private int toplamPuan = 0;
    private int highScore = 0;
    private int kacinciSorudayiz = 0;
    private float kalanSure;
    private bool oyunDevamEdiyor = false;

    private int suankiSoruPuani = 0;
    private float puanCarpani = 1.0f; //joker kullaninca puanin dusmesini bununla yapýcaz
    private bool ciftCevapHakkiVar = false;
    private bool rekorKirildiMi = false; 

    private string secilenSikGecisi; 
    private int sonrakiJokerHedefi = 10000;

    private void Start()
    {
        Time.timeScale = 1f;
        rekorKirildiMi = false;

        //high score'u hafýzada tutan kod. eðer oyun ilk defa açýldýysa 0 olarak kabul eder
        highScore = PlayerPrefs.GetInt("HighScore", 0);
        highScoreText.text = "REKOR : " + highScore;

       // 1 = mutelenmis 0 = acik
        if(PlayerPrefs.GetInt("MusicMute", 0) == 1)
        {
            musicSource.mute = true;
            muzikButonResmi.color = Color.red;
            if (pauseMuzikButonResmi != null) pauseMuzikButonResmi.color = Color.red;
        }

        if(PlayerPrefs.GetInt("SFXMute", 0) == 1)
        {
            sfxSource.mute = true;
            sfxButonResmi.color = Color.red;
            if (pauseSfxButonResmi != null) pauseSfxButonResmi.color = Color.red;
        }
     
        
        girisPaneli.SetActive(true);

       
        gamePanel.SetActive(false);
        bitisPaneli.SetActive(false);
        dogruCevapPaneli.SetActive(false);
        nasilOynanirPaneli.SetActive(false);
        eminMisinPaneli.SetActive(false);
        bolumGecisPaneli.SetActive(false);

    
        geriSayimText.gameObject.SetActive(false);

      
        MuzikCal(0);
    }

    private void Update()
    {
        //süreyi kontrol ettiðimiz kod
        if(oyunDevamEdiyor && kalanSure > 0)
        {
            kalanSure -= Time.deltaTime;

            sureText.text = Mathf.CeilToInt(kalanSure).ToString();

            if(kalanSure <= 0)
            {
                if(sfxSource != null && sesYanlis != null)
                {
                    sfxSource.PlayOneShot(sesYanlis);

                    OyunBitti();
                }
            }


        }
    }

    public void OyunuBaslat()
    {
        sfxSource.PlayOneShot(sesClick);

        girisPaneli.SetActive(false); 
        gamePanel.SetActive(true); 

        //açýlýþtaki ilk ayarlar
        toplamPuan = 0;
        kacinciSorudayiz = 0;
        puanText.text = "PUAN : 0";

        
        joker50Btn.interactable = true;
        jokerSkipBtn.interactable = true;
        jokerX2Btn.interactable = true;

        //geri sayim
        StartCoroutine(GeriSayimYap());

    }

        IEnumerator GeriSayimYap()
    {
        oyunDevamEdiyor = false; 
        geriSayimText.gameObject.SetActive(true); 

        geriSayimText.text = "3";
        yield return new WaitForSeconds(1); 

        geriSayimText.text = "2";
        yield return new WaitForSeconds(1); 

        geriSayimText.text = "1";
        yield return new WaitForSeconds(1); 

        geriSayimText.text = "BAÞLA!";
        yield return new WaitForSeconds(0.5f); 

        geriSayimText.gameObject.SetActive(false); 

        SiradakiSoruyaGec();
        }

    public void SiradakiSoruyaGec()
    {
        kacinciSorudayiz++; //ekranda yazan n.soru için her sorudan sonra 1 artýrdýk

        soruSayacText.text = kacinciSorudayiz + ".Soru";
        
        // --- BOLUM AYARLARI ---
        string kategori = "K1";
        string bolumAdi = "";
        int sure = 20;
        int bazPuan = 1000;
        int ResimveMuzikIndex = 0;

     
        if (kacinciSorudayiz <= 5) {  kategori = "K1"; bolumAdi = "KOLAY-1"; sure = 20; bazPuan = 1000; ResimveMuzikIndex = 1; }
        else if (kacinciSorudayiz <= 10) { kategori = "K2"; bolumAdi = "KOLAY-2"; sure = 20; bazPuan = 1000; ResimveMuzikIndex = 2; }
        else if (kacinciSorudayiz <= 15) { kategori = "O1"; bolumAdi = "ORTA-1"; sure = 30; bazPuan = 2000; ResimveMuzikIndex = 3; }
        else if (kacinciSorudayiz <= 20) { kategori = "O2"; bolumAdi = "ORTA-2"; sure = 30; bazPuan = 2000; ResimveMuzikIndex = 4; }
        else if (kacinciSorudayiz <= 25) { kategori = "Z1"; bolumAdi = "ZOR-1"; sure = 40; bazPuan = 3000; ResimveMuzikIndex = 5; }
        else { kategori = "Z2"; bolumAdi = "ZOR-2"; sure = 40; bazPuan = 3000; ResimveMuzikIndex = 6; }

        //bölüme göre arkaplaný deðiþtiren kod
        if (bolumResimleri.Length > ResimveMuzikIndex)
            BackgroundImage.sprite = bolumResimleri[ResimveMuzikIndex];

        //bölüme göre müziði deðiþtiren fonksiyon
        MuzikCal(ResimveMuzikIndex);

        //degerleri guncelleme
        oyunDevamEdiyor = true;
        kalanSure = sure;
        suankiSoruPuani = bazPuan;
        puanCarpani = 1.0f;
        ciftCevapHakkiVar = false;

        bolumAdiText.text = "BÖLÜM : " + bolumAdi;

        //soruyu cek
        suankiSoru = SoruYoneticisi.Instance.GetirSoru(kategori);
    
        //soruyu ekrana getirme kodu
        if(suankiSoru != null)
        {
            soruMetni.text = suankiSoru.soruMetni;
            textA.text = suankiSoru.secenekA;
            textB.text = suankiSoru.secenekB;
            textC.text = suankiSoru.secenekC;
            textD.text = suankiSoru.secenekD;

            ButonlariSifirla();
        }

        else
        {
            OyunBitti();
        }
    
    
    }

    public void SikTiklandi(string secilen)
    {
        sfxSource.PlayOneShot(sesClick);

        secilenSikGecisi = secilen; //cevabi tut
        oyunDevamEdiyor = true;// sureyi devam ettir
        eminMisinPaneli.SetActive(true); //emin misin panelini ac
    }

    public void CevabiOnayla()
    {
        sfxSource.PlayOneShot(sesClick);
        eminMisinPaneli.SetActive(false);
        GercekCevabiKontrolEt(secilenSikGecisi);
    }

    public void CevabiReddet()
    {
        sfxSource.PlayOneShot(sesClick);
        eminMisinPaneli.SetActive(false);
        oyunDevamEdiyor = true;
    }

    void GercekCevabiKontrolEt(string secilen)
    {
        if(secilen == suankiSoru.dogruCevap)
        {
            
            sfxSource.PlayOneShot(sesDogru);

            //sorunun puanýný toplam puana ekle
            int kazanilan = Mathf.RoundToInt(suankiSoruPuani * puanCarpani);
            toplamPuan += kazanilan;
            puanText.text = "PUAN : " + toplamPuan;
        
            //high score kontrol etme
            if(toplamPuan > highScore)
            {
                highScore = toplamPuan;
                highScoreText.text = "REKOR : " + highScore;
                PlayerPrefs.SetInt("HighScore", highScore);
                rekorKirildiMi = true; //rekor kýrýldý
            }

            if(toplamPuan >= sonrakiJokerHedefi)
            {
                JokerHediyeEt();
                sonrakiJokerHedefi += 10000;
            }
            
            StartCoroutine(YesilÝsikYak());
        }

        else
        {
            //yanlis cevap
            sfxSource.PlayOneShot(sesYanlis);

            if(ciftCevapHakkiVar)
            {
                ciftCevapHakkiVar = false;
                oyunDevamEdiyor = true;

                if (secilen == "A") butonA.interactable = false;
                if (secilen == "B") butonB.interactable = false;
                if (secilen == "C") butonC.interactable = false;
                if (secilen == "D") butonD.interactable = false;
            }

            else
            {
                OyunBitti();
            }
        }
    }

    IEnumerator YesilÝsikYak()
    {
        oyunDevamEdiyor = false; //son saniyeye gelince oyun bitmesin diye süreyi durdurduk
        dogruCevapPaneli.SetActive(true); 
        yield return new WaitForSeconds(1.0f); 
        dogruCevapPaneli.SetActive(false); 
    
        if(kacinciSorudayiz % 5 == 0)
        {
            BolumGecisEkraniAc();
        }

        else
        {
            SiradakiSoruyaGec();
        }
    
    }

    void JokerHediyeEt()
    {
        List<Button> kapaliJokerler = new List<Button>();

        //kullanýlmýþ jokerleri bir listeye aldýk
        if (!joker50Btn.interactable) kapaliJokerler.Add(joker50Btn);
        if (!jokerX2Btn.interactable) kapaliJokerler.Add(jokerX2Btn);
        if (!jokerSkipBtn.interactable) kapaliJokerler.Add(jokerSkipBtn);

        if(kapaliJokerler.Count > 0)
        {
            //hedef puaný aldýktan sonra kullanýlmýþ jokerlerden birini rastgele þekilde aktif ettik
            Button sansli = kapaliJokerler[Random.Range(0, kapaliJokerler.Count)];
            sansli.interactable = true;
            if (sesJoker != null) sfxSource.PlayOneShot(sesJoker);
        }

    }

    public void JokerYariYariya()
    {
        sfxSource.PlayOneShot(sesJoker);
        puanCarpani = 0.8f; //sorunun puaný düþtü
        joker50Btn.interactable = false;

        List<Button> yanlislar = new List<Button>();

        //sorudaki yanlýþ cevaplarý listeledik
        if (suankiSoru.dogruCevap != "A") yanlislar.Add(butonA);
        if (suankiSoru.dogruCevap != "B") yanlislar.Add(butonB);
        if (suankiSoru.dogruCevap != "C") yanlislar.Add(butonC);
        if (suankiSoru.dogruCevap != "D") yanlislar.Add(butonD);

        //ilk yanlýþ butonu sildik
        yanlislar[Random.Range(0, yanlislar.Count)].gameObject.SetActive(false);

        //ikinci yanlýþ butonu sildik
        Button silinecek2 = yanlislar[Random.Range(0, yanlislar.Count)];

        while(silinecek2.gameObject.activeSelf == false)
        {
            silinecek2 = yanlislar[Random.Range(0, yanlislar.Count)];
        }

        silinecek2.gameObject.SetActive(false);
    }

    public void JokerCiftCevap()
    {
        sfxSource.PlayOneShot(sesJoker);
        puanCarpani = 0.8f; //sorunun puaný düþtü
        ciftCevapHakkiVar = true;
        jokerX2Btn.interactable = false;
    }

    public void JokerSkip()
    {
        sfxSource.PlayOneShot(sesJoker);
        jokerSkipBtn.interactable = false;
        
        if(kacinciSorudayiz % 5 == 0)
        {
            BolumGecisEkraniAc();
        }

        else
        {
            SiradakiSoruyaGec();
        }
    }

    void ButonlariSifirla()
    {
        //her sorudan sonra butonlarý sýfýrladýk
        butonA.interactable = true; butonA.gameObject.SetActive(true);
        butonB.interactable = true; butonB.gameObject.SetActive(true);
        butonC.interactable = true; butonC.gameObject.SetActive(true);
        butonD.interactable = true; butonD.gameObject.SetActive(true);
     }

    void MuzikCal(int index)
    {
        if (musicSource.clip == bolumMuzikleri[index]) return;
        musicSource.clip = bolumMuzikleri[index];
        musicSource.Play();
    }

    void BolumGecisEkraniAc()
    {
        sfxSource.PlayOneShot(sesBolumBitisi);
        bolumGecisPaneli.SetActive(true);
        gamePanel.SetActive(false);

        int bitenBolumNo = kacinciSorudayiz / 5;

        string bitenBolumAdi = "";
        if (bitenBolumNo == 1) bitenBolumAdi = "KOLAY-1";
        else if (bitenBolumNo == 2) bitenBolumAdi = "KOLAY-2";
        else if (bitenBolumNo == 3) bitenBolumAdi = "ORTA-1";
        else if (bitenBolumNo == 4) bitenBolumAdi = "ORTA-2";
        else if (bitenBolumNo == 5) bitenBolumAdi = "ZOR-1";
        else if (bitenBolumNo == 6) bitenBolumAdi = "ZOR-2";

        // Yazýyý Ekrana Bas
        gecisMesajText.text = bitenBolumAdi + " BÖLÜMÜNÜ BAÞARIYLA GEÇTÝN!";

        //yýldýzlarý renklendiren kod
        for (int i = 0; i < bitenBolumNo; i++)
        {
            yildizlar[i].SetActive(true);

            if(i < bitenBolumNo)
            {
                yildizlar[i].GetComponent<Image>().color = new Color(1f, 0.84f, 0f, 1f);
            }

            else
            {
                yildizlar[i].GetComponent<Image>().color = Color.gray;
            }
        
        
        }
    
        //final ekraný
        if(bitenBolumNo >= 6)
        {
            gecisMesajText.text = "TÜM BÖLÜMLERÝ GEÇMEYÝ BAÞARDIN!"; // Final mesajý
            devamEtButonu.SetActive(false);
            finalMenuButonu.SetActive(true);
            finalCikisButonu.SetActive(true);
        }
    }

    public void BolumDevamEt()
    {
        sfxSource.PlayOneShot(sesClick);
        bolumGecisPaneli.SetActive(false);
        gamePanel.SetActive(true);
        SiradakiSoruyaGec();
    }

    void OyunBitti()
    {
        oyunDevamEdiyor = false;

        musicSource.Stop();
        gamePanel.SetActive(false);
        bitisPaneli.SetActive(true);
        bitisPuanText.text = "SKOR : " + toplamPuan; //skoru ekrana yazdý

        //high score kýrýldýysa onun ekraný için yapýlan þeyler
        if(rekorKirildiMi)
        {
            bitisBaslikText.text = "YENÝ REKOR!!";
            bitisBaslikText.color = new Color(1f, 0.84f, 0f, 1f);
            if (sesBolumBitisi != null) sfxSource.PlayOneShot(sesBolumBitisi);
            finalCikisButonu.SetActive(true);
            finalMenuButonu.SetActive(true);
            devamEtButonu.SetActive(false);
        }

        else
        {
            bitisBaslikText.text = "OYUN BÝTTÝ";
            bitisBaslikText.color = Color.red;
        }
    }

    public void SahneYenile()
    {
        sfxSource.PlayOneShot(sesClick);
        UnityEngine.SceneManagement.SceneManager.LoadScene(0); //kaybedince veya oyunu bitirince yeniden baþlamamýzý saðlayan kod
    }

    public void OyundanCik()
    {
        sfxSource.PlayOneShot(sesClick);
        Application.Quit();
    }

    public void NasilOynanirAc()
    {
        sfxSource.PlayOneShot(sesClick);
        nasilOynanirPaneli.SetActive(true);
    }

    public void NasilOynanirKapa()
    {
        sfxSource.PlayOneShot(sesClick);
        nasilOynanirPaneli.SetActive(false);
    }

    public void OyunuDurdur()
    {
        musicSource.Pause(); //müziði durdur
        sfxSource.PlayOneShot(sesClick);
        pausePaneli.SetActive(true);
        Time.timeScale = 0f; //geçen süreyi durdur
        oyunDevamEdiyor = false;
    }

    public void OyunuDevamEttir()
    {
        musicSource.UnPause(); //müziði devam ettir
        sfxSource.PlayOneShot(sesClick);
        pausePaneli.SetActive(false);
        Time.timeScale = 1f; //süreyi devam ettir
        oyunDevamEdiyor = true;
    }

    public void MuzikAcKapa()
    {
        sfxSource.PlayOneShot(sesClick);
        musicSource.mute = !musicSource.mute; //müzik açýksa kapat, kapalýysa aç

        //müziðin son durumunu(açýk veya kapalý) kaydeden kod
        PlayerPrefs.SetInt("MusicMute", musicSource.mute ? 1 : 0);
        
       
        if (musicSource.mute)
        {
            muzikButonResmi.color = Color.red;
            if (pauseMuzikButonResmi != null) pauseMuzikButonResmi.color = Color.red;
        }

        
        else
        {
            muzikButonResmi.color = Color.white;
            if (pauseMuzikButonResmi != null) pauseMuzikButonResmi.color = Color.white;
        }
    }

    public void SFXAcKapa()
    {
        sfxSource.PlayOneShot(sesClick);
        sfxSource.mute = !sfxSource.mute;

        //sesin son durumunu(açýk veya kapalý) kaydeden kod
        PlayerPrefs.SetInt("SFXMute", sfxSource.mute ? 1 : 0);

       
        if (sfxSource.mute)
        {
            sfxButonResmi.color = Color.red;
            if (pauseSfxButonResmi != null) pauseSfxButonResmi.color = Color.red;
        }

      
        else
        {
            sfxButonResmi.color = Color.white;
            if (pauseSfxButonResmi != null) pauseSfxButonResmi.color = Color.white;
        }
    }

}
