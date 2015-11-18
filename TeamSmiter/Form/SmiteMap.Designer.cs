namespace TeamSmiter
{
    partial class SmiteMap
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.hScrollMap = new System.Windows.Forms.HScrollBar();
            this.Updator = new System.Windows.Forms.Timer(this.components);
            this.vScrollMap = new System.Windows.Forms.VScrollBar();
            this.picMap = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.picMap)).BeginInit();
            this.SuspendLayout();
            // 
            // hScrollMap
            // 
            this.hScrollMap.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.hScrollMap.Location = new System.Drawing.Point(0, 876);
            this.hScrollMap.Name = "hScrollMap";
            this.hScrollMap.Size = new System.Drawing.Size(937, 22);
            this.hScrollMap.TabIndex = 1;
            this.hScrollMap.Scroll += new System.Windows.Forms.ScrollEventHandler(this.hScrollMap_Scroll);
            // 
            // Updator
            // 
            this.Updator.Interval = 16;
            this.Updator.Tick += new System.EventHandler(this.Updator_Tick);
            // 
            // vScrollMap
            // 
            this.vScrollMap.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.vScrollMap.Location = new System.Drawing.Point(941, 0);
            this.vScrollMap.Name = "vScrollMap";
            this.vScrollMap.Size = new System.Drawing.Size(22, 876);
            this.vScrollMap.TabIndex = 2;
            this.vScrollMap.Scroll += new System.Windows.Forms.ScrollEventHandler(this.vScrollMap_Scroll);
            // 
            // picMap
            // 
            this.picMap.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.picMap.BackColor = System.Drawing.Color.Black;
            this.picMap.Location = new System.Drawing.Point(0, 0);
            this.picMap.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.picMap.Name = "picMap";
            this.picMap.Size = new System.Drawing.Size(940, 876);
            this.picMap.TabIndex = 3;
            this.picMap.TabStop = false;
            // 
            // SmiteMap
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.picMap);
            this.Controls.Add(this.vScrollMap);
            this.Controls.Add(this.hScrollMap);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "SmiteMap";
            this.Size = new System.Drawing.Size(963, 899);
            this.Load += new System.EventHandler(this.SmiteMap_Load);
            ((System.ComponentModel.ISupportInitialize)(this.picMap)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.HScrollBar hScrollMap;
        private System.Windows.Forms.Timer Updator;
        private System.Windows.Forms.VScrollBar vScrollMap;
        private System.Windows.Forms.PictureBox picMap;
    }
}
