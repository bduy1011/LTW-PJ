using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Basic_Photo_Editor.Function_Forms
{
    public partial class Threshold : Form
    {
        private LayerContainer lc;
        private FormMain f;
        private Bitmap origin;
        private Bitmap adjusted;
        private System.Drawing.Imaging.BitmapData bmpData;
        private byte[] imagePixels;
        private int dataSize;

        public Threshold(FormMain f, LayerContainer lc)
        {
            InitializeComponent();
            this.f = f;
            this.lc = lc;
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

        public void Initialize()
        {
            bmpData = origin.LockBits(new Rectangle(0, 0, origin.Width, origin.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, origin.PixelFormat);
            IntPtr ptr = bmpData.Scan0;
            dataSize = Math.Abs(bmpData.Stride) * origin.Height;
            imagePixels = new byte[dataSize];
            System.Runtime.InteropServices.Marshal.Copy(ptr, imagePixels, 0, dataSize);
            origin.UnlockBits(bmpData);
            Adjust();
        }

        int level = 128;
        private void Adjust()
        {
            if (adjusted != null)
            {
                adjusted.Dispose();
                adjusted = null;
            }
            adjusted = new Bitmap(origin);
            bmpData = adjusted.LockBits(new Rectangle(0, 0, adjusted.Width, adjusted.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite, adjusted.PixelFormat);
            IntPtr ptr = bmpData.Scan0;
            byte[] pixels = new byte[dataSize];

            for (int i = 0; i < dataSize; i += 4)
            {
                int n = (int)((float)imagePixels[i] * 0.4f + (float)imagePixels[i + 1] * 0.4f + (float)imagePixels[i + 2] * 0.2f);

                if (n < level)
                {
                    pixels[i] = 0;
                    pixels[i + 1] = 0;
                    pixels[i + 2] = 0;
                }
                else
                {
                    pixels[i] = 255;
                    pixels[i + 1] = 255;
                    pixels[i + 2] = 255;
                }
                pixels[i + 3] = imagePixels[i + 3];

            }

            System.Runtime.InteropServices.Marshal.Copy(pixels, 0, ptr, dataSize);
            adjusted.UnlockBits(bmpData);
            #region bug ProcessUpdate in LayerContainer
            //lc.ProcessUpdate(adjusted, true);
            //f.DSUpdate();
            #endregion
        }

        private void LevelTrack_Scroll(object sender, EventArgs e)
        {
            #region bug levelTrack
            //label3.Text = levelTrack.Value.ToString();
            //level = levelTrack.Value;
            #endregion
            Adjust();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            #region bug ProcessUpdate in LayerContainer
            //lc.ProcessUpdate(origin, true);
            //f.DSUpdate();
            #endregion
        }
    }
}
