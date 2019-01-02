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
using AForge;
using AForge.Video;
using AForge.Video.DirectShow;

namespace MedPic
{
    public partial class Form1 : Form
    {
        Resim   Goruntu = new Resim();
        public FilterInfoCollection webcams;//webcam isminde tanımladığımız değişken bilgisayara kaç kamera bağlıysa onları tutan bir dizi. 
        public VideoCaptureDevice cam;//cam ise bizim kullanacağımız aygıt.


      

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            hScrollBar2.Value = 80;
            hScrollBar1.Value = 80;        
            
            pictureBox1.Image = Goruntu.resim1;
            Goruntu.HistogramEsitle(Goruntu.resim1);


            webcams = new FilterInfoCollection(FilterCategory.VideoInputDevice);//webcam dizisine mevcut kameraları dolduruyoruz.
            if (webcams != null)
            {
                foreach (FilterInfo videocapturedevice in webcams)
                { comboBox1.Items.Add(videocapturedevice.Name); }    //kameralar comboboxta             
                //  comboBox1.SelectedIndex = 0;
                comboBox1.SelectedItem = 0;

            }
        }
        
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            open.Filter = "Image Files(*.jpg; *.jpeg; *.gif; *.bmp; *.png)|*.jpg; *.jpeg; *.gif; *.bmp; *.png";
            if (open.ShowDialog() == DialogResult.OK)
            {   Goruntu.DosyaAdi = open.FileName;
                Goruntu.resim1 = new Bitmap(open.FileName);
                Goruntu.resim2 = new Bitmap(open.FileName);
                Goruntu.data = new int[Goruntu.resim1.Width, Goruntu.resim1.Height];
                int x, y;
                for (x = 0; x < Goruntu.resim1.Width; x++)
                {   for (y = 0; y < Goruntu.resim1.Height; y++)
                    { Goruntu.data[x, y] = -1; }
                }
                pictureBox1.Image = Goruntu.resim1;
                pictureBox2.Image = Goruntu.resim2;
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {   SaveFileDialog save = new SaveFileDialog();
            save.Filter = "Image Files(*.jpg; *.jpeg; *.gif; *.bmp)|*.jpg; *.jpeg; *.gif; *.bmp";
            if (save.ShowDialog() == DialogResult.OK)
            {   Goruntu.resim1 = (Bitmap)pictureBox1.Image.Clone();                
                Goruntu.resim1.Save(save.FileName);
            }
        }

        private void reloadToolStripMenuItem_Click(object sender, EventArgs e)
        {   pictureBox1.Image = Image.FromFile(Goruntu.DosyaAdi);
            Goruntu.resim1 = (Bitmap)pictureBox1.Image.Clone();
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }


        private void redToolStripMenuItem_Click(object sender, EventArgs e)
        {   Bitmap bmp1 = new Bitmap(pictureBox1.Image);
            Bitmap bmp2 = new Bitmap(bmp1);
            bmp2 = Goruntu.GetCom(bmp1, 1, 0, 0); pictureBox2.Image = bmp2;
        }

        private void greenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Bitmap bmp1 = new Bitmap(pictureBox1.Image);
            Bitmap bmp2 = new Bitmap(bmp1);
            bmp2 = Goruntu.GetCom(bmp1, 0, 1, 0); pictureBox2.Image = bmp2;
        }

