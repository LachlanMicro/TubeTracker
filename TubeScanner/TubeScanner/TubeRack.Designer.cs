
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
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.pID = new System.Windows.Forms.Label();
            this.bc = new System.Windows.Forms.Label();
            this.lbl_PlateID = new System.Windows.Forms.Label();
            this.lbl_Barcode = new System.Windows.Forms.Label();
            this.btn_endRun = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Location = new System.Drawing.Point(825, 75);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(134, 37);
            this.button1.TabIndex = 0;
            this.button1.Text = "Scan";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button2.Location = new System.Drawing.Point(825, 141);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(134, 37);
            this.button2.TabIndex = 1;
            this.button2.Text = "Pause";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
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
            this.pID.Font = new System.Drawing.Font("Lucida Sans", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pID.ForeColor = System.Drawing.SystemColors.ActiveCaption;
            this.pID.Location = new System.Drawing.Point(12, 9);
            this.pID.Name = "pID";
            this.pID.Size = new System.Drawing.Size(114, 27);
            this.pID.TabIndex = 8;
            this.pID.Text = "Plate ID:";
            // 
            // bc
            // 
            this.bc.AutoSize = true;
            this.bc.Font = new System.Drawing.Font("Lucida Sans", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bc.ForeColor = System.Drawing.SystemColors.ActiveCaption;
            this.bc.Location = new System.Drawing.Point(12, 39);
            this.bc.Name = "bc";
            this.bc.Size = new System.Drawing.Size(116, 27);
            this.bc.TabIndex = 9;
            this.bc.Text = "Barcode:";
            // 
            // lbl_PlateID
            // 
            this.lbl_PlateID.AutoSize = true;
            this.lbl_PlateID.Font = new System.Drawing.Font("Lucida Sans", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_PlateID.ForeColor = System.Drawing.SystemColors.ActiveCaption;
            this.lbl_PlateID.Location = new System.Drawing.Point(132, 9);
            this.lbl_PlateID.Name = "lbl_PlateID";
            this.lbl_PlateID.Size = new System.Drawing.Size(0, 27);
            this.lbl_PlateID.TabIndex = 10;
            // 
            // lbl_Barcode
            // 
            this.lbl_Barcode.AutoSize = true;
            this.lbl_Barcode.Font = new System.Drawing.Font("Lucida Sans", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_Barcode.ForeColor = System.Drawing.SystemColors.ActiveCaption;
            this.lbl_Barcode.Location = new System.Drawing.Point(134, 39);
            this.lbl_Barcode.Name = "lbl_Barcode";
            this.lbl_Barcode.Size = new System.Drawing.Size(0, 27);
            this.lbl_Barcode.TabIndex = 11;
            // 
            // btn_endRun
            // 
            this.btn_endRun.Location = new System.Drawing.Point(825, 589);
            this.btn_endRun.Name = "btn_endRun";
            this.btn_endRun.Size = new System.Drawing.Size(134, 37);
            this.btn_endRun.TabIndex = 12;
            this.btn_endRun.Text = "End Run";
            this.btn_endRun.UseVisualStyleBackColor = true;
            this.btn_endRun.Click += new System.EventHandler(this.btn_endRun_Click);
            // 
            // TubeRack
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(982, 638);
            this.Controls.Add(this.btn_endRun);
            this.Controls.Add(this.lbl_Barcode);
            this.Controls.Add(this.lbl_PlateID);
            this.Controls.Add(this.bc);
            this.Controls.Add(this.pID);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "TubeRack";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Tube Rack";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Button btn_endRun;
        private System.Windows.Forms.Label pID;
        private System.Windows.Forms.Label bc;
        private System.Windows.Forms.Label lbl_PlateID;
        private System.Windows.Forms.Label lbl_Barcode;
    }
}

