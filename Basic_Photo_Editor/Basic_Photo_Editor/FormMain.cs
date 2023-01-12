using Basic_Photo_Editor.Paint_Tools;
using Basic_Photo_Editor;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace Basic_Photo_Editor
{
    public partial class FormMain : Form
    {
        private WorkSpace Current;

        public FormMain()
        {
            InitializeComponent();
        }
     
        private void MinimizeBtn_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void RestoreBtn_Click(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Maximized)
            {
                this.WindowState = FormWindowState.Normal;
            }
            else this.WindowState = FormWindowState.Maximized;
        }

        private void ExitBtn_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        #region Move form when click into menu
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

        private void menuStrip_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }
        #endregion

        private void FormMain_SizeChanged(object sender, EventArgs e)
        {
            ExitBtn.Left = this.Width - ExitBtn.Width;
            RestoreBtn.Left = ExitBtn.Location.X - RestoreBtn.Width;
            MinimizeBtn.Left = RestoreBtn.Location.X - MinimizeBtn.Width;
            toolPanel.Height = Size.Height - toolPanel.Location.Y;
            propertiesPanel.Height = this.Height - propertiesPanel.Location.Y - statusStrip1.Height;
        }

        private void UnableToKeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        } 
    }
}