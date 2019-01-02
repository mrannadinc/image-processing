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

namespace MedPic
{
    class PSO
    {
        double[] Thresh;  
        public int[] ThreshR;
        public int[] ThreshG;
        public int[] ThreshB; 
        public class HistRGB
        {   public double[] hr;
            public double[] hg;
            public double[] hb;
            public HistRGB()
            {   hr = new double[256];
                hg = new double[256];
                hb = new double[256];
            }
        };

        public class Particles
        {   public double[] Position;
            public double[] Velocity;
            public double[] PBestPosition;
            public double[] TestPosition;
            public double PBest;
            public double EvalValue;
            public Particles(int dim)
            {   Position = new double[dim];
                Velocity = new double[dim];
                PBestPosition = new double[dim];
                TestPosition = new double[dim];
            }
        };


        public PSO(Bitmap img, int Dim, int NumOfParticle, int Generation, int WhichFunc)
        {   Thresh = new double[Dim];
            ThreshR = new int[Dim];
            ThreshG = new int[Dim];
            ThreshB = new int[Dim];
            HistRGB rgbHist = Histogram(img);

            PSOarama(rgbHist.hr, Dim, NumOfParticle, Generation, WhichFunc);
            for (int j = 0; j < Dim; j++)
             { ThreshR[j] = (int)Thresh[j]; }

            PSOarama(rgbHist.hg, Dim, NumOfParticle, Generation, WhichFunc);
            for (int j = 0; j < Dim; j++)
             { ThreshG[j] = (int)Thresh[j]; }

            PSOarama(rgbHist.hb, Dim, NumOfParticle, Generation, WhichFunc);
            for (int j = 0; j < Dim; j++)
             { ThreshB[j] = (int)Thresh[j]; }

        }
        ///////////////////////////////////////////////////////
        public HistRGB Histogram(Bitmap bmp1)
        {   int i, j; Color p9;
            int[] Hr, Hg, Hb;   Hr = new int[256];   Hg = new int[256];   Hb = new int[256];

            for (i = 0; i < 256; i++)
            { Hr[i] = 0; Hg[i] = 0; Hb[i] = 0; }

            for (j = 0; j < bmp1.Height; j++)
            {  for (i = 0; i < bmp1.Width; i++)
                {   p9 = bmp1.GetPixel(i, j);
                    Hr[p9.R] = Hr[p9.R] + 1; Hg[p9.G] = Hg[p9.G] + 1; Hb[p9.B] = Hb[p9.B] + 1;
                }
            }
            HistRGB Hrgb = new HistRGB();

            for (i = 0; i < 256; i++)
            {   Hrgb.hr[i] = (double)(Hr[i]) / (bmp1.Width * bmp1.Height);
                Hrgb.hg[i] = (double)(Hg[i]) / (bmp1.Width * bmp1.Height);
                Hrgb.hb[i] = (double)(Hb[i]) / (bmp1.Width * bmp1.Height);
            }
            return Hrgb;
        }

