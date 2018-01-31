﻿using Microsoft.ProjectOxford.Face.Contract;
using MoreLinq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BCWSnapin
{  
    public partial class BCWSnapin : Form
    {
        FileSystemWatcher fsw = null;
        string watchfolder = String.Empty;
        string apikey = String.Empty;
        public BCWSnapin()
        {
            InitializeComponent();
            if (String.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["watchfolder"]))
                throw new ArgumentNullException("Watchfolder missing.");
            if (String.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["apikey"]))
                throw new ArgumentNullException("Watchfolder missing.");

            watchfolder = System.Configuration.ConfigurationManager.AppSettings["watchfolder"];
            apikey = System.Configuration.ConfigurationManager.AppSettings["apikey"];
        }

        private void BCWSnapin_Load(object sender, EventArgs e)
        {
            fsw = new FileSystemWatcher(watchfolder);
            fsw.EnableRaisingEvents = true;
            fsw.Created += Fsw_Created;
            var maxThreads = 4;
            ThreadPool.SetMaxThreads(maxThreads, maxThreads * 2);
        }

        private async void Fsw_Created(object sender, FileSystemEventArgs e)
        {

           
              var  fii = await new facehelper(apikey).ProcessFile(e.FullPath);
            UpdateLabel(lbl_age,fii.GetFace().FaceAttributes.Age.ToString());
            UpdateLabel(lbl_gender,fii.GetFace().FaceAttributes.Gender);
            UpdateLabel(lbl_smile,String.Format("{0}%", fii.GetFace().FaceAttributes.Smile * 100));
            UpdateLabel(lbl_moustache,String.Format("{0}%", fii.GetFace().FaceAttributes.FacialHair.Moustache * 100));
            UpdateLabel(lbl_haircolor,(fii.GetFace().FaceAttributes.Hair.HairColor.MaxBy(x => x.Confidence)).Color.ToString());
            UpdateLabel(lbl_glasses,fii.GetFace().FaceAttributes.Glasses.ToString());
            UpdateLabel(lbl_faceid,fii.GetFace().FaceId.ToString());
            
            FaceBox.Invoke((MethodInvoker)delegate
            {
                FaceBox.InitialImage = null;
                FaceBox.Image = fii.GetImage();
                
                /*
                FaceBox.Paint += new PaintEventHandler(delegate (Object o, PaintEventArgs ea)
                {
                    
                    ((PaintEventArgs)ea).Graphics.DrawRectangle(new Pen(Color.Red), fii.GetFaceRect());
                });
                */
            });
            
        }
        private void UpdateLabel(Label lbl, string val)
        {
            lbl.Invoke((MethodInvoker)delegate
            {
                lbl.Text = val;
            });
        }
        
    }
}
