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
        private Paint_Tools.Tools tools;

        #region Main Form
        public FormMain()
        {
            InitializeComponent();
        }
        
        private void FormMain_Load(object sender, EventArgs e)
        {
            /*ToolStripManager.Renderer = new Basic_Photo_Editor.ColorTable.MyToolStripRender(new ColorTable.ToolStripColorTable());
            LayerMenuStripEnable(false);
            ColorMenuStripEnable(false);
            FilterMenuStripEnable(false);
            ViewMenuStripEnable(false);
            saveToolStripMenuItem.Enabled = false;
            saveAsToolStripMenuItem.Enabled = false;
            closeToolStripMenuItem.Enabled = false;
            tools = new Paint_Tools.Tools();
            propertiesPanel.Controls.Add(tools.Current);
            hexCode.Text = ColorTranslator.ToHtml(mainColorPic.BackColor);*/
        }
        private void FormMain_SizeChanged(object sender, EventArgs e)
        {
            ExitBtn.Left = this.Width - ExitBtn.Width;
            RestoreBtn.Left = ExitBtn.Location.X - RestoreBtn.Width;
            MinimizeBtn.Left = RestoreBtn.Location.X - MinimizeBtn.Width;
            toolPanel.Height = Size.Height - toolPanel.Location.Y;
            propertiesPanel.Height = this.Height - propertiesPanel.Location.Y - statusStrip1.Height;
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



        private void UnableToKeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }
        #endregion 

        private void LayerMenuStripEnable(bool enable)
        {
            foreach (ToolStripMenuItem item in layerToolStripMenuItem.DropDownItems)
            {
                item.Enabled = enable;
            }
            layerPanel.Enabled = enable;
        }
        private void ColorMenuStripEnable(bool enable)
        {
            foreach (ToolStripMenuItem item in colorToolStripMenuItem.DropDownItems)
            {
                item.Enabled = enable;
            }
        }
        private void FilterMenuStripEnable(bool enable)
        {
            foreach (ToolStripMenuItem item in filterToolStripMenuItem.DropDownItems)
            {
                item.Enabled = enable;
            }
        }
        private void ViewMenuStripEnable(bool enable)
        {
            foreach (ToolStripMenuItem item in viewToolStripMenuItem.DropDownItems)
            {
                item.Enabled = enable;
            }
            bottomPanel.Enabled = enable;
        }
        public void DrawSpaceUpdate()
        {
            Current.LayerContainer.FinalUpdate(Current.DrawSpace.Final_Graphics, Current.DrawSpace.Final);
            Current.DrawSpace.FinalDisplay();
            Current.DrawSpace.CurrentVisible = Current.LayerContainer.Current.Layer.Visible;
            Current.DrawSpace.Invalidate();
        }
        private void AddWorkTab(Bitmap bmp, Color color)
        {
            LayerMenuStripEnable(true);
            ColorMenuStripEnable(true);
            FilterMenuStripEnable(true);
            ViewMenuStripEnable(true);

            if (!workSpaceTabControl.Visible)
                workSpaceTabControl.Visible = true;

            if (Current != null)
            {
                layerPanel.Controls.Remove(Current.LayerContainer);
                historyPanel.Controls.Remove(Current.History);
            }

            DrawSpace drawSpace = new DrawSpace();

            LayerContainer layerContainer = new LayerContainer();
            Layer firstLayer = new Layer(bmp, "Layer1", true);
            layerContainer.AddLayerRow(ref firstLayer);
            layerContainer.ScaleMatrix = drawSpace.ScaleMatrix;

            if (historyPanel.Controls.Count != 0)
                historyPanel.Controls.Clear();
            History history = new History();
            historyPanel.Controls.Add(history);

            WorkSpace newWS = new WorkSpace(drawSpace, layerContainer, history);
            TabPage tab = new TabPage();
            tab.Controls.Add(newWS);
            workSpaceTabControl.TabPages.Add(tab);
            Current = newWS;
            Current.BmpSize = bmp.Size;
            Current.Rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            Current.BmpPixelFormat = bmp.PixelFormat;
            Current.DrawSpace.BGGenerator(color);
            workSpaceTabControl.SelectedIndex = workSpaceTabControl.TabPages.IndexOf(tab);
        }
        
        #region File
        //Open Button
        private void OpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Image Files(*.BMP;*.JPG;*.PNG)|*.bmp;*.jpg;*.png|All files (*.*)|*.*";
                ofd.FilterIndex = 2;
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    Bitmap bmp = new Bitmap(ofd.FileName);
                    AddWorkTab(bmp, Color.Transparent);
                    Current.FilePath = ofd.FileName;
                    Current.Parent.Text = Current.FileName;
                    DrawSpaceUpdate();
                    Current.Saved = true;
                    Current.Stored = true;
                    Current.Working = true;
                    saveAsToolStripMenuItem.Enabled = true;
                    closeToolStripMenuItem.Enabled = true;
                    bmp.Dispose();
                }
            }
        }

        #endregion
      
    }
}