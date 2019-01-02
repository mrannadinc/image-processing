using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Collections;
using System.Threading.Tasks;
using System.Drawing.Drawing2D;

namespace MedPic
{
    class ANNm
    {   public int GirisSayi,K, M, N;
        public  double hata, lamda, alfa; 
        public double[] Giris;
        public double[] d;
        public double[] y;
   
        public  Neuron[] GirisKatman; 
        public  Neuron[] GizliKatman;
        public  Neuron[] CikisKatman;
      

        public  ANNm(int Girissayisi, int Giriskat, int GizliKat,int Cikiskat,double learningrate)
        {   GirisSayi =Girissayisi;   K =Giriskat; M =GizliKat;    N=Cikiskat;               
           lamda = 0.01; alfa = 1.0;  //lamda = learningrate;
            Giris = new double[GirisSayi];
            d = new double[N];
            y = new double[N];
            int i, j;

            for (i = 0; i < GirisSayi; i++)
            { Giris[i] =0.0; }

            for (i = 0; i < N; i++)
            { d[i] =0.0; }

          
                GirisKatman = new Neuron[K];
            for (i = 0; i < K; i++)
               { GirisKatman[i] = new Neuron(GirisSayi, 1); }
          
                GizliKatman = new Neuron[M];
            for (i = 0; i < M; i++)
               { GizliKatman[i] = new Neuron(K, 2); }
       
                CikisKatman = new Neuron[N];
            for (i = 0; i < N; i++)
                { CikisKatman[i] = new Neuron(M, 2); }          

        }


        public void Feedforward(double[] GirisBilgi)
        {
            int i, j;

            for (i = 0; i < GirisSayi; i++)
            { Giris[i] = GirisBilgi[i]; }         


            for (i = 0; i < K; i++)
            {
                for (j = 0; j < GirisSayi; j++)
                {
                    GirisKatman[i].inputs[j] = Giris[j];
                }
            }

            for (i = 0; i < M; i++)
            {
                for (j = 0; j < K; j++)
                {
                    GizliKatman[i].inputs[j] = GirisKatman[j].output;
                }
            }

            for (i = 0; i < N; i++)
            {
                for (j = 0; j < M; j++)
                {
                    CikisKatman[i].inputs[j] = GizliKatman[j].output;
                }
            }

            for (i = 0; i < N; i++)
            {    y[i] = CikisKatman[i].output;   }
            
        }


        public void Backpropagation(double[] CikisBilgi)
        {
            int i, j;
            for (i = 0; i < N; i++)
            { d[i] = CikisBilgi[i]; }
            
            // çıkış katmanı update
            for (i = 0; i < N; i++)
            {
                hata = d[i] - y[i];
                CikisKatman[i].err = hata * CikisKatman[i].dy;
            }

            for (i = 0; i < N; i++)
            {
                for (j = 0; j < M; j++)
                {
                    CikisKatman[i].weights[j] = alfa * CikisKatman[i].weights[j] + lamda * CikisKatman[i].err * GizliKatman[j].output;

                }
                CikisKatman[i].Bias = alfa * CikisKatman[i].Bias + lamda * CikisKatman[i].err * (1.0);
            }

            //gizli katman update

            for (i = 0; i < M; i++)
            {
                hata = 0;
                for (j = 0; j < N; j++)
                {
                    hata = hata + CikisKatman[j].weights[i] * CikisKatman[j].err;
                }
                GizliKatman[i].err = hata * GizliKatman[i].dy;
            }


            for (i = 0; i < M; i++)
            {
                for (j = 0; j < K; j++)
                {
                    GizliKatman[i].weights[j] = alfa * GizliKatman[i].weights[j] + lamda * GizliKatman[i].err * GirisKatman[j].output;
                }

                GizliKatman[i].Bias = alfa * GizliKatman[i].Bias + lamda * GizliKatman[i].err * (1.0);
            }

            // Giriş katmanı update

            for (i = 0; i < K; i++)
            {
                hata = 0;
                for (j = 0; j < M; j++)
                {
                    hata = hata + GizliKatman[j].weights[i] * GizliKatman[j].err;
                }
                GirisKatman[i].err = hata * GirisKatman[i].dy;
            }


            for (i = 0; i < K; i++)
            {
                for (j = 0; j < GirisSayi; j++)
                {
                    GirisKatman[i].weights[j] = alfa * GirisKatman[i].weights[j] + lamda * GirisKatman[i].err * Giris[j];
                }
                GirisKatman[i].Bias = alfa * GirisKatman[i].Bias + lamda * GirisKatman[i].err * (1.0);
            }     

       }


