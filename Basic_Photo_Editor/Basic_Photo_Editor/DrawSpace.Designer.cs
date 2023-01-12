using System.Diagnostics;

namespace Basic_Photo_Editor
{
    partial class DrawSpace
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            if (processing != null) processing.Dispose();
            if (g != null) g.Dispose();
            gF.Dispose();
            gFinal.Dispose();
            gTop.Dispose();
            gProcess.Dispose();
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.finalBox = new System.Windows.Forms.PictureBox();
            this.processBox = new System.Windows.Forms.PictureBox();
            this.topBox = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.finalBox)).BeginInit();
            this.finalBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.processBox)).BeginInit();
            this.processBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.topBox)).BeginInit();
            this.SuspendLayout();
            // 
            // finalBox
            // 
            this.finalBox.Controls.Add(this.processBox);
            this.finalBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.finalBox.Location = new System.Drawing.Point(0, 0);
            this.finalBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.finalBox.Name = "finalBox";
            this.finalBox.Size = new System.Drawing.Size(200, 185);
            this.finalBox.TabIndex = 0;
            this.finalBox.TabStop = false;
            // 
            // processBox
            // 
            this.processBox.Controls.Add(this.topBox);
            this.processBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.processBox.Location = new System.Drawing.Point(0, 0);
            this.processBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.processBox.Name = "processBox";
            this.processBox.Size = new System.Drawing.Size(200, 185);
            this.processBox.TabIndex = 1;
            this.processBox.TabStop = false;
            // 
            // topBox
            // 
            this.topBox.BackColor = System.Drawing.Color.Transparent;
            this.topBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.topBox.Location = new System.Drawing.Point(0, 0);
            this.topBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.topBox.Name = "topBox";
            this.topBox.Size = new System.Drawing.Size(200, 185);
            this.topBox.TabIndex = 3;
            this.topBox.TabStop = false;
            this.topBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Mouse_Down);
            this.topBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Mouse_Move);
            this.topBox.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Mouse_Up);
            // 
            // DrawSpace
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.finalBox);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "DrawSpace";
            this.Size = new System.Drawing.Size(200, 185);
            ((System.ComponentModel.ISupportInitialize)(this.finalBox)).EndInit();
            this.finalBox.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.processBox)).EndInit();
            this.processBox.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.topBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox finalBox;
        private System.Windows.Forms.PictureBox processBox;
        private System.Windows.Forms.PictureBox topBox;
    }
}
