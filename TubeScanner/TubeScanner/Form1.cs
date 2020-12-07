﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;
using TubeScanner.Classes;
using TubeScanner.Controls;

namespace TubeScanner
{
    public partial class Form1 : Form
    {
        private Form1 form;
        private Rack _rack = null;
        RackControl rackControl = null;
        public TScanner _tScanner;
        public OpticonScanner _bs;


        public Form1(Rack rack, TScanner tScanner, OpticonScanner bs)
        {
            _rack = rack;
            _tScanner = tScanner;
            _bs = bs;
            InitializeComponent();
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            rackControl = new RackControl(_rack);
            rackControl.Display(this, new Point(10, 75));
            rackControl.Anchor = (AnchorStyles.Bottom | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Left);

            lbl_PlateID.Text = _rack.PlateID;

        }

        /* Disables the close (X) button on window */
        private const int CP_NOCLOSE_BUTTON = 0x200;
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams param = base.CreateParams;
                param.ClassStyle = param.ClassStyle | CP_NOCLOSE_BUTTON;
                return param;
            }
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            //for(int x = 0;x< _rack.TubeList.Count;x++)
            // {
            //     rackControl.UpdateTubeStatus(x, Status.LOADED);
            // }

            string barcode = String.Empty;

            if (_bs.IsOpen)
            {
                barcode = await _bs.startScan();
            }

            lbl_Barcode.Text = barcode;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            for (int x = 0; x < _rack.TubeList.Count; x++)
            {
                if(x%2 == 0)
                rackControl.UpdateTubeStatus(x, Status.READY_TO_LOAD);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            for (int x = 0; x < _rack.TubeList.Count; x++)
            {
                if (x % 2 == 1)
                    rackControl.UpdateTubeStatus(x, Status.REMOVED);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            for (int x = 0; x < _rack.TubeList.Count; x++)
            {
                if (x % 3 == 0)
                    rackControl.UpdateTubeStatus(x, Status.ERROR);
            }
        }

        private async void button5_Click(object sender, EventArgs e)
        {
            var randY = new Random();
            var randS = new Random();
            for (int x = 0; x < 500; x++)
            {
               rackControl.UpdateTubeStatus(randY.Next(95), (Status)randS.Next(5));
               await Task.Delay(50);
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            for (int x = 0; x < _rack.TubeList.Count; x++)
            {
                rackControl.UpdateTextContent(x, eShowText.NO_SHOW);
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            for (int x = 0; x < _rack.TubeList.Count; x++)
            {
                rackControl.UpdateTextContent(x, eShowText.SHOW_ID);
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            for (int x = 0; x < _rack.TubeList.Count; x++)
            {
                rackControl.UpdateTextContent(x, eShowText.SHOW_NUMBER);
            }
        }

        /* End run- save output file, clear display, return to startup */
        private void btn_endRun_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = System.Windows.Forms.MessageBox.Show("Save Run?", "Ending Run", MessageBoxButtons.YesNoCancel);
            if (dialogResult == DialogResult.Yes)
            {
                /* Save output file */
                string[] currDateTime = DateTime.Today.ToString().Split(' ');
                FileManager outfile = new FileManager();
                outfile.WriteOutputFile("../../IO Files/output log 2.txt", _rack.TubeList, _rack.PlateID, "aaaa", currDateTime[0]);

                quitToStartup();
            }
            else if (dialogResult == DialogResult.No)
            {
                quitToStartup();
            }

        }

        private void quitToStartup()
        {
            /* Clear tubes */
            for (int x = 0; x < _rack.TubeList.Count; x++)
            {
                _rack.TubeList[x].Status = Status.NOT_USED;
            }
            //_rack.TubeList.Clear();

            /* Close test window */
            this.Hide();
        }

    }
}
