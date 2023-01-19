using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.Contracts;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Basic_Photo_Editor.Function_Forms
{
    public partial class Brightness_Contrast : Form
    {
        private FormMain fMain;
        private LayerContainer layerContainer;
        private Bitmap origin, adjusted;
        private float brightness=0f, contrast=0f;
        public Brightness_Contrast(FormMain fMain,LayerContainer layerContainer)
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
                ColorMatrix matrix = new ColorMatrix();
                matrix.Matrix00 = matrix.Matrix11 = matrix.Matrix22 = contrast + 1f;
                matrix.Matrix33 = matrix.Matrix44 = 1f;
                matrix.Matrix40 = matrix.Matrix41 = matrix.Matrix42 = brightness;

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
            label3.Text = brightnessTrack.Value.ToString();
            brightness = (float)brightnessTrack.Value / 100;
            Adjust();
        }

        private void TrackBar2_Scroll(object sender, EventArgs e)
        {
            label4.Text = contrastTrack.Value.ToString();
            contrast = (float)contrastTrack.Value / 100;
            Adjust();
        }

        private void Btn_Cancel_Ok_Click(object sender, EventArgs e)
        {
            layerContainer.ProcessUpdate(origin, true);
            fMain.DrawSpaceUpdate();
        }

        private void Btn_Reset_Click(object sender, EventArgs e)
        {
            brightness = brightnessTrack.Value = 0;
            label3.Text = brightnessTrack.Value.ToString();
            contrast = contrastTrack.Value = 0;
            label4.Text = contrastTrack.Value.ToString();
            Adjust();
        }
    }
}
