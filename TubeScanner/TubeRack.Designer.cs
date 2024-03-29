﻿
namespace TubeScanner
{
    partial class TubeRack
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TubeRack));
            this.btn_pause = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.pID = new System.Windows.Forms.Label();
            this.bc = new System.Windows.Forms.Label();
            this.lbl_PlateID = new System.Windows.Forms.Label();
            this.lbl_Barcode = new System.Windows.Forms.Label();
            this.btn_endRun = new System.Windows.Forms.Button();
            this.lbl_Status = new System.Windows.Forms.Label();
            this.txt_barcodeEntry = new System.Windows.Forms.TextBox();
            this.btn_enter = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btn_pause
            // 
            this.btn_pause.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_pause.Location = new System.Drawing.Point(825, 595);
            this.btn_pause.Name = "btn_pause";
            this.btn_pause.Size = new System.Drawing.Size(134, 37);
            this.btn_pause.TabIndex = 1;
            this.btn_pause.Text = "Pause";
            this.btn_pause.UseVisualStyleBackColor = true;
            this.btn_pause.Click += new System.EventHandler(this.btn_Pause_Click);
            // 
            // button3
            // 
            this.button3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button3.Location = new System.Drawing.Point(825, 203);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(134, 37);
            this.button3.TabIndex = 2;
            this.button3.Text = "Hide Labels";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button4
            // 
            this.button4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button4.Location = new System.Drawing.Point(825, 264);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(134, 37);
            this.button4.TabIndex = 3;
            this.button4.Text = "Row/Column Labels";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // button5
            // 
            this.button5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button5.Location = new System.Drawing.Point(825, 327);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(134, 37);
            this.button5.TabIndex = 4;
            this.button5.Text = "Well # Labels";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // pID
            // 
            this.pID.AutoSize = true;
            this.pID.Font = new System.Drawing.Font("Lucida Sans", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pID.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.pID.Location = new System.Drawing.Point(6, 40);
            this.pID.Name = "pID";
            this.pID.Size = new System.Drawing.Size(100, 23);
            this.pID.TabIndex = 8;
            this.pID.Text = "Plate ID:";
            // 
            // bc
            // 
            this.bc.AutoSize = true;
            this.bc.Font = new System.Drawing.Font("Lucida Sans", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bc.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.bc.Location = new System.Drawing.Point(457, 40);
            this.bc.Name = "bc";
            this.bc.Size = new System.Drawing.Size(101, 23);
            this.bc.TabIndex = 9;
            this.bc.Text = "Barcode:";
            // 
            // lbl_PlateID
            // 
            this.lbl_PlateID.AutoSize = true;
            this.lbl_PlateID.Font = new System.Drawing.Font("Lucida Sans", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_PlateID.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.lbl_PlateID.Location = new System.Drawing.Point(110, 40);
            this.lbl_PlateID.Name = "lbl_PlateID";
            this.lbl_PlateID.Size = new System.Drawing.Size(0, 23);
            this.lbl_PlateID.TabIndex = 10;
            // 
            // lbl_Barcode
            // 
            this.lbl_Barcode.AutoSize = true;
            this.lbl_Barcode.Font = new System.Drawing.Font("Lucida Sans", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_Barcode.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.lbl_Barcode.Location = new System.Drawing.Point(561, 40);
            this.lbl_Barcode.Name = "lbl_Barcode";
            this.lbl_Barcode.Size = new System.Drawing.Size(0, 23);
            this.lbl_Barcode.TabIndex = 11;
            // 
            // btn_endRun
            // 
            this.btn_endRun.Location = new System.Drawing.Point(825, 647);
            this.btn_endRun.Name = "btn_endRun";
            this.btn_endRun.Size = new System.Drawing.Size(134, 75);
            this.btn_endRun.TabIndex = 12;
            this.btn_endRun.Text = "End Run";
            this.btn_endRun.UseVisualStyleBackColor = true;
            this.btn_endRun.Click += new System.EventHandler(this.btn_endRun_Click);
            // 
            // lbl_Status
            // 
            this.lbl_Status.AutoSize = true;
            this.lbl_Status.Font = new System.Drawing.Font("Lucida Sans", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_Status.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.lbl_Status.Location = new System.Drawing.Point(7, 689);
            this.lbl_Status.Name = "lbl_Status";
            this.lbl_Status.Size = new System.Drawing.Size(415, 27);
            this.lbl_Status.TabIndex = 14;
            this.lbl_Status.Text = "SCAN OR ENTER PLATE BARCODE";
            // 
            // txt_barcodeEntry
            // 
            this.txt_barcodeEntry.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txt_barcodeEntry.Location = new System.Drawing.Point(428, 685);
            this.txt_barcodeEntry.Name = "txt_barcodeEntry";
            this.txt_barcodeEntry.Size = new System.Drawing.Size(276, 35);
            this.txt_barcodeEntry.TabIndex = 15;
            // 
            // btn_enter
            // 
            this.btn_enter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_enter.Location = new System.Drawing.Point(710, 684);
            this.btn_enter.Name = "btn_enter";
            this.btn_enter.Size = new System.Drawing.Size(103, 37);
            this.btn_enter.TabIndex = 16;
            this.btn_enter.Text = "Enter";
            this.btn_enter.UseVisualStyleBackColor = true;
            this.btn_enter.Click += new System.EventHandler(this.btn_enter_Click);
            // 
            // TubeRack
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(982, 734);
            this.Controls.Add(this.btn_enter);
            this.Controls.Add(this.txt_barcodeEntry);
            this.Controls.Add(this.lbl_Status);
            this.Controls.Add(this.btn_endRun);
            this.Controls.Add(this.lbl_Barcode);
            this.Controls.Add(this.lbl_PlateID);
            this.Controls.Add(this.bc);
            this.Controls.Add(this.pID);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.btn_pause);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "TubeRack";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Tube Rack";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button btn_pause;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Button btn_endRun;
        private System.Windows.Forms.Label pID;
        private System.Windows.Forms.Label bc;
        private System.Windows.Forms.Label lbl_PlateID;
        private System.Windows.Forms.Label lbl_Barcode;
        private System.Windows.Forms.Label lbl_Status;
        private System.Windows.Forms.TextBox txt_barcodeEntry;
        private System.Windows.Forms.Button btn_enter;
    }
}

