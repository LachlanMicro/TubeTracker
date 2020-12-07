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
            this.btnFileBrowse = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.txtImportInputFilename = new System.Windows.Forms.Label();
            this.lbl_inputFile = new System.Windows.Forms.Label();
            this.btn_connect = new System.Windows.Forms.Button();
            this.lbl_BS = new System.Windows.Forms.Label();
            this.lbl_TS = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnFileBrowse
            // 
            this.btnFileBrowse.Location = new System.Drawing.Point(297, 141);
            this.btnFileBrowse.Name = "btnFileBrowse";
            this.btnFileBrowse.Size = new System.Drawing.Size(152, 61);
            this.btnFileBrowse.TabIndex = 0;
            this.btnFileBrowse.Text = "Load File";
            this.btnFileBrowse.UseVisualStyleBackColor = true;
            this.btnFileBrowse.Click += new System.EventHandler(this.btnFileBrowse_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(297, 340);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(152, 61);
            this.button2.TabIndex = 1;
            this.button2.Text = "Start Run";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_ClickAsync);
            // 
            // txtImportInputFilename
            // 
            this.txtImportInputFilename.AutoSize = true;
            this.txtImportInputFilename.Location = new System.Drawing.Point(145, 31);
            this.txtImportInputFilename.Name = "txtImportInputFilename";
            this.txtImportInputFilename.Size = new System.Drawing.Size(0, 13);
            this.txtImportInputFilename.TabIndex = 2;
            // 
            // lbl_inputFile
            // 
            this.lbl_inputFile.AutoSize = true;
            this.lbl_inputFile.Location = new System.Drawing.Point(63, 31);
            this.lbl_inputFile.Name = "lbl_inputFile";
            this.lbl_inputFile.Size = new System.Drawing.Size(53, 13);
            this.lbl_inputFile.TabIndex = 3;
            this.lbl_inputFile.Text = "Input File:";
            // 
            // btn_connect
            // 
            this.btn_connect.Location = new System.Drawing.Point(297, 226);
            this.btn_connect.Name = "btn_connect";
            this.btn_connect.Size = new System.Drawing.Size(152, 61);
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
            this.lbl_BS.Location = new System.Drawing.Point(455, 226);
            this.lbl_BS.Name = "lbl_BS";
            this.lbl_BS.Size = new System.Drawing.Size(196, 22);
            this.lbl_BS.TabIndex = 5;
            this.lbl_BS.Text = "BARCODE OFFLINE";
            // 
            // lbl_TS
            // 
            this.lbl_TS.AutoSize = true;
            this.lbl_TS.Font = new System.Drawing.Font("Lucida Sans", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_TS.ForeColor = System.Drawing.Color.Red;
            this.lbl_TS.Location = new System.Drawing.Point(455, 265);
            this.lbl_TS.Name = "lbl_TS";
            this.lbl_TS.Size = new System.Drawing.Size(234, 22);
            this.lbl_TS.TabIndex = 6;
            this.lbl_TS.Text = "INSTRUMENT OFFLINE";
            // 
            // Startup
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.lbl_TS);
            this.Controls.Add(this.lbl_BS);
            this.Controls.Add(this.btn_connect);
            this.Controls.Add(this.lbl_inputFile);
            this.Controls.Add(this.txtImportInputFilename);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.btnFileBrowse);
            this.Name = "Startup";
            this.Text = "Form2";
            this.Load += new System.EventHandler(this.Startup_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnFileBrowse;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label txtImportInputFilename;
        private System.Windows.Forms.Label lbl_inputFile;
        private System.Windows.Forms.Button btn_connect;
        private System.Windows.Forms.Label lbl_BS;
        private System.Windows.Forms.Label lbl_TS;
    }
}