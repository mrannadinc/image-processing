using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace MedPic
{
    public class Fitness_Function
    {
        public static double[] o_pr;

        public static string o_function_name;

        public static void m_Fitness_Hesapla(GA_Birey birey)
        {


            try
            {
                Fitness_Function myClassObj = new Fitness_Function();
                Type myTypeObj = myClassObj.GetType();

                object[] obj = new object[1];
                obj[0] = birey;
                MethodInfo myMethodInfo = myTypeObj.GetMethod(o_function_name);
                myMethodInfo.Invoke(myClassObj, obj);
            }
            catch (Exception)
            {

                throw;
            }

        }



        public static void Kapur_Fonksiyonu(GA_Birey birey)
        {

            int[,] Tgenetic = new int[1, RGB_Parametreler.o_esik_sayisi];
            // Tgenetic min=0; max=255 ve bütün renk kanalları için (Tr, Tg, Tb) 0<Tr1<Tr2<...<TrN<255

            int[] Tr = new int[RGB_Parametreler.o_esik_sayisi];


            for (int i = 0; i < RGB_Parametreler.o_esik_sayisi; i++)
            {
                Tr[i] = birey.o_renk_katsayilari[0, i];

            }



            double[] Hr = new double[RGB_Parametreler.o_esik_sayisi + 1];

            double[] Wr = new double[RGB_Parametreler.o_esik_sayisi + 1];


            int[] Ttr = new int[RGB_Parametreler.o_esik_sayisi + 2];
            Ttr[0] = 0;
            Ttr[RGB_Parametreler.o_esik_sayisi + 1] = 256;



            for (int i = 1; i < Ttr.Length - 1; i++)
            {
                Ttr[i] = Tr[i - 1];

            }

            for (int i = 0; i < RGB_Parametreler.o_esik_sayisi + 1; i++)
            {
                for (int j = Ttr[i]; j < Ttr[i + 1]; j++)
                {

                    Wr[i] = Wr[i] + o_pr[j];
                }


            }




            for (int i = 0; i < RGB_Parametreler.o_esik_sayisi + 1; i++)
            {
                for (int j = Ttr[i]; j < Ttr[i + 1]; j++)
                {
                    if (o_pr[j] > 0 && Wr[i] > 0)
                        Hr[i] = Hr[i] - (o_pr[j] / (Wr[i])) * Math.Log(o_pr[j] / Wr[i]);
                }


            }

            double Fitness = new double();

            for (int i = 0; i < RGB_Parametreler.o_esik_sayisi + 1; i++)
            {
                Fitness += Hr[i];
            }


            birey.o_uygunluk_degeri = Fitness;


        }

        public static void Otsu_Fonksiyonu(GA_Birey birey)
        {
            int[,] Tgenetic = new int[1, RGB_Parametreler.o_esik_sayisi];
            // Tgenetic min=0; max=255 ve bütün renk kanalları için (Tr, Tg, Tb) 0<Tr1<Tr2<...<TrN<255

            int[] Tr = new int[RGB_Parametreler.o_esik_sayisi];

            for (int i = 0; i < RGB_Parametreler.o_esik_sayisi; i++)
            {
                Tr[i] = birey.o_renk_katsayilari[0, i];

            }


            double[] Hr = new double[RGB_Parametreler.o_esik_sayisi + 1];

            double[] Wr = new double[RGB_Parametreler.o_esik_sayisi + 1];


            double[] Mr = new double[RGB_Parametreler.o_esik_sayisi + 1];


            int[] Ttr = new int[RGB_Parametreler.o_esik_sayisi + 2];
            Ttr[0] = 0;
            Ttr[RGB_Parametreler.o_esik_sayisi + 1] = 256;



            for (int i = 1; i < Ttr.Length - 1; i++)
            {
                Ttr[i] = Tr[i - 1];

            }

            for (int i = 0; i < RGB_Parametreler.o_esik_sayisi + 1; i++)
            {
                for (int j = Ttr[i]; j < Ttr[i + 1]; j++)
                {

                    Wr[i] = Wr[i] + o_pr[j];
                }


            }
            //



            for (int i = 0; i < RGB_Parametreler.o_esik_sayisi + 1; i++)
            {
                for (int j = Ttr[i]; j < Ttr[i + 1]; j++)
                {
                    if (Wr[i] > 0)
                        Mr[i] = Mr[i] + o_pr[j] * j / Wr[i];
                }


            }



            double Mtr = new double();

            /*
            for (int i = 0; i < RGB_Parametreler.o_esik_sayisi + 1; i++)
            {
                Mtr += Mr[i] * Wr[i];
               
            }*/

            Mtr = 0;
            for (int j = 0; j < 256; j++)
            {

                Mtr = Mtr + o_pr[j] * j;
            }




            double[] Gr = new double[RGB_Parametreler.o_esik_sayisi + 1];

            for (int i = 0; i < RGB_Parametreler.o_esik_sayisi + 1; i++)
            {
                Gr[i] = Wr[i] * Math.Pow((Mr[i] - Mtr), 2);

            }


            double Fitness = new double();

            for (int i = 0; i < RGB_Parametreler.o_esik_sayisi + 1; i++)
            {
                Fitness = Fitness + Gr[i];
            }


            birey.o_uygunluk_degeri = Fitness;



        }
    }
    
}