        private void blueToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Bitmap bmp1 = new Bitmap(pictureBox1.Image);
            Bitmap bmp2 = new Bitmap(bmp1);
            bmp2 = Goruntu.GetCom(bmp1, 0, 0, 1); pictureBox2.Image = bmp2;
        }

        private void cToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Bitmap bmp1 = new Bitmap(pictureBox1.Image);
            Bitmap bmp2 = new Bitmap(bmp1);
            bmp2 = Goruntu.GetCom(bmp1, -1, 1, 1);     //p9 = Color.FromArgb(255 - p9.R, p9.G, p9.B); cyan
            pictureBox2.Image = bmp2;
        }

        private void mToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Bitmap bmp1 = new Bitmap(pictureBox1.Image);
            Bitmap bmp2 = new Bitmap(bmp1);
            bmp2 = Goruntu.GetCom(bmp1, 1, -1, 1);  // p9 = Color.FromArgb(p9.R, 255 - p9.G, p9.B); magenta
            pictureBox2.Image = bmp2;
        }

        private void yToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Bitmap bmp1 = new Bitmap(pictureBox1.Image);
            Bitmap bmp2 = new Bitmap(bmp1);
            bmp2 = Goruntu.GetCom(bmp1, 1, 1, -1);   // p9 = Color.FromArgb(p9.R, p9.G, 255 - p9.B); yellow
            pictureBox2.Image = bmp2;
        }

        private void cMYToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Bitmap bmp1 = new Bitmap(pictureBox1.Image);
            Bitmap bmp2 = new Bitmap(bmp1);
            bmp2 = Goruntu.GetCom(bmp1, -1, -1, -1); pictureBox2.Image = bmp2;
        }

        private void grayToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Bitmap bmp1 = new Bitmap(pictureBox1.Image);
            Bitmap bmp2 = new Bitmap(bmp1);
            bmp2 = Goruntu.Gray(bmp1); pictureBox2.Image = bmp2;
        }


        private void randGToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Bitmap bmp1 = new Bitmap(pictureBox1.Image);
            Bitmap bmp2 = new Bitmap(bmp1);
            bmp2 = Goruntu.GetCom(bmp1, 1, 1, 0); pictureBox2.Image = bmp2;
        }

        private void randBToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Bitmap bmp1 = new Bitmap(pictureBox1.Image);
            Bitmap bmp2 = new Bitmap(bmp1);
            bmp2 = Goruntu.GetCom(bmp1, 1, 0, 1); pictureBox2.Image = bmp2;
        }

        private void gandBToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Bitmap bmp1 = new Bitmap(pictureBox1.Image);
            Bitmap bmp2 = new Bitmap(bmp1);
            bmp2 = Goruntu.GetCom(bmp1, 0, 1, 1); pictureBox2.Image = bmp2;
        }

        private void translateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Bitmap bmp1 = new Bitmap(pictureBox1.Image);
            Bitmap bmp2 = new Bitmap(bmp1);
            Resim.Nokta3D konum = new Resim.Nokta3D(100, 100, 0);
            bmp2 = Goruntu.TranslateImage(bmp1, konum); pictureBox2.Image = bmp2;
        }

        private void rotateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Bitmap bmp1 = new Bitmap(pictureBox1.Image);
            Bitmap bmp2 = new Bitmap(bmp1);

            Goruntu.resim1 = bmp1;    bmp2 = Goruntu.RotateImage(-60); pictureBox2.Image = bmp2;

            Resim.Nokta3D pivot = new Resim.Nokta3D(100, 100, 0);
            pivot = new Resim.Nokta3D((double)bmp1.Width / 2, (double)bmp1.Height / 2, 0);
           //  bmp2 =Goruntu. RotateImageA(bmp1, 130); pictureBox2.Image = bmp2;            
          //  bmp2 = Goruntu.RotateImagePivot(bmp1, pivot, -60); pictureBox2.Image = bmp2;
            //   bmp2 = Goruntu.RotateImage(30); pictureBox2.Image = bmp2;

             
             
          /*
              Pen pen1 = new System.Drawing.Pen(Color.Blue, 2F);
              Graphics g;
              // g = pictureBox1.CreateGraphics();
               g = Graphics.FromImage(bmp1);
                g.DrawRectangle(pen1, 100, 100, 50, 60);
                g.RotateTransform(30);
                g.DrawImage(bmp1, 0, 0);
                 pictureBox2.Image = bmp1;
            */

            /*
           
             // Now draw the old bitmap on it.
             using (Graphics g = Graphics.FromImage(bmp1))
             {   Pen pen1 = new System.Drawing.Pen(Color.Blue, 2F);
                 g.DrawRectangle(pen1, 100, 100, 50, 60);
                 g.TranslateTransform(bmp1.Width / 2.0f, bmp1.Height / 2.0f);
                 g.RotateTransform(-50);
                 g.TranslateTransform(-bmp1.Width / 2.0f, -bmp1.Height / 2.0f);
                 // g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                 // g.Clear(Color.Black);
                 g.DrawImage(bmp1, 0, 0);
             }
            pictureBox2.Image = bmp1;         
         */


        }

        private void scaleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Bitmap bmp1 = new Bitmap(pictureBox1.Image);
            Bitmap bmp2 = new Bitmap(bmp1);
           // bmp2 = Goruntu.ScaleImageA(bmp1, 2.0, 1.0); pictureBox2.Image = bmp2;
            bmp2 = Goruntu.Resize(1.25); pictureBox2.Image = bmp2;
        }

        private void kaotikToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Bitmap bmp1 = new Bitmap(pictureBox1.Image);
            Bitmap bmp2 = new Bitmap(bmp1);
            bmp2 = Goruntu.MandelbrotSet(bmp1, 2, -2, 2, -2); pictureBox2.Image = bmp2;

        }


        private void poolingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Bitmap bmp1 = new Bitmap(pictureBox1.Image);
            Bitmap bmp2 = new Bitmap(bmp1);
            bmp2 = Goruntu.Pooling(bmp1, 3, 2); pictureBox2.Image = bmp2;
        }


        private void histogramToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Bitmap bmp1 = new Bitmap(pictureBox1.Image);
            Bitmap bmp2 = new Bitmap(pictureBox1.Image);
            double[] hr, hg, hb; hr = new double[256]; hg = new double[256]; hb = new double[256];
            double[] sr, sg, sb; sr = new double[256]; sg = new double[256]; sb = new double[256];
            double[] kumr, kumg, kumb; kumr = new double[256]; kumg = new double[256]; kumb = new double[256];
            int[] T; T = new int[3]; int i, m; double max;

         
            i = 5;
            double[] maske= new double[i];
            maske = Goruntu.createFilter(i, 3);
            
            Goruntu.Histogram(bmp1, hr, hg, hb);
            Goruntu.HistogramtFile(bmp1, hr, hg, hb);

         //   sr = Goruntu.getGaussHist(hr, maske, i);

            chart1.Series["Series1"].Points.Clear();
            chart1.Series["Series2"].Points.Clear();
            chart1.Series["Series3"].Points.Clear();
            for (i = 0; i < 256; i++)
            {
                chart1.Series["Series1"].Points.AddY(1 * hr[i]);
                chart1.Series["Series2"].Points.AddY(1 * hg[i]);
                chart1.Series["Series3"].Points.AddY(1 * hb[i]);
            }
        }

        private void histoEqualToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Bitmap bmp1 = new Bitmap(pictureBox1.Image);
            Bitmap bmp2 = new Bitmap(bmp1);
            double[] hr, hg, hb; hr = new double[256]; hg = new double[256]; hb = new double[256];
            double[] her, heg, heb; her = new double[256]; heg = new double[256]; heb = new double[256];
            Goruntu.Histogram(bmp1, hr, hg, hb);
            bmp2 = Goruntu.HistogramEsitle(bmp1);
            Goruntu.Histogram(bmp2, her, heg, heb);
            chart1.Series["Series1"].Points.Clear();
            chart1.Series["Series2"].Points.Clear();
            chart1.Series["Series3"].Points.Clear();
            for (int i = 0; i < 256; i++)
            {
                chart1.Series["Series1"].Points.AddY(hr[i]);
                chart1.Series["Series2"].Points.AddY(her[i]);
                chart1.Series["Series3"].Points.AddY(her[i]);
            }
            pictureBox2.Image = bmp2;
        }



        private void sigmoidToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Bitmap bmp1 = new Bitmap(pictureBox1.Image);
            Bitmap bmp2 = new Bitmap(bmp1);
            Bitmap bmp3 = new Bitmap(bmp1);
            int[] T; T = new int[3];
            double x, y, b;

            Goruntu.getMean(bmp1, T);
            // getOtsu(bmp1, T);

            b = 25;
            textBox1.Text = Convert.ToString(T[0]);
            textBox2.Text = Convert.ToString(T[1]);
            textBox3.Text = Convert.ToString(T[2]);
            bmp2 = Goruntu.Sigmoid(bmp1, T[0], T[1], T[2], (int)b / 3);
            pictureBox2.Image = bmp2;

            chart1.Series["Series1"].Points.Clear();
            chart1.Series["Series2"].Points.Clear();
            chart1.Series["Series3"].Points.Clear();

            for (x = 0; x < 256; x = x + 1)
            {
                y = 255 / (1 + Math.Exp(-(double)(x - T[0]) / (T[0] / 3)));
                chart1.Series["Series1"].Points.AddY(y);
                chart1.Series["Series2"].Points.AddY(y);
                chart1.Series["Series3"].Points.AddY(y);
            }
        }



        private void fFTToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Bitmap bmp1 = new Bitmap(pictureBox1.Image);
            Bitmap bmp2 = new Bitmap(pictureBox1.Image);
            int i, N, M; Color p9; N = bmp1.Width;
            double[] hr, hg, hb;
            Complex[] input; Complex[] output; Complex[] donus;
            hr = new double[N]; hg = new double[N]; hb = new double[N];
            input = new Complex[N]; output = new Complex[N]; donus = new Complex[N];

            for (i = 0; i < N; i++)
            {
                p9 = bmp1.GetPixel(i, 100);
                Complex temp = new Complex((double)p9.R, 0);
                input[i] = temp; hr[i] = temp.real; hg[i] = temp.imag;
                p9 = Color.FromArgb(0, 255, 0);
                bmp2.SetPixel(i, 100, p9);
            }
            pictureBox1.Image = bmp2;

            output = Goruntu.FFT(input, -1);
            donus = Goruntu.FFT(output, 1); hb = Goruntu.ArrayFromComplex(donus, 1, 1, 10);
            // input = Goruntu.ArrayShift(output); donus= Goruntu.FFT(input, 1); hb = Goruntu.ArrayFromComplex(donus, 1, 1, 1);

            chart1.Series["Series1"].Points.Clear();
            chart1.Series["Series2"].Points.Clear();
            chart1.Series["Series3"].Points.Clear();
            for (i = 0; i < N; i++)
            {
                chart1.Series["Series1"].Points.AddY(input[i].real);
                //  chart1.Series["Series2"].Points.AddY(donus[i].magnitude);
                //  chart1.Series["Series1"].Points.AddY(output[i].magnitude);
                chart1.Series["Series2"].Points.AddY(255 * hb[i]);
            }


            N = bmp1.Width; M = bmp1.Height;
            Complex[,] bfxy = new Complex[N, M];
            Complex[,] fxy = new Complex[N, M];
            Complex[,] ifxy = new Complex[N, M];
            Complex[,] mfxy = new Complex[N, M];


            // fxy = Goruntu.getFFTa(bmp1); bmp2 = Goruntu.BitmapFromComplex(fxy, 1, 2, 10);
            //   fxy =getFFTb(bmp1);                                      bmp2 = BitmapFromComplex(fxy, 1,2,10); 
            //  fxy =getFFTc(bmp1);                                      bmp2 = BitmapFromComplex(fxy, 1,2,10);

            fxy = Goruntu.getFFTa(bmp1); mfxy = Goruntu.FFTShift(fxy); bmp2 = Goruntu.BitmapFromComplex(mfxy, 1, 2, 10);
            //  fxy =getFFTb(bmp1);   mfxy = FFTShift(fxy);              bmp2 = BitmapFromComplex(mfxy, 1,2,10); 
            //  fxy =getFFTc(bmp1);   mfxy = FFTShift(fxy);              bmp2 = BitmapFromComplex(mfxy, 1,2,10);

            //  fxy =getFFTa(bmp1);   ifxy = FFT2Da(fxy,-1);             bmp2 = BitmapFromComplex(ifxy, 1,1,1);
            //  fxy =getFFTb(bmp1);   ifxy = FFT2Da(fxy,-1);             bmp2 = BitmapFromComplex(ifxy, 1,1,1); 
            //  fxy =getFFTc(bmp1);   ifxy = FFT2Dc(fxy,-1);             bmp2 = BitmapFromComplex(ifxy, 1,1,1);

            //bfxy = ComplexFromBitmap(bmp1);   fxy = FFT2Da(bfxy,1);     bmp2 = BitmapFromComplex(fxy, 1, 2, 10);

            // fxy =getFFTa(bmp1);   mfxy =Hilbert(fxy);                           bmp2 = BitmapFromComplex(mfxy, 1,2,10);
            //  fxy = getFFTa(bmp1);  mfxy = Hilbert(fxy);  ifxy = FFT2Da(mfxy, -1); bmp2 = BitmapFromComplex(ifxy, 1, 1, 1);  
            // Fxybitmap = Modulate(Fxy, bmp3);          bmp2 = BitmapFromComplex(Fxybitmap, 1, 2);
            pictureBox2.Image = bmp2;      
           
        }



        private void noiseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Bitmap bmp1 = new Bitmap(pictureBox1.Image);
            Bitmap bmp2 = new Bitmap(bmp1);
            bmp2 = Goruntu.VarimageN(bmp1, 4); pictureBox2.Image = bmp2;
        }




        private void averageToolStripMenuItem_Click(object sender, EventArgs e)
        {   Bitmap bmp1 = new Bitmap(pictureBox1.Image);
            Bitmap bmp2 = new Bitmap(bmp1);
            double[] PSNR; PSNR = new double[3];
            double[] MSE; MSE = new double[3];
            bmp2 = Goruntu.MeanFilter(bmp1, 3);
            // bmp2 =BilateralFilter(bmp1,5,128,4);
           // Goruntu.GetMSE(bmp1,bmp2, MSE, PSNR);
            Goruntu.GetSSIM(bmp1, bmp2, MSE);
            Goruntu.HistogramCompare(bmp1, bmp2, MSE);
            textBox1.Text = Convert.ToString(MSE[0]);
            textBox2.Text = Convert.ToString(MSE[1]);
            textBox3.Text = Convert.ToString(MSE[2]);
            pictureBox2.Image = bmp2;
        }


        private void medyanToolStripMenuItem_Click(object sender, EventArgs e)
        {   Bitmap bmp1 = new Bitmap(pictureBox1.Image);
            Bitmap bmp2 = new Bitmap(bmp1);
            double[] PSNR; PSNR = new double[3];
            double[] MSE; MSE = new double[3];
            bmp2 = Goruntu.MedyanFilter(bmp1, 3);
            // bmp2 = GradFilter(bmp1, 3);
            Goruntu.GetMSE(bmp1, bmp2, MSE, PSNR);
            textBox1.Text = Convert.ToString(MSE[0]);
            textBox2.Text = Convert.ToString(MSE[1]);
            textBox3.Text = Convert.ToString(MSE[2]);
            pictureBox2.Image = bmp2;
        }

              

        private void diffusionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            double[] PSNR; PSNR = new double[3];
            double[] MSE; MSE = new double[3];
            int kr, kg, kb, t, x; int[] T; T = new int[3]; Color p1, p2;
            Bitmap bmp1 = new Bitmap(pictureBox1.Image);
            Bitmap bmp2 = new Bitmap(bmp1);
            Bitmap bmp3 = new Bitmap(bmp1);

            kr = 12; kg = 12; kb = 12;
            for (t = 0; t <5; t++)
            {
                bmp2 = Goruntu.FilterDiffusion(bmp1, kr, kg, kb, 1, 1);
                Goruntu.GetMSE(bmp1, bmp2, MSE, PSNR);
                //GetSSIM(bmp1, bmp2, MSE);
                bmp1 = (Bitmap)bmp2.Clone();
                textBox1.Text = Convert.ToString(MSE[0]);
                textBox2.Text = Convert.ToString(MSE[1]);
                textBox3.Text = Convert.ToString(MSE[2]);
            }

            Goruntu.GetMSE(bmp3, bmp2, MSE, PSNR);
            textBox1.Text = Convert.ToString(MSE[0]);
            textBox2.Text = Convert.ToString(MSE[1]);
            textBox3.Text = Convert.ToString(MSE[2]);

            chart1.Series["Series1"].Points.Clear();
            chart1.Series["Series2"].Points.Clear();

            for (x = 0; x < bmp1.Width; x++)
            {   p1 = bmp1.GetPixel(x, 150);
                p2 = bmp2.GetPixel(x, 150);
                chart1.Series["Series1"].Points.AddY(p1.R);
                chart1.Series["Series2"].Points.AddY(p2.R);
            }
            pictureBox2.Image = bmp2;
        }

        private void aDiffusionToolStripMenuItem_Click(object sender, EventArgs e)
        {
           

        }

        private void gaussianToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Bitmap bmp1 = new Bitmap(pictureBox1.Image);
            Bitmap bmp2 = new Bitmap(bmp1);
            double[] PSNR; PSNR = new double[3];
            double[] MSE; MSE = new double[3];
         //   bmp2 = Goruntu.GaussFilter(bmp1, 3, 1.78);
            bmp2 = Goruntu.GaussFunction(bmp1, 1.41,0.41,30);        
            Goruntu.GetMSE(bmp1, bmp2, MSE, PSNR);
            textBox1.Text = Convert.ToString(PSNR[0]);
            textBox2.Text = Convert.ToString(PSNR[1]);
            textBox3.Text = Convert.ToString(PSNR[2]);
            pictureBox2.Image = bmp2;
        }




        private void convulationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Bitmap bmp1 = new Bitmap(pictureBox1.Image);
            Bitmap bmp2 = new Bitmap(bmp1);
            double[,] w; w = new double[3, 3];
            /*
            w[0, 0] = -1.0; w[0, 1] = -1.0; w[0, 2] = 2.0;
            w[1, 0] = -1.0; w[1, 1] = 0.0; w[1, 2] = 2.0;
            w[2, 0] = -1.0; w[2, 1] = 2.0; w[2, 2] = 2.0;
            */
            w[0, 0] = -1.0; w[0, 1] = -1.0; w[0, 2] = -1.0;
            w[1, 0] = -1.0; w[1, 1] = 8.0; w[1, 2] = -1.0;
            w[2, 0] = -1.0; w[2, 1] = -1.0; w[2, 2] = -1.0;

            bmp2 = Goruntu.Convulation(bmp1, w,2); pictureBox2.Image = bmp2;
        }


        private void smootingToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void bilateralToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Bitmap bmp1 = new Bitmap(pictureBox1.Image);
            Bitmap bmp2 = new Bitmap(bmp1);
            double[] PSNR; PSNR = new double[3];
            double[] MSE; MSE = new double[3];
           bmp2 = Goruntu.BilateralFilter(bmp1, 3,128, 0.2);
       
            Goruntu.GetMSE(bmp1, bmp2, MSE, PSNR);
            textBox1.Text = Convert.ToString(PSNR[0]);
            textBox2.Text = Convert.ToString(PSNR[1]);
            textBox3.Text = Convert.ToString(PSNR[2]);
            pictureBox2.Image = bmp2;
        }  


     private void derivativeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Bitmap bmp1 = new Bitmap(pictureBox1.Image);
            Bitmap bmp2 = new Bitmap(bmp1);
          // bmp2 = Goruntu.GradientX(bmp1); pictureBox2.Image = bmp2;
            bmp2 = Goruntu.GradientY(bmp1); pictureBox2.Image = bmp2;
        }


        private void gradientToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Bitmap bmp1 = new Bitmap(pictureBox1.Image);
            Bitmap bmp2 = new Bitmap(bmp1);
            bmp2 = Goruntu.Gradient(bmp1); pictureBox2.Image = bmp2;
        }

        private void laplaceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Bitmap bmp1 = new Bitmap(pictureBox1.Image);
            Bitmap bmp2 = new Bitmap(bmp1);
            bmp2 = Goruntu.Laplace(bmp1); pictureBox2.Image = bmp2;
        }

       

        private void logToolStripMenuItem_Click(object sender, EventArgs e)
        {     Bitmap bmp1 = new Bitmap(pictureBox1.Image);
            Bitmap bmp2 = new Bitmap(bmp1);
            bmp2 = Goruntu.LogFilter(bmp1,3,1.4); pictureBox2.Image = bmp2;
        }

       
       private void gaborToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Bitmap bmp1 = new Bitmap(pictureBox1.Image);
            Bitmap bmp2 = new Bitmap(bmp1);
            bmp2 = Goruntu.GaborFunction(bmp1,1.4, 1.4,30,0.5,0.5); pictureBox2.Image = bmp2;
           // bmp2 = Goruntu.GaborFilter(bmp1,3,1.0, 1.0,190,1.25,1.25); pictureBox2.Image = bmp2;

           /*
            int banksayi = 2;
            double[] zigmaxb = new double[banksayi];
            double[] zigmayb = new double[banksayi];
            double[] tetab = new double[banksayi];
            double[] fxb = new double[banksayi];
            double[] fyb = new double[banksayi];
            zigmaxb[0] = 0.25; zigmaxb[1] = 1.5;
            zigmayb[0] = 1.5; zigmayb[1] = 0.5;
            tetab[0] = 0.0; tetab[1] =270.0;
            fxb[0] = 0.5; fxb[1] = 1.5;
            fyb[0] = 0.5; fyb[1] = 1.5;
            bmp2 = Goruntu.GaborFilterBank(bmp1, 5, zigmaxb, zigmayb, tetab, fxb, fyb, 1);
            pictureBox2.Image = bmp2; 
            * */
       }



        private void regionAToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Bitmap bmp1 = new Bitmap(pictureBox1.Image);
            Bitmap bmp2 = new Bitmap(bmp1);
            Color p9;
            double mem; int x, y;
            if (Goruntu.Pixelsecimi == true)
            {
                for (x = 0; x < bmp1.Width; x++)
                {
                    for (y = 0; y < bmp1.Height; y++)
                    {
                        p9 = bmp1.GetPixel(x, y); mem = Goruntu.MemberShip3(p9, Goruntu.c1, 42, 3);
                        if (mem > Goruntu.memTresh) Goruntu.c2 = Color.FromArgb(0, 255, 0); else Goruntu.c2 = p9;
                        bmp2.SetPixel(x, y, Goruntu.c2);
                    }
                }
                pictureBox2.Image = bmp2;
            }
            else MessageBox.Show("Önce Picture1 de Pixel Secimi yapınız....!");
           Goruntu.Pixelsecimi = true;
        }

        private void regionBToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Bitmap bmp1 = new Bitmap(pictureBox1.Image);
            Bitmap bmp2 = new Bitmap(bmp1);
            Bitmap bmp3 = new Bitmap(bmp1);
            Bitmap bmp4 = new Bitmap(bmp1);
            Bitmap bmp5 = new Bitmap(bmp1);
            int[] T; T = new int[3];
            int[,] data1; data1 = new int[bmp1.Width, bmp1.Height];
         
            bmp2 = Goruntu.RegionbParam(bmp1, data1, 0.96, 2); pictureBox2.Image = bmp2;
            //   bmp2 = RegionbParam(bmp1, data1, memTresh, 3); pictureBox2.Image = bmp2;
            //   bmp2 = Regionb(bmp1, data1, 0.90); pictureBox2.Image = bmp2;         
           
            bmp2 = Goruntu.BolgeGoster2(bmp1, data1, 0, 0, Color.FromArgb(0, 255, 255)); pictureBox2.Image = bmp2;
            Goruntu.data = data1; Goruntu.Pixelsecimi = true;
        }


        private void recursiveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Goruntu.RecursiveGrowing();
        }


        private void binaryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Bitmap bmp1 = new Bitmap(pictureBox1.Image); Bitmap bmp2 = new Bitmap(bmp1);
            int[,] data1; data1 = new int[bmp1.Width, bmp1.Height];
            int EsikSayisi; EsikSayisi =0; int[] T; T = new int[3];
            int[] Tr = new int[EsikSayisi]; int[] Tg = new int[EsikSayisi]; int[] Tb = new int[EsikSayisi];
            double[] h = new double[8];

            if (EsikSayisi == 0)
            {
                bmp2 = Goruntu.MakeBinary(bmp1, 1); pictureBox2.Image = bmp2;
                //    bmp2 = Make2E(bmp1,1);  pictureBox2.Image = bmp2;
            }
            if (EsikSayisi == 1)
            {
                Goruntu.getMean(bmp1, T);
                //  getMedyan(bmp1, T);
                //  OtsuKapurTekEsik(bmp1, T,1);
                //  OtsuKapurTekEsik(bmp1, T,2);                                    
                Goruntu.OtsuKapurTekEsikNew(bmp1, T, 1);
               
                Tr[0] = T[0]; Tg[0] = T[1]; Tb[0] = T[2];
             
                textBox1.Text = Convert.ToString(T[0]);
                textBox2.Text = Convert.ToString(T[1]);
                textBox3.Text = Convert.ToString(T[2]);

              //  bmp2 = Goruntu.MakeMultiEsikGray(bmp1, data1, Tr, 3); pictureBox2.Image = bmp2;
            }

        
            // int c = RegionFilter2(data1, 3); textBox3.Text = Convert.ToString(c);
            Goruntu.data = data1;
        }



        private void tDHistoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Bitmap bmp1 = new Bitmap(pictureBox1.Image);
            Bitmap bmp2 = new Bitmap(bmp1);
           
            int[,] data1; data1 = new int[bmp1.Width, bmp1.Height];

          

            bmp2 = Goruntu.RegionTDH(bmp1, data1, 10, 0.60, 0,Color.FromArgb(0, 0, 255)); pictureBox2.Image = bmp2;
          
            Goruntu.data = data1;
        }



        private void sDHistoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Bitmap bmp1 = new Bitmap(pictureBox1.Image);
            Bitmap bmp2 = new Bitmap(bmp1);
            int[] clabel; clabel = new int[256];
            int EsikSayisi; EsikSayisi = 2;
            int[] Tr = new int[EsikSayisi];
            int[] T; T = new int[3];
            int[,] data1; data1 = new int[bmp1.Width, bmp1.Height];

            pictureBox2.Image = bmp2;
            bmp2 =Goruntu.Region1DH(bmp1, data1, clabel, 10,  0.90,1,  Color.FromArgb(0, 0, 255)); pictureBox2.Image = bmp2;   
          

            Goruntu.Pixelsecimi = true;
        }


        private void lBGToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Bitmap bmp1 = new Bitmap(pictureBox1.Image);
            Bitmap bmp2 = new Bitmap(bmp1); double J; int cluster =4;
            Color[] c = new Color[cluster];
            double[] h = new double[cluster];
               
            bmp2 = Goruntu.LBG3(bmp1, Goruntu.data, c, h, cluster); pictureBox2.Image = bmp2;
            // J = Performans4(bmp1, data); textBox1.Text = Convert.ToString(J);
           // J = Goruntu.Performans5(bmp1, Goruntu.data); textBox1.Text = Convert.ToString(J);
             // bmp2 = BolgeGoster(bmp1, data, 0, 1, Color.FromArgb(0, 0, 255)); pictureBox2.Image = bmp2;  

            chart1.Series["Series1"].Points.Clear();
            chart1.Series["Series2"].Points.Clear();
            chart1.Series["Series3"].Points.Clear();
            for (int i = 0; i < cluster; i++)
            {
                chart1.Series["Series1"].Points.AddY(1 * h[i]);
                chart1.Series["Series2"].Points.AddY(1 * h[i]);
                chart1.Series["Series3"].Points.AddY(1 * h[i]);
            }



            listBox1.DataSource = null;
            listBox1.DataSource =c;  
    }
     

        private void kmeansToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Bitmap bmp1 = new Bitmap(pictureBox1.Image);
            Bitmap bmp2 = new Bitmap(bmp1); double J; int cluster =4;
            Color[] c = new Color[cluster];
            double[] h = new double[cluster];

          bmp2 = Goruntu.Kmeans(bmp1, Goruntu.data, cluster, 100); pictureBox2.Image = bmp2;
           //bmp2 = Goruntu.Kmeans2(bmp1, Goruntu.data, c,h,cluster, 20); pictureBox2.Image = bmp2;
         
            // J = Performans4(bmp1, data); textBox1.Text = Convert.ToString(J);
            J = Goruntu.Performans5(bmp1, Goruntu.data); textBox1.Text = Convert.ToString(J);
            //  bmp2 = BolgeGoster(bmp1, data, 0, 1, Color.FromArgb(0, 0, 255)); pictureBox2.Image = bmp2;  

            chart1.Series["Series1"].Points.Clear();
            chart1.Series["Series2"].Points.Clear();
            chart1.Series["Series3"].Points.Clear();
            for (int i = 0; i <cluster; i++)
            {
                chart1.Series["Series1"].Points.AddY(1 * h[i]);
                chart1.Series["Series2"].Points.AddY(1 * h[i]);
                chart1.Series["Series3"].Points.AddY(1 * h[i]);
            }

        }


        private void fcmToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Bitmap bmp1 = new Bitmap(pictureBox1.Image);
            Bitmap bmp2 = new Bitmap(bmp1); double J;
            Color[] renkler = new Color[4];

            bmp2 = Goruntu.fcm2(bmp1, Goruntu.data, 4, renkler, 10, 2.25); pictureBox2.Image = bmp2;
            // J = Performans4(bmp1, data); textBox1.Text = Convert.ToString(J);
            J = Goruntu.Performans5(bmp1, Goruntu.data); textBox1.Text = Convert.ToString(J);
            //  bmp2 = BolgeGoster(bmp1, data, 0, 1, Color.FromArgb(0, 0, 255)); pictureBox2.Image = bmp2;  

           
          
          
        }



        private void cokluEsikToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Bitmap bmp1 = new Bitmap(pictureBox1.Image); Bitmap bmp2 = new Bitmap(bmp1);
            int[,] data1; data1 = new int[bmp1.Width, bmp1.Height];
            int EsikSayisi; EsikSayisi =1;
            int[] Tr = new int[EsikSayisi]; int[] Tg = new int[EsikSayisi]; int[] Tb = new int[EsikSayisi];
            int ClusterSayisi = (int)Math.Pow((EsikSayisi + 1), 3);
            double[] h = new double[ClusterSayisi];


            if (EsikSayisi == 1)
            {
                  int[] T; T = new int[3];
                 Goruntu.OtsuKapurTekEsikNew(bmp1, T, 1);   Tr[0] = T[0]; Tg[0] = T[1]; Tb[0] = T[2];

              // Goruntu.Genetik(bmp1, Tr, 1);  Goruntu.Genetik(bmp1, Tg, 2);   Goruntu.Genetik(bmp1, Tb, 3);
              
                /*
                PSO Esikler = new PSO(bmp1, EsikSayisi, 200, 200, 1);
                Tr = Esikler.ThreshR; Tg = Esikler.ThreshG; Tb = Esikler.ThreshB;
               */
             
                /*
                 ABC abc = new ABC(bmp1, 350, 350, EsikSayisi, 20, 1);     //public ABC(Bitmap image, int NumP,int maxCycle, int Dim, int limit)   Nump: Bee Numbers  - maxCycyle: iteration 
                 for (int j = 0; j < EsikSayisi; j++)
                 {   Tr[j] = abc.m_ThreshR[j];
                     Tg[j] = abc.m_ThreshG[j];
                     Tb[j] = abc.m_ThreshB[j];
                 }
               */

                //  bmp2 = Goruntu.MakeMultiEsikGray(bmp1, data1, Tr, 3); pictureBox2.Image = bmp2;
                bmp2 = Goruntu.MakeMultiEsik(bmp1, data1, h, Tr, Tr, Tr, 5); pictureBox2.Image = bmp2;
                textBox1.Text = Convert.ToString(Tr[0]);
                textBox2.Text = Convert.ToString(Tg[0]);
                textBox3.Text = Convert.ToString(Tb[0]);


            }
           
            if (EsikSayisi == 2)
            {   
               // bmp2=Goruntu.OtsuKapurEsik2(bmp1, Tr, Tg, Tb, 1); pictureBox2.Image = bmp2;
                // Goruntu.OtsuKapurCoklu(bmp1, Tr, Tg, Tb, 1); textBox1.Text = Convert.ToString(Tr[0]) + ":" + Convert.ToString(Tr[1]);
                // Goruntu.Genetik(bmp1, Tr, 1);  Goruntu.Genetik(bmp1, Tg, 2);   Goruntu.Genetik(bmp1, Tb, 3);
               
             
                PSO  Esikler = new PSO(bmp1, EsikSayisi, 200, 200, 1);
               Tr = Esikler.ThreshR; Tg = Esikler.ThreshG;  Tb = Esikler.ThreshB; 
               
                /*
                 ABC abc = new ABC(bmp1, 350, 350, EsikSayisi, 20, 1);     //public ABC(Bitmap image, int NumP,int maxCycle, int Dim, int limit)   Nump: Bee Numbers  - maxCycyle: iteration 
                 for (int j = 0; j < EsikSayisi; j++)
                 {   Tr[j] = abc.m_ThreshR[j];
                     Tg[j] = abc.m_ThreshG[j];
                     Tb[j] = abc.m_ThreshB[j];
                 }
                */

               //  bmp2 = Goruntu.MakeMultiEsikGray(bmp1, data1, Tr, 3); pictureBox2.Image = bmp2;
                 bmp2 = Goruntu.MakeMultiEsik(bmp1, data1, h, Tr, Tr, Tr, 5);  pictureBox2.Image = bmp2;

                textBox1.Text = Convert.ToString(Tr[0]) + "ve " + Convert.ToString(Tr[1]);
                textBox2.Text = Convert.ToString(Tg[0]) + "ve " + Convert.ToString(Tg[1]);
                textBox3.Text = Convert.ToString(Tb[0]) + "ve " + Convert.ToString(Tb[1]);

             
            }

           
            if (EsikSayisi == 3)
            {
                             
                 //  Goruntu.OtsuKapurCoklu(bmp1, Tr, Tg, Tb, 1); textBox1.Text = Convert.ToString(Tr[0]) + "ve " + Convert.ToString(Tr[1]) + "ve" + Convert.ToString(Tr[2]);
                 //  Goruntu.Genetik(bmp1, Tr, 1);  Goruntu.Genetik(bmp1, Tg, 2);   Goruntu.Genetik(bmp1, Tb, 3);
             
                        
                PSO  Esikler = new PSO(bmp1, EsikSayisi, 50, 350, 1);
                Tr = Esikler.ThreshR; Tg = Esikler.ThreshG; Tb = Esikler.ThreshB;
                for (int j = 0; j < EsikSayisi; j++)
                {   Tr[j] = Esikler.ThreshR[j];
                    Tg[j] = Esikler.ThreshG[j];
                    Tb[j] = Esikler.ThreshB[j];
                }
            
                /*
                ABC abc = new ABC(bmp1, 350, 350, EsikSayisi, 20, 1);     //public ABC(Bitmap image, int NumP,int maxCycle, int Dim, int limit)   Nump: Bee Numbers  - maxCycyle: iteration 
                for (int j = 0; j < EsikSayisi; j++)
                {
                    Tr[j] = abc.m_ThreshR[j];
                    Tg[j] = abc.m_ThreshG[j];
                    Tb[j] = abc.m_ThreshB[j];
                }
                */


            
                

               //  bmp2 = Goruntu.MakeMultiEsikGray(bmp1, data1, Tr, 3); pictureBox2.Image = bmp2;
                 bmp2 = Goruntu.MakeMultiEsik(bmp1, data1, h, Tr, Tr, Tr, 4); pictureBox2.Image = bmp2;

                textBox1.Text = Convert.ToString(Tr[0]) + "ve " + Convert.ToString(Tr[1]) + "ve" + Convert.ToString(Tr[2]);
                textBox2.Text = Convert.ToString(Tg[0]) + "ve " + Convert.ToString(Tg[1]) + "ve " + Convert.ToString(Tg[2]);
                textBox3.Text = Convert.ToString(Tb[0]) + "ve " + Convert.ToString(Tb[1]) + "ve " + Convert.ToString(Tb[2]);
            }

            if (EsikSayisi > 3)
            {  
                //Genetik(bmp1, Tr, 1); Genetik(bmp1, Tg, 2); Genetik(bmp1, Tb, 3);
                /*
                PSO  Esikler = new PSO(bmp1, EsikSayisi, 250, 100, 2);
                for (int j = 0; j < EsikSayisi; j++)
                {  Tr[j] =  Esikler.ThreshR[j];
                    Tg[j] =  Esikler.ThreshG[j];
                    Tb[j] =  Esikler.ThreshB[j];
                }
                */

                ABC abc = new ABC(bmp1, 350, 350, EsikSayisi, 20, 1);     //public ABC(Bitmap image, int NumP,int maxCycle, int Dim, int limit)   Nump: Bee Numbers  - maxCycyle: iteration 
                for (int j = 0; j < EsikSayisi; j++)
                {
                    Tr[j] = abc.m_ThreshR[j];
                    Tg[j] = abc.m_ThreshG[j];
                    Tb[j] = abc.m_ThreshB[j];
                }

              //  bmp2 = Goruntu.MakeMultiEsikGray(bmp1, data1, Tr, 3); pictureBox2.Image = bmp2;
                bmp2 = Goruntu.MakeMultiEsik(bmp1, data1, h, Tr, Tr, Tr, 5); pictureBox2.Image = bmp2;

                textBox1.Text = Convert.ToString(Tr[0]) + "ve " + Convert.ToString(Tr[1]) + "ve" + Convert.ToString(Tr[2]);
                textBox2.Text = Convert.ToString(Tg[0]) + "ve " + Convert.ToString(Tg[1]) + "ve " + Convert.ToString(Tg[2]);
                textBox3.Text = Convert.ToString(Tb[0]) + "ve " + Convert.ToString(Tb[1]) + "ve " + Convert.ToString(Tb[2]);
            }
            // int c = RegionFilter2(data1, 3); textBox3.Text = Convert.ToString(c);
            Goruntu.data = data1;           
        }




        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            Bitmap bmp1 = new Bitmap(pictureBox1.Image);
            Bitmap bmp2 = new Bitmap(bmp1);
            Bitmap bmp3 = new Bitmap(bmp1);
            int xm, ym, n, x, y, w; Color p9;       
            xm = e.X; ym = e.Y;
            Goruntu.c1 = bmp1.GetPixel(xm, ym); Goruntu.Pixelsecimi = true;
              
         //    bmp2 = Goruntu.PointGrowing(bmp1, xm, ym);   pictureBox2.Image = bmp2;
           // bmp2 = Goruntu.NesneSec(bmp1, xm, ym); pictureBox2.Image = bmp2;
          //   bmp2 = Goruntu.NesneSay(bmp1, xm, ym); pictureBox2.Image = bmp2; 
               bmp2 = Goruntu.NesneTani(bmp1, xm, ym); pictureBox2.Image = bmp2;                      
             //  bmp3 = Goruntu.Sekil2(bmp2,1);  pictureBox2.Image = bmp3;  
           //  Sekil(bmp3);                   
            /* Rectangle a = new Rectangle(xmin, ymin, xmax-xmin, ymax-ymin);    bmp2 = Kesal(bmp1, a); pictureBox2.Image = bmp2; */
        }

        private void sobelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Bitmap bmp1 = new Bitmap(pictureBox1.Image);
            Bitmap bmp2 = new Bitmap(bmp1);
            Bitmap bmp3 = new Bitmap(bmp1);
            int[] T; T = new int[3];
            bmp2 = Goruntu.Gradient(bmp1);
            Goruntu.getMean(bmp2, T);
            //   getMedyan(bmp2, T);
            // OtsuTekEsik(bmp2, T); 
            // KapurTekEsik(bmp2, T);
            // EntropyHalfEsik(bmp2, T);

            textBox1.Text = Convert.ToString(T[0]);
            textBox2.Text = Convert.ToString(T[1]);
            textBox3.Text = Convert.ToString(T[2]);
            bmp3 = Goruntu.Threshold(bmp2, T[0], T[1], T[2]);
            // bmp1 = GetCom(bmp3,-1,-1,-1);
            bmp2 = Goruntu.Threshold(bmp1, 50, 50, 50);
            pictureBox2.Image = bmp3;
        }

        private void fuzzySobelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Bitmap bmp1 = new Bitmap(pictureBox1.Image);
            Bitmap bmp2 = new Bitmap(bmp1);
            bmp2 = Goruntu.FuzzyGradient(bmp1); pictureBox2.Image = bmp2;
        }

        private void cannyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Bitmap bmp1 = new Bitmap(pictureBox1.Image);
            Bitmap bmp2 = new Bitmap(bmp1);
            Bitmap bmp3 = new Bitmap(bmp1);
            Bitmap bmp4 = new Bitmap(bmp1);
            int[] T; T = new int[3]; double Tl;
            int[] Tg; Tg = new int[3]; int[] Tb; Tb = new int[3];

            bmp2 = Goruntu.Gradient(bmp1);

            Goruntu.getMean(bmp2, T);
            // getMedyan(bmp2, T);
            // getMAD(bmp2, T);
            // getAAD(bmp2, T);
            //   OtsuTekEsik(bmp2, T);
            // KapurTekEsik(bmp2, T);          
           
            // OtsuMultiEsik2(bmp2, T,Tg,Tb);
            bmp3 = Goruntu.GaussFilter(bmp1, 3, 1.5);
            // Tl = (double)((T[0] + T[1] + T[2]) / 3);
            textBox1.Text = Convert.ToString(T[0]);
            textBox2.Text = Convert.ToString(T[1]);
            textBox3.Text = Convert.ToString(T[2]);
            //bmp4 = CannyEdge(bmp3, Tl / 2, Tl);           
            // bmp4 = CannyEdge(bmp3, 5, 10); pictureBox2.Image = bmp4;
            bmp4 = Goruntu.CannyEdge(bmp3, T[0] / 2, T[0]); pictureBox2.Image = bmp4;
        }

        private void fuzzyCannyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Bitmap bmp1 = new Bitmap(pictureBox1.Image);
            Bitmap bmp2 = new Bitmap(bmp1);
            Bitmap bmp3 = new Bitmap(bmp1);
            Bitmap bmp4 = new Bitmap(bmp1);
            int[] T; T = new int[3]; double Tl;

            bmp2 = Goruntu.FuzzyGradient(bmp1);
            //  getMean(bmp2, T);
            //  getMedyan(bmp2, T);
            //  getMAD(bmp2, T);
            //  getAAD(bmp2, T);
            Goruntu.OtsuKapurTekEsik(bmp2, T, 1);
            //  KapurTekesik(bmp2, T);
         
            bmp3 = Goruntu.GaussFilter(bmp1, 3, 1.5);
            Tl = (double)((T[0] + T[1] + T[2]) / 3);
            textBox1.Text = Convert.ToString(T[0]);
            textBox2.Text = Convert.ToString(T[1]);
            textBox3.Text = Convert.ToString(T[2]);
            bmp4 = Goruntu.CannyEdge(bmp3, Tl / 2, Tl);
            //bmp4 = CannyEdge(bmp3, 40,60);
            pictureBox2.Image = bmp4;
        }

        private void similarityToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Bitmap bmp1 = new Bitmap(pictureBox1.Image);
            Bitmap bmp2 = new Bitmap(bmp1);
            bmp2 = Goruntu.Benzerlik(bmp1, 8, 3);
            //bmp2 = Goruntu.FuzzyBenzerlik(bmp1, 1);
            pictureBox2.Image = bmp2;
            int[] T; T = new int[3];
            Goruntu.getMean(bmp2, T);
            // OtsuTekEsik(bmp2, T); 

            // KapurTekEsik(bmp2, T);
            textBox1.Text = Convert.ToString(T[0]);
        }

        private void waveletToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Bitmap bmp1 = new Bitmap(pictureBox1.Image);
            Bitmap bmp2 = new Bitmap(bmp1);
            Bitmap bmph = new Bitmap(bmp1.Width / 2, bmp1.Height / 2);
            Bitmap bmpv = new Bitmap(bmph);
            Bitmap bmpd = new Bitmap(bmph);
            Bitmap bmpf = new Bitmap(bmph);

        
            int[] T; T = new int[3]; int i, j, x1, x2, x3; Color c1, c2, c3;
            Rectangle o = new Rectangle(0, 0, bmp1.Width / 2, bmp1.Height / 2);
            Rectangle h = new Rectangle(bmp1.Width / 2, 0, bmp1.Width / 2, bmp1.Height / 2);
            Rectangle v = new Rectangle(0, bmp1.Height / 2, bmp1.Width / 2, bmp1.Height / 2);
            Rectangle d = new Rectangle(bmp1.Width / 2, bmp1.Height / 2, bmp1.Width / 2, bmp1.Height / 2);

           bmp2 = Goruntu.Haar(bmp1);
            bmph = Goruntu.Kesal(bmp2, h);
            bmpv = Goruntu.Kesal(bmp2, v);
            bmpd = Goruntu.Kesal(bmp2, d);
            

           bmp2 = Goruntu.KesalHaar(bmp1, 3); pictureBox2.Image = bmp2;

            for (j = 0; j < bmph.Height; j++)
            {
                for (i = 0; i < bmph.Width; i++)
                {
                    c1 = bmph.GetPixel(i, j); c2 = bmpv.GetPixel(i, j); c3 = bmpd.GetPixel(i, j);
                    // x1 = (int)((c1.R+c2.R+c3.R)/3);
                    // x2 = (int)((c1.G + c2.G +c3.G) /3);
                    // x3 = (int)((c1.B + c2.B + c3.B) /3);
                    x1 = Math.Max(c1.R, Math.Max(c2.R, c3.R));
                    x2 = Math.Max(c1.G, Math.Max(c2.G, c3.G));
                    x3 = Math.Max(c1.B, Math.Max(c2.B, c3.B));
                    c2 = Color.FromArgb(x1, x2, x3);
                    bmpf.SetPixel(i, j, c2);
                }
            }

            // getMean(bmpf, T);
            Goruntu.OtsuKapurTekEsik(bmpf, T, 1);
            textBox1.Text = Convert.ToString(T[0]);
            textBox2.Text = Convert.ToString(T[1]);
            textBox3.Text = Convert.ToString(T[2]);
            bmpd = Goruntu.Threshold(bmpf, T[0] / 2, T[1] / 2, T[2] / 2);
            //bmpd =LGS(bmpf, T[0], T[1], T[2],5);
           // pictureBox2.Image = bmpf;
        }

        private void houghToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Bitmap bmp1 = new Bitmap(pictureBox1.Image);
            Bitmap bmp2 = new Bitmap(bmp1);
            //  bmp2 =Goruntu.Hough(bmp1); pictureBox2.Image = bmp2;
            //  bmp2 = Goruntu.Hough3(bmp1,360,360); pictureBox2.Image = bmp2;
            bmp2 = Goruntu.Hough4(bmp1, 360, 400, 0); pictureBox2.Image = bmp2;
        }

        private void haarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Bitmap bmp1 = new Bitmap(pictureBox1.Image);
            Bitmap bmp2 = new Bitmap(bmp1);
            bmp2 = Goruntu.Haar(bmp1); pictureBox2.Image = bmp2;
        }



        private void button1_Click(object sender, EventArgs e)
        { pictureBox2.Image = (Bitmap)(pictureBox1.Image.Clone());  }

        private void button2_Click(object sender, EventArgs e)
        {
            pictureBox1.Image = (Bitmap)(pictureBox2.Image.Clone());

        }


        private void button3_Click(object sender, EventArgs e)
        {
            Bitmap bmp1 = new Bitmap(pictureBox1.Image);
            Bitmap bmp2 = new Bitmap(pictureBox2.Image);
            double[] PSNR; PSNR = new double[3];
            double[] MSE; MSE = new double[3];
            double[] ssim; ssim = new double[3];
          
            Goruntu.GetMSE(bmp1, bmp2, MSE, PSNR);
          
         

            textBox1.Text = Convert.ToString(PSNR[0]);
            textBox2.Text = Convert.ToString(PSNR[1]);
            textBox3.Text = Convert.ToString(PSNR[2]);
            /*
             GetSSIM(bmp1, bmp2, ssim);
             textBox1.Text = Convert.ToString(ssim[0]);
             textBox2.Text = Convert.ToString(ssim[1]);
             textBox3.Text = Convert.ToString(ssim[2]);
             */

            /*
            chart1.Series["Series1"].Points.Clear();
            chart1.Series["Series2"].Points.Clear();

            Color p1, p2; int i;
            for (i = 0; i < bmp1.Width; i++)
            {
                p1 = bmp1.GetPixel(i, 150);
                p2 = bmp2.GetPixel(i, 150);
                chart1.Series["Series1"].Points.AddY(p1.R);
                chart1.Series["Series2"].Points.AddY(p2.R);
            }
          */
       
          //  Bitmap bmp3 = new Bitmap(bmp1);
          //  bmp3 = Goruntu.ResimBenzer(bmp1, bmp2); pictureBox2.Image = bmp3;

        }

        private void button4_Click(object sender, EventArgs e)
        {  Bitmap bmp1 = new Bitmap(pictureBox1.Image);
            Bitmap bmp2 = new Bitmap(bmp1);  double J;    
           // RegionFilter(data,3);
           // J = Goruntu.Performans4(bmp1, Goruntu.data); textBox1.Text = Convert.ToString(J);
              J = Goruntu.Performans5(bmp1, Goruntu.data); textBox1.Text = Convert.ToString(J);
         //  bmp2 = Goruntu.BolgeGoster(bmp1, Goruntu.data, 5, 1, Color.FromArgb(0, 0, 255)); pictureBox2.Image = bmp2; 
          //  bmp2 = Goruntu.BolgeGoster2(bmp1, Goruntu.data, 3, 1, Color.FromArgb(0, 0, 255)); pictureBox2.Image = bmp2;  
                
        }

        private void button5_Click(object sender, EventArgs e)
        {   Bitmap bmp1 = new Bitmap(pictureBox2.Image);
            Bitmap bmp2 = new Bitmap(bmp1); 

         
          //  textBox1.Text =Goruntu.RLEencode2("aaaxqqqqcceee");
           // textBox2.Text =Goruntu.RLEdecode("4x3c");
        }

        private void freemanToolStripMenuItem_Click(object sender, EventArgs e)
        {
            double[] q = new double[8]; int i;
              Bitmap bmp1 = new Bitmap(pictureBox1.Image);
              Bitmap bmp2 = new Bitmap(pictureBox1.Image);
              Goruntu.resim1 = bmp1;
              Goruntu.chaincode = "";
              Goruntu.ComplexChain.Clear();
           // Goruntu.FreemanChain8(bmp1);
           // Goruntu.FreemanChain81(bmp1);
           // Goruntu.FreemanChain82(bmp1); 
            //  Goruntu.FreemanChain83(bmp1);
            //  Goruntu.FreemanChain84(bmp1);
            //  Goruntu.FreemanChain85(bmp1);

             
/*
             Complex[] myArr = (Complex[]) Goruntu.ComplexChain.ToArray( typeof( Complex ) );
             textBox3.Text = Convert.ToString(myArr.Length);
             textBox2.Text = Convert.ToString(Goruntu.chaincode.Length);
             textBox1.Text = Goruntu.chaincode;  pictureBox2.Image = Goruntu.resim2; 
           
          Goruntu.ChainCodeHistogram(Goruntu.chaincode, q); Goruntu.ChainCodeHistogram(Goruntu.chaincode, q);
         */  
        //   bmp1 = Goruntu.StringToImage(bmp1, Goruntu.xs,Goruntu.ys, Goruntu.chaincode); pictureBox2.Image = bmp1;

          //  bmp1 = Goruntu.ComplexToImage(bmp1, Goruntu.xs, Goruntu.ys,Goruntu.ComplexChain); pictureBox2.Image = bmp1;
            /*
            chart1.Series["Series1"].Points.Clear();           
            for (int i = 0; i < 8; i++)
            {  chart1.Series["Series1"].Points.AddXY(i, q[i]); }
            */

          /*
            chart1.Series["Series2"].Points.Clear();
            foreach (Complex value in Goruntu.ComplexChain)
            {  chart1.Series["Series2"].Points.AddY(value.magnitude);  }
           */
           
            double aci;
            aci = Goruntu.OryantasyonBul2(bmp1);
            textBox3.Text = Convert.ToString(57.29*aci);
            bmp1 = Goruntu.Oryantasyon(bmp1); pictureBox2.Image = bmp1;       
        
           
        }




        private void sekilHistogramToolStripMenuItem_Click(object sender, EventArgs e)
        {
            double[] q = new double[8]; int i;
            Bitmap bmp1 = new Bitmap(pictureBox1.Image);
            Bitmap bmp2 = new Bitmap(pictureBox1.Image);
           // bmp1 = Goruntu.GetCom(bmp2, -1, -1, -1);
            bmp2 = Goruntu.ShellHistogram2(bmp1, q); pictureBox2.Image = bmp2;
           // bmp2 = Goruntu.SectorHistogram(bmp1, q); pictureBox2.Image = bmp2;

            FileStream fs = new FileStream("c:\\Medpic\\dataset.txt", FileMode.Append, FileAccess.Write);
            StreamWriter dosya = new StreamWriter(fs);
            String sline = "";
            String satir = Convert.ToString(q[0]);
            if (satir.Length < 8) satir = satir + "00000000";
            satir = satir.Substring(0, 8);
            sline = satir;
            for (i = 1; i < q.Length; i++)
            {
                satir = Convert.ToString(q[i]);
                if (satir.Length < 8) satir = satir + "00000000";
                satir = satir.Substring(0, 8); textBox1.Text = satir;
                sline = sline + ":" + satir;
            }
            satir = textBox3.Text;
            sline = sline + ":" + satir; textBox2.Text = sline;
            dosya.WriteLine("{0}", sline);
            dosya.Close();

            chart1.Series["Series1"].Points.Clear();
            for (i = 0; i < q.Length; i++)
            { chart1.Series["Series1"].Points.AddXY(i, q[i]); }


        }

        private void harfTaniToolStripMenuItem_Click(object sender, EventArgs e)
        {
            double[] q = new double[16];
            double[] gelen = new double[16];
            double[] sonuc = new double[16];
            double sim; double enbenzer = 0;
            String bulunan = "";
            Bitmap bmp1 = new Bitmap(pictureBox1.Image);
            Bitmap bmp2 = new Bitmap(pictureBox1.Image);
            bmp1 = Goruntu.GetCom(bmp2, -1, -1, -1);
            Goruntu.ShellHistogram3(bmp1, q);
            chart1.Series["Series1"].Points.Clear();
            for (int i = 0; i < q.Length; i++)
            { chart1.Series["Series1"].Points.AddXY(i + 1, q[i]); }


            using (StreamReader dosya = new StreamReader("c:\\Medpic\\alfabe16.txt"))
            {
                string line;
                while ((line = dosya.ReadLine()) != null)
                {
                    string[] bil = line.Split(':');

                    for (int i = 0; i < bil.Length - 1; i++)
                    {
                        gelen[i] = Convert.ToDouble(bil[i]);

                    }
                    sim = Goruntu.HistogramKarsilastir(q, gelen);
                    if (sim > enbenzer)
                    {
                        enbenzer = sim; textBox1.Text = Convert.ToString(enbenzer);
                        for (int i = 0; i < bil.Length - 1; i++)
                        {
                            sonuc[i] = gelen[i];
                            bulunan = bil[16]; textBox2.Text = bulunan;
                        }
                    }
                }  //dosya
            }//dosya
            chart1.Series["Series1"].Points.Clear();
            chart1.Series["Series2"].Points.Clear();
            for (int i = 0; i < q.Length; i++)
            {
                chart1.Series["Series1"].Points.AddXY(i + 1, q[i]);
                chart1.Series["Series2"].Points.AddXY(i + 1, sonuc[i]);
            }
        }



        private void cameraToolStripMenuItem_Click(object sender, EventArgs e)
        {
            cam = new VideoCaptureDevice(webcams[comboBox1.SelectedIndex].MonikerString);
            cam.NewFrame += new NewFrameEventHandler(cam_NewFrame);
            // cam.DesiredFrameRate = 15;
            // cam.DesiredFrameSize = new Size(320, 240);  // Görüntü büyüklügü
            //cam.DesiredFrameSize = new Size(pictureBox1.Width, pictureBox1.Height);
            cam.Start();//kamerayı başlatıyoruz.
            timer1.Enabled = true;// kamera açılırken timer disable olsun.     
        }


        void cam_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
           // Bitmap bmp1 = (Bitmap)eventArgs.Frame.Clone();//kısaca bu event'ta kameradan alınan görüntüyü picturebox'a atıyoruz.
           // pictureBox1.Image = bmp1;
            Goruntu.resim1 = (Bitmap)eventArgs.Frame.Clone();

        }

        private void KameraBaslat()
        {
            textBox1.Text = "Kamera Başlatılıyor...";
            if (cam != null)
            {
                if (!cam.IsRunning)
                { cam.Start(); }
            }
        }

        private void KameraDurdur()
        {
            textBox1.Text = "Kamera Durduruluyor...";

            if (cam != null)
            {
                if (cam.IsRunning)
                {
                    cam.SignalToStop();
                    cam = null;
                }
            }
        }

        Bitmap bmpSaved;
        private void timer1_Tick(object sender, EventArgs e)
        {
            pictureBox1.Image = Goruntu.resim1;
            pictureBox2.Image = Goruntu.resim1;

            Bitmap bmp1 = new Bitmap(pictureBox1.Image);
            Bitmap bmp2 = new Bitmap(pictureBox1.Image);

            if (bmpSaved != null && bmp1 != null && bmpSaved.Width >= pictureBox1.Width && bmpSaved.Height >= pictureBox1.Height)
            {
                for (int w = 0; w != bmpSaved.Width; w++)
                {
                    for (int h = 0; h != bmpSaved.Height; h++)
                    {
                        Color c1 = bmp1.GetPixel(w, h);
                        Color c2 = bmpSaved.GetPixel(w, h);
                        double benzerlik = Goruntu.MemberShip3(c1, c2, 128, 2);
                        Color c3 = benzerlik < 0.75 ? Color.FromArgb(255, 0, 0) : c1;
                        bmp2.SetPixel(w, h, c3);
                    }
                }

                pictureBox2.Image = bmp2;
            }
            bmpSaved = bmp1;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            double[] x = new double[5]; double[] y= new double[10];

            x[0] = 0; x[1] = 10; x[2] = 20; x[3] =30; x[4] = 40;
            chart1.Series["Series1"].Points.Clear();
            DiziKatla(x, y);
            for (int i = 0; i <y.Length; i++)
            {  chart1.Series["Series1"].Points.AddXY(i,y[i]);  }

            textBox3.Text = Convert.ToString(x.Length);

        }


       
        public void DiziKatla(double []x, double [] y)
        {   for (int i = 0; i < (x.Length-1); i++)
              {   textBox2.Text = Convert.ToString(i);                
                  if (i < (x.Length - 1)) 
                 {  y[2 * i] = x[i];  y[2 * i + 1] = (x[i] + x[i + 1]) / 2;  }
                
                 if (i == (x.Length - 2))
                 {   y[2 * (i + 1)] = x[i + 1];     y[2 * (i +1)+1] = x[i + 1]; textBox1.Text = Convert.ToString(i); } 
              }
         }

        private void pictureBox2_MouseClick(object sender, MouseEventArgs e)
        {
             OpenFileDialog open = new OpenFileDialog();
            open.Filter = "Image Files(*.jpg; *.jpeg; *.gif; *.bmp; *.png)|*.jpg; *.jpeg; *.gif; *.bmp; *.png";
            if (open.ShowDialog() == DialogResult.OK)
            {
                Goruntu.DosyaAdi = open.FileName;
                Goruntu.resim2 = new Bitmap(open.FileName);
                pictureBox2.Image = Goruntu.resim2;
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            int boyut = 2;
            double[] gelen = new double[boyut];
            PSOgenel Suru = new PSOgenel(boyut, 150, 50, 2);
            for (int j = 0; j < boyut; j++)
            { gelen[j] = Suru.Cozum[j]; }
            textBox1.Text = Convert.ToString(gelen[0]);
            textBox2.Text = Convert.ToString(gelen[1]);

        
        }

        private void button8_Click(object sender, EventArgs e)
        {
            int i, N, GirisSayi; GirisSayi =8; N =6;  
            double[] Giris = new double[GirisSayi];
            double[] d = new double[N];
            double[] y = new double[N];

            Random r = new Random();
            for (i = 0; i < GirisSayi; i++)
              { Giris[i] = r.NextDouble(); }
            for (i = 0; i < N; i++)
            { d[i] = r.NextDouble(); y[i] = 0.0; }

            ANNs Siniflandir = new ANNs(GirisSayi, 5, 5, N, 0.01);
            //Siniflandir.Train(Giris, d, 100);
            Siniflandir.TrainFile(100);
           // Siniflandir.TrainFileAlfabe(100);
           // Siniflandir.SaveWeight();
         
            chart1.Series["Series1"].Points.Clear();
            chart1.Series["Series2"].Points.Clear();
            chart1.Series["Series3"].Points.Clear();
            for (i = 0; i < N; i++)
            {
                chart1.Series["Series1"].Points.AddY(d[i]);
                chart1.Series["Series2"].Points.AddY(d[i]);
                chart1.Series["Series3"].Points.AddY(Siniflandir.y[i]);
            }
        }

        private void lBPToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Bitmap bmp1 = new Bitmap(pictureBox1.Image);
            Bitmap bmp2 = new Bitmap(bmp1);
       
            int[] w = new int[8];

            
            w[0] = 1;  w[1] = 2;   w[2] = 4;
            w[3] = 8;              w[4] = 16;
            w[5] = 32; w[6] = 64;  w[7] = 128;          
            bmp2 = Goruntu.LBP(bmp1); pictureBox2.Image = bmp2;
            
            
           /* w[0] = 1;    w[1] = 2;  w[2] =4;
            w[3] = 128;             w[4] =8;
            w[5] = 64;   w[6] =32;  w[7] =16;
            
            bmp2 = Goruntu.LBP3(bmp1, w); pictureBox2.Image = bmp2;  */        

        }

        private void videoToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void shellToolStripMenuItem_Click(object sender, EventArgs e)
        {
            double[] q = new double[4]; int i;
            Bitmap bmp1 = new Bitmap(pictureBox1.Image);
            Bitmap bmp2 = new Bitmap(pictureBox1.Image);
           
            bmp2 = Goruntu.ShellHistogram4(bmp1, q); pictureBox2.Image = bmp2;

            FileStream fs = new FileStream("c:\\Medpic\\Kabuk.txt", FileMode.Append, FileAccess.Write);
            StreamWriter dosya = new StreamWriter(fs);
            String sline = "";
            String satir = Convert.ToString(q[0]);
            if (satir.Length < 8) satir = satir + "00000000";
            satir = satir.Substring(0, 8);
            sline = satir;
            for (i = 1; i < q.Length; i++)
            {
                satir = Convert.ToString(q[i]);
                if (satir.Length < 8) satir = satir + "00000000";
                satir = satir.Substring(0, 8); textBox1.Text = satir;
                sline = sline + ":" + satir;
            }
            satir = textBox3.Text;
            sline = sline + ":" + satir; textBox2.Text = sline;
            dosya.WriteLine("{0}", sline);
            dosya.Close();

            chart1.Series["Series1"].Points.Clear();
            for (i = 0; i < q.Length; i++)
            { chart1.Series["Series1"].Points.AddXY(i+1, q[i]); }

        }

        private void dFTToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Bitmap bmp1 = new Bitmap(pictureBox1.Image);
            Bitmap bmp2 = new Bitmap(pictureBox1.Image);
            int i, N, M; Color p9; N = bmp1.Width;
            double[] hr, hg, hb;
            Complex[] input; Complex[] output; Complex[] donus;
            hr = new double[N]; hg = new double[N]; hb = new double[N];
            input = new Complex[N]; output = new Complex[N]; donus = new Complex[N];

            for (i = 0; i < N; i++)
            {
                p9 = bmp1.GetPixel(i, 100);
                Complex temp = new Complex((double)p9.R, 0);
                input[i] = temp; hr[i] = temp.real; hg[i] = temp.imag;
                p9 = Color.FromArgb(0, 255, 0);
                bmp2.SetPixel(i, 100, p9);
            }
            pictureBox1.Image = bmp2;

            output = Goruntu.DFT(input, -1);
            donus = Goruntu.DFT(output, 1);
            hb = Goruntu.ArrayFromComplex(donus, 1, 1, 1);

            // input = Goruntu.ArrayShift(output); output = Goruntu.FFT(input, -1); hb = Goruntu.ArrayFromComplex(output, 1, 1, 1);

            chart1.Series["Series1"].Points.Clear();
            chart1.Series["Series2"].Points.Clear();
            chart1.Series["Series3"].Points.Clear();
            for (i = 0; i < N; i++)
            {
                //  chart1.Series["Series1"].Points.AddY(input[i].real);
                chart1.Series["Series2"].Points.AddY(output[i].magnitude);
                chart1.Series["Series1"].Points.AddY(donus[i].magnitude);
                //  chart1.Series["Series2"].Points.AddY(255*hb[i]);                  
            }


            N = bmp1.Width; M = bmp1.Height;
            Complex[,] Katsayi = new Complex[N, M];
            Katsayi = Goruntu.getDFT2D(bmp1);
            bmp2 = Goruntu.BitmapFromComplex(Katsayi, 1, 2, 100); pictureBox1.Image = bmp2;

            /*
            Complex[,] geri = new Complex[N, M];
            geri = Goruntu.DFT2D(Katsayi, -1);
            bmp2 = Goruntu.BitmapFromComplex(Katsayi, 1, 2, 100); pictureBox2.Image = bmp2;
             * */
        }

        private void hilbertToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Bitmap bmp1 = new Bitmap(pictureBox1.Image);
            Bitmap bmp2 = new Bitmap(pictureBox1.Image);
            int i, N, M; Color p9; N = bmp1.Width;
            double[] hr, hg, hb;
            Complex[] input; Complex[] output; Complex[] donus;
            hr = new double[N]; hg = new double[N]; hb = new double[N];
            input = new Complex[N]; output = new Complex[N]; donus = new Complex[N];

            for (i = 0; i < N; i++)
            {
                p9 = bmp1.GetPixel(i, 100);
                Complex temp = new Complex((double)p9.R, 0);
                input[i] = temp; hr[i] = temp.real; hg[i] = temp.imag;
                p9 = Color.FromArgb(0, 255, 0);
                bmp2.SetPixel(i, 100, p9);
            }
            pictureBox1.Image = bmp2;

            output = Goruntu.Hilbert(input, -1);
            donus = Goruntu.DFT(output, 1);
            // hb = Goruntu.ArrayFromComplex(donus, 1, 1, 1);

            // input = Goruntu.ArrayShift(output); output = Goruntu.FFT(input, -1); hb = Goruntu.ArrayFromComplex(output, 1, 1, 1);

            chart1.Series["Series1"].Points.Clear();
            chart1.Series["Series2"].Points.Clear();
            chart1.Series["Series3"].Points.Clear();
            for (i = 0; i < N; i++)
            {
                // chart1.Series["Series1"].Points.AddY(input[i].real);
                // chart1.Series["Series2"].Points.AddY(output[i].magnitude);
                chart1.Series["Series1"].Points.AddY(donus[i].magnitude);
                // chart1.Series["Series2"].Points.AddY(hb[i]);                  
            }


            N = bmp1.Width; M = bmp1.Height;
            Complex[,] Katsayi = new Complex[N, M];
            Katsayi = Goruntu.getHilbert2D(bmp1);
            bmp2 = Goruntu.BitmapFromComplex(Katsayi, 1, 2, 10); pictureBox1.Image = bmp2;


            Complex[,] geri = new Complex[N, M];
            geri = Goruntu.DFT2D(Katsayi, -1);
            bmp2 = Goruntu.BitmapFromComplex(Katsayi, 1, 2, 100); pictureBox2.Image = bmp2;

        }

        private void button9_Click(object sender, EventArgs e)
        {
            Bitmap bmp1 = new Bitmap(pictureBox1.Image);
            Bitmap bmp2 = new Bitmap(bmp1);
            bmp2 = Goruntu.gazi3(bmp1,100,150,200); pictureBox2.Image = bmp2;
        }

        private void pictureToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }



        /*
         private void aBCToolStripMenuItem_Click(object sender, EventArgs e)
         {

         }
         */









    }
}