        public void Train(double[] GirisBilgi, double[] CikisBilgi, int iterasyon, double[] ANNCikis)
        {
            int i, j;
            for (i = 0; i < GirisSayi; i++)
            { Giris[i] = GirisBilgi[i]; }

            for (i = 0; i < N; i++)
            { d[i] = CikisBilgi[i]; }

           for ( i = 0; i <iterasyon; i++)
            {   Feedforward(Giris);
                Backpropagation(d);
            }

           for (i = 0; i < N; i++)
           { ANNCikis[i] = y[i];   }
          
        }


        public void TrainFile(int iterasyon)
        {
            int i, j,t;
                   
            t = 0;
            do
            {

                using (StreamReader dosya = new StreamReader("c:\\Medpic\\iris.txt"))
                {
                    string line,paket;
                    while ((line = dosya.ReadLine()) != null)
                    {
                        string[] bil = line.Split(',');
                       
                         paket = bil[0].Replace(".", ","); Giris[0] = Convert.ToDouble(paket);
                         paket = bil[1].Replace(".", ","); Giris[1] = Convert.ToDouble(paket);
                         paket = bil[1].Replace(".", ","); Giris[2] = Convert.ToDouble(paket);
                         paket = bil[3].Replace(".", ","); Giris[3] = Convert.ToDouble(paket);
           
                     

                        if (bil[4] == "Iris-setosa")
                        {
                            d[0] = 1.0;
                            d[1] = 0.0;
                            d[2] = 0.0;
                        }
                        else if (bil[4] == "Iris-versicolor")
                        {
                            d[0] = 0.0;
                            d[1] = 1.0;
                            d[2] = 0.0;
                        }
                        else if (bil[4] == "Iris-virginica")
                        {
                            d[0] = 0.0;
                            d[1] = 0.0;
                            d[2] = 1.0;
                        }
                        else
                        {
                            d[0] = 0.0;
                            d[1] = 0.0;
                            d[2] = 0.0;
                        }




                        for (i = 0; i < K; i++)
                        {
                            for (j = 0; j < GirisSayi; j++)
                            {
                                GirisKatman[i].inputs[j] = Giris[j];
                            }
                        }



                        for (i = 0; i < M; i++)
                        {
                            for (j = 0; j < K; j++)
                            {
                                GizliKatman[i].inputs[j] = GirisKatman[j].output;
                            }
                        }

                        for (i = 0; i < N; i++)
                        {
                            for (j = 0; j < M; j++)
                            {
                                CikisKatman[i].inputs[j] = GizliKatman[j].output;
                            }
                        }



                        for (i = 0; i < N; i++)
                        {
                            y[i] = CikisKatman[i].output;

                        }


                        // çıkış katmanı update
                        for (i = 0; i < N; i++)
                        {
                            hata = d[i] - y[i];
                            CikisKatman[i].err = hata * CikisKatman[i].dy;
                        }

                        for (i = 0; i < N; i++)
                        {
                            for (j = 0; j < M; j++)
                            {
                                CikisKatman[i].weights[j] = alfa * CikisKatman[i].weights[j] + lamda * CikisKatman[i].err * GizliKatman[j].output;

                            }
                            CikisKatman[i].Bias = alfa * CikisKatman[i].Bias + lamda * CikisKatman[i].err * (1.0);
                        }

                        //gizli katman update

                        for (i = 0; i < M; i++)
                        {
                            hata = 0;
                            for (j = 0; j < N; j++)
                            {
                                hata = hata + CikisKatman[j].weights[i] * CikisKatman[j].err;
                            }
                            GizliKatman[i].err = hata * GizliKatman[i].dy;
                        }


                        for (i = 0; i < M; i++)
                        {
                            for (j = 0; j < K; j++)
                            {
                                GizliKatman[i].weights[j] = alfa * GizliKatman[i].weights[j] + lamda * GizliKatman[i].err * GirisKatman[j].output;
                            }

                            GizliKatman[i].Bias = alfa * GizliKatman[i].Bias + lamda * GizliKatman[i].err * (1.0);
                        }

                        // Giriş katmanı update

                        for (i = 0; i < K; i++)
                        {
                            hata = 0;
                            for (j = 0; j < M; j++)
                            {
                                hata = hata + GizliKatman[j].weights[i] * GizliKatman[j].err;
                            }
                            GirisKatman[i].err = hata * GirisKatman[i].dy;
                        }



                        for (i = 0; i < K; i++)
                        {
                            for (j = 0; j < GirisSayi; j++)
                            {
                                GirisKatman[i].weights[j] = alfa * GirisKatman[i].weights[j] + lamda * GirisKatman[i].err * Giris[j];
                            }
                            GirisKatman[i].Bias = alfa * GirisKatman[i].Bias + lamda * GirisKatman[i].err * (1.0);
                        }


                   Feedforward(Giris);
                   Backpropagation(d);

                    }  //dosya
                }//dosya

               t = t + 1;
      } while (t < iterasyon);

   }

