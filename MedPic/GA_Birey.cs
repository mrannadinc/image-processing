using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MedPic
{
    public class GA_Birey
    {

        public int[,] o_renk_katsayilari = new int[1, RGB_Parametreler.o_esik_sayisi];

        public double o_uygunluk_degeri;
        public double o_uygunluk_yuzdesi;
        public int o_populasyon_indeksi;
        public static Random r1 = new Random();

        /// 
        /// yapıcı metod bireyler yaratılırken onların ağırlık katsayılarını ve uygunluk değerini varsayılan değelere atıyor
        ///
        public GA_Birey()
        {
            int b1 = r1.Next();
            if (b1 % 2 == 0)
            {
                m_Renk_Katsayi_Olustur_1();
            }
            else
            {
                m_Renk_Katsayi_Olustur_2();
            }
            m_Uygunluk_Deger_Olustur();
        }


        /// 
        /// yeni yaratılan bir bireyin  renk katsayıları sıfır ile 255 arasında değerler ile oluşturuluyor
        /// 

        private void m_Renk_Katsayi_Olustur_1()
        {
            int min, max, gecici;
            double gecici2;


            for (int i = 0; i < o_renk_katsayilari.GetLength(0); i++)
            {
                min = 0;
                max = 0;

                for (int j = 0; j < o_renk_katsayilari.GetLength(1); j++)
                {
                    gecici = o_renk_katsayilari.GetLength(1) + 1 - j;
                    gecici2 = 256 * Math.Sin(Math.PI / (gecici));

                    max = (int)gecici2;

                    o_renk_katsayilari[i, j] = r1.Next(min, max);

                    min = o_renk_katsayilari[i, j] + 1;
                }
            }


        }

        /// 
        /// yeni yaratılan bir bireyin  renk katsayıları sıfır ile 255 arasında değerler ile oluşturuluyor
        /// 
        private void m_Renk_Katsayi_Olustur_2()
        {
            int min, max;

            for (int i = 0; i < o_renk_katsayilari.GetLength(0); i++)
            {
                min = 0;

                for (int j = 0; j < o_renk_katsayilari.GetLength(1); j++)
                {
                    max = 256 / (o_renk_katsayilari.GetLength(1) - j);
                    o_renk_katsayilari[i, j] = r1.Next(min, max);
                    min = o_renk_katsayilari[i, j] + 2;
                }
            }

        }

        /// 
        /// yeni yaratılan bir bireyin  uygunluk değeri sıfır olarak başlangıç değerine atandı
        /// 
        private void m_Uygunluk_Deger_Olustur()
        {
            o_uygunluk_degeri = new double();
        }


        public static bool m_BireyKontrol(int[,] o_renk_katsayilari, string hata_kod_blogu)
        {
            int gecici = 0;
            bool hata = false;

            for (int i = 0; i < o_renk_katsayilari.GetLength(0); i++)
            {
                gecici = 0;
                for (int j = 0; j < o_renk_katsayilari.GetLength(1); j++)
                {

                    if (o_renk_katsayilari[i, j] > gecici || gecici == 0)
                    {
                        gecici = o_renk_katsayilari[i, j];
                    }
                    else
                    {
                        hata = true;
                    }
                }
            }

            return hata;

        }

    }
    
}
