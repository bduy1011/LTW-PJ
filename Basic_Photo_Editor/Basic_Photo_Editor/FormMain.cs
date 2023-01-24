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

        public float opacityVal;

        #region Main Form
        public FormMain()
        {
            InitializeComponent();
        }
        
        private void FormMain_Load(object sender, EventArgs e)
        {
            ToolStripManager.Renderer = new Basic_Photo_Editor.ColorTable.MyToolStripRender(new ColorTable.ToolStripColorTable());
            LayerMenuStripEnable(false);
            ColorMenuStripEnable(false);
            FilterMenuStripEnable(false);
            ViewMenuStripEnable(false);
            saveToolStripMenuItem.Enabled = false;
            saveAsToolStripMenuItem.Enabled = false;
            closeToolStripMenuItem.Enabled = false;
            tools = new Paint_Tools.Tools();
            propertiesPanel.Controls.Add(tools.Current);
            hexCode.Text = ColorTranslator.ToHtml(mainColorPic.BackColor);
        }
        private void FormMain_SizeChanged(object sender, EventArgs e)
        {
            ExitBtn.Left = this.Width - ExitBtn.Width;
            RestoreBtn.Left = ExitBtn.Location.X - RestoreBtn.Width;
            MinimizeBtn.Left = RestoreBtn.Location.X - MinimizeBtn.Width;
            toolPanel.Height = Size.Height - toolPanel.Location.Y;
            propertiesPanel.Height = this.Height - propertiesPanel.Location.Y - statusStrip1.Height;
            layerPanel.Height = this.Height - layerPanel.Location.Y - statusStrip1.Height;
            workSpaceTabControl.Width = this.Width - rightPanel.Width - leftPanel.Width;
            workSpaceTabControl.Height = this.Height - bottomPanel.Height - statusStrip1.Height - menuStrip.Height;
            if (Current != null)
            {
                Current.LayerContainer.Height = layerPanel.Height - blendPanel.Height - layerToolStrip.Height - statusStrip1.Height;
            }
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
            DrawSpaceInit();
            LayerContainerInit();
            Current.DrawSpace.BGGenerator(color);
            workSpaceTabControl.SelectedIndex = workSpaceTabControl.TabPages.IndexOf(tab);
        }

        #region File
        //New File Button 
        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (Function_Forms.NewFile_Form newFileForm = new Function_Forms.NewFile_Form())
            {
                newFileForm.ColorFore = mainColorPic.BackColor;
                newFileForm.ColorBack = subColorPic.BackColor;

                if (newFileForm.ShowDialog() == DialogResult.OK)
                {
                    Bitmap bmp = new Bitmap(newFileForm.ImageSize.Width, newFileForm.ImageSize.Height);
                    AddWorkTab(bmp, newFileForm.BGColor);
                    Current.FileName = newFileForm.FileName;
                    Current.Parent.Text = Current.FileName;
                    DrawSpaceUpdate();
                    Current.Saved = true;
                    Current.Working = true;
                    saveAsToolStripMenuItem.Enabled = true;
                    closeToolStripMenuItem.Enabled = true;
                    bmp.Dispose();
                }
            }
        }
        //Open Button
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
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
        //Save As Button 
        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Current.Working)
            {
                using (SaveFileDialog saveFileDialog = new SaveFileDialog())
                {
                    saveFileDialog.FileName = Current.FileName;
                    saveFileDialog.Filter = "Image (*.BMP)|*.bmp|JPEG Image (*.JPEG)|*.jpeg|PNG Image (*.PNG)|*.png";
                    saveFileDialog.DefaultExt = "png";
                    saveFileDialog.FilterIndex = 3;
                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        Current.DrawSpace.Final.Save(saveFileDialog.FileName);
                        Current.FilePath = saveFileDialog.FileName;
                        Current.Saved = true;
                        saveToolStripMenuItem.Enabled = false;
                        Current.Stored = true;
                    }
                }
            }
        }
        //Save Button
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Current.Working)
            {
                if (Current.Stored)
                {
                    Current.DrawSpace.Final.Save(Current.FilePath);
                    Current.Saved = true;
                    saveToolStripMenuItem.Enabled = false;
                }
                else
                {
                    saveAsToolStripMenuItem_Click(this, e);
                }
                Current.Parent.Text = Current.FileName;
            }
        }
        //Close Button
        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Current.Working)
            {
                if (Current.Saved)
                {
                    DialogResult dialogResult = MessageBox.Show("Your work haven't saved yet.\nDo you want to save it", "Photo Editor",
                    MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
                    if (dialogResult == DialogResult.Yes)
                    {
                        saveToolStripMenuItem_Click(sender, e);
                    }
                    else if (dialogResult == DialogResult.Cancel)

                    {
                        return;
                    }
                }

                tools.Select.Selected = false;

                workSpaceTabControl.SelectedTab.Controls.Remove(Current);
                Current.LayerContainer.Dispose();
                Current.History.Dispose();
                Current.Dispose();
                workSpaceTabControl.SelectedTab.Dispose();

                if (workSpaceTabControl.TabCount == 0)
                {
                    LayerMenuStripEnable(false);
                    ColorMenuStripEnable(false);
                    FilterMenuStripEnable(false);
                    ViewMenuStripEnable(false);
                    Current.Working = false;
                    closeToolStripMenuItem.Enabled = false;
                    saveToolStripMenuItem.Enabled = false;
                    saveAsToolStripMenuItem.Enabled = false;
                    Current.FileName = Current.FilePath = "";
                    workSpaceTabControl.Visible = false;
                    Current = null;
                }
                else Current = (WorkSpace)workSpaceTabControl.SelectedTab.Controls[0];
            }
        }
        //Exit Button 
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int tabcount = workSpaceTabControl.TabPages.Count;
            for (int i = 0; i < tabcount; i++)
            {
                closeToolStripMenuItem_Click(null, null);
            }
            this.Close();
        }
        #endregion

        #region DrawSpace
        private void DrawSpaceInit()
        {
            Current.DrawSpace.Location = new System.Drawing.Point(0, 0);
            Current.DrawSpace.Name = "workspace";
            Current.DrawSpace.Size = Current.BmpSize;
            Current.DrawSpace.Tools = tools;
            Current.DrawSpace.Init();
            Current.DrawSpace.Event.MouseDown += DS_MouseDown;
            Current.DrawSpace.Event.MouseLeave += DS_MouseLeave;
            Current.DrawSpace.Event.MouseMove += DS_MouseMove;
            Current.DrawSpace.Event.MouseUp += DS_MouseUp;

            CenterBtn_Click(centerBtn, null);
        }

        public void DrawSpaceUpdate()
        {
            Current.LayerContainer.FinalUpdate(Current.DrawSpace.Final_Graphics, Current.DrawSpace.Final);
            Current.DrawSpace.FinalDisplay();
            Current.DrawSpace.CurrentVisible = Current.LayerContainer.Current.Layer.Visible;
            Current.DrawSpace.Invalidate();
        }

        public void DrawSpaceProcessUpdate(HistoryEvent e)
        {
            Current.Saved = false;
            Current.Parent.Text = Current.FileName + "*";
            saveToolStripMenuItem.Enabled = true;

            Bitmap bmp;
            if (!tools.Select.Selected) bmp = (Bitmap)Current.DrawSpace.ProcessBoxImage.Clone();
            else bmp = (Current.DrawSpace.ProcessBoxImage as Bitmap).Clone(tools.Select.FixedRect, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            if (e == HistoryEvent.Draw || e == HistoryEvent.Transform || e == HistoryEvent.Fill || e == HistoryEvent.Erase)
            {
                Current.LayerContainer.ProcessUpdate(bmp);
            }
            else if (e == HistoryEvent.DrawFilter || e == HistoryEvent.Clear)
            {
                Current.LayerContainer.ProcessUpdate(bmp, filter: true);
            }

            Current.DrawSpace.ClearProcess();
            bmp.Dispose();

            Current.History.Add(e, Current.LayerContainer.Current);
        }

        private void DS_MouseDown(object sender, MouseEventArgs e)
        {
            if (tools.Tool == Basic_Photo_Editor.Paint_Tools.Tool.Picker)
            {
                mainColorPic.BackColor = tools.Picker.Color;
            }
        }

        private void DS_MouseMove(object sender, MouseEventArgs e)
        {
            mouseLocation.Text = e.Location.ToString();
        }

        private void DS_MouseLeave(object sender, EventArgs e)
        {
            mouseLocation.Text = "";
        }

        private void DS_MouseUp(object sender, MouseEventArgs e)
        {
            switch (tools.Tool)
            {
                case Basic_Photo_Editor.Paint_Tools.Tool.Pen:
                    DrawSpaceProcessUpdate(HistoryEvent.Draw);
                    break;
                case Basic_Photo_Editor.Paint_Tools.Tool.Eraser:
                    DrawSpaceProcessUpdate(HistoryEvent.Erase);
                    break;
                case Basic_Photo_Editor.Paint_Tools.Tool.Transform:
                    if (!tools.Select.Selected && tools.Transform.Done)
                    {
                        DrawSpaceProcessUpdate(HistoryEvent.Transform);
                        tools.Transform.Reset();
                        tools.Transform.Done = false;
                    }
                    break;
                case Basic_Photo_Editor.Paint_Tools.Tool.Shape:
                    if (tools.Shape.Drawed)
                        DrawSpaceProcessUpdate(HistoryEvent.Draw);
                    break;
                case Basic_Photo_Editor.Paint_Tools.Tool.Line:
                    if (tools.Line.Drawed)
                        DrawSpaceProcessUpdate(HistoryEvent.Draw);
                    break;
                case Basic_Photo_Editor.Paint_Tools.Tool.Bucket:
                    DrawSpaceProcessUpdate(HistoryEvent.Draw);
                    break;
            }

            DrawSpaceUpdate();
        }
        #endregion

        #region Layer
        private void LayerContainerInit()
        {
            Current.LayerContainer.AutoScroll = true;
            Current.LayerContainer.Location = new Point(3, 85);
            Current.LayerContainer.Name = "Current.LayerContainer";
            Current.LayerContainer.Size = new Size(layerPanel.Width - 6, layerPanel.Height - 87);
            Current.LayerContainer.Tool = tools;
            layerPanel.Controls.Add(Current.LayerContainer);
            blendModeBox.SelectedIndex = 0;
            opacityVal = 100f;
            OpacityBarUpdate();
            LayerButtonCheck();
        }

        public void LayerButtonCheck()
        {
            if (Current.LayerContainer.CurrentIndex == Current.LayerContainer.Count - 1)
            {
                mergeLStripButton.Enabled = true;
                downLStripButton.Enabled = true;
                upLStripButton.Enabled = false;

                mergeToolStripMenuItem.Enabled = true;
            }
            else if (Current.LayerContainer.CurrentIndex == 0)
            {
                downLStripButton.Enabled = false;
                mergeLStripButton.Enabled = false;
                upLStripButton.Enabled = true;

                mergeToolStripMenuItem.Enabled = false;
            }
            else
            {
                mergeLStripButton.Enabled = true;
                downLStripButton.Enabled = true;
                upLStripButton.Enabled = true;

                mergeToolStripMenuItem.Enabled = true;
            }

            if (Current.LayerContainer.Count > 1)
            {
                deleteLStripButton.Enabled = true;
                duplicateLStripButton.Enabled = true;

                deleteLayerToolStripMenuItem.Enabled = true;
                duplicateToolStripMenuItem.Enabled = true;
            }
            else
            {
                deleteLStripButton.Enabled = false;
                upLStripButton.Enabled = false;
                downLStripButton.Enabled = false;
                mergeLStripButton.Enabled = false;

                deleteLayerToolStripMenuItem.Enabled = false;
                mergeToolStripMenuItem.Enabled = false;
            }
        }

        public void OpacityBarUpdate()
        {
            using (Graphics g = opacityBar.CreateGraphics())
            {
                label10.Text = ((int)opacityVal).ToString();
                int w = (int)Math.Ceiling(((float)opacityVal / 100) * opacityBar.Width);
                g.Clear(opacityBar.BackColor);
                g.FillRectangle(Brushes.Gainsboro, new Rectangle(0, 0, w, opacityBar.Height));
            }
        }
        #endregion

        #region ColorPanel
        private bool colorIsPicking = false;
        private void ColorWheel_MouseDown(object sender, MouseEventArgs e)
        {
            colorIsPicking = true;
        }

        private void ColorWheel_MouseMove(object sender, MouseEventArgs e)
        {
            if (colorIsPicking)
            {
                using (Bitmap bmp = new Bitmap(colorWheel.Image))
                {
                    if (e.X > 0 && e.Y > 0 && e.X < colorWheel.Width && e.Y < colorWheel.Height)
                    {
                        Color c = bmp.GetPixel(e.X, e.Y);
                        if (c.A == 255)
                            mainColorPic.BackColor = c;
                    }
                }
            }
        }

        private void ColorWheel_MouseUp(object sender, MouseEventArgs e)
        {
            colorIsPicking = false;
        }

        private void ColorWheel_MouseClick(object sender, MouseEventArgs e)
        {
            using (Bitmap bmp = new Bitmap(colorWheel.Image))
            {
                Color c = bmp.GetPixel(e.X, e.Y);
                if (c.A == 255)
                    mainColorPic.BackColor = c;
            }
        }

        private void ColorSwitch_Click(object sender, EventArgs e)
        {
            Color tmp = mainColorPic.BackColor;
            mainColorPic.BackColor = subColorPic.BackColor;
            subColorPic.BackColor = tmp;
        }


        #endregion

        #region BottomPanel

        private void ViewMenuStripEnable(bool enable)
        {
            foreach (ToolStripMenuItem item in viewToolStripMenuItem.DropDownItems)
            {
                item.Enabled = enable;
            }
            bottomPanel.Enabled = enable;
        }

        private void ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Current == null) return;

            float zoom = 0;
            switch (comboBox1.SelectedIndex)
            {
                //50%
                case 0:
                    zoom = 50;
                    break;
                //75%
                case 1:
                    zoom = 75;
                    break;
                //100%
                case 2:
                    zoom = 100;
                    break;
                //150%
                case 3:
                    zoom = 150;
                    break;
                //200%
                case 4:
                    zoom = 200;
                    break;
                //200%
                case 5:
                    zoom = 300;
                    break;
                //200%
                case 6:
                    zoom = 400;
                    break;
            }

            Current.DrawSpace.Scaling(zoom / 100);
            comboBox1.Text = ((int)(Current.DrawSpace.Zoom * 100)).ToString() + '%';
            DrawSpaceUpdate();
        }

        private void ZoomOutBtn_Click(object sender, EventArgs e)
        {
            float zoom = float.Parse(comboBox1.Text.Substring(0, comboBox1.Text.Length - 1));
            if (zoom <= 50) return;

            float n;
            float m = zoom;
            foreach (string text in comboBox1.Items)
            {
                n = float.Parse(text.Substring(0, text.Length - 1));
                if (n < zoom)
                {
                    m = n;
                }
                else
                {
                    zoom = m;
                    break;
                }
            }

            Current.DrawSpace.Scaling(zoom / 100);
            comboBox1.Text = ((int)(Current.DrawSpace.Zoom * 100)).ToString() + '%';
            DrawSpaceUpdate();
        }

        private void ZoomInBtn_Click(object sender, EventArgs e)
        {
            float zoom = float.Parse(comboBox1.Text.Substring(0, comboBox1.Text.Length - 1));
            if (zoom >= 400) return;

            float n;
            foreach (string text in comboBox1.Items)
            {
                n = float.Parse(text.Substring(0, text.Length - 1));
                if (zoom < n)
                {
                    zoom = n;
                    break;
                }
            }

            Current.DrawSpace.Scaling(zoom / 100);
            comboBox1.Text = ((int)(Current.DrawSpace.Zoom * 100)).ToString() + '%';
            DrawSpaceUpdate();
        }

        private void CenterBtn_Click(object sender, EventArgs e)
        {
            Current.DrawSpace.SetCenter();
            comboBox1.Text = ((int)(Current.DrawSpace.Zoom * 100)).ToString() + '%';
            DrawSpaceUpdate();
        }
        #endregion
    }
}