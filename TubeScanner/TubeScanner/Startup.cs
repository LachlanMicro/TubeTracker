using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using TubeScanner.Classes;

namespace TubeScanner
{

    public partial class Startup : Form
    {
        Rack rack;
        public string plateID;

        private bool inputFileValid = false;
        private bool devicesValid = false;

        TScanner _tScanner = new TScanner();
        OpticonScanner _bs;

        public Startup()
        {
            InitializeComponent();

            rack = new Rack(8, 12);
        }

        private void Startup_Load(object sender, EventArgs e)
        {
            devicesValid = ConnectDevices();
            readyToStart();
        }

        private async void btn_connect_Click(object sender, EventArgs e)
        {
            _tScanner.dP.Stop();
            _bs.Stop();
            devicesValid = ConnectDevices();
            readyToStart();
        }

        private async void btnFileBrowse_Click(object sender, EventArgs e)
        {
            using (System.Windows.Forms.OpenFileDialog dialog = new System.Windows.Forms.OpenFileDialog())
            {
                dialog.Filter = "Input Files (.txt;.csv;)|*.txt;*.csv;";
                dialog.Multiselect = false;

                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    if (!string.IsNullOrEmpty(dialog.FileName))
                    {
                        if (File.Exists(dialog.FileName))
                        {
                            rack = new Rack(8, 12);
                            rack.InputFilename = dialog.FileName;

                            if (await FileManager.LoadInputFile(rack))
                            {
                                lbl_file.Text = "FILE VALID";
                                lbl_file.ForeColor = Color.Green;
                                inputFileValid = true;
                            }
                            else
                            {
                                lbl_file.Text = "FILE INVALID";
                                lbl_file.ForeColor = Color.Red;
                                inputFileValid = false;
                            }

                            readyToStart();
                        }
                    }
                }
            }
        }

      
        private void btn_runStart_ClickAsync(object sender, EventArgs e)
        {
            /* TEST IF DEVICES ARE STILL CONNECTED
             * IF TRUE, SHOW TUBE RACK FORM
             * IF FALSE, DISPLAY MESSAGE BOX STATING DISCONNECTION HAS OCCURED
             */

            if (_tScanner.dP.IsOpen && _bs.IsOpen)
            {
                Form1 form = new Form1(rack, _tScanner, _bs);
                form.ShowDialog();
            }
            else
            {
                MessageBox.Show("Device/s disconnected!\n Reconnect Intrument and Barcode scanner and try again");
                btn_runStart.Enabled = false;
            }
 
        }



        //if (ConnectDevices())
        //{
        //    TScanner _tScanner = new TScanner();
        //    OpticonScanner _bs = new OpticonScanner(_tScanner.deviceConnectionMonitor._scannerComPortsList[0]);

        //    _tScanner.autoConnect();

        //    if (_tScanner.deviceConnectionMonitor._scannerConnected)
        //    {
        //        _bs.Start();
        //    }
        //_tScanner.dP.Stop();
        //_bs.Stop();
        //}
        //else
        //{
        //    MessageBox.Show("Device/s not connected! Try reconnecting.");
        //}

        private bool ConnectDevices()
        { 
            bool connected = true;

            try
            {
                _bs = new OpticonScanner(_tScanner.deviceConnectionMonitor._scannerComPortsList[0]);
                lbl_BS.ForeColor = Color.Green;
                _bs.Start();
            }
            catch (ArgumentOutOfRangeException)
            {
                lbl_BS.ForeColor = Color.Red;
                connected = false;
            }
           
             _tScanner.autoConnect();
            //await _tScanner.DleCommands.sendNullCommand();

            if (_tScanner.dP.IsOpen)
            {
                lbl_TS.ForeColor = Color.Green;
            }
            else
            {
                lbl_TS.ForeColor = Color.Red;
                connected = false;
            }

            //_tScanner.dP.Stop();
            //_bs.Stop();

            return connected;
        }

        private void readyToStart()
        {
            if (inputFileValid && devicesValid)
            {
                btn_runStart.Enabled = true;
            }
            else
            {
                btn_runStart.Enabled = false;
            }
        }
    }
}
