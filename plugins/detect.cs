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
        private Thread captureThread;
    public QRDetection(){
    capture = new VideoCapture;
    displayBox = new PictureBox{

            SizeMode = PictureBoxSizeMode.StretchImage,
            Dock = DockStyle.Fill
    }

    }
    }
}
    