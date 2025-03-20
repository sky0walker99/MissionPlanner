using System;
using System.Collections.Generic;
using System.Linq;
using MissionPlanner;
using MissionPlanner.GCSViews;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;
using Emgu.CV.Util;
using Emgu.CV.Barcode;

namespace QRCodeDetection{
    public class QRDetection :  MissionPlanner.Plugin.Plugin
    {
        public override string Name => "QR Code Detector";
        public override string Version => "1.0";
        public override string Author => "Your Name";
        private VideoCapture capture; 
        private PictureBox displayBox;
        private BarcodeDetector detector; 
        private Thread captureThread; 
        private bool isRunning = true; 

   public override bool Init()
        {
            try
            {
                capture = new VideoCapture(0);
                if (!capture.IsOpened)
                {
                    CustomMessageBox.Show("Failed to open camera.", "Error");
                    return false;
                }
                detector = new BarcodeDetector(new[] { BarCodeType.QR_CODE });
                displayBox = new PictureBox
                {
                    SizeMode = PictureBoxSizeMode.StretchImage,
                    Dock = DockStyle.Fill
                };

                MainV2.instance.BeginInvoke((MethodInvoker)delegate
                {
                    var tab = MainV2.instance.Controls.Find("FlightData", true).FirstOrDefault() as TabPage;
                    if (tab != null)
                        tab.Controls.Add(displayBox);
                    else
                        Console.WriteLine("FlightData tab not found.");
                });

                isRunning = true;
                captureThread = new Thread(CaptureLoop) { IsBackground = true };
                captureThread.Start();
                return true;
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show($"Initialization failed: {ex.Message}", "Error");
                return false;
            }
        }
    private void CaptureLoop()
        {
            while (isRunning)
            {
                try
                {
                    Mat frame = capture.QueryFrame();
                    if (frame == null)
                    {
                        Thread.Sleep(100); 
                        continue;
                    }
                    VectorOfCvString codes = new VectorOfCvString();
                    VectorOfInt types = new VectorOfInt();
                    Mat points = new Mat();
                    detector.DetectAndDecode(frame, codes, types, points);

                    for (int i = 0; i < points.Rows; i++)
                    {
                        PointF[] pointArray = points.GetArray<PointF>(i);
                        if (pointArray.Length >= 4)
                        {
                            for (int j = 0; j < pointArray.Length; j++)
                            {
                                CvInvoke.Line(frame, 
                                    Point.Round(pointArray[j]), 
                                    Point.Round(pointArray[(j + 1) % 4]), 
                                    new MCvScalar(0, 255, 0), 2);
                            }
                            
                            Console.WriteLine($"QR Code Detected: {codes[i].ToString()}");
                        }
                    }

                    // Update the PictureBox on the UI thread
                    MainV2.instance.BeginInvoke((MethodInvoker)delegate
                    {
                        displayBox.Image?.Dispose(); // Dispose of the previous image
                        displayBox.Image = frame.ToBitmap();
                    });

                    Thread.Sleep(33);                 }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in capture loop: {ex.Message}");
                }
            }
        }
        public override bool Exit()
        {
            isRunning = false;
            captureThread.Join(1000);
            capture?.Dispose();
            detector?.Dispose();
            displayBox?.Dispose();
            return true;
        }
    }
}
    