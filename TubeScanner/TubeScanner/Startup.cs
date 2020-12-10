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

        public TScanner _tScanner = new TScanner();
        public OpticonScanner _bs;

        //public static bool isConnected = false;

        public Startup()
        {
            InitializeComponent();
            rack = new Rack(8, 12);
        }

        private void Startup_Load(object sender, EventArgs e)
        {
            /* Check if devices connected */
            devicesValid = ConnectDevices();
            readyToStart();
            EmptyInputFile();
        }

        private async void btn_connect_Click(object sender, EventArgs e)
        {
            /* '.Stop()' allows the program to check the connections again */
            if (_tScanner.dP.IsOpen)
            {
                _tScanner.dP.Stop();
            }
            if (_bs.IsOpen)
            {
                _bs.Stop();
            }

            /* Check if devices connected */
            devicesValid = ConnectDevices();
            readyToStart();
        }

        /* FOR INPUT FILE */
        private async void btnFileBrowse_Click(object sender, EventArgs e)
        {
            using (System.Windows.Forms.OpenFileDialog dialog = new System.Windows.Forms.OpenFileDialog())
            {
                dialog.Filter = "Input Files (.txt;.csv;)|*.txt;*.csv;";
                dialog.Multiselect = false;

                /* Open file browsing window, .txt and .csv only */
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
            /* TEST IF DEVICES ARE STILL CONNECTED */
            if (_tScanner.dP.IsOpen && _bs.IsOpen)
            {
            /*IF TRUE, SHOW TUBE RACK FORM */
            Form1 form = new Form1(this, rack, _tScanner, _bs);
                form.ShowDialog();
            }
            else
            {
                /*IF FALSE, DISPLAY MESSAGE BOX STATING DISCONNECTION HAS OCCURED*/
               MessageBox.Show("Device/s disconnected!\n Reconnect Intrument and Barcode scanner and try again");
                devicesValid = ConnectDevices();
                readyToStart();
            }
 
        }

        public bool ConnectDevices()
        { 
            bool connected = true;


            if (_tScanner.deviceConnectionMonitor._scannerComPortsList.Count > 0)
            {
                _bs = new OpticonScanner(this, _tScanner.deviceConnectionMonitor._scannerComPortsList[0], _tScanner.deviceConnectionMonitor);
                lbl_BS.ForeColor = Color.Green;
                _bs.Start();
            }
            else
            {
                _bs = new OpticonScanner(this, "COM0", _tScanner.deviceConnectionMonitor);
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

        /* TODO: tube scanner fails to open */

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

        public void EmptyInputFile()
        {
            rack.InputFilename = "";
            lbl_file.ForeColor = Color.Red;
            lbl_file.Text = "NO FILE";
            inputFileValid = false;
            readyToStart();
        }

    }
}
