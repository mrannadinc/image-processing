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
    public class ANNt
    {
        public int GirisSayi, K, M;
        public double hata, lamda, alfa;
        public double[] Giris;
        public double d;
        public double y;

    public  class Neuron       
    {   public int n,i;   
        public double[] inputs;
        public double[] weights;
        public double v,y,tipi;
        public double dy;
        public double err;
        public double Bias;

        private Random r = new Random(); 

        public Neuron(int inputnumber, int tip)
        {  inputs = new double[inputnumber];
           weights= new double[inputnumber];
           n = inputnumber; tipi = tip;
           for( i=0;i<n;i++)
            {  weights[i] = r.NextDouble();
               inputs[i] = 0.0;
            }
           Bias = r.NextDouble(); err = 0.0;
        }

        public double output
        {  get
            {   v = 0.0;
                for (i = 0; i <n; i++)
                {  v=v+ weights[i] * inputs[i];  }
                v = v + Bias * (1.0);
                if (tipi == 1)
                  {   y =v;
                      dy =1;
                  }
                else if (tipi ==2)
                  {   y = Sigmoid(v);
                      dy = y * (1 - y);
                  }
                else
                  {   y =0;
                      dy =0;
                   }
                return y;            
            }
        }
  
        public  double Sigmoid(double x)
        {   return 1.0 / (1.0 + Math.Exp(-x)); }       

       }


      public Neuron[] GirisKatman;
      public Neuron[] GizliKatman;
      public Neuron   CikisKatman;

      public  ANNt(int Girissayisi, int Giriskat, int GizliKat,double learningrate)
        {   GirisSayi =Girissayisi;   K =Giriskat; M =GizliKat;               
            lamda = 0.05; alfa = 0.99;  //lamda = learningrate;      
            Giris = new double[GirisSayi];        
            d =0.0; y =0.0;
            int i, j;
            for (i = 0; i < GirisSayi; i++)
            { Giris[i] =0.0; }          

               GirisKatman = new Neuron[K];
            for (i = 0; i < K; i++)
            { GirisKatman[i] = new Neuron(GirisSayi, 1); }
  
               GizliKatman = new Neuron[M];
            for (i = 0; i < M; i++)
            { GizliKatman[i] = new Neuron(K, 2); }

              CikisKatman = new Neuron(M, 1);        
        }     
    

      public void Feedforward(double[] GirisBilgi)
      {
          int i, j;
         
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

             for (j = 0; j < M; j++)
              {
                  CikisKatman.inputs[j] = GizliKatman[j].output;
              }
     
          y = CikisKatman.output;
      }


      public void Backpropagation(double CikisBilgi)
      {
          int i, j;
              d = CikisBilgi;         
          //  çıkış katmanı update
              hata = d - y;
              CikisKatman.err = hata * CikisKatman.dy;         

             for (j = 0; j < M; j++)
              {
                  CikisKatman.weights[j] = alfa * CikisKatman.weights[j] + lamda * CikisKatman.err * GizliKatman[j].output;
              }
                  CikisKatman.Bias = alfa * CikisKatman.Bias + lamda * CikisKatman.err * (1.0);          

          //gizli katman update
          for (i = 0; i < M; i++)
          {
                 hata = CikisKatman.weights[i] * CikisKatman.err;
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

           for (j = 0; j < M; j++)
              {
                  dosya3.WriteLine("{0:N4}\t{1:N5}", i, CikisKatman.weights[j]);

              }
              dosya3.WriteLine("{0:N4}\t{1:N5}", i, CikisKatman.Bias);
         
          dosya3.Close();
      }

      public void Train(double[] GirisBilgi, double CikisBilgi, int iterasyon)
      {
          int i, j;
          for (i = 0; i < GirisSayi; i++)
          { Giris[i] = GirisBilgi[i]; }
          d = CikisBilgi;
          for (i = 0; i < iterasyon; i++)
          {    Feedforward(Giris);
               Backpropagation(d);
          }
      }




      public void TrainFile(int iterasyon)
      {
          int i, j,t;
          t = 0;
          do
          {
              using (StreamReader dosya = new StreamReader("c:\\Medpic10\\iris.txt"))
              {
                  string line, paket;
                  while ((line = dosya.ReadLine()) != null)
                  {
                      string[] bil = line.Split(',');

                      paket = bil[0].Replace(".", ","); Giris[0] = Convert.ToDouble(paket);
                      paket = bil[1].Replace(".", ","); Giris[1] = Convert.ToDouble(paket);
                      paket = bil[1].Replace(".", ","); Giris[2] = Convert.ToDouble(paket);
                      paket = bil[3].Replace(".", ","); Giris[3] = Convert.ToDouble(paket);

                      d = 1.0;                 


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

                     
                          for (j = 0; j < M; j++)
                          {
                              CikisKatman.inputs[j] = GizliKatman[j].output;
                          }                  

                      y = CikisKatman.output;                  


                      // çıkış katmanı update                      
                          hata = d - y;
                          CikisKatman.err = hata * CikisKatman.dy;
                     

                       for (j = 0; j < M; j++)
                          {
                              CikisKatman.weights[j] = alfa * CikisKatman.weights[j] + lamda * CikisKatman.err * GizliKatman[j].output;

                          }
                          CikisKatman.Bias = alfa * CikisKatman.Bias + lamda * CikisKatman.err * (1.0);
                    

                      //gizli katman update
                      for (i = 0; i < M; i++)
                      {
                          hata = 0;
                          hata = hata + CikisKatman.weights[i] * CikisKatman.err;
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



      public void Test(double[] GirisBilgi, double ANNCikis)
      {
          int i, j;
          for (i = 0; i < GirisSayi; i++)
          { Giris[i] = GirisBilgi[i]; }
          Feedforward(Giris);
           ANNCikis = y;
      }



    }
}
