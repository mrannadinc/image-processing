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
    public class ANNs
    {
        public int GirisSayi, K, M, N;
        public double hata, lamda, alfa;
        public double[] Giris;
        public double[] d;
        public double[] y;


    public  class Neuron       
    {   public int n,i;   
        public double[] inputs;
        public double[] weights;
        private double v,y,tipi;
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
    public Neuron[] CikisKatman;

      public  ANNs(int Girissayisi, int Giriskat, int GizliKat,int Cikiskat,double learningrate)
        {   GirisSayi =Girissayisi;   K =Giriskat; M =GizliKat;    N=Cikiskat;               
             lamda = 0.05; alfa = 0.99;  //lamda = learningrate;           
            int i, j;
             Giris = new double[GirisSayi];        
             d = new double[N];          
             y = new double[N];
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
            { CikisKatman[i] = new Neuron(M, 1); }          
         
        }


     
    

      public void Feedforward(double[] GirisBilgi)
      {    int i, j;
         

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
          { y[i] = CikisKatman[i].output; }

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

      public void Train(double[] GirisBilgi, double[] CikisBilgi, int iterasyon)
      {
          int i, j;

          for (i = 0; i < GirisSayi; i++)
          { Giris[i] = GirisBilgi[i]; }

          for (i = 0; i < N; i++)
          { d[i] = CikisBilgi[i]; }

          for (i = 0; i < iterasyon; i++)
          {
              Feedforward(Giris);
              Backpropagation(d);
          }
      }




      public void TrainFile(int iterasyon)
      {
          int i, j,t;
         
          t = 0;
          do
          {

              using (StreamReader dosya = new StreamReader("c:\\Medpic\\iris.txt"))
              {
                  string line, paket;
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



      public void TrainFileAlfabe(int iterasyon)
      {
          int i, j, t;

          t = 0;
          do
          {

              using (StreamReader dosya = new StreamReader("c:\\Medpic\\dataset.txt"))
              {
                  string line, paket;
                  while ((line = dosya.ReadLine()) != null)
                  {
                      string[] bil = line.Split(',');

                      paket = bil[0].Replace(".", ","); Giris[0] = Convert.ToDouble(paket);
                      paket = bil[1].Replace(".", ","); Giris[1] = Convert.ToDouble(paket);
                      paket = bil[1].Replace(".", ","); Giris[2] = Convert.ToDouble(paket);
                      paket = bil[3].Replace(".", ","); Giris[3] = Convert.ToDouble(paket);
                      paket = bil[4].Replace(".", ","); Giris[4] = Convert.ToDouble(paket);
                      paket = bil[5].Replace(".", ","); Giris[5] = Convert.ToDouble(paket);
                      paket = bil[6].Replace(".", ","); Giris[6] = Convert.ToDouble(paket);
                      paket = bil[7].Replace(".", ","); Giris[7] = Convert.ToDouble(paket);

                  


                      if (bil[8] == "0")
                      {
                          d[0] = 1.0;
                          d[1] = 0.0;
                          d[2] = 0.0;
                          d[3] = 0.0;
                          d[4] = 0.0;
                          d[5] = 0.0;
                      }
                      else if (bil[8] == "1")
                      {
                          d[0] = 0.0;
                          d[1] = 1.0;
                          d[2] = 0.0;
                          d[3] = 0.0;
                          d[4] = 0.0;
                          d[5] = 0.0;
                      }
                      else if (bil[8] == "2")
                      {
                          d[0] = 1.0;
                          d[1] = 1.0;
                          d[2] = 0.0;
                          d[3] = 0.0;
                          d[4] = 0.0;
                          d[5] = 0.0;
                      }
                      else if (bil[8] == "3")
                      {
                          d[0] = 0.0;
                          d[1] = 0.0;
                          d[2] = 1.0;
                          d[3] = 0.0;
                          d[4] = 0.0;
                          d[5] = 0.0;
                      }
                      else if (bil[8] == "4")
                      {
                          d[0] = 1.0;
                          d[1] = 0.0;
                          d[2] = 1.0;
                          d[3] = 0.0;
                          d[4] = 0.0;
                          d[5] = 0.0;
                      }
                      else if (bil[8] == "5")
                      {
                          d[0] = 0.0;
                          d[1] = 1.0;
                          d[2] = 1.0;
                          d[3] = 0.0;
                          d[4] = 0.0;
                          d[5] = 0.0;
                      }
                      else if (bil[8] == "6")
                      {
                          d[0] = 1.0;
                          d[1] = 1.0;
                          d[2] = 1.0;
                          d[3] = 0.0;
                          d[4] = 0.0;
                          d[5] = 0.0;
                      }
                      else if (bil[8] == "7")
                      {
                          d[0] = 0.0;
                          d[1] = 0.0;
                          d[2] = 0.0;
                          d[3] = 1.0;
                          d[4] = 0.0;
                          d[5] = 0.0;
                      }
                      else if (bil[8] == "8")
                      {
                          d[0] = 1.0;
                          d[1] = 0.0;
                          d[2] = 0.0;
                          d[3] = 1.0;
                          d[4] = 0.0;
                          d[5] = 0.0;
                      }
                      else if (bil[8] == "9")
                      {
                          d[0] = 0.0;
                          d[1] = 1.0;
                          d[2] = 0.0;
                          d[3] = 1.0;
                          d[4] = 0.0;
                          d[5] = 0.0;
                      }
                      else if (bil[8] == "A")
                      {
                          d[0] = 1.0;
                          d[1] = 1.0;
                          d[2] = 0.0;
                          d[3] = 1.0;
                          d[4] = 0.0;
                          d[5] = 0.0;
                      }
                      else if (bil[8] == "B")
                      {
                          d[0] = 0.0;
                          d[1] = 0.0;
                          d[2] = 1.0;
                          d[3] = 1.0;
                          d[4] = 0.0;
                          d[5] = 0.0;
                      }
                      else if (bil[8] == "C")
                      {
                          d[0] = 1.0;
                          d[1] = 0.0;
                          d[2] = 1.0;
                          d[3] = 1.0;
                          d[4] = 0.0;
                          d[5] = 0.0;
                      }
                      else if (bil[8] == "D")
                      {
                          d[0] = 0.0;
                          d[1] = 1.0;
                          d[2] = 1.0;
                          d[3] = 1.0;
                          d[4] = 0.0;
                          d[5] = 0.0;
                      }
                      else if (bil[8] == "E")
                      {
                          d[0] = 1.0;
                          d[1] = 1.0;
                          d[2] = 1.0;
                          d[3] = 1.0;
                          d[4] = 0.0;
                          d[5] = 0.0;
                      }
                      else if (bil[8] == "F")
                      {
                          d[0] = 0.0;
                          d[1] = 0.0;
                          d[2] = 0.0;
                          d[3] = 0.0;
                          d[4] = 1.0;
                          d[5] = 0.0;
                      }
                      else if (bil[8] == "G")
                      {
                          d[0] = 1.0;
                          d[1] = 0.0;
                          d[2] = 0.0;
                          d[3] = 0.0;
                          d[4] = 1.0;
                          d[5] = 0.0;
                      }
                      else if (bil[8] == "H")
                      {
                          d[0] = 0.0;
                          d[1] = 1.0;
                          d[2] = 0.0;
                          d[3] = 0.0;
                          d[4] = 1.0;
                          d[5] = 0.0;
                      }
                      else if (bil[8] == "I")
                      {
                          d[0] = 1.0;
                          d[1] = 1.0;
                          d[2] = 0.0;
                          d[3] = 0.0;
                          d[4] = 1.0;
                          d[5] = 0.0;
                      }
                      else if (bil[8] == "J")
                      {
                          d[0] = 0.0;
                          d[1] = 0.0;
                          d[2] = 1.0;
                          d[3] = 0.0;
                          d[4] = 1.0;
                          d[5] = 0.0;
                      }
                      else if (bil[8] == "K")
                      {
                          d[0] = 1.0;
                          d[1] = 0.0;
                          d[2] = 1.0;
                          d[3] = 0.0;
                          d[4] = 1.0;
                          d[5] = 0.0;
                      }
                      else if (bil[8] == "L")
                      {
                          d[0] = 0.0;
                          d[1] = 1.0;
                          d[2] = 1.0;
                          d[3] = 0.0;
                          d[4] = 1.0;
                          d[5] = 0.0;
                      }
                      else if (bil[8] == "M")
                      {
                          d[0] = 1.0;
                          d[1] = 1.0;
                          d[2] = 1.0;
                          d[3] = 0.0;
                          d[4] = 1.0;
                          d[5] = 0.0;
                      }
                      else if (bil[8] == "N")
                      {
                          d[0] = 0.0;
                          d[1] = 0.0;
                          d[2] = 0.0;
                          d[3] = 1.0;
                          d[4] = 1.0;
                          d[5] = 0.0;
                      }
                      else if (bil[8] == "O")
                      {
                          d[0] = 1.0;
                          d[1] = 0.0;
                          d[2] = 0.0;
                          d[3] = 1.0;
                          d[4] = 1.0;
                          d[5] = 0.0;
                      }
                      else if (bil[8] == "P")
                      {
                          d[0] = 0.0;
                          d[1] = 1.0;
                          d[2] = 0.0;
                          d[3] = 1.0;
                          d[4] = 1.0;
                          d[5] = 0.0;
                      }
                      else if (bil[8] == "R")
                      {
                          d[0] = 1.0;
                          d[1] = 1.0;
                          d[2] = 0.0;
                          d[3] = 1.0;
                          d[4] = 1.0;
                          d[5] = 0.0;
                      }
                      else if (bil[8] == "S")
                      {
                          d[0] = 0.0;
                          d[1] = 0.0;
                          d[2] = 1.0;
                          d[3] = 1.0;
                          d[4] = 1.0;
                          d[5] = 0.0;
                      }
                      else if (bil[8] == "T")
                      {
                          d[0] = 1.0;
                          d[1] = 0.0;
                          d[2] = 1.0;
                          d[3] = 1.0;
                          d[4] = 1.0;
                          d[5] = 0.0;
                      }
                      else if (bil[8] == "U")
                      {
                          d[0] = 0.0;
                          d[1] = 1.0;
                          d[2] = 1.0;
                          d[3] = 1.0;
                          d[4] = 1.0;
                          d[5] = 0.0;
                      }
                      else if (bil[8] == "V")
                      {
                          d[0] = 1.0;
                          d[1] = 1.0;
                          d[2] = 1.0;
                          d[3] = 1.0;
                          d[4] = 1.0;
                          d[5] = 0.0;
                      }
                      else if (bil[8] == "Y")
                      {
                          d[0] = 0.0;
                          d[1] = 0.0;
                          d[2] = 0.0;
                          d[3] = 0.0;
                          d[4] = 0.0;
                          d[5] = 1.0;
                      }
                      else if (bil[8] == "Z")
                      {
                          d[0] = 1.0;
                          d[1] = 0.0;
                          d[2] = 0.0;
                          d[3] = 0.0;
                          d[4] = 0.0;
                          d[5] = 1.0;
                      }
                      else if (bil[8] == "a")
                      {
                          d[0] = 0.0;
                          d[1] = 1.0;
                          d[2] = 0.0;
                          d[3] = 0.0;
                          d[4] = 0.0;
                          d[5] = 1.0;
                      }
                      else if (bil[8] == "b")
                      {
                          d[0] = 1.0;
                          d[1] = 1.0;
                          d[2] = 0.0;
                          d[3] = 0.0;
                          d[4] = 0.0;
                          d[5] = 1.0;
                      }
                      else if (bil[8] == "c")
                      {
                          d[0] = 0.0;
                          d[1] = 0.0;
                          d[2] = 1.0;
                          d[3] = 0.0;
                          d[4] = 0.0;
                          d[5] = 1.0;
                      }
                      else if (bil[8] == "d")
                      {
                          d[0] = 1.0;
                          d[1] = 0.0;
                          d[2] = 1.0;
                          d[3] = 0.0;
                          d[4] = 0.0;
                          d[5] = 1.0;
                      }
                      else if (bil[8] == "e")
                      {
                          d[0] = 0.0;
                          d[1] = 1.0;
                          d[2] = 1.0;
                          d[3] = 0.0;
                          d[4] = 0.0;
                          d[5] = 1.0;
                      }
                      else if (bil[8] == "f")
                      {
                          d[0] = 1.0;
                          d[1] = 1.0;
                          d[2] = 1.0;
                          d[3] = 0.0;
                          d[4] = 0.0;
                          d[5] = 1.0;
                      }
                      else if (bil[8] == "g")
                      {
                          d[0] = 0.0;
                          d[1] = 0.0;
                          d[2] = 0.0;
                          d[3] = 1.0;
                          d[4] = 0.0;
                          d[5] = 1.0;
                      }
                      else if (bil[8] == "h")
                      {
                          d[0] = 1.0;
                          d[1] = 0.0;
                          d[2] = 0.0;
                          d[3] = 1.0;
                          d[4] = 0.0;
                          d[5] = 1.0;
                      }
                      else if (bil[8] == "i")
                      {
                          d[0] = 0.0;
                          d[1] = 1.0;
                          d[2] = 0.0;
                          d[3] = 1.0;
                          d[4] = 0.0;
                          d[5] = 1.0;
                      }
                      else if (bil[8] == "j")
                      {
                          d[0] = 1.0;
                          d[1] = 1.0;
                          d[2] = 0.0;
                          d[3] = 1.0;
                          d[4] = 0.0;
                          d[5] = 1.0;
                      }
                      else if (bil[8] == "k")
                      {
                          d[0] = 0.0;
                          d[1] = 0.0;
                          d[2] = 1.0;
                          d[3] = 1.0;
                          d[4] = 0.0;
                          d[5] = 1.0;
                      }
                      else if (bil[8] == "l")
                      {
                          d[0] = 1.0;
                          d[1] = 0.0;
                          d[2] = 1.0;
                          d[3] = 1.0;
                          d[4] = 0.0;
                          d[5] = 1.0;
                      }
                      else if (bil[8] == "m")
                      {
                          d[0] = 0.0;
                          d[1] = 1.0;
                          d[2] = 1.0;
                          d[3] = 1.0;
                          d[4] = 0.0;
                          d[5] = 1.0;
                      }
                      else if (bil[8] == "n")
                      {
                          d[0] = 1.0;
                          d[1] = 1.0;
                          d[2] = 1.0;
                          d[3] = 1.0;
                          d[4] = 0.0;
                          d[5] = 1.0;
                      }
                      else if (bil[8] == "o")
                      {
                          d[0] = 0.0;
                          d[1] = 0.0;
                          d[2] = 0.0;
                          d[3] = 0.0;
                          d[4] = 1.0;
                          d[5] = 1.0;
                      }
                      else if (bil[8] == "p")
                      {
                          d[0] = 1.0;
                          d[1] = 0.0;
                          d[2] = 0.0;
                          d[3] = 0.0;
                          d[4] = 1.0;
                          d[5] = 1.0;
                      }
                      else if (bil[8] == "r")
                      {
                          d[0] = 0.0;
                          d[1] = 1.0;
                          d[2] = 0.0;
                          d[3] = 0.0;
                          d[4] = 1.0;
                          d[5] = 1.0;
                      }
                      else if (bil[8] == "s")
                      {
                          d[0] = 1.0;
                          d[1] = 1.0;
                          d[2] = 0.0;
                          d[3] = 0.0;
                          d[4] = 1.0;
                          d[5] = 1.0;
                      }
                      else if (bil[8] == "t")
                      {
                          d[0] = 0.0;
                          d[1] = 0.0;
                          d[2] = 1.0;
                          d[3] = 0.0;
                          d[4] = 1.0;
                          d[5] = 1.0;
                      }
                      else if (bil[8] == "u")
                      {
                          d[0] = 1.0;
                          d[1] = 0.0;
                          d[2] = 1.0;
                          d[3] = 0.0;
                          d[4] = 1.0;
                          d[5] = 1.0;
                      }
                      else if (bil[8] == "v")
                      {
                          d[0] = 0.0;
                          d[1] = 1.0;
                          d[2] = 1.0;
                          d[3] = 0.0;
                          d[4] = 1.0;
                          d[5] = 1.0;
                      }
                      else if (bil[8] == "y")
                      {
                          d[0] = 1.0;
                          d[1] = 1.0;
                          d[2] = 1.0;
                          d[3] = 0.0;
                          d[4] = 1.0;
                          d[5] = 1.0;
                      }
                      else if (bil[8] == "z")
                      {
                          d[0] = 0.0;
                          d[1] = 0.0;
                          d[2] = 0.0;
                          d[3] = 1.0;
                          d[4] = 1.0;
                          d[5] = 1.0;
                      }
                      else
                      {
                          d[0] = 0.0;
                          d[1] = 0.0;
                          d[2] = 0.0;
                          d[3] = 0.0;
                          d[4] = 0.0;
                          d[5] = 0.0;
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
