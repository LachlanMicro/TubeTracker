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
            this.SuspendLayout();
            // 
            // btnFileBrowse
            // 
            this.btnFileBrowse.Location = new System.Drawing.Point(323, 159);
            this.btnFileBrowse.Name = "btnFileBrowse";
            this.btnFileBrowse.Size = new System.Drawing.Size(96, 35);
            this.btnFileBrowse.TabIndex = 0;
            this.btnFileBrowse.Text = "Load File";
            this.btnFileBrowse.UseVisualStyleBackColor = true;
            this.btnFileBrowse.Click += new System.EventHandler(this.btnFileBrowse_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(323, 252);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(96, 35);
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
            // Startup
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
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
    }
}