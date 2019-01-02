using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedPic
{
    class Neuron       // http://www.codingvision.net/miscellaneous/c-backpropagation-tutorial-xor
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

        public void adjustWeights(double error)
        {  for(int i=0;i<n;i++)
           {  weights[i] =weights[i]+error * inputs[i];}  }
        }

    
}
