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
    public partial class Pixelate : Form
    {
        private FormMain f;
        private LayerContainer lc;
        private Bitmap origin;
        private Bitmap adjusted;

        public Pixelate(FormMain f, LayerContainer lc)
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

        int pixel = 1;
        private void Adjust()
        {
            if (adjusted != null)
            {
                adjusted.Dispose();
                adjusted = null;
            }
            adjusted = new Bitmap(origin);

            if (pixel != 1)
            {
                int k = pixel / 2;
                using (Graphics g = Graphics.FromImage(adjusted))
                {
                    g.Clear(Color.Transparent);
                    for (int y = 0; y < adjusted.Height + pixel; y += pixel)
                    {
                        if (y >= adjusted.Height) y = adjusted.Height - 1;
                        for (int x = 0; x < adjusted.Width + pixel; x += pixel)
                        {
                            if (x >= adjusted.Width) x = adjusted.Width - 1;
                            g.FillRectangle(new SolidBrush(origin.GetPixel(x, y)), x - k, y - k, pixel, pixel);
                            if (x == adjusted.Width - 1) break;
                        }
                        if (y == adjusted.Height - 1) break;
                    }
                }
            }
            #region bug ProcessUpdate in LayerContainer
            //lc.ProcessUpdate(adjusted, true);
            #endregion
            f.DSUpdate();
        }

        private void PixelTrack_Scroll(object sender, EventArgs e)
        {
            #region bug pixelTrack
            //pixel = pixelTrack.Value;
            //label3.Text = pixelTrack.Value.ToString();
            #endregion
            Adjust();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            #region bug ProcessUpdate in LayerContainer
            //lc.ProcessUpdate(origin, true);
            #endregion
            f.DSUpdate();
        }
    }
}
