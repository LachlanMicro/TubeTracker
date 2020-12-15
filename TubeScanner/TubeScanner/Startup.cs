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

        Configuration config = new Configuration();

        TScanner _tScanner = new TScanner();
        OpticonScanner _bs;

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

            /* Select default value for interval combo box- middle of values (10) */
            
        }

        /* Connect button- user clicks to upate which devices are connected */
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

        /* File Load button- user clicks to open a file dialog to import an Input File, checks if valid */
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
                                lbl_file.Text = "VALID INPUT FILE";
                                lbl_file.ForeColor = Color.Green;
                                inputFileValid = true;
                            }
                            else
                            {
                                lbl_file.Text = "INVALID INPUT FILE";
                                lbl_file.ForeColor = Color.Red;
                                inputFileValid = false;
                            }

                            readyToStart();
                        }
                    }
                }
            }
        }

        /* Start Run button- checks if devices are still connected, then open the tube rack screen */
        private void btn_runStart_ClickAsync(object sender, EventArgs e)
        {

            if (_tScanner.dP.IsOpen)
            {
                _tScanner.dP.Stop();
            }
            if (_bs.IsOpen)
            {
                _bs.Stop();
            }

            devicesValid = ConnectDevices();
            readyToStart();

            /* TEST IF DEVICES ARE STILL CONNECTED */
            if (_tScanner.dP.IsOpen && _bs.IsOpen)
            {
                /*IF TRUE, SHOW TUBE RACK FORM */
                TubeRack form = new TubeRack(this, rack, _tScanner, _bs);
                    form.ShowDialog();
            }
            else
            {
                _bs.Stop();
                /*IF FALSE, DISPLAY MESSAGE BOX STATING DISCONNECTION HAS OCCURED*/
                MessageBox.Show("Device/s disconnected!\nReconnect Instrument and Barcode scanner and try again");
                devicesValid = ConnectDevices();
                readyToStart();
            }
 
        }

        private void btn_Config_Click(object sender, EventArgs e)
        {
            config.Show();
        }

        /* Checks connection for the usb connected devices */
        private bool ConnectDevices()
        {
            bool connected = true;

            if (_tScanner.deviceConnectionMonitor._scannerComPortsList.Count > 0)
            {
                _bs = new OpticonScanner(_tScanner.deviceConnectionMonitor._scannerComPortsList[0]);
                lbl_BS.Text = "BARCODE SCANNER CONNECTED";
                lbl_BS.ForeColor = Color.Green;
                _bs.Start();
            }
            else
            {
                _bs = new OpticonScanner("COM0");
                lbl_BS.Text = "BARCODE SCANNER NOT CONNECTED";
                lbl_BS.ForeColor = Color.Red;
                connected = false;
            }
                
             _tScanner.autoConnect();
            //await _tScanner.DleCommands.sendNullCommand();

            if (_tScanner.dP.IsOpen)
            {
                lbl_TS.Text = "TUBE TRACKER CONNECTED";
                lbl_TS.ForeColor = Color.Green;
            }
            else
            {
                lbl_TS.Text = "TUBE TRACKER NOT CONNECTED";
                lbl_TS.ForeColor = Color.Red;
                connected = false;
            }

            //_tScanner.dP.Stop();
            //_bs.Stop();

            return connected;
        }

        /* Enables/diables the Start Run button if able to scan */
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

        /* Unloads the input file */
        public void EmptyInputFile()
        {
            rack.InputFilename = "";
            lbl_file.ForeColor = Color.Red;
            lbl_file.Text = "NO INPUT FILE";
            inputFileValid = false;
            readyToStart();
        }

        public void barcodeNotConnected()
        {
            lbl_BS.Text = "BARCODE SCANNER NOT CONNECTED";
            lbl_BS.ForeColor = Color.Red;
            MessageBox.Show("Barcode scanner was disconnected. Your current run will need to be started from the beginning.");
        }

        public void tubeNotConnected()
        {
            lbl_TS.Text = "TUBE TRACKER NOT CONNECTED";
            lbl_TS.ForeColor = Color.Red;
            MessageBox.Show("Tube scanner was disconnected. Your current run will need to be started from the beginning.");
        }
    }
}
