using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Basic_Photo_Editor
{
    public partial class FormMain : Form
    {
        private workSpace Current;
        public FormMain()
        {
            InitializeComponent();
        }

        private void fToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
        #region Cần thêm hàm add workTab và DSUpdate
        private void openToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Image Files(*.BMP;*.JPG;*.PNG)|*.bmp;*.jpg;*.png|All files (*.*)|*.*";
                ofd.FilterIndex = 2;
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    Bitmap bmp = new Bitmap(ofd.FileName);
                    //AddWorkTab();
                    Current.FilePath = ofd.FileName;
                    Current.Parent.Text = Current.FileName;
                    //DSUpdate();
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

        #region Save Button: code ham save
        private void saveToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (Current.Working)
            {
                if (Current.Stored)
                {
                    //Current.DrawSpace.Final.Save(Current.FilePath);
                    Current.Saved = true;
                    saveToolStripMenuItem.Enabled = false;
                }
                else
                {
                    //SaveAsToolStripMenuItem_Click(this, e);
                }
                Current.Parent.Text = Current.FileName;
            }
        }

        #endregion

        #region  NewButton: Can Tao form New file
        private void newToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            using (Forms.NewFileForm nff = new Forms.NewFileForm())
            {
                nff.ColorFore = mainColorPic.BackColor;
                nff.ColorBack = subColorPic.BackColor;

                if (nff.ShowDialog() == DialogResult.OK)
                {
                    Bitmap bmp = new Bitmap(nff.ImageSize.Width, nff.ImageSize.Height);
                    //AddWorkTab(bmp, nff.BGColor);
                    Current.FileName = nff.FileName;
                    Current.Parent.Text = Current.FileName;
                    //DSUpdate();
                    Current.Saved = true;
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
