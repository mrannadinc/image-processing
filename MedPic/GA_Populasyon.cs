using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace MedPic
{
    public class GA_Populasyon
    {
        public int o_jenerasyon_sayisi = new int(); // Populasyonu sonlandırma kriteri olarak kullanılır
        public double o_pop_snlndrm_dgr = new double(); // Bireylerin Uygunluk Değerlerine Bağlı Olarak Populasyonu sonlandırma kriteri olarak kullanılır
        public static int o_mutasyon_ktsys = new int();
        public int o_birey_sayisi = new int();
        public string o_ebeveyn_secim_yontemi;
        public string o_caprazlama_yontemi;
        public string o_mutasyon_yontemi;
        public string o_sonlandirma_kriteri;



        public double o_pop_tplm_uygnlk_dgr = new double(); // Populasyonun o anki jenerasyon için uygunluk değerlerinin toplamını tutar
        public double[] o_pop_ort_uyg_deg;
        public int o_guncel_jen_num = new int(); // Populasyonun o anki jenerasyonunun numarasını tutar.


        public static string[] o_ebeveyn_secim_yontemler = new string[2] { "rulet", "ikili" };
        public static string[] o_caprazlama_yontemler = new string[4] { "caprazlama_1", "caprazlama_2", "caprazlama_3", "cprzlm_karisik" };
        public static string[] o_mutasyon_yontemler = new string[4] { "mutasyon_1", "mutasyon_2", "mutasyon_3", "mtsyn_karisik" };
        public static string[] o_sonlandirma_kriterleri = new string[3] { "JS", "UD", "JSUD" };
        public static string o_sayfa_adi = "Genetik_kNN_Test$";
        public static string o_pop_olstrm_zmn;


        public GA_Birey[] Populasyon;

        public static Random r1 = new Random(); // Rulet tekerleğini oluşturmada kullanılır
        public int[] o_ebeveyn_indeks = new int[2] { -1, -1 }; // ebeveyn indekslerini varsayılan olarak -1 -1 belirledik, atama yapılmazsa belli olsun diye
        public int o_en_iyi_birey_indeks = -1;
        public int o_en_kotu_birey_indeks = -1;
        public ArrayList o_en_iyi_birey_indeksler; // Populasyonun en iyi bireylerine ait indeks numaralarını tutar


        public ArrayList o_turnuva_indeksler;// turnuvaya katılacak olan bireylerin indeksleri


        GA_Birey ebeveyn_1;
        GA_Birey ebeveyn_2;

        GA_Birey cocuk_1;
        GA_Birey cocuk_2;



        public GA_Populasyon(int jenerasyon_sayisi, double sonlandirma_degeri, int birey_sayisi, string ebeveyn_secim_yontemi, string caprazlama_yontemi, string mutasyon_yontemi, string sonlandirma_kriter)
        {
            o_jenerasyon_sayisi = jenerasyon_sayisi;
            o_pop_snlndrm_dgr = sonlandirma_degeri;
            o_birey_sayisi = birey_sayisi;
            o_ebeveyn_secim_yontemi = ebeveyn_secim_yontemi;
            o_caprazlama_yontemi = caprazlama_yontemi;
            o_mutasyon_yontemi = mutasyon_yontemi;
            o_sonlandirma_kriteri = sonlandirma_kriter;

        }

        public GA_Birey m_Eniyi_Bireyi_Olustur()
        {


            m_PopulasyonYarat();

            m_UygnlkDgrHsplPoplsyn();


            for (o_guncel_jen_num = 0; o_guncel_jen_num < o_jenerasyon_sayisi; o_guncel_jen_num++)
            {

                m_PopOrtUygDegHspl(o_guncel_jen_num);


                if (m_UremeSonlandir())
                {

                    break;
                }

                else
                {
                    m_EbeveynSecimi();  // Çaprazlama için gerekli ebeveynlerin seçimi
                    m_Caprazlama();     // Mutasyona uğrayacak bireylerin oluşturulması
                    m_Mutasyon();
                    m_PopulasyonGuncelle();
                }

            }

            return Populasyon[o_en_iyi_birey_indeks];
        }

        private void m_PopulasyonYarat()
        {
            Populasyon = new GA_Birey[o_birey_sayisi];

            o_pop_ort_uyg_deg = new double[o_jenerasyon_sayisi]; // her bir jenerasyonun ortalama uygunluk değerini tutar

            for (int i = 0; i < o_birey_sayisi; i++)
            {
                Populasyon[i] = new GA_Birey();
                GA_Birey.m_BireyKontrol(Populasyon[i].o_renk_katsayilari, "m_PopulasyonYarat");
            }

            o_pop_olstrm_zmn = DateTime.Now.ToShortDateString() + "//" + DateTime.Now.ToLongTimeString() + "//" + Fitness_Function.o_function_name;
        }

        private void m_PopulasyonGuncelle()
        {
            // Mutasyon Sonrası Oluşan Çocuk Bireylerin Uygunluk Değerlerinin Hesaplanması

            m_UygnlkDgrHsplBirey(cocuk_1);
            m_UygnlkDgrHsplBirey(cocuk_2);


            // Populasyondaki Bireylerden Uygunluk Değeri En Kötü Olan Bireyin Bulunması
            m_EnKotuBireyScm();

            // Çocuk bireylerden iyi olanın populasyondaki en kötü bireyin yerini alması

            if (cocuk_1.o_uygunluk_degeri > cocuk_2.o_uygunluk_degeri)
            {
                Populasyon[o_en_kotu_birey_indeks] = cocuk_1;
            }
            else
            {
                Populasyon[o_en_kotu_birey_indeks] = cocuk_2;
            }
        }


        /// 
        /// Populasyondaki Tüm Bireylerin Uygunluk Değerlerini Hesaplar
        /// 
        private void m_UygnlkDgrHsplPoplsyn()
        {

            for (int i = 0; i < o_birey_sayisi; i++)
            {
                Fitness_Function.m_Fitness_Hesapla(Populasyon[i]);
            }
        }

        /// 
        /// Bireyin Uygunluk Değerini Hesaplar
        /// 
        private void m_UygnlkDgrHsplBirey(GA_Birey Birey)
        {

            Fitness_Function.m_Fitness_Hesapla(Birey);

        }
        /// 
        /// Güncel Populasyonun O Anki Bireylerinin Uygunluk Değerlerinin Toplamını Hesaplar
        /// 
        private void m_PopTplmUygDegHsp()
        {
            o_pop_tplm_uygnlk_dgr = 0d;
            for (int i = 0; i < o_birey_sayisi; i++)
            {
                o_pop_tplm_uygnlk_dgr += Populasyon[i].o_uygunluk_degeri;
            }
        }

        /// 
        /// Güncel Populasyondaki bireylerin Uygunluk değerlerini ortalaması
        /// 
        private void m_PopOrtUygDegHspl(int jenerasyon_no)
        {
            m_PopTplmUygDegHsp();
            o_pop_ort_uyg_deg[jenerasyon_no] = o_pop_tplm_uygnlk_dgr / o_birey_sayisi;

        }

        /// 
        /// Bireylerin Uygunluk Değerlerinin Populasyona Göre Yüzdelerini Hesaplar
        /// 
        private void m_BireyUygYzdHspl()
        {

            for (int i = 0; i < o_birey_sayisi; i++)
            {
                Populasyon[i].o_uygunluk_yuzdesi = 100d * ((Populasyon[i].o_uygunluk_degeri) / (o_pop_tplm_uygnlk_dgr));
            }
        }

        /// 
        /// Populasyon İçerisinden Uygunluk Değeri En Küçük Olan Bireyi Araştırıyoruz
        /// 
        private void m_EnKotuBireyScm()
        {
            double gecici_deger = double.MaxValue;

            for (int i = 0; i < o_birey_sayisi; i++)
            {
                if (Populasyon[i].o_uygunluk_degeri < gecici_deger) // En küçük uygunluk değerine sahip bireyi araştırıyoruz
                {
                    gecici_deger = Populasyon[i].o_uygunluk_degeri;
                    o_en_kotu_birey_indeks = i;
                }
            }


        }

        /// 
        /// Hangi Ebeveyn Seçim Yöntemi Kullanılırsa Kullanılsın Öncelikle Populasyondaki Bireylerin Uygunluk Değerleri Belirlenmelidir
        /// 
        private void m_EbeveynSecimi()
        {



            if (o_ebeveyn_secim_yontemi == o_ebeveyn_secim_yontemler[0])
            {
                m_Rulet_Ebeveyn();
            }
            else if (o_ebeveyn_secim_yontemi == o_ebeveyn_secim_yontemler[1])
            {
                m_Ikili_Ebeveyn();
            }
            else
            {
                //
            }
        }

        private void m_Rulet_Ebeveyn()
        {

            double gecici_sayi = new double(); // rulet tekerleğinde üretilen rastgele sayı
            int ebeveyn_sayisi = new int(); // her ebeveyn seçildiğinde bir artar 2 olduğunda ebeveynler tamamdır



            // i.	Bireylerin uygunluk değeri bir önceki adımda hesaplandı


            //ii.	Toplam Uygunluk Değeri Hesaplanır 

            m_PopTplmUygDegHsp();

            //iii.  Bireylerin yüzdeleri hesaplanır: 
            m_BireyUygYzdHspl();


            // iv.  Rulet tekerleği oluşturulur: 



            //v.	Rastgele sayı üretimi: 

            //vi.	Ebeveynlerin belirlenmesi: 

            for (; ; ) // ebeveynleri oluşturuncaya kadar
            {
                gecici_sayi = (double)r1.Next(0, 100);
                double toplam = new double();
                for (int j = 0; j < o_birey_sayisi; j++)
                {
                    toplam += Populasyon[j].o_uygunluk_yuzdesi;

                    if (gecici_sayi <= toplam && o_ebeveyn_indeks.Contains(j) == false)
                    {
                        o_ebeveyn_indeks[ebeveyn_sayisi] = j;
                        ebeveyn_sayisi += 1;
                        break;
                    }

                }
                if (ebeveyn_sayisi >= 2)
                {
                    break;
                }
            }

        }


        private void m_Ikili_Ebeveyn()
        {

            // Populasyon içerisinden rastgele dört birey seç (her turnuva için 2 birey)

            o_turnuva_indeksler = new ArrayList();
            o_turnuva_indeksler.Clear();
            int uretilen_indeks = new int();

            for (; ; ) // 4 adet farklı indeksli birey buluncaya kadar
            {
                uretilen_indeks = r1.Next(0, o_birey_sayisi);

                if (o_turnuva_indeksler.Contains(uretilen_indeks) == false)
                {
                    o_turnuva_indeksler.Add(uretilen_indeks);
                    if (o_turnuva_indeksler.Count >= 4)
                    {
                        break;
                    }
                }

            }

            // Seçilen bireylerin uygunluk değerlerini hesaplamaya tekrar gerek yok zaten m_EbeveynSecimi adı altında hesaplandı

            //  m_UygnlkDgrHsplBirylr(o_turnuva_indeksler);
            // Turnuvaları başlat ve her turnuvanın en iyi bireyini seç

            m_ikili_turnuva(o_turnuva_indeksler);
            // En iyi iki birey ebeveynlerdir

        }

        private void m_ikili_turnuva(ArrayList bireyler)
        {
            int a = 0;
            //int dongu_sayisi=bireyler.Count
            for (int i = 0; i <= (bireyler.Count / 2); i = i + 2)
            {

                if (Populasyon[Convert.ToInt32(bireyler[i])].o_uygunluk_degeri > Populasyon[Convert.ToInt32(bireyler[i])].o_uygunluk_degeri)
                {
                    o_ebeveyn_indeks[a] = i;
                }
                else
                {
                    o_ebeveyn_indeks[a] = i + 1;
                }

                a++;
            }

        }

        private bool m_UremeSonlandir()
        {
            bool sonlandirma = false;

            if (o_sonlandirma_kriteri == o_sonlandirma_kriterleri[0]) // Jenerasyon Sayısına Bağlı Sonlandırma İşlemi
            {
                sonlandirma = m_Kriter_JS(sonlandirma);
            }
            else if (o_sonlandirma_kriteri == o_sonlandirma_kriterleri[1])// Uygunluk Değerine Bağlı Sonlandırma Kriteri
            {
                sonlandirma = m_Kriter_UD(sonlandirma);
            }

            else if (o_sonlandirma_kriteri == o_sonlandirma_kriterleri[2])// Jenerasyon Sayısı ve Uygunluk Değerine Bağlı Sonlandırma Kriteri
            {
                sonlandirma = m_Kriter_JS(sonlandirma);
                sonlandirma = m_Kriter_UD(sonlandirma);
            }
            else
            {
                // ...
            }

            return sonlandirma;

        }


        private bool m_Kriter_JS(bool sonlandirma)// Jenerasyon Sayısına Bağlı Sonlandırma İşlemi
        {
            if (o_guncel_jen_num == o_jenerasyon_sayisi - 1)
            {
                m_EnBykUygDgrBireyBul();
                sonlandirma = true;
            }
            return sonlandirma;
        }
        private bool m_Kriter_UD(bool sonlandirma)
        {

            // Populasyon taranacak ve uygunluk değeri pop_sonlandırma değerine büyük eşit olan birey bulunursa populasyondaki en iyi birey seçilerek populasyon sonlandırılacak
            for (int i = 0; i < o_birey_sayisi; i++)
            {
                if (Populasyon[i].o_uygunluk_degeri >= o_pop_snlndrm_dgr) // Uygunluk değeri şartı gerçekleşmiş
                {
                    // Sonlandırma değeri şartını sağlayan bireyleri bul
                    m_SnlndrmSglynBrylrBul();

                    // En büyük uygunluk değerine sahip olan bireyi (en iyi bireyi) bul
                    m_EnBykUygDgrBireyBul();
                    sonlandirma = true;

                    break;
                }
            }
            return sonlandirma;
        }

        /// <summary>
        /// populasyon Sonlandırma değerinden daha büyük uygunluk değerine sahip olan bireylerin indekslerini bulur ve  o_en_iyi_birey_indeksler listesini oluşturur
        /// </summary>
        private void m_SnlndrmSglynBrylrBul()
        {
            o_en_iyi_birey_indeksler = new ArrayList();


            for (int i = 0; i < o_birey_sayisi; i++)
            {
                if (Populasyon[i].o_uygunluk_degeri >= o_pop_snlndrm_dgr)
                {
                    o_en_iyi_birey_indeksler.Add(i);
                }
            }
        }

        /// <summary>
        /// /// populasyon Sonlandırma değerinden daha büyük uygunluk değerine sahip olan bireylerin arasından uygunluk değeri en büyük bireyin indeksi bulunur ve populasyonun   o_en_iyi_birey_indeks değeri olarak atanır
        /// </summary>
        private void m_EnBykUygDgrBireyBul()
        {

            double gecici_enbuyuk = double.MinValue;
            o_en_iyi_birey_indeks = -1000;

            for (int i = 0; i < o_birey_sayisi; i++)
            {
                if (Populasyon[i].o_uygunluk_degeri > gecici_enbuyuk)
                {
                    gecici_enbuyuk = Populasyon[i].o_uygunluk_degeri;
                    o_en_iyi_birey_indeks = i;
                }
            }

        }

        private void m_Caprazlama()
        {
            ebeveyn_1 = new GA_Birey();
            GA_Birey.m_BireyKontrol(ebeveyn_1.o_renk_katsayilari, "m_Caprazlama_Ebeveyn_1_Yaratma");

            ebeveyn_2 = new GA_Birey();
            GA_Birey.m_BireyKontrol(ebeveyn_2.o_renk_katsayilari, "m_Caprazlama_Ebeveyn_2_Yaratma");

            cocuk_1 = new GA_Birey();
            GA_Birey.m_BireyKontrol(cocuk_1.o_renk_katsayilari, "m_Caprazlama_Cocuk_1_Yaratma");


            cocuk_2 = new GA_Birey();
            GA_Birey.m_BireyKontrol(cocuk_2.o_renk_katsayilari, "m_Caprazlama_Cocuk_2_Yaratma");


            ebeveyn_1 = Populasyon[o_ebeveyn_indeks[0]];
            ebeveyn_2 = Populasyon[o_ebeveyn_indeks[1]];
            GA_Birey.m_BireyKontrol(ebeveyn_1.o_renk_katsayilari, "m_Caprazlama_Ebeveyn_1_Atama");
            GA_Birey.m_BireyKontrol(ebeveyn_2.o_renk_katsayilari, "m_Caprazlama_Ebeveyn_2_Atama");

            if (o_caprazlama_yontemi == o_caprazlama_yontemler[0])
            {
                m_Cprzlm_1TekNkt();
            }
            else if (o_caprazlama_yontemi == o_caprazlama_yontemler[1])
            {
                m_Cprzlm_2IkiNkt();
            }
            else if (o_caprazlama_yontemi == o_caprazlama_yontemler[2])
            {
                m_Cprzlm_3TekBcml();
            }

            else if (o_caprazlama_yontemi == o_caprazlama_yontemler[3])
            {
                m_Cprzlm_4Krsk();
            }
            else
            {
                //...
            }
        }


        private void m_Cprzlm_1TekNkt()
        {

            //int rgb = r1.Next(0, RGB_Parametreler.o_renk_katsayilari.GetLength(0));  // r-g-b den hangisinde çaprazlama yapılacağını belirler


            for (int i = 0; i < RGB_Parametreler.o_esik_sayisi; i++)
            {
                cocuk_1.o_renk_katsayilari[0, i] = ebeveyn_1.o_renk_katsayilari[0, i];
                cocuk_2.o_renk_katsayilari[0, i] = ebeveyn_2.o_renk_katsayilari[0, i];

            }
            GA_Birey.m_BireyKontrol(ebeveyn_2.o_renk_katsayilari, "m_Cprzlm_1TekNkt_Ebeveyn_2");
            GA_Birey.m_BireyKontrol(cocuk_2.o_renk_katsayilari, "m_Cprzlm_1TekNkt_Cocuk_2");
            GA_Birey.m_BireyKontrol(ebeveyn_1.o_renk_katsayilari, "m_Cprzlm_1TekNkt_Ebeveyn_1");
            GA_Birey.m_BireyKontrol(cocuk_1.o_renk_katsayilari, "m_Cprzlm_1TekNkt_Cocuk_1");
        }

        private void m_Cprzlm_2IkiNkt()
        {

            // int rgb = r1.Next(0, RGB_Parametreler.o_renk_katsayilari.GetLength(0));  // r-g-b den hangisinde çaprazlama yapılacağını belirler
            int rgb2;

            for (; ; )
            {
                rgb2 = r1.Next(0, RGB_Parametreler.o_renk_katsayilari.GetLength(0));
                if (0 != rgb2)
                {
                    break;
                }
            }


            for (int i = 0; i < RGB_Parametreler.o_esik_sayisi; i++)
            {
                cocuk_1.o_renk_katsayilari[0, i] = ebeveyn_1.o_renk_katsayilari[0, i];
                cocuk_2.o_renk_katsayilari[0, i] = ebeveyn_2.o_renk_katsayilari[0, i];
                cocuk_1.o_renk_katsayilari[0, rgb2] = ebeveyn_1.o_renk_katsayilari[0, rgb2];
                cocuk_2.o_renk_katsayilari[0, rgb2] = ebeveyn_2.o_renk_katsayilari[0, rgb2];
            }
            GA_Birey.m_BireyKontrol(ebeveyn_1.o_renk_katsayilari, "m_Cprzlm_2IkiNkt_Ebeveyn_1");
            GA_Birey.m_BireyKontrol(ebeveyn_2.o_renk_katsayilari, "m_Cprzlm_2IkiNkt_Ebeveyn_2");
            GA_Birey.m_BireyKontrol(cocuk_1.o_renk_katsayilari, "m_Cprzlm_2IkiNkt_Cocuk_1");
            GA_Birey.m_BireyKontrol(cocuk_2.o_renk_katsayilari, "m_Cprzlm_2IkiNkt_Cocuk_2");

        }

        private void m_Cprzlm_3TekBcml()
        {
            //int rgb = r1.Next(0, RGB_Parametreler.o_renk_katsayilari.GetLength(0));  // r-g-b den hangisinde çaprazlama yapılacağını belirler
            int rgb2;

            for (; ; )
            {
                rgb2 = r1.Next(0, RGB_Parametreler.o_renk_katsayilari.GetLength(0));
                if (0 != rgb2)
                {
                    break;
                }
            }


            for (int i = 0; i < RGB_Parametreler.o_esik_sayisi; i++)
            {
                cocuk_1.o_renk_katsayilari[0, i] = ebeveyn_2.o_renk_katsayilari[0, i];
                cocuk_2.o_renk_katsayilari[0, i] = ebeveyn_1.o_renk_katsayilari[0, i];
                cocuk_1.o_renk_katsayilari[0, rgb2] = ebeveyn_2.o_renk_katsayilari[0, rgb2];
                cocuk_2.o_renk_katsayilari[0, rgb2] = ebeveyn_1.o_renk_katsayilari[0, rgb2];
            }

            GA_Birey.m_BireyKontrol(ebeveyn_1.o_renk_katsayilari, "m_Cprzlm_3TekBcml_Ebeveyn_1");
            GA_Birey.m_BireyKontrol(ebeveyn_2.o_renk_katsayilari, "m_Cprzlm_3TekBcml_Ebeveyn_2");
            GA_Birey.m_BireyKontrol(cocuk_1.o_renk_katsayilari, "m_Cprzlm_3TekBcml_Cocuk_1");
            GA_Birey.m_BireyKontrol(cocuk_2.o_renk_katsayilari, "m_Cprzlm_3TekBcml_Cocuk_2");
        }

        private void m_Cprzlm_4Krsk()
        {
            // diğer 3 yöntemi rastgele kullanır

            sbyte cprzlm_yontem = Convert.ToSByte(r1.Next(1, o_caprazlama_yontemler.Length - 1)); //

            if (cprzlm_yontem == 1)
            {
                m_Cprzlm_1TekNkt();
            }
            else if (cprzlm_yontem == 2)
            {
                m_Cprzlm_2IkiNkt();
            }
            else
            {
                m_Cprzlm_3TekBcml();
            }

        }

        private void m_Mutasyon()
        {
            if (o_jenerasyon_sayisi % 100 == 0)  // 100 jenerasyonda bir kez mutasyon
            {


                if (o_mutasyon_yontemi == o_mutasyon_yontemler[0])
                {
                    m_Mtsyn_1BitTersCvr();
                }
                else if (o_mutasyon_yontemi == o_mutasyon_yontemler[1])
                {
                    m_Mtsyn_2SiraDegis();
                }
                else if (o_mutasyon_yontemi == o_mutasyon_yontemler[2])
                {
                    m_Mtsyn_3SayiEkleCkr();
                }
                else if (o_mutasyon_yontemi == o_mutasyon_yontemler[3])
                {
                    m_Mtsyn_4Krsk();
                }
                else
                {
                    //... Exception
                }

            }

        }


        private void m_Mtsyn_1BitTersCvr()
        {

            //int rgb = r1.Next(0, RGB_Parametreler.o_renk_katsayilari.GetLength(0));  // r-g-b den hangisinde mutasyon yapılacağını belirler
            int mtsyn_nkts = r1.Next(0, RGB_Parametreler.o_esik_sayisi - 1);

            int min_deger, max_deger;

            if (mtsyn_nkts == 0)
            {
                min_deger = 0;
                max_deger = cocuk_1.o_renk_katsayilari[0, 1] - 1;
                if (m_MutasyonOnay(min_deger, max_deger))
                {
                    cocuk_1.o_renk_katsayilari[0, mtsyn_nkts] = r1.Next(min_deger, max_deger);
                }


                max_deger = cocuk_2.o_renk_katsayilari[0, 1] - 1;
                if (m_MutasyonOnay(min_deger, max_deger))
                {
                    cocuk_2.o_renk_katsayilari[0, mtsyn_nkts] = r1.Next(min_deger, max_deger);
                }



            }
            else if (mtsyn_nkts == RGB_Parametreler.o_esik_sayisi - 1)
            {
                min_deger = cocuk_1.o_renk_katsayilari[0, mtsyn_nkts - 1] + 1;
                max_deger = 256;


                if (m_MutasyonOnay(min_deger, max_deger))
                {
                    cocuk_1.o_renk_katsayilari[0, mtsyn_nkts] = r1.Next(min_deger, max_deger);
                }

                min_deger = cocuk_2.o_renk_katsayilari[0, mtsyn_nkts - 1] + 1;

                if (m_MutasyonOnay(min_deger, max_deger))
                {
                    cocuk_2.o_renk_katsayilari[0, mtsyn_nkts] = r1.Next(min_deger, max_deger);

                }


            }
            else
            {
                min_deger = cocuk_1.o_renk_katsayilari[0, mtsyn_nkts - 1] + 1;
                max_deger = cocuk_1.o_renk_katsayilari[0, mtsyn_nkts + 1] - 1;
                if (m_MutasyonOnay(min_deger, max_deger))
                {
                    cocuk_1.o_renk_katsayilari[0, mtsyn_nkts] = r1.Next(min_deger, max_deger);

                }


                min_deger = cocuk_2.o_renk_katsayilari[0, mtsyn_nkts - 1] + 1;
                max_deger = cocuk_2.o_renk_katsayilari[0, mtsyn_nkts + 1] - 1;

                if (m_MutasyonOnay(min_deger, max_deger))
                {
                    cocuk_2.o_renk_katsayilari[0, mtsyn_nkts] = r1.Next(min_deger, max_deger);

                }


            }
            GA_Birey.m_BireyKontrol(cocuk_1.o_renk_katsayilari, "m_Mtsyn_1BitTersCvr_Cocuk_1");
            GA_Birey.m_BireyKontrol(cocuk_2.o_renk_katsayilari, "m_Mtsyn_1BitTersCvr_Cocuk_2");

        }

        private void m_Mtsyn_2SiraDegis()
        {



            int mtsyn_nkts = r1.Next(0, RGB_Parametreler.o_esik_sayisi - 1);

            int min_deger, max_deger;

            if (mtsyn_nkts == 0)
            {
                min_deger = 0;
                max_deger = cocuk_1.o_renk_katsayilari[0, mtsyn_nkts + 1] - 1;
                if (m_MutasyonOnay(min_deger, max_deger))
                {
                    cocuk_1.o_renk_katsayilari[0, mtsyn_nkts] = r1.Next(min_deger, max_deger);

                }

                max_deger = cocuk_1.o_renk_katsayilari[0, mtsyn_nkts + 1] - 1;
                if (m_MutasyonOnay(min_deger, max_deger))
                {
                    cocuk_1.o_renk_katsayilari[0, mtsyn_nkts] = r1.Next(min_deger, max_deger);

                }



                max_deger = cocuk_2.o_renk_katsayilari[0, mtsyn_nkts + 1] - 1;

                if (m_MutasyonOnay(min_deger, max_deger))
                {
                    cocuk_2.o_renk_katsayilari[0, mtsyn_nkts] = r1.Next(min_deger, max_deger);

                }

                max_deger = cocuk_2.o_renk_katsayilari[0, mtsyn_nkts + 1] - 1;

                if (m_MutasyonOnay(min_deger, max_deger))
                {
                    cocuk_2.o_renk_katsayilari[0, mtsyn_nkts] = r1.Next(min_deger, max_deger);

                }




            }
            else if (mtsyn_nkts == RGB_Parametreler.o_esik_sayisi - 1)
            {
                min_deger = cocuk_1.o_renk_katsayilari[0, mtsyn_nkts - 1] + 1;
                max_deger = 256;

                if (m_MutasyonOnay(min_deger, max_deger))
                {
                    cocuk_1.o_renk_katsayilari[0, mtsyn_nkts] = r1.Next(min_deger, max_deger);

                }

                min_deger = cocuk_1.o_renk_katsayilari[0, mtsyn_nkts - 1] + 1;
                if (m_MutasyonOnay(min_deger, max_deger))
                {
                    cocuk_1.o_renk_katsayilari[0, mtsyn_nkts] = r1.Next(min_deger, max_deger);

                }


                min_deger = cocuk_2.o_renk_katsayilari[0, mtsyn_nkts - 1] + 1;

                if (m_MutasyonOnay(min_deger, max_deger))
                {
                    cocuk_2.o_renk_katsayilari[0, mtsyn_nkts] = r1.Next(min_deger, max_deger);

                }

                min_deger = cocuk_2.o_renk_katsayilari[0, mtsyn_nkts - 1] + 1;
                if (m_MutasyonOnay(min_deger, max_deger))
                {
                    cocuk_2.o_renk_katsayilari[0, mtsyn_nkts] = r1.Next(min_deger, max_deger);

                }


            }
            else
            {
                min_deger = cocuk_1.o_renk_katsayilari[0, mtsyn_nkts - 1] + 1;
                max_deger = cocuk_1.o_renk_katsayilari[0, mtsyn_nkts + 1] - 1;

                if (m_MutasyonOnay(min_deger, max_deger))
                {
                    cocuk_1.o_renk_katsayilari[0, mtsyn_nkts] = r1.Next(min_deger, max_deger);

                }


                min_deger = cocuk_1.o_renk_katsayilari[0, mtsyn_nkts - 1] + 1;
                max_deger = cocuk_1.o_renk_katsayilari[0, mtsyn_nkts + 1] - 1;

                if (m_MutasyonOnay(min_deger, max_deger))
                {
                    cocuk_1.o_renk_katsayilari[0, mtsyn_nkts] = r1.Next(min_deger, max_deger);

                }



                min_deger = cocuk_2.o_renk_katsayilari[0, mtsyn_nkts - 1] + 1;
                max_deger = cocuk_2.o_renk_katsayilari[0, mtsyn_nkts + 1] - 1;
                if (m_MutasyonOnay(min_deger, max_deger))
                {
                    cocuk_2.o_renk_katsayilari[0, mtsyn_nkts] = r1.Next(min_deger, max_deger);

                }

                min_deger = cocuk_2.o_renk_katsayilari[0, mtsyn_nkts - 1] + 1;
                max_deger = cocuk_2.o_renk_katsayilari[0, mtsyn_nkts + 1] - 1;
                if (m_MutasyonOnay(min_deger, max_deger))
                {
                    cocuk_2.o_renk_katsayilari[0, mtsyn_nkts] = r1.Next(min_deger, max_deger);

                }


            }
            GA_Birey.m_BireyKontrol(cocuk_1.o_renk_katsayilari, "m_Mtsyn_2SiraDegis_Cocuk_1");
            GA_Birey.m_BireyKontrol(cocuk_2.o_renk_katsayilari, "m_Mtsyn_2SiraDegis_Cocuk_2");
        }

        private void m_Mtsyn_3SayiEkleCkr()
        {
            //int rgb = r1.Next(0, RGB_Parametreler.o_renk_katsayilari.GetLength(0));  // r-g-b den hangisinde çaprazlama yapılacağını belirler

            int mtsyn_nkts = r1.Next(0, RGB_Parametreler.o_esik_sayisi - 1);


            if (mtsyn_nkts == 0)
            {
                if (mtsyn_nkts % 2 == 0)  // mutasyon noktası çift ise toplama tek ise çıkartma işlemi uygulanacak
                {
                    if (o_mutasyon_ktsys + cocuk_1.o_renk_katsayilari[0, mtsyn_nkts] < cocuk_1.o_renk_katsayilari[0, mtsyn_nkts + 1])
                    {
                        cocuk_1.o_renk_katsayilari[0, mtsyn_nkts] = o_mutasyon_ktsys + cocuk_1.o_renk_katsayilari[0, mtsyn_nkts];
                    }
                    if (o_mutasyon_ktsys + cocuk_2.o_renk_katsayilari[0, mtsyn_nkts] < cocuk_2.o_renk_katsayilari[0, mtsyn_nkts + 1])
                    {
                        cocuk_2.o_renk_katsayilari[0, mtsyn_nkts] = o_mutasyon_ktsys + cocuk_2.o_renk_katsayilari[0, mtsyn_nkts];
                    }
                }

                else
                {
                    if (cocuk_1.o_renk_katsayilari[0, mtsyn_nkts] - o_mutasyon_ktsys > 0)
                    {
                        cocuk_1.o_renk_katsayilari[0, mtsyn_nkts] = cocuk_1.o_renk_katsayilari[0, mtsyn_nkts] - o_mutasyon_ktsys;
                    }
                    if (cocuk_2.o_renk_katsayilari[0, mtsyn_nkts] - o_mutasyon_ktsys > 0)
                    {
                        cocuk_2.o_renk_katsayilari[0, mtsyn_nkts] = cocuk_2.o_renk_katsayilari[0, mtsyn_nkts] - o_mutasyon_ktsys;
                    }
                }



            }
            else if (mtsyn_nkts == RGB_Parametreler.o_esik_sayisi - 1)
            {

                if (mtsyn_nkts % 2 == 0)
                {
                    if (o_mutasyon_ktsys + cocuk_1.o_renk_katsayilari[0, mtsyn_nkts] <= 255)
                    {
                        cocuk_1.o_renk_katsayilari[0, mtsyn_nkts] = o_mutasyon_ktsys + cocuk_1.o_renk_katsayilari[0, mtsyn_nkts];
                    }
                    if (o_mutasyon_ktsys + cocuk_2.o_renk_katsayilari[0, mtsyn_nkts] <= 255)
                    {
                        cocuk_2.o_renk_katsayilari[0, mtsyn_nkts] = o_mutasyon_ktsys + cocuk_2.o_renk_katsayilari[0, mtsyn_nkts];
                    }
                }

                else
                {
                    if (cocuk_1.o_renk_katsayilari[0, mtsyn_nkts] - o_mutasyon_ktsys > cocuk_1.o_renk_katsayilari[0, mtsyn_nkts - 1])
                    {
                        cocuk_1.o_renk_katsayilari[0, mtsyn_nkts] = cocuk_1.o_renk_katsayilari[0, mtsyn_nkts] - o_mutasyon_ktsys;
                    }


                    if (cocuk_2.o_renk_katsayilari[0, mtsyn_nkts] - o_mutasyon_ktsys > cocuk_2.o_renk_katsayilari[0, mtsyn_nkts - 1])
                    {
                        cocuk_2.o_renk_katsayilari[0, mtsyn_nkts] = cocuk_2.o_renk_katsayilari[0, mtsyn_nkts] - o_mutasyon_ktsys;
                    }
                }
            }

            else
            {

                if (mtsyn_nkts % 2 == 0)
                {
                    if (o_mutasyon_ktsys + cocuk_1.o_renk_katsayilari[0, mtsyn_nkts] < cocuk_1.o_renk_katsayilari[0, mtsyn_nkts + 1])
                    {
                        cocuk_1.o_renk_katsayilari[0, mtsyn_nkts] = o_mutasyon_ktsys + cocuk_1.o_renk_katsayilari[0, mtsyn_nkts];
                    }
                    if (o_mutasyon_ktsys + cocuk_2.o_renk_katsayilari[0, mtsyn_nkts] < cocuk_2.o_renk_katsayilari[0, mtsyn_nkts + 1])
                    {
                        cocuk_2.o_renk_katsayilari[0, mtsyn_nkts] = o_mutasyon_ktsys + cocuk_2.o_renk_katsayilari[0, mtsyn_nkts];
                    }
                }
                else
                {
                    if (cocuk_1.o_renk_katsayilari[0, mtsyn_nkts] - o_mutasyon_ktsys > cocuk_1.o_renk_katsayilari[0, mtsyn_nkts - 1])
                    {
                        cocuk_1.o_renk_katsayilari[0, mtsyn_nkts] = cocuk_1.o_renk_katsayilari[0, mtsyn_nkts] - o_mutasyon_ktsys;
                    }


                    if (cocuk_2.o_renk_katsayilari[0, mtsyn_nkts] - o_mutasyon_ktsys > cocuk_2.o_renk_katsayilari[0, mtsyn_nkts - 1])
                    {
                        cocuk_2.o_renk_katsayilari[0, mtsyn_nkts] = cocuk_2.o_renk_katsayilari[0, mtsyn_nkts] - o_mutasyon_ktsys;
                    }
                }

            }
            GA_Birey.m_BireyKontrol(cocuk_1.o_renk_katsayilari, "m_Mtsyn_3SayiEkleCkr_Cocuk_1");
            GA_Birey.m_BireyKontrol(cocuk_2.o_renk_katsayilari, "m_Mtsyn_3SayiEkleCkr_Cocuk_2");
        }


        private void m_Mtsyn_4Krsk()
        {
            sbyte mtsyn_yontem = Convert.ToSByte(r1.Next(0, o_mutasyon_yontemler.Length)); // sıfırdan başlatma sebebi 1ve 2 için yöntem 2 uygulanarak yöntem 2 nin uygulanma şansını artırmak 3 ve 4 için yöntem 3 uygulanarak yöntem 1 ün uygulanma şansını artırmak
            if (mtsyn_yontem == 0)
            {
                m_Mtsyn_1BitTersCvr();
            }
            else if (mtsyn_yontem == 1 || mtsyn_yontem == 2)
            {
                m_Mtsyn_2SiraDegis();
            }
            else
            {
                m_Mtsyn_3SayiEkleCkr();
            }
        }
        private bool m_MutasyonOnay(int min_deger, int max_deger)
        {
            if (max_deger - min_deger >= 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

    }
    
}
