using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Basic_Photo_Editor.Paint_Tools
{
    public partial class Picker : UserControl
    {
        public Color Color { get; set; }
        public Picker()
        {
            InitializeComponent();
            Dock = DockStyle.Fill;
        }

        public void GetColor(Bitmap bmp, PointF p)
        {
            Color = bmp.GetPixel((int)p.X, (int)p.Y);
        }
    }
}