        public void SaveWeight()
        {
            int i, j; 
            FileStream fs1 = new FileStream("c:\\Medpic\\GirisKatman.txt", FileMode.Create, FileAccess.Write);
            StreamWriter dosya1 = new StreamWriter(fs1);
            for (i = 0; i < K; i++)
            {
                for (j = 0; j < GirisSayi; j++)
                {
                    dosya1.WriteLine("{0:N4}\t{1:N5}", i, GirisKatman[i].weights[j]);
                }
                dosya1.WriteLine("{0:N4}\t{1:N5}", i, GirisKatman[i].Bias);
            }
            dosya1.Close();

            FileStream fs2 = new FileStream("c:\\Medpic\\GizliKatman.txt", FileMode.Create, FileAccess.Write);
            StreamWriter dosya2 = new StreamWriter(fs2);
            for (i = 0; i < M; i++)
            {
                for (j = 0; j < K; j++)
                {
                    dosya2.WriteLine("{0:N4}\t{1:N5}", i, GizliKatman[i].weights[j]);
                }
                dosya2.WriteLine("{0:N4}\t{1:N5}", i, GizliKatman[i].Bias);
            }
            dosya2.Close();

            FileStream fs3 = new FileStream("c:\\Medpic\\CikisKatman.txt", FileMode.Create, FileAccess.Write);
            StreamWriter dosya3 = new StreamWriter(fs3);

            for (i = 0; i < N; i++)
            {
                for (j = 0; j < M; j++)
                {
                    dosya3.WriteLine("{0:N4}\t{1:N5}", i, CikisKatman[i].weights[j]);

                }
                dosya3.WriteLine("{0:N4}\t{1:N5}", i, CikisKatman[i].Bias);
            }
            dosya3.Close();
       }



     public void Test(double[] GirisBilgi, double[] ANNCikis)
        {
            int i, j;
            for (i = 0; i < GirisSayi; i++)
             { Giris[i] = GirisBilgi[i]; }
          
            Feedforward(Giris);

            for (i = 0; i < N; i++)
            { ANNCikis[i] = y[i]; }

        }



    }
}
