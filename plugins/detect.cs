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

        private VideoCapture capture; // EmguCV Class for VideoCapture
        private PictureBox displayBox;
        private BarcodeDetector detector; // EmguCV Class for QRDetection
        private Thread captureThread; // Thread for running video capture loop
        private bool isRunning = true; 
    public QRDetection(){
    capture = new VideoCapture;
    displayBox = new PictureBox{

            SizeMode = PictureBoxSizeMode.StretchImage,
            Dock = DockStyle.Fill
    }

    }
    private void CaptureLoop()
        {
            while (isRunning) // Continue while the plugin is running
            {
                // Capture a frame from the camera
                Mat frame = capture.QueryFrame();

                if (frame != null) // Ensure a frame was captured
                {
                    // Detect and decode QR codes in the frame
                    VectorOfCvString codes = new VectorOfCvString();
                    VectorOfInt types = new VectorOfInt();
                    Mat points = new Mat();
                    detector.DetectAndDecode(frame, codes, types, points);

                    // Optionally, draw rectangles around detected QR codes
                    for (int i = 0; i < points.Rows; i++)
                    {
                        PointF[] point = points.GetArray(i);
                        // You can use EmguCV drawing functions here, e.g., CvInvoke.Rectangle
                    }

                    // Update the PictureBox with the latest frame
                    // Use BeginInvoke to ensure UI updates are on the main thread
                    MainV2.instance.BeginInvoke((MethodInvoker)delegate
                    {
                        displayBox.Image = frame.Bitmap;
                    });
                }

            }
        }
        public override bool Exit()
        {
            isRunning = false;
            captureThread.Join();
            capture.Dispose();
            return true;
        }
    }
}
    