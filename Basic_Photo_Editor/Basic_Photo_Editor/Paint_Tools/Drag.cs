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
    public partial class Drag : UserControl
    {
        Point p;
        public Drag()
        {
            InitializeComponent();
        }

        public void GetLocation(ref MouseEventArgs e)
        {
            p = e.Location;
        }

        public void Dragging(object sender, MouseEventArgs e)
        {
            Control c = sender as Control;

            c.Left += e.X - p.X;
            c.Top += e.Y - p.Y;

            c.Refresh();
        }
    }
}
