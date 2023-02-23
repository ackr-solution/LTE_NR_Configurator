namespace Configurator
{
    partial class WaitForm_Loading
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
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.pPanel_Loading = new DevExpress.XtraWaitForm.ProgressPanel();
            this.tlPanel_Loading = new System.Windows.Forms.TableLayoutPanel();
            this.tlPanel_Loading.SuspendLayout();
            this.SuspendLayout();
            // 
            // pPanel_Loading
            // 
            this.pPanel_Loading.Appearance.BackColor = System.Drawing.Color.Transparent;
            this.pPanel_Loading.Appearance.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pPanel_Loading.Appearance.Options.UseBackColor = true;
            this.pPanel_Loading.Appearance.Options.UseFont = true;
            this.pPanel_Loading.AppearanceCaption.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.pPanel_Loading.AppearanceCaption.Options.UseFont = true;
            this.pPanel_Loading.AppearanceDescription.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.pPanel_Loading.AppearanceDescription.Options.UseFont = true;
            this.pPanel_Loading.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pPanel_Loading.ImageHorzOffset = 20;
            this.pPanel_Loading.Location = new System.Drawing.Point(0, 16);
            this.pPanel_Loading.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.pPanel_Loading.Name = "pPanel_Loading";
            this.pPanel_Loading.Size = new System.Drawing.Size(287, 35);
            this.pPanel_Loading.TabIndex = 0;
            // 
            // tlPanel_Loading
            // 
            this.tlPanel_Loading.AutoSize = true;
            this.tlPanel_Loading.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tlPanel_Loading.BackColor = System.Drawing.Color.Transparent;
            this.tlPanel_Loading.ColumnCount = 1;
            this.tlPanel_Loading.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlPanel_Loading.Controls.Add(this.pPanel_Loading, 0, 0);
            this.tlPanel_Loading.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlPanel_Loading.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tlPanel_Loading.Location = new System.Drawing.Point(0, 0);
            this.tlPanel_Loading.Name = "tlPanel_Loading";
            this.tlPanel_Loading.Padding = new System.Windows.Forms.Padding(0, 13, 0, 13);
            this.tlPanel_Loading.RowCount = 1;
            this.tlPanel_Loading.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlPanel_Loading.Size = new System.Drawing.Size(287, 67);
            this.tlPanel_Loading.TabIndex = 1;
            // 
            // WaitForm_Loading
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(287, 67);
            this.Controls.Add(this.tlPanel_Loading);
            this.DoubleBuffered = true;
            this.Name = "WaitForm_Loading";
            this.ShowOnTopMode = DevExpress.XtraWaitForm.ShowFormOnTopMode.AboveAll;
            this.tlPanel_Loading.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraWaitForm.ProgressPanel pPanel_Loading;
        private System.Windows.Forms.TableLayoutPanel tlPanel_Loading;
    }
}