        private int Mx(int L1, int L2, double[] hist)  //Mean
        {   double w, m; int i, t;  w = 0; m = 0;
            for (i = L1; i <= L2; i++)
            {   w = w + hist[i];
                m = m + i * hist[i];
            }
            if (w > 0) t = (int)(m / w); else t = 0;
            return t;
        }

        
        /////////////////////////pso//////////////////////////////
        public void  PSOarama(double[] Hist, int Dimention, int NumOfParticle, int Generation, int WhichFunc)
        {   double[] GlobalBestPosition; GlobalBestPosition=new double[Dimention];
            int bestind; int i, j,d;
            double Gbest;
            double C1 = 1.25;
            double C2 = 1.25;
            double W = 0.97;
            double vmax = 255;           

            int minzero = 0;
            int maxzero = 255;
            for ( i = 0; i < 255; i++)
             {  if (Hist[i] == 0)   minzero++;   else break;    }
            for ( i = 255; i > 0; i--)
             {  if (Hist[i] == 0) maxzero--;  else break;   }

            Particles[] particle = new Particles[NumOfParticle];
            for ( j = 0; j < NumOfParticle; j++)
            {   particle[j] = null;  particle[j] = new Particles(Dimention); }
                                          

            Random rnd = new Random();
            ///////////////////////////INITIAL//////////////////////
            double adim =(maxzero-minzero) /(Dimention+1);   
        
            for (j = 0; j < NumOfParticle; j++)
            {  for (d =0; d < Dimention; d++)
                { // particle[j].Position[d] =(double) rnd.Next((int) (minzero+d*adim), (int)(minzero+(d+1)*adim));
                 //  particle[j].Position[d]= (minzero+d*adim-adim/2) +rnd.NextDouble() *adim ;
                     particle[j].Position[d] =minzero + d*adim + rnd.NextDouble()*adim;
                     particle[j].Velocity[d] = 0.3;
                     particle[j].PBestPosition[d] = particle[j].Position[d];
                     particle[j].TestPosition[d] = particle[j].Position[d];                 
                } 
            }            

             for (j = 0; j < NumOfParticle; j++)
              {   if (WhichFunc == 1)   { particle[j].EvalValue = OtsuGenel(particle[j], Hist); }
                  if (WhichFunc == 2)   { particle[j].EvalValue = KapurGenel(particle[j], Hist); }
                  particle[j].PBest = particle[j].EvalValue;
             }

       
            bestind = 0;    Gbest = 0;         
            /////////////////////////  LOOP  /////////////////////////
            for (i = 0; i < Generation; i++)
            {
                        for (j = 0; j < NumOfParticle; j++)
                        {   if (WhichFunc == 1)  { particle[j].EvalValue = OtsuGenel(particle[j], Hist); }
                            if (WhichFunc == 2)  { particle[j].EvalValue = KapurGenel(particle[j], Hist); }
                            if (particle[j].EvalValue > particle[j].PBest) 
                            {   particle[j].PBest = particle[j].EvalValue;
                                for ( d = 0; d < Dimention; d++)
                                { particle[j].PBestPosition[d] = particle[j].Position[d]; }
                            }
                         }

                
                       for ( j = 0; j < NumOfParticle; j++)
                        {   if (particle[j].PBest > Gbest)
                              {   Gbest = particle[j].PBest; bestind = j; }
                            for (d = 0; d < Dimention; d++)
                              {  GlobalBestPosition[d] = particle[bestind].Position[d]; }
                        }


                        for (j = 0; j < NumOfParticle; j++)
                        {  
                            for ( d = 0; d < Dimention; d++)
                            {        particle[j].Velocity[d] = W * particle[j].Velocity[d] +
                                           C1 * rnd.NextDouble() * (particle[j].PBestPosition[d] - particle[j].Position[d]) +
                                           C2 * rnd.NextDouble() * (GlobalBestPosition[d] - particle[j].Position[d]);
                               
                                  if(particle[j].Velocity[d]>vmax) particle[j].Velocity[d]=vmax;
                                   particle[j].TestPosition[d] = particle[j].Position[d] + particle[j].Velocity[d];

                                    /*
                                   int test = ParticleTest(particle[j]);
                                   if (test == 1)
                                     { particle[j].Position[d] = particle[j].TestPosition[d]; }
                                   else
                                     {   for ( int k = 0; k < Dimention; k++)
                                         { particle[j].Position[k] = rnd.Next((int)(minzero + k * adim), (int)(minzero + (k + 1) * adim)); } 
                                     }
                                   */
                                   if (particle[j].TestPosition[d] > maxzero) { particle[j].TestPosition[d] = maxzero; }
                                   if (particle[j].TestPosition[d] < minzero) { particle[j].TestPosition[d] = minzero; }

                                   if ((d > 0) && particle[j].TestPosition[d - 1] > particle[j].TestPosition[d])
                                     {
                                      // particle[j].Position[d] =(double) rnd.Next((int)(minzero + d * adim), (int)(minzero + (d + 1) * adim));
                                       particle[j].Position[d] = minzero + d * adim + rnd.NextDouble() * adim;
                                     }
                                   else
                                       particle[j].Position[d] = particle[j].TestPosition[d];
                            }                     
                        }
             }


            for (d = 0; d < Dimention; d++)
            { Thresh[d]=particle[bestind].PBestPosition[d];
             // Thresh[d] = GlobalBestPosition[d];
            }         
         
        }  ////////////////////////   LOOP  ////////////////////////



        int  ParticleTest(Particles Part)
        {   int k,i, d = Part.Position.Length;
             k =1;
            for (i = 0; i < (d-1) ; i++)
            {  if (Part.Position[i] >Part.Position[i + 1])
                    return 0;
               else k= 1; 
            }
            return k;         
        }


        ///////////////////////objectiv Function////////////////////////


        private double Zigx(int L1, int L2, double[] hist)  //otsu local variance
        {   double w, zig, m, mT;
            int i;
            w = 0; m = 0; mT = 0; zig = 0;
            for (i = 0; i < hist.Length; i++)
             { mT = mT + i * hist[i]; }

            for (i = L1; i <= L2; i++)
             {   w = w + hist[i];
                m = m + i * hist[i];
             }
            if (w > 0) m = m / w;
            zig = w * (m - mT) * (m - mT);
            return zig;
        }

        ///////////////////////objectiv Function1////////////////////////

        double OtsuGenel(Particles Part, double[] Hist)
        {   int i,dim = Part.Position.Length;           
            int[] T = new int[dim + 2];
            double[] zigma = new double[dim + 1];
            T[0] = 0;    T[dim + 1] =256;
            for (i = 0; i < dim; i++)
             {   T[i + 1] = (int)Part.Position[i];  }
            for (i = 0; i < dim + 1; i++)
             {  if (T[i] > T[i + 1])  return 0;  }

            for (i = 0; i < dim + 1; i++)
              { zigma[i] = Zigx(T[i], T[i + 1] - 1, Hist);  }
            return zigma.Sum();
        }




        ///////////////////////objectiv Function2////////////////////////
            public double Ex(int L1, int L2, double[] hist)
            {   double w, E; w = 0; E = 0; int i;
                for (i = L1; i <= L2; i++)
                 { w = w + hist[i]; }
                for (i = L1; i <= L2; i++)
                 { if (hist[i] > 0 && w > 0) E = E - (hist[i] / w) * Math.Log(hist[i] / w); }
                return E;
            }

            double KapurGenel(Particles Part, double[] Hist)
            {   int i,dim = Part.Position.Length;
                int[] T = new int[dim + 2];
                double[] H = new double[dim + 1];
                T[0] = 0;  T[dim + 1] = 256;

                for (i = 0; i < dim; i++)
                  {  T[i + 1] = (int)Part.Position[i];  }
                for (i = 0; i < dim + 1; i++)
                  {    if (T[i] > T[i + 1])  return 0;  }

                for (i = 0; i < dim + 1; i++)
                  {  H[i] = Ex(T[i], T[i + 1] - 1, Hist);  }

                return H.Sum();
            }


    }
}
