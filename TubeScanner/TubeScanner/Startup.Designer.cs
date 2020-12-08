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
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // btnFileBrowse
            // 
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
            this.btn_connect.Location = new System.Drawing.Point(12, 213);
            this.btn_connect.Name = "btn_connect";
            this.btn_connect.Size = new System.Drawing.Size(160, 52);
            this.btn_connect.TabIndex = 4;
            this.btn_connect.Text = "Reconnect Devices";
            this.btn_connect.UseVisualStyleBackColor = true;
            this.btn_connect.Click += new System.EventHandler(this.btn_connect_Click);
            // 
            // lbl_BS
            // 
            this.lbl_BS.AutoSize = true;
            this.lbl_BS.Font = new System.Drawing.Font("Lucida Sans", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_BS.ForeColor = System.Drawing.Color.Red;
            this.lbl_BS.Location = new System.Drawing.Point(519, 9);
            this.lbl_BS.Name = "lbl_BS";
            this.lbl_BS.Size = new System.Drawing.Size(105, 22);
            this.lbl_BS.TabIndex = 5;
            this.lbl_BS.Text = "BARCODE";
            // 
            // lbl_TS
            // 
            this.lbl_TS.AutoSize = true;
            this.lbl_TS.Font = new System.Drawing.Font("Lucida Sans", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_TS.ForeColor = System.Drawing.Color.Red;
            this.lbl_TS.Location = new System.Drawing.Point(645, 9);
            this.lbl_TS.Name = "lbl_TS";
            this.lbl_TS.Size = new System.Drawing.Size(143, 22);
            this.lbl_TS.TabIndex = 6;
            this.lbl_TS.Text = "INSTRUMENT";
            // 
            // lbl_file
            // 
            this.lbl_file.AutoSize = true;
            this.lbl_file.Font = new System.Drawing.Font("Lucida Sans", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_file.ForeColor = System.Drawing.Color.Red;
            this.lbl_file.Location = new System.Drawing.Point(12, 9);
            this.lbl_file.Name = "lbl_file";
            this.lbl_file.Size = new System.Drawing.Size(190, 22);
            this.lbl_file.TabIndex = 7;
            this.lbl_file.Text = "FILE NOT LOADED";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(308, 102);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(220, 224);
            this.pictureBox1.TabIndex = 8;
            this.pictureBox1.TabStop = false;
            // 
            // Startup
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.lbl_file);
            this.Controls.Add(this.lbl_TS);
            this.Controls.Add(this.lbl_BS);
            this.Controls.Add(this.btn_connect);
            this.Controls.Add(this.btn_runStart);
            this.Controls.Add(this.btnFileBrowse);
            this.Name = "Startup";
            this.Text = "Form2";
            this.Load += new System.EventHandler(this.Startup_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
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
    }
}