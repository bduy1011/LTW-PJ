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

        
        private void FilterMenuStripEnable(bool enable)
        {
            foreach (ToolStripMenuItem item in filterToolStripMenuItem.DropDownItems)
            {
                item.Enabled = enable;
            }
        }
        #region ShortCut Keys

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch (keyData)
            {
                //File
                case (Keys.Control | Keys.N):
                    newToolStripMenuItem_Click(newToolStripMenuItem, null);
                    return true;
                case (Keys.Control | Keys.O):
                    openToolStripMenuItem_Click(openToolStripMenuItem, null);
                    return true;
                case (Keys.Control | Keys.S):
                    if (saveToolStripMenuItem.Enabled)
                        saveToolStripMenuItem_Click(saveToolStripMenuItem, null);
                    return true;
                case (Keys.Control | Keys.W):
                    if (closeToolStripMenuItem.Enabled)
                        closeToolStripMenuItem_Click(closeToolStripMenuItem, null);
                    return true;

                //Edit
                case (Keys.Control | Keys.C):
                    copyToolStripMenuItem_Click(copyToolStripMenuItem, null);
                    return true;
                case (Keys.Control | Keys.V):
                    pasteToolStripMenuItem_Click(pasteToolStripMenuItem, null);
                    return true;
                case (Keys.Control | Keys.X):
                    cutToolStripMenuItem_Click(cutToolStripMenuItem, null);
                    return true;
                case (Keys.Control | Keys.Z):
                    undoToolStripMenuItem_Click(undoToolStripMenuItem, null);
                    return true;
                default:
                    return base.ProcessCmdKey(ref msg, keyData);
            }
        }
        #endregion

        #region WorkSpace
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

        private void WorkSpaceTabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (workSpaceTabControl.TabCount == 0) return;

            layerPanel.Controls.Remove(Current.LayerContainer);
            historyPanel.Controls.Remove(Current.History);
            Current = (WorkSpace)workSpaceTabControl.SelectedTab.Controls[0];
            layerPanel.Controls.Add(Current.LayerContainer);
            LayerButtonCheck();
            opacityVal = Current.LayerContainer.Current.Layer.Opacity;
            OpacityBarUpdate();
            historyPanel.Controls.Add(Current.History);
            saveToolStripMenuItem.Enabled = !Current.Saved;
            BlendModeBoxUpdate(Current.LayerContainer.Current.Blend);
        }
        #endregion

        #region Menu

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

        #region Edit
        //Copy 
        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Current == null) return;
            if (tools.Select.Selected)
            {
                Bitmap bmp;
                bmp = Current.LayerContainer.Current.Layer.Image.Clone(tools.Select.FixedRect, Current.BmpPixelFormat);
                Clipboard.SetImage(bmp);
            }
        }
        //Lay anh tu ClipBoard
        private Image GetImageFromClipboard()
        {
            if (Clipboard.GetDataObject() == null) return null;
            if (Clipboard.GetDataObject().GetDataPresent(DataFormats.Dib))
            {
                var dib = ((System.IO.MemoryStream)Clipboard.GetData(DataFormats.Dib)).ToArray();
                var width = BitConverter.ToInt32(dib, 4);
                var height = BitConverter.ToInt32(dib, 8);
                var bpp = BitConverter.ToInt16(dib, 14);
                if (bpp == 32)
                {
                    var gch = System.Runtime.InteropServices.GCHandle.Alloc(dib, System.Runtime.InteropServices.GCHandleType.Pinned);
                    Bitmap bmp = null;
                    try
                    {
                        var ptr = new IntPtr((long)gch.AddrOfPinnedObject() + 52);
                        bmp = new Bitmap(width, height, width * 4, System.Drawing.Imaging.PixelFormat.Format32bppArgb, ptr);
                        return new Bitmap(bmp);
                    }
                    finally
                    {
                        gch.Free();
                        if (bmp != null) bmp.Dispose();
                    }
                }
            }
            return Clipboard.ContainsImage() ? Clipboard.GetImage() : null;
        }
        //Paste
        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Current == null) return;
            Bitmap bmp = (Bitmap)GetImageFromClipboard();
            if (bmp == null) return;
            bmp.RotateFlip(RotateFlipType.Rotate180FlipX);
            UncheckAll();
            tools.Select.Selected = true;
            tools.Select.Rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            tools.Select.Fix(Current.Rect);
            tools.Transform.Done = false;
            tools.Transform.Rect = tools.Select.Rect;
            tools.Transform.StartPoint = tools.Select.Rect.Location;
            Current.LayerContainer.Current.Layer.Stacking();
            tools.Transform.Image = bmp;
            Current.DrawSpace.TransformRectDisplay();
            LayerMenuStripEnable(false);
            ColorMenuStripEnable(false);
            FilterMenuStripEnable(false);
            tools.Tool = Basic_Photo_Editor.Paint_Tools.Tool.Transform;
            transformStripButton.Checked = true;
            transformStripButton.CheckState = CheckState.Checked;
            ChangeTool();
        }
        //Cut
        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Current == null) return;
            if (tools.Select.Selected)
            {
                Bitmap bmp;
                bmp = Current.LayerContainer.Current.Layer.Image.Clone(tools.Select.FixedRect, Current.BmpPixelFormat);
                Clipboard.SetImage(bmp);
                Current.LayerContainer.Current.Layer.Stacking();
                tools.Eraser.MakeTransparent(Current.LayerContainer.Current.Layer.Image, tools.Select.FixedRect);
                DrawSpaceUpdate();
                Current.History.Add(HistoryEvent.Erase, Current.LayerContainer.Current);
            }
        }
        //Undo
        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Current == null) return;
            if (Current.History.Remove())
                DrawSpaceUpdate();
        }
        #endregion

        #region View
        private void zoomInToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ZoomInBtn_Click(zoomInBtn, null);
        }

        private void zoomOutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ZoomOutBtn_Click(zoomOutBtn, null);
        }

        private void centerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CenterBtn_Click(centerBtn, null);
        }
        #endregion

        #region Layer  
        private void NewLayerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Current != null)
                NewLStripButton_Click(sender, e);
        }

        private void DeleteLayerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Current != null && deleteLStripButton.Enabled)
                DeleteLStripButton_Click(sender, e);
        }

        private void ClearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Current != null)
                ClearLStripButton_Click(sender, e);
        }

        private void RenameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Current != null)
                RenameLStripButton_Click(sender, e);
        }

        private void DuplicateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Current != null && duplicateLStripButton.Enabled)
                DuplicateLStripButton_Click(sender, e);
        }

        private void MergeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Current != null && mergeLStripButton.Enabled)
                MergeLStripButton_Click(sender, e);
        }

        private void FillToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Current == null) return;

            Bitmap bmp = new Bitmap(Current.BmpSize.Width, Current.BmpSize.Height);
            using (Graphics g = Graphics.FromImage(bmp))
            using (SolidBrush brush = new SolidBrush(mainColorPic.BackColor))
            {
                g.FillRectangle(brush, 0, 0, Current.BmpSize.Width, Current.BmpSize.Height);
            }
            Current.DrawSpace.ProcessBoxImage = bmp;
            DrawSpaceProcessUpdate(HistoryEvent.Fill);
            DrawSpaceUpdate();
        }
        #endregion

        #region Tool
        private void toolsToolStripMenuItem_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            ToolStripItem item = e.ClickedItem;
            switch (item.Text)
            {
                case "Transform":
                    transformStripButton_Click(transformStripButton, null);
                    break;
                case "Select":
                    selectStripButton_Click(selectStripButton, null);
                    break;
                case "Drag":
                    dragStripButton_Click(dragStripButton, null);
                    break;
                case "Pen":
                    penStripButton_Click(penStripButton, null);
                    break;
                case "Eraser":
                    eraserStripButton_Click(eraserStripButton, null);
                    break;
                case "Color Picker":
                    pickerStripButton_Click(pickerStripButton, null);
                    break;
                case "Shape":
                    shapeStripButton_Click(shapeStripButton, null);
                    break;
                case "Line":
                    lineStripButton_Click(lineStripButton, null);
                    break;
                case "Bucket":
                    bucketStripButton_Click(bucketStripButton, null);
                    break;
            }
        }
        #endregion

        #region Color
        private void ColorMenuStripEnable(bool enable)
        {
            foreach (ToolStripMenuItem item in colorToolStripMenuItem.DropDownItems)
            {
                item.Enabled = enable;
            }
        }

        private void colorBalanceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (Function_Forms.ColorBalance colorBalance = new Function_Forms.ColorBalance(this, Current.LayerContainer))
            {
                if (!tools.Select.Selected)
                {
                    colorBalance.Image = Current.LayerContainer.Current.Layer.Image;
                }
                else
                {
                    colorBalance.Image = Current.LayerContainer.Current.Layer.Image.Clone(tools.Select.FixedRect, Current.BmpPixelFormat);
                }
                if (colorBalance.ShowDialog() == DialogResult.OK)
                {
                    Current.DrawSpace.ProcessBoxImage = colorBalance.Image;
                    DrawSpaceProcessUpdate(HistoryEvent.DrawFilter);
                    DrawSpaceUpdate();
                }
            }
        }

        private void brightnessAndContrastToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (Function_Forms.Brightness_Contrast brightnessContrast = new Function_Forms.Brightness_Contrast(this, Current.LayerContainer))
            {
                if (!tools.Select.Selected)
                    brightnessContrast.Image = Current.LayerContainer.Current.Layer.Image;
                else
                    brightnessContrast.Image = Current.LayerContainer.Current.Layer.Image.Clone(tools.Select.FixedRect, Current.BmpPixelFormat);

                if (brightnessContrast.ShowDialog() == DialogResult.OK)
                {
                    Current.DrawSpace.ProcessBoxImage = brightnessContrast.Image;
                    DrawSpaceProcessUpdate(HistoryEvent.DrawFilter);
                    DrawSpaceUpdate();
                }
            }
        }

        private void hueAndSaturationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (Function_Forms.Hue_Saturation hue_Saturation = new Function_Forms.Hue_Saturation(this, Current.LayerContainer))
            {
                if (!tools.Select.Selected)
                    hue_Saturation.Image = Current.LayerContainer.Current.Layer.Image;
                else
                    hue_Saturation.Image = Current.LayerContainer.Current.Layer.Image.Clone(tools.Select.FixedRect, Current.BmpPixelFormat);

                hue_Saturation.Initialize();

                if (hue_Saturation.ShowDialog() == DialogResult.OK)
                {
                    Current.DrawSpace.ProcessBoxImage = hue_Saturation.Image;
                    DrawSpaceProcessUpdate(HistoryEvent.DrawFilter);
                    DrawSpaceUpdate();
                }
            }
        }

        private void invertToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Create a new bitmap to hold the inverted image
            Bitmap bitmap;
            float x, y;
            if (!tools.Select.Selected)
            {
                // If no selection, clone the entire current image
                bitmap = (Bitmap)Current.DrawSpace.ProcessBoxImage.Clone();
                x = y = 0;
            }
            else
            {
                // If selection, clone the portion of the current image specified by the selection tool's fixed rectangle
                bitmap = (Current.DrawSpace.ProcessBoxImage as Bitmap).Clone(tools.Select.FixedRect, Current.BmpPixelFormat);
                x = tools.Select.FixedRect.X;
                y = tools.Select.FixedRect.Y;
            }
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                // Create a color matrix to invert the colors of the image
                System.Drawing.Imaging.ColorMatrix matrix = new System.Drawing.Imaging.ColorMatrix();
                matrix.Matrix00 = matrix.Matrix11 = matrix.Matrix22 = -1f;
                matrix.Matrix33 = matrix.Matrix40 = matrix.Matrix41 = matrix.Matrix42 = matrix.Matrix44 = 1f;

                using (System.Drawing.Imaging.ImageAttributes attributes = new System.Drawing.Imaging.ImageAttributes())
                {
                    attributes.SetColorMatrix(matrix);
                    g.DrawImage(Current.LayerContainer.Current.Layer.Image, new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                        x, y, bitmap.Width, bitmap.Height, GraphicsUnit.Pixel, attributes);
                }
                // Set the inverted image as the current image
                Current.DrawSpace.ProcessBoxImage = new Bitmap(bitmap);
            }
            bitmap.Dispose();
            DrawSpaceProcessUpdate(HistoryEvent.DrawFilter);
            DrawSpaceUpdate();
        }

        private void thresholdToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (Function_Forms.Threshold threshold = new Function_Forms.Threshold(this, Current.LayerContainer))
            {
                if (!tools.Select.Selected)
                    threshold.Image = Current.LayerContainer.Current.Layer.Image;
                else
                    threshold.Image = Current.LayerContainer.Current.Layer.Image.Clone(tools.Select.FixedRect, Current.BmpPixelFormat);

                threshold.Initialize();

                if (threshold.ShowDialog() == DialogResult.OK)
                {
                    Current.DrawSpace.ProcessBoxImage = threshold.Image;
                    DrawSpaceProcessUpdate(HistoryEvent.DrawFilter);
                    DrawSpaceUpdate();
                }
            }
        }

        private void grayscaleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Bitmap bitmap;
            float x, y;
            if (!tools.Select.Selected)
            {
                bitmap = (Bitmap)Current.DrawSpace.ProcessBoxImage.Clone();
                x = y = 0;
            }
            else
            {
                bitmap = (Current.DrawSpace.ProcessBoxImage as Bitmap).Clone(tools.Select.FixedRect, Current.BmpPixelFormat);
                x = tools.Select.FixedRect.X;
                y = tools.Select.FixedRect.Y;
            }

            using (Graphics g = Graphics.FromImage(bitmap))
            {
                System.Drawing.Imaging.ColorMatrix matrix = new System.Drawing.Imaging.ColorMatrix(
                   new float[][]
                   {
                         new float[] {.3f, .3f, .3f, 0, 0},
                         new float[] {.59f, .59f, .59f, 0, 0},
                         new float[] {.11f, .11f, .11f, 0, 0},
                         new float[] {0, 0, 0, 1, 0},
                         new float[] {0, 0, 0, 0, 1}
                   });

                using (System.Drawing.Imaging.ImageAttributes attributes = new System.Drawing.Imaging.ImageAttributes())
                {
                    attributes.SetColorMatrix(matrix);
                    g.DrawImage(Current.LayerContainer.Current.Layer.Image, new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                        x, y, bitmap.Width, bitmap.Height, GraphicsUnit.Pixel, attributes);
                }

                Current.DrawSpace.ProcessBoxImage = new Bitmap(bitmap);
            }

            bitmap.Dispose();

            DrawSpaceProcessUpdate(HistoryEvent.DrawFilter);
            DrawSpaceUpdate();
        }
        #endregion

        #region Help
        private void AboutPhotoEditorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Nothing to see here", "Notice");
        }
        #endregion

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

        private void LayerMenuStripEnable(bool enable)
        {
            foreach (ToolStripMenuItem item in layerToolStripMenuItem.DropDownItems)
            {
                item.Enabled = enable;
            }
            layerPanel.Enabled = enable;
        }

        private void NewLStripButton_Click(object sender, EventArgs e)
        {
            using (Function_Forms.NewLayer nlf = new Function_Forms.NewLayer())
            {
                nlf.SetDefaultName(Current.LayerContainer.Count);
                if (nlf.ShowDialog() == DialogResult.OK)
                {
                    string name = nlf.LayerName;
                    bool visible = nlf.IsVisible;
                    using (Bitmap newBmp = new Bitmap(Current.BmpSize.Width, Current.BmpSize.Height))
                    {
                        newBmp.MakeTransparent();
                        Layer layer = new Layer(newBmp, name, visible);
                        Current.LayerContainer.AddLayerRow(ref layer);
                        LayerButtonCheck();
                        blendModeBox.SelectedIndex = 0;
                        opacityVal = Current.LayerContainer.Current.Layer.Opacity;
                        OpacityBarUpdate();
                        DrawSpaceProcessUpdate(HistoryEvent.NewL);
                        DrawSpaceUpdate();
                    }
                }
            }
        }

        private void DeleteLStripButton_Click(object sender, EventArgs e)
        {
            Current.LayerContainer.RemoveLayerRow();
            LayerButtonCheck();
            DrawSpaceProcessUpdate(HistoryEvent.DeleteL);
            DrawSpaceUpdate();
        }

        private void DownLStripButton_Click(object sender, EventArgs e)
        {
            Current.LayerContainer.MoveDown();
            LayerButtonCheck();
            DrawSpaceProcessUpdate(HistoryEvent.Ldown);
            DrawSpaceUpdate();
        }

        private void UpLStripButton_Click(object sender, EventArgs e)
        {
            Current.LayerContainer.MoveUp();
            LayerButtonCheck();
            DrawSpaceProcessUpdate(HistoryEvent.Lup);
            DrawSpaceUpdate();
        }

        private void ClearLStripButton_Click(object sender, EventArgs e)
        {
            Bitmap bmp = new Bitmap(Current.BmpSize.Width, Current.BmpSize.Height);
            Current.DrawSpace.ProcessBoxImage = bmp;
            DrawSpaceProcessUpdate(HistoryEvent.Clear);
            DrawSpaceUpdate();
        }

        private void RenameLStripButton_Click(object sender, EventArgs e)
        {
            using (Function_Forms.LayerRename lr = new Function_Forms.LayerRename())
            {
                lr.DefaultName = Current.LayerContainer.Current.Text;
                if (lr.ShowDialog() == DialogResult.OK)
                {
                    if (lr.NewName != "")
                    {
                        Current.LayerContainer.Current.Text = lr.NewName;
                        Current.LayerContainer.UpdateName();
                    }
                }
            }
        }

        private void MergeLStripButton_Click(object sender, EventArgs e)
        {
            Current.LayerContainer.Merge();
            LayerButtonCheck();
            DrawSpaceProcessUpdate(HistoryEvent.MergeL);
            DrawSpaceUpdate();
        }

        private void DuplicateLStripButton_Click(object sender, EventArgs e)
        {
            Current.LayerContainer.Duplicate();
            LayerButtonCheck();
            DrawSpaceProcessUpdate(HistoryEvent.DuplicateL);
            DrawSpaceUpdate();
        }

        private void OpacityBar_MouseMoveOrDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                opacityVal = (float)e.Location.X / opacityBar.Width * 100;
                if (opacityVal > 100) opacityVal = 100;
                if (opacityVal < 0) opacityVal = 0;
                OpacityBarUpdate();
            }
        }

        private void OpacityBar_MouseUp(object sender, MouseEventArgs e)
        {
            Current.LayerContainer.Current.Opacity = opacityVal;
            Current.LayerContainer.Current.Layer.Opacity = opacityVal;
            DrawSpaceProcessUpdate(HistoryEvent.Opacity);
            DrawSpaceUpdate();
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

        private void BlendModeBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!blendboxupdate)
            {
                Current.LayerContainer.Current.Blend = (Blend)blendModeBox.SelectedIndex;
                DrawSpaceUpdate();
                if (Current.LayerContainer.Current.BlendCount == 1)
                    return;

                Current.History.Add(HistoryEvent.Blend, Current.LayerContainer.Current);
            }
        }

        bool blendboxupdate = false;
        public void BlendModeBoxUpdate(Blend mode)
        {
            blendboxupdate = true;
            blendModeBox.Text = mode.ToString("G");
            blendboxupdate = false;
        }
        #endregion

        #region LeftPanel

        #region Color_Panel
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

        private int redVal = 0;
        private int blueVal = 0;
        private int greenVal = 0;

        // When MainColorPic change color -> Update color bar (R,G,B)
        private void MainColorPic_BackColorChanged(object sender, EventArgs e)
        {
            redVal = mainColorPic.BackColor.R;
            greenVal = mainColorPic.BackColor.G;
            blueVal = mainColorPic.BackColor.B;

            BarUpdate(ref redBar, Color.Pink, redVal);
            BarUpdate(ref greenBar, Color.PaleGreen, greenVal);
            BarUpdate(ref blueBar, Color.LightSteelBlue, blueVal);

            label7.Text = redVal.ToString();
            label8.Text = greenVal.ToString();
            label9.Text = blueVal.ToString();

            hexCode.Text = ColorTranslator.ToHtml(mainColorPic.BackColor);
            tools.Color = mainColorPic.BackColor;
        }

        // Update -> Fill color for bar 
        private void BarUpdate(ref PictureBox bar, Color c, int val)
        {
            using (SolidBrush b = new SolidBrush(c))
            {
                using (Graphics g = bar.CreateGraphics())
                {
                    g.Clear(bar.BackColor);
                    g.FillRectangle(b, new Rectangle(0, 0, val / 2, bar.Height));
                }
            }
        }

        // Check and set value of bar 
        private void ValCheck(ref int n)
        {
            if (n > 255) n = 255;
            if (n < 0) n = 0;
        }

        // Change color of mainColorPic
        private void BarVal(ref int val, ref PictureBox bar, ref MouseEventArgs e)
        {
            val = (int)(((double)e.Location.X / bar.Width) * 255);
            ValCheck(ref val);
            mainColorPic.BackColor = Color.FromArgb(redVal, greenVal, blueVal);
        }

        // When move bar -> change mainColorPic by function BarVal
        private void RedBar_MouseMoveOrDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                BarVal(ref redVal, ref redBar, ref e);
            }
        }

        // When move bar -> change mainColorPic by function BarVal
        private void GreenBar_MouseMoveOrDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                BarVal(ref greenVal, ref greenBar, ref e);
            }
        }

        // When move bar -> change mainColorPic by function BarVal
        private void BlueBar_MouseMoveOrDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                BarVal(ref blueVal, ref blueBar, ref e);
            }
        }
        #endregion

        #region Tool_Panel
        //Ham Kiem Tra
        private void UncheckAll()
        {
            foreach (ToolStripButton button in mToolStrip.Items)
            {
                button.Checked = false;
                button.CheckState = CheckState.Unchecked;
            }
            foreach (ToolStripButton button in pToolStrip.Items)
            {
                button.Checked = false;
                button.CheckState = CheckState.Unchecked;
            }
            foreach (ToolStripButton button in sToolStrip.Items)
            {
                button.Checked = false;
                button.CheckState = CheckState.Unchecked;
            }
            if (tools.Tool == Basic_Photo_Editor.Paint_Tools.Tool.Transform)
            {
                if (Current != null)
                {
                    LayerMenuStripEnable(true);
                    ColorMenuStripEnable(true);
                    FilterMenuStripEnable(true);
                }

                if (tools.Select.Selected)
                {
                    Current.DrawSpace.TransformForceDraw();
                    tools.Transform.Image.Dispose();
                    Current.DrawSpace.ClearTop();
                    tools.Transform.Reset();
                    DS_MouseUp(null, null);
                }
            }
        }
        //Thay Doi Cong Cu
        private void ChangeTool()
        {
            if (propertiesPanel.Controls.Count != 0)
            {
                propertiesPanel.Controls.Remove(propertiesPanel.Controls[0]);
                propertiesPanel.Controls.Add(tools.Current);
            }
            else propertiesPanel.Controls.Add(tools.Current);
        }
        //Di chuyen 
        private void transformStripButton_Click(object sender, EventArgs e)
        {
            UncheckAll();
            (sender as ToolStripButton).Checked = true;
            (sender as ToolStripButton).CheckState = CheckState.Checked;
            if (tools.Select.Selected)
            {
                //Kiem tra
                tools.Transform.Done = false;
                tools.Transform.Rect = tools.Select.Rect;
                tools.Transform.StartPoint = tools.Select.Rect.Location;
                tools.Transform.Image = Current.LayerContainer.Current.Layer.Image.Clone(tools.Select.FixedRect, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                Current.LayerContainer.Current.Layer.Stacking();
                tools.Eraser.MakeTransparent(Current.LayerContainer.Current.Layer.Image, tools.Select.FixedRect);
                DrawSpaceUpdate();
                Current.DrawSpace.TransformRectDisplay();
            }
            LayerMenuStripEnable(false);
            ColorMenuStripEnable(false);
            FilterMenuStripEnable(false);
            tools.Tool = Paint_Tools.Tool.Transform;

            ChangeTool();
        }
        //Pen Tool Event
        private void penStripButton_Click(object sender, EventArgs e)
        {
            UncheckAll();
            (sender as ToolStripButton).Checked = true;
            (sender as ToolStripButton).CheckState = CheckState.Checked;
            tools.Tool = Paint_Tools.Tool.Pen;
            ChangeTool();
        }
        //Select Tool
        private void selectStripButton_Click(object sender, EventArgs e)
        {
            UncheckAll();
            (sender as ToolStripButton).Checked = true;
            (sender as ToolStripButton).CheckState = CheckState.Checked;
            tools.Tool = Paint_Tools.Tool.Select;
            ChangeTool();
        }
        //Drag (move) tool
        private void dragStripButton_Click(object sender, EventArgs e)
        {
            UncheckAll();
            (sender as ToolStripButton).Checked = true;
            (sender as ToolStripButton).CheckState = CheckState.Checked;
            tools.Tool = Paint_Tools.Tool.Drag;
            ChangeTool();
        }
        //Eraser tool
        private void eraserStripButton_Click(object sender, EventArgs e)
        {
            UncheckAll();
            (sender as ToolStripButton).Checked = true;
            (sender as ToolStripButton).CheckState = CheckState.Checked;
            tools.Tool = Paint_Tools.Tool.Eraser;
            ChangeTool();
        }
        //picker color tool
        private void pickerStripButton_Click(object sender, EventArgs e)
        {
            UncheckAll();
            (sender as ToolStripButton).Checked = true;
            (sender as ToolStripButton).CheckState = CheckState.Checked;
            tools.Tool = Paint_Tools.Tool.Picker;
            ChangeTool();
        }
        //bucket color tool
        private void bucketStripButton_Click(object sender, EventArgs e)
        {
            UncheckAll();
            (sender as ToolStripButton).Checked = true;
            (sender as ToolStripButton).CheckState = CheckState.Checked;
            tools.Tool = Paint_Tools.Tool.Bucket;
            ChangeTool();
        }
        //DrawShape Tool
        private void shapeStripButton_Click(object sender, EventArgs e)
        {
            UncheckAll();
            (sender as ToolStripButton).Checked = true;
            (sender as ToolStripButton).CheckState = CheckState.Checked;
            tools.Tool = Paint_Tools.Tool.Shape;
            ChangeTool();
        }
        //DrawLine Tool
        private void lineStripButton_Click(object sender, EventArgs e)
        {
            UncheckAll();
            (sender as ToolStripButton).Checked = true;
            (sender as ToolStripButton).CheckState = CheckState.Checked;
            tools.Tool = Paint_Tools.Tool.Line;
            ChangeTool();
        }
        #endregion

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
