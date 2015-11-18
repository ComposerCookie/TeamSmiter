namespace TeamSmiter
{
    partial class TeamGodEdit
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
            this.components = new System.ComponentModel.Container();
            this.grpChampBox = new System.Windows.Forms.GroupBox();
            this.picChampBox = new System.Windows.Forms.PictureBox();
            this.vScrollBox = new System.Windows.Forms.VScrollBar();
            this.hScrollBox = new System.Windows.Forms.HScrollBar();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.lstTeam2 = new System.Windows.Forms.ListBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.lstTeam1 = new System.Windows.Forms.ListBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.chkMultiline = new System.Windows.Forms.CheckBox();
            this.Updator = new System.Windows.Forms.Timer(this.components);
            this.grpChampBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picChampBox)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpChampBox
            // 
            this.grpChampBox.Controls.Add(this.picChampBox);
            this.grpChampBox.Controls.Add(this.vScrollBox);
            this.grpChampBox.Controls.Add(this.hScrollBox);
            this.grpChampBox.Location = new System.Drawing.Point(3, 1);
            this.grpChampBox.Margin = new System.Windows.Forms.Padding(4);
            this.grpChampBox.Name = "grpChampBox";
            this.grpChampBox.Padding = new System.Windows.Forms.Padding(4);
            this.grpChampBox.Size = new System.Drawing.Size(1181, 408);
            this.grpChampBox.TabIndex = 0;
            this.grpChampBox.TabStop = false;
            this.grpChampBox.Text = "God Lists";
            // 
            // picChampBox
            // 
            this.picChampBox.BackColor = System.Drawing.Color.Black;
            this.picChampBox.Location = new System.Drawing.Point(8, 23);
            this.picChampBox.Margin = new System.Windows.Forms.Padding(4);
            this.picChampBox.Name = "picChampBox";
            this.picChampBox.Size = new System.Drawing.Size(1152, 354);
            this.picChampBox.TabIndex = 8;
            this.picChampBox.TabStop = false;
            // 
            // vScrollBox
            // 
            this.vScrollBox.Location = new System.Drawing.Point(1160, 23);
            this.vScrollBox.Name = "vScrollBox";
            this.vScrollBox.Size = new System.Drawing.Size(18, 354);
            this.vScrollBox.TabIndex = 7;
            this.vScrollBox.Scroll += new System.Windows.Forms.ScrollEventHandler(this.vScrollBox_Scroll);
            // 
            // hScrollBox
            // 
            this.hScrollBox.Location = new System.Drawing.Point(8, 377);
            this.hScrollBox.Name = "hScrollBox";
            this.hScrollBox.Size = new System.Drawing.Size(1152, 18);
            this.hScrollBox.TabIndex = 6;
            this.hScrollBox.Scroll += new System.Windows.Forms.ScrollEventHandler(this.hScrollBox_Scroll);
            // 
            // groupBox1
            // 
            this.groupBox1.Location = new System.Drawing.Point(3, 417);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox1.Size = new System.Drawing.Size(367, 148);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "God Info";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.groupBox4);
            this.groupBox2.Controls.Add(this.groupBox3);
            this.groupBox2.Location = new System.Drawing.Point(377, 417);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox2.Size = new System.Drawing.Size(807, 224);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Teams:";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.lstTeam2);
            this.groupBox4.Location = new System.Drawing.Point(406, 22);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(394, 193);
            this.groupBox4.TabIndex = 1;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Red";
            // 
            // lstTeam2
            // 
            this.lstTeam2.FormattingEnabled = true;
            this.lstTeam2.ItemHeight = 16;
            this.lstTeam2.Location = new System.Drawing.Point(6, 21);
            this.lstTeam2.Name = "lstTeam2";
            this.lstTeam2.Size = new System.Drawing.Size(381, 164);
            this.lstTeam2.TabIndex = 1;
            this.lstTeam2.Click += new System.EventHandler(this.lstTeam2_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.lstTeam1);
            this.groupBox3.Location = new System.Drawing.Point(7, 22);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(393, 193);
            this.groupBox3.TabIndex = 0;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Blue";
            // 
            // lstTeam1
            // 
            this.lstTeam1.FormattingEnabled = true;
            this.lstTeam1.ItemHeight = 16;
            this.lstTeam1.Location = new System.Drawing.Point(6, 21);
            this.lstTeam1.Name = "lstTeam1";
            this.lstTeam1.Size = new System.Drawing.Size(381, 164);
            this.lstTeam1.TabIndex = 0;
            this.lstTeam1.Click += new System.EventHandler(this.lstTeam1_Click);
            // 
            // btnSave
            // 
            this.btnSave.DialogResult = System.Windows.Forms.DialogResult.Yes;
            this.btnSave.Location = new System.Drawing.Point(114, 608);
            this.btnSave.Margin = new System.Windows.Forms.Padding(4);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(103, 33);
            this.btnSave.TabIndex = 3;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.No;
            this.btnCancel.Location = new System.Drawing.Point(3, 608);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(4);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(103, 33);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // chkMultiline
            // 
            this.chkMultiline.AutoSize = true;
            this.chkMultiline.Location = new System.Drawing.Point(3, 579);
            this.chkMultiline.Margin = new System.Windows.Forms.Padding(4);
            this.chkMultiline.Name = "chkMultiline";
            this.chkMultiline.Size = new System.Drawing.Size(180, 21);
            this.chkMultiline.TabIndex = 5;
            this.chkMultiline.Text = "Show Gods On Multiline";
            this.chkMultiline.UseVisualStyleBackColor = true;
            // 
            // Updator
            // 
            this.Updator.Interval = 16;
            this.Updator.Tick += new System.EventHandler(this.Updator_Tick);
            // 
            // TeamGodEdit
            // 
            this.AcceptButton = this.btnSave;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(1189, 644);
            this.Controls.Add(this.chkMultiline);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.grpChampBox);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "TeamGodEdit";
            this.Text = "Team\'s God Edit";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.TeamGodEdit_FormClosing);
            this.Load += new System.EventHandler(this.TeamGodEdit_Load);
            this.grpChampBox.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picChampBox)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox grpChampBox;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.CheckBox chkMultiline;
        private System.Windows.Forms.PictureBox picChampBox;
        private System.Windows.Forms.VScrollBar vScrollBox;
        private System.Windows.Forms.HScrollBar hScrollBox;
        private System.Windows.Forms.Timer Updator;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.ListBox lstTeam2;
        private System.Windows.Forms.ListBox lstTeam1;
    }
}