using System.Collections.Generic;


[System.Serializable]
public class Soru
{
    public int id; 

    public string kategori; //sorunun kategorisi
    public string soruMetni;

    //Secenekler (a,b,c,d)
    public string secenekA;
    public string secenekB;
    public string secenekC;
    public string secenekD;

    public string dogruCevap; //sadece harf tutulacak burada

}
