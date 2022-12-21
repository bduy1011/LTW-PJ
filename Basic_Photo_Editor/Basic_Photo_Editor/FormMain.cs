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
        //private Tools.Tools tool;

        #region Form

        public FormMain()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
          
        }

        private void Form1_SizeChanged(object sender, EventArgs e)
        {
          
        }

        private void NoKeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

        private void Form1_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void MinMaxBtn_Click(object sender, EventArgs e)
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
            ExitToolStripMenuItem_Click(null, null);
        }

        #endregion

        #region Key shortcut

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch (keyData)
            {
                //
                //file
                //
                case (Keys.Control | Keys.N):
                    NewToolStripMenuItem_Click(newToolStripMenuItem, null);
                    return true;
                case (Keys.Control | Keys.O):
                    OpenToolStripMenuItem_Click(openToolStripMenuItem, null);
                    return true;
                case (Keys.Control | Keys.S):
                    if (saveToolStripMenuItem.Enabled)
                        SaveToolStripMenuItem_Click(saveToolStripMenuItem, null);
                    return true;
                case (Keys.Control | Keys.W):
                    if (closeToolStripMenuItem.Enabled)
                        CloseToolStripMenuItem_Click(closeToolStripMenuItem, null);
                    return true;
                //
                //edit
                //
                case (Keys.Control | Keys.C):
                    CopyToolStripMenuItem_Click(copyToolStripMenuItem, null);
                    return true;
                case (Keys.Control | Keys.V):
                    PasteToolStripMenuItem_Click(pasteToolStripMenuItem, null);
                    return true;
                case (Keys.Control | Keys.X):
                    CutToolStripMenuItem_Click(cutToolStripMenuItem, null);
                    return true;
                case (Keys.Control | Keys.Z):
                    UndoToolStripMenuItem_Click(undoToolStripMenuItem, null);
                    return true;
                //
                //view
                //
                case (Keys.Control | Keys.Add):
                    ZoomInToolStripMenuItem_Click(zoomInToolStripMenuItem, null);
                    return true;
                case (Keys.Control | Keys.Subtract):
                    ZoomOutToolStripMenuItem_Click(zoomOutToolStripMenuItem, null);
                    return true;
                case (Keys.Control | Keys.Shift | Keys.C):
                    CenterToolStripMenuItem_Click(centerToolStripMenuItem, null);
                    return true;
                //
                //tool
                //
                case Keys.T:
                    TransformStripButton_Click(transformStripButton, null);
                    return true;
                case Keys.A:
                    SelectStripButton_Click(selectStripButton, null);
                    return true;
                case Keys.H:
                    DragStripButton_Click(dragStripButton, null);
                    return true;
                case Keys.B:
                    PenStripButton_Click(penStripButton, null);
                    return true;
                case Keys.E:
                    EraserStripButton_Click(eraserStripButton, null);
                    return true;
                case Keys.P:
                    PickerStripButton_Click(pickerStripButton, null);
                    return true;
                case Keys.S:
                    ShapeStripButton_Click(shapeStripButton, null);
                    return true;
                case Keys.L:
                    LineStripButton_Click(lineStripButton, null);
                    return true;
                case Keys.F:
                    BucketStripButton_Click(bucketStripButton, null);
                    return true;
                //
                //layer
                //
                case (Keys.Control | Keys.Shift | Keys.N):
                    if (layerPanel.Enabled)
                        NewLayerToolStripMenuItem_Click(newLayerToolStripMenuItem, null);
                    return true;
                case (Keys.Control | Keys.Shift | Keys.D):
                    if (layerPanel.Enabled)
                        DeleteLayerToolStripMenuItem_Click(deleteLayerToolStripMenuItem, null);
                    return true;
                case (Keys.Control | Keys.Shift | Keys.Delete):
                    if (layerPanel.Enabled)
                        DeleteLayerToolStripMenuItem_Click(deleteLayerToolStripMenuItem, null);
                    return true;
                case (Keys.Control | Keys.Shift | Keys.R):
                    if (layerPanel.Enabled)
                        RenameToolStripMenuItem_Click(renameToolStripMenuItem, null);
                    return true;
                case (Keys.Control | Keys.Shift | Keys.J):
                    if (layerPanel.Enabled)
                        DuplicateToolStripMenuItem_Click(duplicateToolStripMenuItem, null);
                    return true;
                case (Keys.Control | Keys.Shift | Keys.K):
                    if (layerPanel.Enabled)
                        MergeToolStripMenuItem_Click(mergeToolStripMenuItem, null);
                    return true;
                case (Keys.Control | Keys.Shift | Keys.F):
                    if (layerPanel.Enabled)
                        FillToolStripMenuItem_Click(fillToolStripMenuItem, null);
                    return true;
                case (Keys.Up):
                    if (upLStripButton.Enabled && layerPanel.Enabled)
                        UpLStripButton_Click(upLStripButton, null);
                    return true;
                case (Keys.Down):
                    if (downLStripButton.Enabled && layerPanel.Enabled)
                        DownLStripButton_Click(downLStripButton, null);
                    return true;

                default:
                    return base.ProcessCmdKey(ref msg, keyData);
            }
        }

        #endregion

        #region MenuStip

        #region File menu

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
                    DSUpdate();
                    Current.Saved = true;
                    Current.Stored = true;
                    Current.Working = true;
                    saveAsToolStripMenuItem.Enabled = true;
                    closeToolStripMenuItem.Enabled = true;
                    bmp.Dispose();
                }
            }
        }
        #region chua lam
        private void NewToolStripMenuItem_Click(object sender, EventArgs e)
        {
          
        }
        #endregion
        private void SaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Current.Working)
            {
                if (Current.Stored)
                {
                    #region space
                    //Current.DrawSpace.Final.Save(Current.FilePath);
                    #endregion
                    Current.Saved = true;
                    saveToolStripMenuItem.Enabled = false;
                }
                else
                {
                    SaveAsToolStripMenuItem_Click(this, e);
                }
                Current.Parent.Text = Current.FileName;
            }
        }
        private void SaveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Current.Working)
            {
                using (SaveFileDialog sfd = new SaveFileDialog())
                {
                    sfd.FileName = Current.FileName;
                    sfd.Filter = "Bitmap Image (*.BMP)|*.bmp|JPEG Image (*.JPEG)|*.jpeg|PNG Image (*.PNG)|*.png";
                    sfd.FilterIndex = 3;
                    sfd.DefaultExt = "png";
                    if (sfd.ShowDialog() == DialogResult.OK)
                    {
                        #region space
                        //Current.DrawSpace.Final.Save(sfd.FileName);
                        #endregion
                        Current.FilePath = sfd.FileName;
                        Current.Saved = true;
                        saveToolStripMenuItem.Enabled = false;
                        Current.Stored = true;
                    }
                }
            }
        }

        private void CloseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
        }

        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
          
        }

        #endregion

        #region Edit menu

        private void CopyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Current == null) return;
            #region bug
            /*
            if (tools.Select.Selected)
            {
                Bitmap bmp;
                bmp = Current.LayerContainer.Current.Layer.Image.Clone(tools.Select.FixedRect, Current.BmpPixelFormat);
                Clipboard.SetImage(bmp);
            }*/
            #endregion
        }



        private void CutToolStripMenuItem_Click(object sender, EventArgs e)
        {
          
        }

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

        private void PasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
           
        }

        private void UndoToolStripMenuItem_Click(object sender, EventArgs e)
        {
           
        }

        #endregion

        #region Tool menu

        private void ToolsToolStripMenuItem_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            ToolStripItem item = e.ClickedItem;
            switch (item.Text)
            {
                case "Transform":
                    TransformStripButton_Click(transformStripButton, null);
                    break;
                case "Select":
                    SelectStripButton_Click(selectStripButton, null);
                    break;
                case "Drag":
                    DragStripButton_Click(dragStripButton, null);
                    break;
                case "Pen":
                    PenStripButton_Click(penStripButton, null);
                    break;
                case "Eraser":
                    EraserStripButton_Click(eraserStripButton, null);
                    break;
                case "Color Picker":
                    PickerStripButton_Click(pickerStripButton, null);
                    break;
                case "Shape":
                    ShapeStripButton_Click(shapeStripButton, null);
                    break;
                case "Line":
                    LineStripButton_Click(lineStripButton, null);
                    break;
                case "Bucket":
                    BucketStripButton_Click(bucketStripButton, null);
                    break;
            }
        }

        #endregion

        #region View menu
        private void ZoomInToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ZoomInBtn_Click(zoomInBtn, null);
        }

        private void ZoomOutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ZoomOutBtn_Click(zoomInBtn, null);
        }

        private void CenterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CenterBtn_Click(centerBtn, null);
        }

        #endregion

        #region Layer menu

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
        private void RenameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Current != null)
                RenameLStripButton_Click(sender, e);
        }

        private void ClearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Current != null)
                ClearLStripButton_Click(sender, e);
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
        }

        #endregion

        #region Color menu

        private void ColorMenuStripEnable(bool enable)
        {
            foreach (ToolStripMenuItem item in colorToolStripMenuItem.DropDownItems)
            {
                item.Enabled = enable;
            }
        }
        private void BrightnessAndContrastToolStripMenuItem_Click(object sender, EventArgs e)
        {
           
        }

        private void HueAndSaturationToolStripMenuItem_Click(object sender, EventArgs e)
        {
           
        }

        private void InvertToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
        }

        private void GrayscaleToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void ThresholdToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
        }

        private void ColorBalanceToolStripMenuItem_Click(object sender, EventArgs e)
        {
          
        }

        #endregion

        #region Filter menu

        private void FilterMenuStripEnable(bool enable)
        {
            foreach (ToolStripMenuItem item in filterToolStripMenuItem.DropDownItems)
            {
                item.Enabled = enable;
            }
        }

        private void NoiseToolStripMenuItem_Click(object sender, EventArgs e)
        {
           
        }

        private void PixelateToolStripMenuItem_Click(object sender, EventArgs e)
        {
           
        }

        private void BlurToolStripMenuItem_Click(object sender, EventArgs e)
        {
           
        }

        #endregion

        #region Help menu
        private void AboutPhotoEditorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Empty", "Notify");
        }

        #endregion

        #endregion

        #region WorkSpace

        private void AddWorkTab(Bitmap bmp, Color color)
        {
           
        }

        private void WorkSpaceTabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
           
        }

        #endregion

        #region DrawSpace

        private void DrawSpaceInit()
        {
            
        }

        public void DSProcessUpdate()
        {
           
        }

        public void DSUpdate()
        {
            
        }

        private void DS_MouseDown(object sender, MouseEventArgs e)
        {
            
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
           
        }

        #endregion

        #region LeftPanel

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

        private int redVal = 0;
        private int blueVal = 0;
        private int greenVal = 0;

        private void MainColorPic_BackColorChanged(object sender, EventArgs e)
        {
           
        }

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

        private void ValCheck(ref int n)
        {
            if (n > 255) n = 255;
            if (n < 0) n = 0;
        }

        private void BarVal(ref int val, ref PictureBox bar, ref MouseEventArgs e)
        {
            val = (int)(((double)e.Location.X / bar.Width) * 255);
            ValCheck(ref val);
            mainColorPic.BackColor = Color.FromArgb(redVal, greenVal, blueVal);
        }

        private void RedBar_MouseMoveOrDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                BarVal(ref redVal, ref redBar, ref e);
            }
        }

        private void GreenBar_MouseMoveOrDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                BarVal(ref greenVal, ref greenBar, ref e);
            }
        }

        private void BlueBar_MouseMoveOrDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                BarVal(ref blueVal, ref blueBar, ref e);
            }
        }

        #endregion

        #region ToolPanel

        private void TransformStripButton_Click(object sender, EventArgs e)
        {

        }

        private void SelectStripButton_Click(object sender, EventArgs e)
        {
          
        }

        private void DragStripButton_Click(object sender, EventArgs e)
        {
           
        }

        private void PenStripButton_Click(object sender, EventArgs e)
        {
           
        }

        private void EraserStripButton_Click(object sender, EventArgs e)
        {
           
        }

        private void PickerStripButton_Click(object sender, EventArgs e)
        {
            
        }

        private void ShapeStripButton_Click(object sender, EventArgs e)
        {
            
        }

        private void LineStripButton_Click(object sender, EventArgs e)
        {
            
        }

        private void BucketStripButton_Click(object sender, EventArgs e)
        {
            
        }

        private void UncheckAll()
        {
            
        }

        private void ChangeTool()
        {
            
        }

        #endregion

        #endregion

        #region Layer

        private void LayerContainerInit()
        {
           
        }

        private void LayerMenuStripEnable(bool enable)
        {
            
        }

        private void NewLStripButton_Click(object sender, EventArgs e)
        {
           
        }

        private void DeleteLStripButton_Click(object sender, EventArgs e)
        {

        }
        private void RenameLStripButton_Click(object sender, EventArgs e)
        {
            
        }

        private void ClearLStripButton_Click(object sender, EventArgs e)
        {
           
        }

        private void DownLStripButton_Click(object sender, EventArgs e)
        {
            
        }

        private void UpLStripButton_Click(object sender, EventArgs e)
        {
            
        }

        public void LayerButtonCheck()
        {
           

        }

        private void MergeLStripButton_Click(object sender, EventArgs e)
        {
          
        }

        private void DuplicateLStripButton_Click(object sender, EventArgs e)
        {
           
        }

        public float opacityVal;
        private void OpacityBar_MouseMoveOrDown(object sender, MouseEventArgs e)
        {
            
        }

        public void OpacityBarUpdate()
        {
           
        }

        private void OpacityBar_MouseUp(object sender, MouseEventArgs e)
        {
            
        }

        private void BlendModeBox_SelectedIndexChanged(object sender, EventArgs e)
        {
           
        }

        bool blendboxupdate = false;
       

        #endregion

        #region BottomPanel

        private void ViewMenuStripEnable(bool enable)
        {
            
        }

        private void ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
           
        }

        private void ZoomOutBtn_Click(object sender, EventArgs e)
        {
            
        }

        private void ZoomInBtn_Click(object sender, EventArgs e)
        {
           
        }

        private void CenterBtn_Click(object sender, EventArgs e)
        {
           
        }


        #endregion

    }
}