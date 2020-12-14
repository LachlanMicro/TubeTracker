namespace TubeScanner
{
    partial class Startup
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Startup));
            this.btnFileBrowse = new System.Windows.Forms.Button();
            this.btn_runStart = new System.Windows.Forms.Button();
            this.btn_connect = new System.Windows.Forms.Button();
            this.lbl_BS = new System.Windows.Forms.Label();
            this.lbl_TS = new System.Windows.Forms.Label();
            this.lbl_file = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.lbl_title = new System.Windows.Forms.Label();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.lbl_loading = new System.Windows.Forms.Label();
            this.btn_Config = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.SuspendLayout();
            // 
            // btnFileBrowse
            // 
            this.btnFileBrowse.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnFileBrowse.Location = new System.Drawing.Point(12, 299);
            this.btnFileBrowse.Name = "btnFileBrowse";
            this.btnFileBrowse.Size = new System.Drawing.Size(160, 52);
            this.btnFileBrowse.TabIndex = 0;
            this.btnFileBrowse.Text = "Load File";
            this.btnFileBrowse.UseVisualStyleBackColor = true;
            this.btnFileBrowse.Click += new System.EventHandler(this.btnFileBrowse_Click);
            // 
            // btn_runStart
            // 
            this.btn_runStart.Enabled = false;
            this.btn_runStart.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_runStart.Location = new System.Drawing.Point(12, 386);
            this.btn_runStart.Name = "btn_runStart";
            this.btn_runStart.Size = new System.Drawing.Size(160, 52);
            this.btn_runStart.TabIndex = 1;
            this.btn_runStart.Text = "Start Run";
            this.btn_runStart.UseVisualStyleBackColor = true;
            this.btn_runStart.Click += new System.EventHandler(this.btn_runStart_ClickAsync);
            // 
            // btn_connect
            // 
            this.btn_connect.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_connect.Location = new System.Drawing.Point(12, 213);
            this.btn_connect.Name = "btn_connect";
            this.btn_connect.Size = new System.Drawing.Size(160, 52);
            this.btn_connect.TabIndex = 4;
            this.btn_connect.Text = "Connect";
            this.btn_connect.UseVisualStyleBackColor = true;
            this.btn_connect.Click += new System.EventHandler(this.btn_connect_Click);
            // 
            // lbl_BS
            // 
            this.lbl_BS.Font = new System.Drawing.Font("Lucida Sans", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_BS.ForeColor = System.Drawing.Color.Red;
            this.lbl_BS.Location = new System.Drawing.Point(466, 6);
            this.lbl_BS.Name = "lbl_BS";
            this.lbl_BS.Size = new System.Drawing.Size(173, 39);
            this.lbl_BS.TabIndex = 5;
            this.lbl_BS.Text = "BARCODE SCANNER NOT CONNECTED";
            this.lbl_BS.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbl_TS
            // 
            this.lbl_TS.Font = new System.Drawing.Font("Lucida Sans", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_TS.ForeColor = System.Drawing.Color.Red;
            this.lbl_TS.Location = new System.Drawing.Point(645, 6);
            this.lbl_TS.Name = "lbl_TS";
            this.lbl_TS.Size = new System.Drawing.Size(143, 36);
            this.lbl_TS.TabIndex = 6;
            this.lbl_TS.Text = "TUBE TRACKER NOT CONNECTED";
            this.lbl_TS.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbl_file
            // 
            this.lbl_file.Font = new System.Drawing.Font("Lucida Sans", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_file.ForeColor = System.Drawing.Color.Red;
            this.lbl_file.Location = new System.Drawing.Point(292, 16);
            this.lbl_file.Name = "lbl_file";
            this.lbl_file.Size = new System.Drawing.Size(184, 18);
            this.lbl_file.TabIndex = 7;
            this.lbl_file.Text = "NO INPUT FILE";
            this.lbl_file.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(264, 86);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(408, 277);
            this.pictureBox1.TabIndex = 8;
            this.pictureBox1.TabStop = false;
            // 
            // lbl_title
            // 
            this.lbl_title.AutoSize = true;
            this.lbl_title.Font = new System.Drawing.Font("Calibri", 32.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_title.Location = new System.Drawing.Point(53, -3);
            this.lbl_title.Name = "lbl_title";
            this.lbl_title.Size = new System.Drawing.Size(233, 53);
            this.lbl_title.TabIndex = 9;
            this.lbl_title.Text = "BSD Tracker";
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox2.Image")));
            this.pictureBox2.InitialImage = ((System.Drawing.Image)(resources.GetObject("pictureBox2.InitialImage")));
            this.pictureBox2.Location = new System.Drawing.Point(2, 6);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(58, 42);
            this.pictureBox2.TabIndex = 10;
            this.pictureBox2.TabStop = false;
            // 
            // lbl_loading
            // 
            this.lbl_loading.AutoSize = true;
            this.lbl_loading.Font = new System.Drawing.Font("Calibri", 26.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_loading.Location = new System.Drawing.Point(394, 366);
            this.lbl_loading.Name = "lbl_loading";
            this.lbl_loading.Size = new System.Drawing.Size(0, 42);
            this.lbl_loading.TabIndex = 11;
            // 
            // btn_Config
            // 
            this.btn_Config.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_Config.Location = new System.Drawing.Point(628, 386);
            this.btn_Config.Name = "btn_Config";
            this.btn_Config.Size = new System.Drawing.Size(160, 52);
            this.btn_Config.TabIndex = 12;
            this.btn_Config.Text = "Config";
            this.btn_Config.UseVisualStyleBackColor = true;
            this.btn_Config.Click += new System.EventHandler(this.btn_Config_Click);
            // 
            // Startup
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.btn_Config);
            this.Controls.Add(this.lbl_loading);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.lbl_title);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.lbl_file);
            this.Controls.Add(this.lbl_TS);
            this.Controls.Add(this.lbl_BS);
            this.Controls.Add(this.btn_connect);
            this.Controls.Add(this.btn_runStart);
            this.Controls.Add(this.btnFileBrowse);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Startup";
            this.Text = "BSD Tracker";
            this.Load += new System.EventHandler(this.Startup_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnFileBrowse;
        private System.Windows.Forms.Button btn_runStart;
        private System.Windows.Forms.Button btn_connect;
        private System.Windows.Forms.Label lbl_BS;
        private System.Windows.Forms.Label lbl_TS;
        private System.Windows.Forms.Label lbl_file;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label lbl_title;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.Label lbl_loading;
        private System.Windows.Forms.Button btn_Config;
    }
}