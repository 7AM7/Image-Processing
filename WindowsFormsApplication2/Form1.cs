using AForge;
using AForge.Imaging;
using AForge.Imaging.Filters;
using AForge.Math.Geometry;
using AForge.Video;
using AForge.Video.DirectShow;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private FilterInfoCollection CuptreDevice;
        private VideoCaptureDevice FinalFrame=null;
        Color color;
        float hue;
        private void Form1_Load(object sender, EventArgs e)
        {
            CuptreDevice = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            foreach (FilterInfo Device in CuptreDevice)
            {
                
                comboBox1.Items.Add(Device.Name);
            }
            comboBox1.SelectedIndex = 0;
            FinalFrame = new VideoCaptureDevice();

        }
        

        private void button1_Click(object sender, EventArgs e)
        {
            FinalFrame = new VideoCaptureDevice(CuptreDevice[comboBox1.SelectedIndex].MonikerString);
            FinalFrame.NewFrame += FinalFrame_NewFrame;
            FinalFrame.Start();
        }



        private void FinalFrame_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            try
            {

                Bitmap video = (Bitmap)eventArgs.Frame.Clone();
                Bitmap tmp = video.Clone() as Bitmap;
                //Create color filter

                EuclideanColorFiltering filter = new EuclideanColorFiltering();
                filter.CenterColor = new AForge.Imaging.RGB(color.R, color.G, color.B);
                filter.Radius = 90;
                filter.ApplyInPlace(tmp);

                BlobCounter blobcounter = new BlobCounter();
                blobcounter.MinHeight = Convert.ToInt32(numericUpDown1.Value);
                blobcounter.MinWidth = Convert.ToInt32(numericUpDown2.Value);
                blobcounter.FilterBlobs = true;
                blobcounter.ObjectsOrder = ObjectsOrder.Size;
                //locate blobs
                blobcounter.ProcessImage(tmp);
                Rectangle[] rects = blobcounter.GetObjectsRectangles();
                Blob[] bBlobs = blobcounter.GetObjectsInformation();
                
                SimpleShapeChecker shapeChecker = new SimpleShapeChecker();
                //draw rectangle around the biggest blob
                Pen yellowPen = new Pen(Color.Red, 5);

                
                foreach (Rectangle recs in rects)
                    if (rects.Length > 0)
                    {
                        Rectangle objectRect1 = recs;
                        Graphics g = Graphics.FromImage(video);

                        using (Pen pen = new Pen(Color.FromArgb(160, 255, 160), 5))
                        {
                            g.DrawRectangle(pen, objectRect1);
                            PointF drawPoin = new PointF(objectRect1.X, objectRect1.Y);

                            int objectX = objectRect1.X + objectRect1.Width / 2 - video.Width / 2;

                            int objectY = video.Height / 2 - (objectRect1.Y + objectRect1.Height / 2);

                            String Blobinformation = "X= " + objectX.ToString() + "\nY= " + objectY.ToString() + "\nSize=" + objectRect1.Size.ToString();

                            g.DrawString(Blobinformation, new Font("Arial", 16), new SolidBrush(Color.Blue), drawPoin);
                            if (objectY == 1)
                            {
                                // MessageBox.Show("BOOOOM!!!");
                                Console.WriteLine("BOOOOM!!!");
                                Console.WriteLine("X: {0} Y: {1}",objectX,objectY);
                                video.SetResolution(500, 200); 
                             //   g.Dispose();
                                break;
                            }
                        }
                       
                        g.Dispose();
                    }
					
					//circle
                //for (int i = 0, n = bBlobs.Length; i < n; i++)
                //{
                //    List<IntPoint> edgePoints = blobcounter.GetBlobsEdgePoints(bBlobs[i]);
                //    Graphics g = Graphics.FromImage(video);
                //    AForge.Point center;
                //    float radius;

                //    // is circle ?
                //    if (shapeChecker.IsCircle(edgePoints, out center, out radius))
                //    {
                //        g.DrawEllipse(yellowPen,
                //            (float)(center.X - radius), (float)(center.Y - radius),
                //            (float)(radius * 2), (float)(radius * 2));
                //    }
                //}
               //
                //yellowPen.Dispose();
                pictureBox1.Image = video;
            }
            catch(Exception)
            {
                numericUpDown1.Value = 5;
                numericUpDown1.Value = 5;
                MessageBox.Show("Erorr!!");
            }
         //   pictureBox2.Image = grayImage;


        }

   //     public Rectangle objectRect1 { get; set; }
        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {

            if (FinalFrame.IsRunning == true)
            {
                FinalFrame.Stop();
            }
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            colorDialog1.ShowDialog();
            color = (Color)colorDialog1.Color;
            hue = color.GetHue();

        }

        private void button3_Click(object sender, EventArgs e)
        {

            if (FinalFrame.IsRunning == true)
            {
                FinalFrame.Stop();

            }
            pictureBox1.Image = null;
        }



        private void Form1_Leave(object sender, EventArgs e)
        {

            if (FinalFrame.IsRunning == true)
            {
                FinalFrame.Stop();

            }
            pictureBox1.Image = null;
        }




    }
}
