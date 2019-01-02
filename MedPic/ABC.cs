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
    class ABC
    {
        public double[] Hist;
        public double[] m_Thresh;
        public int[] m_ThreshR;
        public int[] m_ThreshG;
        public int[] m_ThreshB;
        private int m_limit;
        private Bitmap m_img;
        private int m_NP;
        private static int m_Dim;
        private int m_maxCycle;
        private int m_FoodNumber;
        private int m_whichFunc;

        private double m_lb = 0; /*lower bound of the parameters. */
        private double m_ub = 255; /*upper bound of the parameters. lb and ub can be defined as arrays for the problems of which parameters have different bounds*/
        private static int runtime = 1; /*Algorithm can be run many times in order to see its robustness*/

        ////////////////////////////////////////////////
        double ObjValSol;              /*Objective function value of new solution*/
        double FitnessSol;              /*Fitness value of new solution*/
        int neighbour, param2change;   /*param2change corrresponds to j, neighbour corresponds to k in equation v_{ij}=x_{ij}+\phi_{ij}*(x_{kj}-x_{ij})*/

        double GlobalMax;                       /*Optimum solution obtained by ABC algorithm*/
        double[] GlobalParams;     /*Parameters of the optimum solution*/
        double[] GlobalMins;   /*GlobalMins holds the GlobalMin of each run in multiple runs*/
        double r; /*a random number in the range [0,1)*/
        public HistRGB rgbHist;
        ///////////////////////////////////////////////
        public class HistRGB
        {
            public double[] hr;
            public double[] hg;
            public double[] hb;

            public HistRGB()
            {
                hr = new double[256];
                hg = new double[256];
                hb = new double[256];
            }
        };

        class Bee
        {
            //original// public static double[][] Foods=new double[FoodNumber][D];        /*Foods is the population of food sources. Each row of Foods matrix is a vector holding D parameters to be optimized. The number of rows of Foods matrix equals to the FoodNumber*/
            public double[] Foods;
            public double[] solution;
            public double f;        /*f is a vector holding objective function values associated with food sources */
            public double fitness;      /*fitness is a vector holding fitness (quality) values associated with food sources*/
            public double trial;         /*trial is a vector holding trial numbers through which solutions can not be improved*/
            public double prob;          /*prob is a vector holding probabilities of food sources (solutions) to be chosen*/

            public Bee()
            {
                Foods = new double[m_Dim];
                solution = new double[m_Dim];
                f = 0;
                fitness = 0;
                trial = 0;
                prob = 0;
            }
        };

        Bee[] m_bees;

        public ABC(Bitmap image, int NumP, int maxCycle, int Dim, int limit, int WhichFunc)
        {
            GlobalMax = 0;
            m_Thresh = new double[Dim];
            m_ThreshR = new int[Dim];
            m_ThreshG = new int[Dim];
            m_ThreshB = new int[Dim];
            GlobalParams = new double[Dim];
            m_img = image;
            GlobalMins = new double[runtime];
            m_Dim = Dim;
            m_NP = NumP;
            m_maxCycle = maxCycle;
            m_FoodNumber = m_NP / 2;
            m_whichFunc = WhichFunc;
            //rgbHist = new HistRGB();
            //rgbHist = null;
            rgbHist = Histogram(image);
            m_limit = limit;

            Hist = null;
            Hist = rgbHist.hr;

            for (int i = 0; i < 255; i++)
            {
                if (rgbHist.hr[i] == 0)
                {
                    m_lb++;
                }
                else
                {
                    break;
                }
            }
            for (int i = 255; i >= 0; i--)
            {
                if (rgbHist.hr[i] == 0)
                {
                    m_ub--;
                }
                else
                {
                    break;
                }
            }

            GlobalMax = 0;
            m_Thresh = null;
            m_Thresh = new double[Dim];
            ABCExe();

            for (int i = 0; i < m_Thresh.Length; i++)
            {
                m_ThreshR[i] = (int)m_Thresh[i];
            }

            Hist = null;
            Hist = rgbHist.hg;


            m_lb = 0;
            m_ub = 255;
            for (int i = 0; i < 255; i++)
            {
                if (rgbHist.hg[i] == 0)
                {
                    m_lb++;
                }
                else
                {
                    break;
                }
            }
            for (int i = 255; i >= 0; i--)
            {
                if (rgbHist.hg[i] == 0)
                {
                    m_ub--;
                }
                else
                {
                    break;
                }
            }
            GlobalMax = 0;
            m_Thresh = null;
            m_Thresh = new double[Dim];
            ABCExe();
            for (int i = 0; i < m_Thresh.Length; i++)
            {
                m_ThreshG[i] = (int)m_Thresh[i];
            }

            Hist = null;
            Hist = rgbHist.hb;


            m_lb = 0;
            m_ub = 255;
            for (int i = 0; i < 255; i++)
            {
                if (rgbHist.hb[i] == 0)
                {
                    m_lb++;
                }
                else
                {
                    break;
                }
            }
            for (int i = 255; i >= 0; i--)
            {
                if (rgbHist.hb[i] == 0)
                {
                    m_ub--;
                }
                else
                {
                    break;
                }
            }
            m_Thresh = null;
            m_Thresh = new double[Dim];
            GlobalMax = 0;
            ABCExe();

            for (int i = 0; i < m_Thresh.Length; i++)
            {
                m_ThreshB[i] = (int)m_Thresh[i];
            }


        }

        Random rnd = new Random();

        public void ABCExe()
        {
            for (int run = 0; run < runtime; run++)
            {
                initial();


                for (int iter = 0; iter < m_maxCycle; iter++)
                {
                    SendEmployedBees();
                    CalculateProbabilities();
                    SendOnlookerBees();
                    MemorizeBestSource();
                    SendScoutBees();
                }

            }

        }






        void init(int index)
        {

            double adim = 255 / (m_Dim + 1);
            m_bees[index] = null;
            m_bees[index] = new Bee();
            for (int j = 0; j < m_Dim; j++)
            {
                // m_bees[index].Foods[j] = m_lb + j * adim + rnd.NextDouble() * adim;   

                m_bees[index].Foods[j] = m_lb + rnd.NextDouble() * (m_ub - m_lb);
                m_bees[index].solution[j] = m_bees[index].Foods[j];
            }

            m_bees[index].f = calculateFunction(m_bees[index].solution);
            m_bees[index].fitness = CalculateFitness(m_bees[index].f);
            m_bees[index].trial = 0;
        }


        /*All food sources are initialized */
        void initial()
        {
            int i;
            m_bees = new Bee[m_FoodNumber];
            for (i = 0; i < m_FoodNumber; i++)
            {
                init(i);
            }

        }
        /// Get Histogram Data
        public HistRGB Histogram(Bitmap bmp1)
        {
            int i, j; Color p9;
            int[] Hr, Hg, Hb; Hr = new int[256]; Hg = new int[256]; Hb = new int[256];

            for (i = 0; i < 256; i++)
            { Hr[i] = 0; Hg[i] = 0; Hb[i] = 0; }

            for (j = 0; j < bmp1.Height; j++)
            {
                for (i = 0; i < bmp1.Width; i++)
                {
                    p9 = bmp1.GetPixel(i, j);
                    Hr[p9.R] = Hr[p9.R] + 1; Hg[p9.G] = Hg[p9.G] + 1; Hb[p9.B] = Hb[p9.B] + 1;
                }
            }
            HistRGB Hrgb = new HistRGB();

            for (i = 0; i < 256; i++)
            {
                Hrgb.hr[i] = (double)(Hr[i]) / (bmp1.Width * bmp1.Height);
                Hrgb.hg[i] = (double)(Hg[i]) / (bmp1.Width * bmp1.Height);
                Hrgb.hb[i] = (double)(Hb[i]) / (bmp1.Width * bmp1.Height);
            }
            return Hrgb;
        }

        //Mean
        private int Mx(int L1, int L2, double[] hist)
        {
            double w, m; int i, t;
            w = 0; m = 0;
            for (i = L1; i <= L2; i++)
            {
                w = w + hist[i];
                m = m + i * hist[i];
            }
            if (w > 0) t = (int)(m / w); else t = 0;
            return t;
        }


        ///////////////////////OBJECTIVE FUNCTION - OTSU ////////////////////////


        double calculateFunction(double[] solution)
        {
           if(m_whichFunc == 1)
            return OtsuGenel(solution);
           if (m_whichFunc == 2)
               return KapurGenel(solution);
           else
               return 1;
        }


        private double Zigx(int L1, int L2, double[] hist)  //otsu local variance
        {
            double w, zig, m, mT;
            int i;
            w = 0; m = 0; mT = 0; zig = 0;
            for (i = 0; i < hist.Length; i++)
            { mT = mT + i * hist[i]; }

            for (i = L1; i <= L2; i++)
            {
                w = w + hist[i];
                m = m + i * hist[i];
            }
            if (w > 0)
            {
                m = m / w;
            }
            zig = w * (m - mT) * (m - mT);
            return zig;
        }


        double OtsuGenel(double[] solution)
        {
            int i = m_Dim;
            int[] T = new int[m_Dim + 2];
            double[] zigma = new double[m_Dim + 1];
            T[0] = 0; T[m_Dim + 1] = 256;

            for (i = 0; i < m_Dim; i++)
            { T[i + 1] = (int)solution[i]; }    // { T[i + 1] = (int) m_bees[index].solution[i]; }

            for (i = 0; i < m_Dim + 1; i++)
            {
                if (T[i] > T[i + 1])
                    return 0;
            }

            for (i = 0; i < m_Dim + 1; i++)
            { zigma[i] = Zigx(T[i], T[i + 1] - 1, Hist); }
            return zigma.Sum();
        }

        ///////////////////////amaç fonksiyonu 2///////////////////////
        public double Ex(int L1, int L2, double[] hist)
        {
            double w, E; w = 0; E = 0; int i;
            for (i = L1; i <= L2; i++)
            { w = w + hist[i]; }
            for (i = L1; i <= L2; i++)
            { if (hist[i] > 0 && w > 0) E = E - (hist[i] / w) * Math.Log(hist[i] / w); }
            return E;
        }
        double KapurGenel(double[] solution)
        {
            int i, dim = m_Dim;
            int[] T = new int[dim + 2];
            double[] H = new double[dim + 1];
            T[0] = 0; T[dim + 1] = 256;

            for (i = 0; i < dim; i++)
            { T[i + 1] = (int)solution[i]; }
            for (i = 0; i < dim + 1; i++)
            {
                if (T[i] > T[i + 1])
                    return 0;
            }

            for (i = 0; i < dim + 1; i++)
            { H[i] = Ex(T[i], T[i + 1] - 1, Hist); }

            return H.Sum();
        }


        /// ABC Start Point ///

        /*Fitness function*/

        double CalculateFitness(double fun)
        {
            double result = 0;
            if (fun >= 0)
            {
                result = 1 / (fun + 1);
            }
            else
            {

                result = 1 + Math.Abs(fun);
            }
            return result;
        }


        /*The best food source is memorized*/
        void MemorizeBestSource()
        {

            for (int i = 0; i < m_FoodNumber; i++)
            {
                if (m_bees[i].f > GlobalMax)
                {
                    GlobalMax = m_bees[i].f;
                    for (int j = 0; j < m_Dim; j++)
                    {
                        GlobalParams[j] = m_bees[i].Foods[j];
                        m_Thresh[j] = GlobalParams[j];
                    }
                }
            }
        }





        void SendEmployedBees()
        {

            /*Employed Bee Phase*/
            for (int i = 0; i < m_FoodNumber; i++)
            {
                /*The parameter to be changed is determined randomly*/
                r = rnd.NextDouble();
                param2change = (int)(r * m_Dim);

                /*A randomly chosen solution is used in producing a mutant solution of the solution i*/
                r = rnd.NextDouble();
                neighbour = (int)(r * m_FoodNumber);

                /*Randomly selected solution must be different from the solution i*/
                // while(neighbour==i)
                // {
                // r = (   (double)Math.random()*32767 / ((double)(32767)+(double)(1)) );
                // neighbour=(int)(r*FoodNumber);
                // }
                for (int j = 0; j < m_Dim; j++)
                    m_bees[i].solution[j] = m_bees[i].Foods[j];

                /*v_{ij}=x_{ij}+\phi_{ij}*(x_{kj}-x_{ij}) */

                r = rnd.NextDouble();
                m_bees[i].solution[param2change] = m_bees[i].Foods[param2change] + ((-2) + (r * 4)) * (m_bees[i].Foods[param2change] - m_bees[neighbour].Foods[param2change]);

                /*if generated parameter value is out of boundaries, it is shifted onto the boundaries*/
                if (m_bees[i].solution[param2change] < m_lb)
                    m_bees[i].solution[param2change] = m_lb;
                if (m_bees[i].solution[param2change] > m_ub)
                    m_bees[i].solution[param2change] = m_ub;
                for (int d = 0; d < m_Dim; d++)
                {
                    if ((d > 0) && m_bees[i].solution[d - 1] > m_bees[i].solution[d])
                    {
                        // particle[j].Position[d] =(double) rnd.Next((int)(minzero + d * adim), (int)(minzero + (d + 1) * adim));
                        init(i);
                       
                    }
                }
                ObjValSol = calculateFunction(m_bees[i].solution);
                FitnessSol = CalculateFitness(ObjValSol);

                /*a greedy selection is applied between the current solution i and its mutant*/
                if (FitnessSol > m_bees[i].fitness)
                {

                    /*If the mutant solution is better than the current solution i, replace the solution with the mutant and reset the trial counter of solution i*/
                    m_bees[i].trial = 0;
                    for (int j = 0; j < m_Dim; j++)
                        m_bees[i].Foods[j] = m_bees[i].solution[j];
                    m_bees[i].f = ObjValSol;
                    m_bees[i].fitness = FitnessSol;
                }
                else
                {   /*if the solution i can not be improved, increase its trial counter*/
                    m_bees[i].trial = m_bees[i].trial + 1;
                }


            }

            /*end of employed bee phase*/

        }


        /* A food source is chosen with the probability which is proportioal to its quality*/
        /*Different schemes can be used to calculate the probability values*/
        /*For example prob(i)=fitness(i)/sum(fitness)*/
        /*or in a way used in the metot below prob(i)=a*fitness(i)/max(fitness)+b*/
        /*probability values are calculated by using fitness values and normalized by dividing maximum fitness value*/

        void CalculateProbabilities()
        {
            int i;
            double maxfit;
            maxfit = m_bees[0].fitness;
            for (i = 1; i < m_FoodNumber; i++)
            {
                if (m_bees[i].fitness > maxfit)
                    maxfit = m_bees[i].fitness;
            }
            for (i = 0; i < m_FoodNumber; i++)
            {
                m_bees[i].prob = (0.9 * (m_bees[i].fitness / maxfit)) + 0.1;
            }
        }

        void SendOnlookerBees()
        {
            //double tempRandom = rnd.NextDouble(); // returns a random number between 0.0 and 1.0
            int i, j, t;
            i = 0;
            t = 0;
            /*onlooker Bee Phase*/
            while (t < m_FoodNumber)
            {

                r = rnd.NextDouble();
                if (r < m_bees[i].prob) /*choose a food source depending on its probability to be chosen*/
                {
                    t++;

                    /*The parameter to be changed is determined randomly*/
                    r = rnd.NextDouble();
                    param2change = (int)(r * m_Dim);

                    /*A randomly chosen solution is used in producing a mutant solution of the solution i*/
                    r = rnd.NextDouble();
                    neighbour = (int)(r * m_FoodNumber);

                    /*Randomly selected solution must be different from the solution i*/
                    while (neighbour == i)
                    {
                        //System.out.println(Math.random()*32767+"  "+32767);
                        r = rnd.NextDouble();
                        neighbour = (int)(r * m_FoodNumber);
                    }
                    for (j = 0; j < m_Dim; j++)
                        m_bees[i].solution[j] = m_bees[i].Foods[j];

                    /*v_{ij}=x_{ij}+\phi_{ij}*(x_{kj}-x_{ij}) */
                    r = rnd.NextDouble();
                    m_bees[i].solution[param2change] = m_bees[i].Foods[param2change] + ((-2) + (r * 4)) * (m_bees[i].Foods[param2change] - m_bees[neighbour].Foods[param2change]);

                    /*if generated parameter value is out of boundaries, it is shifted onto the boundaries*/
                    if (m_bees[i].solution[param2change] < m_lb)
                        m_bees[i].solution[param2change] = m_lb;
                    if (m_bees[i].solution[param2change] > m_ub)
                        m_bees[i].solution[param2change] = m_ub;
                    ObjValSol = calculateFunction(m_bees[i].solution);
                    FitnessSol = CalculateFitness(ObjValSol);

                    /*a greedy selection is applied between the current solution i and its mutant*/
                    if (FitnessSol > m_bees[i].fitness)
                    {
                        /*If the mutant solution is better than the current solution i, replace the solution with the mutant and reset the trial counter of solution i*/
                        m_bees[i].trial = 0;
                        for (j = 0; j < m_Dim; j++)
                            m_bees[i].Foods[j] = m_bees[i].solution[j];
                        m_bees[i].f = ObjValSol;
                        m_bees[i].fitness = FitnessSol;
                    }
                    else
                    {   /*if the solution i can not be improved, increase its trial counter*/
                        m_bees[i].trial = m_bees[i].trial + 1;
                    }
                } /*if */
                i++;
                if (i == m_FoodNumber)
                    i = 0;
            }/*while*/

            /*end of onlooker bee phase     */
        }

        /*determine the food sources whose trial counter exceeds the "limit" value. In Basic ABC, only one scout is allowed to occur in each cycle*/
        void SendScoutBees()
        {
            //int maxtrialindex =0;
            //for (int i=1;i<m_FoodNumber;i++)
            //        {
            //            if (m_bees[i].trial > m_bees[maxtrialindex].trial)
            //         maxtrialindex=i;
            //        }
            //if(m_bees[maxtrialindex].trial>= m_limit)
            //{
            //    init(maxtrialindex);
            //}

            for (int i = 1; i < m_FoodNumber; i++)
            {
                if (m_bees[i].trial >= m_limit)

                    init(i);
            }
        }


        //double calculateFunction(int index)
        //{
        //    return OtsuGenel(index);
        //}


        //double calculateFunction(double[] sol)
        //{
        //    return Rastrigin(sol);
        //}

        double sphere(double[] sol)
        {
            int j;
            double top = 0;
            for (j = 0; j < m_Dim; j++)
            {
                top = top + sol[j] * sol[j];
            }
            return top;
        }

        double Rosenbrock(double[] sol)
        {
            int j;
            double top = 0;
            for (j = 0; j < m_Dim - 1; j++)
            {
                top = top + 100 * Math.Pow((sol[j + 1] - Math.Pow((sol[j]), (double)2)), (double)2) + Math.Pow((sol[j] - 1), (double)2);
            }
            return top;
        }

        double Griewank(double[] sol)
        {
            int j;
            double top1, top2, top;
            top = 0;
            top1 = 0;
            top2 = 1;
            for (j = 0; j < m_Dim; j++)
            {
                top1 = top1 + Math.Pow((sol[j]), (double)2);
                top2 = top2 * Math.Cos((((sol[j]) / Math.Sqrt((double)(j + 1))) * Math.PI) / 180);

            }
            top = (1 / (double)4000) * top1 - top2 + 1;
            return top;
        }

        double Rastrigin(double[] sol)
        {
            int j;
            double top = 0;

            for (j = 0; j < m_Dim; j++)
            {
                top = top + (Math.Pow(sol[j], (double)2) - 10 * Math.Cos(2 * Math.PI * sol[j]) + 10);
            }
            return top;
        }


    }
}
