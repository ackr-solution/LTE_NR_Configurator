
namespace RecoveryTool
{
    partial class UcRecovery
    {
        /// <summary> 
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 구성 요소 디자이너에서 생성한 코드

        /// <summary> 
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            this.btn_Recover_S = new System.Windows.Forms.Button();
            this.btn_Recover_H = new System.Windows.Forms.Button();
            this.check_Reboot_8821 = new System.Windows.Forms.CheckBox();
            this.text_MeasSw = new System.Windows.Forms.TextBox();
            this.lc_Main = new DevExpress.XtraLayout.LayoutControl();
            this.lcGroup_Main = new DevExpress.XtraLayout.LayoutControlGroup();
            this.lcItem_MeasSw = new DevExpress.XtraLayout.LayoutControlItem();
            this.btn_Recover_Sitem = new DevExpress.XtraLayout.LayoutControlItem();
            this.btn_Recover_Hitem = new DevExpress.XtraLayout.LayoutControlItem();
            this.lcItem_Reboot_8821 = new DevExpress.XtraLayout.LayoutControlItem();
            this.label_Status = new DevExpress.XtraLayout.SimpleLabelItem();
            this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.emptySpaceItem2 = new DevExpress.XtraLayout.EmptySpaceItem();
            ((System.ComponentModel.ISupportInitialize)(this.lc_Main)).BeginInit();
            this.lc_Main.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.lcGroup_Main)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcItem_MeasSw)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btn_Recover_Sitem)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btn_Recover_Hitem)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcItem_Reboot_8821)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.label_Status)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).BeginInit();
            this.SuspendLayout();
            // 
            // btn_Recover_S
            // 
            this.btn_Recover_S.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_Recover_S.ForeColor = System.Drawing.Color.Black;
            this.btn_Recover_S.Location = new System.Drawing.Point(3, 3);
            this.btn_Recover_S.Name = "btn_Recover_S";
            this.btn_Recover_S.Size = new System.Drawing.Size(356, 96);
            this.btn_Recover_S.TabIndex = 0;
            this.btn_Recover_S.Text = "Soft Recover";
            this.btn_Recover_S.UseVisualStyleBackColor = true;
            this.btn_Recover_S.Click += new System.EventHandler(this.btn_Recover_S_Click);
            // 
            // btn_Recover_H
            // 
            this.btn_Recover_H.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold);
            this.btn_Recover_H.ForeColor = System.Drawing.Color.Black;
            this.btn_Recover_H.Location = new System.Drawing.Point(3, 103);
            this.btn_Recover_H.Name = "btn_Recover_H";
            this.btn_Recover_H.Size = new System.Drawing.Size(356, 96);
            this.btn_Recover_H.TabIndex = 1;
            this.btn_Recover_H.Text = "Hard Recover";
            this.btn_Recover_H.UseVisualStyleBackColor = true;
            this.btn_Recover_H.Click += new System.EventHandler(this.btn_Recover_H_Click);
            // 
            // check_Reboot_8821
            // 
            this.check_Reboot_8821.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.check_Reboot_8821.Location = new System.Drawing.Point(3, 203);
            this.check_Reboot_8821.Name = "check_Reboot_8821";
            this.check_Reboot_8821.Padding = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.check_Reboot_8821.Size = new System.Drawing.Size(356, 20);
            this.check_Reboot_8821.TabIndex = 2;
            this.check_Reboot_8821.Text = "Reboot MT8821C";
            this.check_Reboot_8821.UseVisualStyleBackColor = true;
            // 
            // text_MeasSw
            // 
            this.text_MeasSw.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.text_MeasSw.Location = new System.Drawing.Point(153, 279);
            this.text_MeasSw.Name = "text_MeasSw";
            this.text_MeasSw.Size = new System.Drawing.Size(46, 20);
            this.text_MeasSw.TabIndex = 4;
            this.text_MeasSw.Text = "120";
            this.text_MeasSw.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // lc_Main
            // 
            this.lc_Main.Controls.Add(this.text_MeasSw);
            this.lc_Main.Controls.Add(this.btn_Recover_S);
            this.lc_Main.Controls.Add(this.btn_Recover_H);
            this.lc_Main.Controls.Add(this.check_Reboot_8821);
            this.lc_Main.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lc_Main.Location = new System.Drawing.Point(0, 0);
            this.lc_Main.Name = "lc_Main";
            this.lc_Main.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(571, 0, 650, 601);
            this.lc_Main.Root = this.lcGroup_Main;
            this.lc_Main.Size = new System.Drawing.Size(362, 302);
            this.lc_Main.TabIndex = 2;
            // 
            // lcGroup_Main
            // 
            this.lcGroup_Main.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.lcItem_MeasSw,
            this.btn_Recover_Sitem,
            this.btn_Recover_Hitem,
            this.lcItem_Reboot_8821,
            this.label_Status,
            this.emptySpaceItem1,
            this.emptySpaceItem2});
            this.lcGroup_Main.Name = "lcGroup_Main";
            this.lcGroup_Main.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.lcGroup_Main.Size = new System.Drawing.Size(362, 302);
            this.lcGroup_Main.TextVisible = false;
            // 
            // lcItem_MeasSw
            // 
            this.lcItem_MeasSw.Control = this.text_MeasSw;
            this.lcItem_MeasSw.Location = new System.Drawing.Point(0, 276);
            this.lcItem_MeasSw.Name = "lcItem_MeasSw";
            this.lcItem_MeasSw.OptionsTableLayoutItem.RowIndex = 4;
            this.lcItem_MeasSw.Padding = new DevExpress.XtraLayout.Utils.Padding(5, 2, 2, 2);
            this.lcItem_MeasSw.Size = new System.Drawing.Size(200, 24);
            this.lcItem_MeasSw.Text = "MX10A Starting Time (s)";
            this.lcItem_MeasSw.TextSize = new System.Drawing.Size(135, 14);
            // 
            // btn_Recover_Sitem
            // 
            this.btn_Recover_Sitem.Control = this.btn_Recover_S;
            this.btn_Recover_Sitem.Location = new System.Drawing.Point(0, 0);
            this.btn_Recover_Sitem.MaxSize = new System.Drawing.Size(0, 100);
            this.btn_Recover_Sitem.MinSize = new System.Drawing.Size(24, 100);
            this.btn_Recover_Sitem.Name = "btn_Recover_Sitem";
            this.btn_Recover_Sitem.Size = new System.Drawing.Size(360, 100);
            this.btn_Recover_Sitem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.btn_Recover_Sitem.TextSize = new System.Drawing.Size(0, 0);
            this.btn_Recover_Sitem.TextVisible = false;
            // 
            // btn_Recover_Hitem
            // 
            this.btn_Recover_Hitem.Control = this.btn_Recover_H;
            this.btn_Recover_Hitem.Location = new System.Drawing.Point(0, 100);
            this.btn_Recover_Hitem.MaxSize = new System.Drawing.Size(0, 100);
            this.btn_Recover_Hitem.MinSize = new System.Drawing.Size(24, 100);
            this.btn_Recover_Hitem.Name = "btn_Recover_Hitem";
            this.btn_Recover_Hitem.OptionsTableLayoutItem.RowIndex = 1;
            this.btn_Recover_Hitem.Size = new System.Drawing.Size(360, 100);
            this.btn_Recover_Hitem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.btn_Recover_Hitem.TextSize = new System.Drawing.Size(0, 0);
            this.btn_Recover_Hitem.TextVisible = false;
            // 
            // lcItem_Reboot_8821
            // 
            this.lcItem_Reboot_8821.Control = this.check_Reboot_8821;
            this.lcItem_Reboot_8821.Location = new System.Drawing.Point(0, 200);
            this.lcItem_Reboot_8821.Name = "lcItem_Reboot_8821";
            this.lcItem_Reboot_8821.OptionsTableLayoutItem.RowIndex = 2;
            this.lcItem_Reboot_8821.Size = new System.Drawing.Size(360, 24);
            this.lcItem_Reboot_8821.TextSize = new System.Drawing.Size(0, 0);
            this.lcItem_Reboot_8821.TextVisible = false;
            // 
            // label_Status
            // 
            this.label_Status.AllowHotTrack = false;
            this.label_Status.AutoSizeMode = DevExpress.XtraLayout.SimpleLabelAutoSizeMode.None;
            this.label_Status.Location = new System.Drawing.Point(0, 224);
            this.label_Status.Name = "label_Status";
            this.label_Status.Padding = new DevExpress.XtraLayout.Utils.Padding(5, 2, 2, 2);
            this.label_Status.Size = new System.Drawing.Size(360, 40);
            this.label_Status.TextSize = new System.Drawing.Size(135, 14);
            // 
            // emptySpaceItem1
            // 
            this.emptySpaceItem1.AllowHotTrack = false;
            this.emptySpaceItem1.Location = new System.Drawing.Point(0, 264);
            this.emptySpaceItem1.Name = "emptySpaceItem1";
            this.emptySpaceItem1.Size = new System.Drawing.Size(360, 12);
            this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
            // 
            // emptySpaceItem2
            // 
            this.emptySpaceItem2.AllowHotTrack = false;
            this.emptySpaceItem2.Location = new System.Drawing.Point(200, 276);
            this.emptySpaceItem2.MaxSize = new System.Drawing.Size(160, 0);
            this.emptySpaceItem2.MinSize = new System.Drawing.Size(160, 10);
            this.emptySpaceItem2.Name = "emptySpaceItem2";
            this.emptySpaceItem2.Size = new System.Drawing.Size(160, 24);
            this.emptySpaceItem2.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.emptySpaceItem2.TextSize = new System.Drawing.Size(0, 0);
            // 
            // UcRecovery
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lc_Main);
            this.MinimumSize = new System.Drawing.Size(170, 220);
            this.Name = "UcRecovery";
            this.Size = new System.Drawing.Size(362, 302);
            ((System.ComponentModel.ISupportInitialize)(this.lc_Main)).EndInit();
            this.lc_Main.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.lcGroup_Main)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcItem_MeasSw)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btn_Recover_Sitem)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btn_Recover_Hitem)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcItem_Reboot_8821)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.label_Status)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btn_Recover_S;
        private System.Windows.Forms.Button btn_Recover_H;
        private System.Windows.Forms.CheckBox check_Reboot_8821;
        private System.Windows.Forms.TextBox text_MeasSw;
        private DevExpress.XtraLayout.LayoutControl lc_Main;
        private DevExpress.XtraLayout.LayoutControlGroup lcGroup_Main;
        private DevExpress.XtraLayout.LayoutControlItem lcItem_MeasSw;
        private DevExpress.XtraLayout.LayoutControlItem btn_Recover_Sitem;
        private DevExpress.XtraLayout.LayoutControlItem btn_Recover_Hitem;
        private DevExpress.XtraLayout.LayoutControlItem lcItem_Reboot_8821;
        private DevExpress.XtraLayout.SimpleLabelItem label_Status;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem2;
    }
}
