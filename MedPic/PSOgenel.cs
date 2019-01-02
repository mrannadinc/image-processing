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
    class PSOgenel
    {   public double[] Cozum;
        public class Particles
        {   public double[] Position;
            public double[] Velocity;
            public double[] PBestPosition;
            public double PBest;
            public double EvalValue;
            public Particles(int dim)
            {   Position = new double[dim];
                Velocity = new double[dim];
                PBestPosition = new double[dim];                
            }
        };

   
        public PSOgenel( int Dim, int NumOfParticle, int Generation, int WhichFunc)
        {   Cozum = new double[Dim];
            PSOarama( Dim, NumOfParticle, Generation, WhichFunc);                

        }
      
              
        public void  PSOarama( int Dimention, int NumOfParticle, int Generation, int WhichFunc)
        {   double[] GlobalBestPosition; GlobalBestPosition=new double[Dimention];
            double Gbest; int gbestid;int d,i, j;
            double C1 = 1.49;
            double C2 = 1.49;
            double W = 0.72;
            double r0,r1, r2;
            double vmax =10.0;

            double minX =-4.0;
            double maxX = 4.0;
         
            Particles[] particle = new Particles[NumOfParticle];

            for ( j = 0; j < NumOfParticle; j++)
            {   particle[j] = null;
                particle[j] = new Particles(Dimention); 
            }     
           

            Random rnd = new Random();      
           
            for (j = 0; j < NumOfParticle; j++)
            { 
                for ( d =0; d < Dimention; d++)
                {    r0=rnd.NextDouble();
                     particle[j].Position[d] = minX+(maxX-minX)*r0;
                     particle[j].Velocity[d] = 0.3;
                     particle[j].PBestPosition[d] = particle[j].Position[d];                       
                }           
             }            


              for ( j = 0; j < NumOfParticle; j++)
              {
                  if (WhichFunc == 1)  { particle[j].EvalValue = function1(particle[j]); }
                  if (WhichFunc == 2) { particle[j].EvalValue =  function2(particle[j]); }
                  particle[j].PBest = particle[j].EvalValue;
               }

           
            gbestid = 0;  Gbest = 0;            
            for ( i = 0; i < Generation; i++)
            {
                        for ( j = 0; j < NumOfParticle; j++)
                        {  
                            if (WhichFunc == 1)    { particle[j].EvalValue =function1(particle[j]); }
                            if (WhichFunc == 2)    { particle[j].EvalValue =function2(particle[j]); }
                            if (particle[j].EvalValue > particle[j].PBest) 
                            {   particle[j].PBest = particle[j].EvalValue;
                                for ( d = 0; d < Dimention; d++)
                                { particle[j].PBestPosition[d] = particle[j].Position[d]; }
                            }
                        }

                
                       for ( j = 0; j < NumOfParticle; j++)
                        {  
                            if (particle[j].PBest > Gbest)
                              {   Gbest = particle[j].PBest;
                                  gbestid = j;
                              }
                            for (d = 0; d < Dimention; d++)
                              {   GlobalBestPosition[d] = particle[gbestid].Position[d];  }
                        }


                        for ( j = 0; j < NumOfParticle; j++)
                        {  
                            for ( d = 0; d < Dimention; d++)
                            {   r1 = rnd.NextDouble(); r2 = rnd.NextDouble();
                                particle[j].Velocity[d] = W * particle[j].Velocity[d] +
                                                          C1 * r1 * (particle[j].PBestPosition[d] - particle[j].Position[d]) +
                                                          C2 * r2 * (GlobalBestPosition[d] - particle[j].Position[d]);
                               
                                if(particle[j].Velocity[d]>vmax) particle[j].Velocity[d]=vmax;
                                particle[j].Position[d] = particle[j].Position[d] + particle[j].Velocity[d];

                                if (particle[j].Position[d] > maxX) { particle[j].Position[d] = maxX; }
                                if (particle[j].Position[d] < minX) { particle[j].Position[d] = minX; }
                                                              
                            }                     
                        }
             } ///generation

            for ( d = 0; d < Dimention; d++)
            { Cozum[d] = GlobalBestPosition[d];  }         
         
        }  


    
   

        ///////////////////////objectiv Function1////////////////////////-
        double function1(Particles Part)
        {   double x,y,z,q1,q2,zigma; zigma=1.0;
            x = Part.Position[0];
            y = Part.Position[1];
            q1 =(x-1)*(x-1);    q2 =(y-1)*(y-1);           
            z = (q1+q2) / (2 * zigma * zigma);
            z =Math.Exp(-z);
            return z;
        }

        ///////////////////////objectiv Function2////////////////////////      

            double function2(Particles Part)
            {   double x, y, z, q1, q2, zigma; zigma = 2.0;
                x = Part.Position[0];
                y = Part.Position[1];
                q1 = (x - 2) * (x - 2); q2 = (y - 2) * (y - 2);
                z = (q1 + q2) / (2 * zigma * zigma);
                z = Math.Exp(-z);
                return z;
            }


    }
}
