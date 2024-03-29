﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Basic_Photo_Editor.Function_Forms
{
    public partial class ColorBalance : Form
    {
        private LayerContainer layerContainer;
        private FormMain fMain;
        private Bitmap origin;
        private Bitmap adjusted;

        public ColorBalance(FormMain fMain, LayerContainer layerContainer)
        {
            InitializeComponent();
            this.fMain = fMain;
            this.layerContainer = layerContainer;
        }

        public Bitmap Image
        {
            set
            {
                origin = new Bitmap(value);
                adjusted = new Bitmap(origin);
            }
            get
            {
                return adjusted;
            }
        }

        float cyanVal, magentaVal, yellowVal;
        private void Adjust()
        {
            if (adjusted != null)
            {
                adjusted.Dispose();
                adjusted = null;
            }

            adjusted = new Bitmap(origin);
            using (ImageAttributes imageAttributes = new ImageAttributes())
            {
                float[][] m =
                {
                    new float[]{ 1 - (-cyanVal + magentaVal / 2 + yellowVal / 2), 0, 0, 0, 0 },
                    new float[]{ 0, 1 - (cyanVal / 2 - magentaVal + yellowVal / 2), 0, 0, 0 },
                    new float[]{ 0, 0, 1 - (cyanVal / 2 + magentaVal / 2 - yellowVal), 0, 0 },
                    new float[]{ 0, 0, 0, 1, 0 },
                    new float[]{ 0, 0, 0, 0, 1 }
                };
                ColorMatrix matrix = new ColorMatrix(m);

                imageAttributes.SetColorMatrix(matrix);
                using (Graphics g = Graphics.FromImage(adjusted))
                {
                    g.DrawImage(adjusted, new Rectangle(0, 0, adjusted.Width, adjusted.Height), 0, 0, origin.Width, origin.Height, GraphicsUnit.Pixel, imageAttributes);
                }
            }
            layerContainer.ProcessUpdate(adjusted, true);
            fMain.DrawSpaceUpdate();
        }

        private void TrackBar1_Scroll(object sender, EventArgs e)
        {
            label3.Text = cyanTrack.Value.ToString();
            cyanVal = (float)cyanTrack.Value / 100;
            Adjust();
        }

        private void TrackBar2_Scroll(object sender, EventArgs e)
        {
            label4.Text = magentaTrack.Value.ToString();
            magentaVal = (float)magentaTrack.Value / 100;
            Adjust();
        }

        private void TrackBar3_Scroll(object sender, EventArgs e)
        {
            label6.Text = yellowTrack.Value.ToString();
            yellowVal = (float)yellowTrack.Value / 100;
            Adjust();
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            cyanVal = cyanTrack.Value = 0;
            label3.Text = cyanTrack.Value.ToString();
            magentaVal = magentaTrack.Value = 0;
            label4.Text = magentaTrack.Value.ToString();
            yellowVal = yellowTrack.Value = 0;
            label6.Text = yellowTrack.Value.ToString();
            Adjust();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            layerContainer.ProcessUpdate(origin, true);
            fMain.DrawSpaceUpdate();
        }
    }
}