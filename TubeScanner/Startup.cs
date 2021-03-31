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
        public bool devicesValid = false;

        Configuration config = new Configuration();

        TScanner _tScanner = new TScanner();
        OpticonScanner _bs = null;

        private bool LEDon = false;

        public Startup()
        {
            InitializeComponent();

            rack = new Rack(8, 12);
        }

        private async void Startup_Load(object sender, EventArgs e)
        {
            /* Check if devices connected */
            devicesValid = ConnectDevices();
            readyToStart();
            EmptyInputFile();

            await _bs.stopScan();

            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 12; col++)
                {
                    await _tScanner.DleCommands.selectLED((UInt16)(row + 1), (UInt16)(col + 1), DleCommands.LedColour.LED_GREEN, DleCommands.LedState.LED_STATE_OFF);
                }
            }
        }

        /* Connect button- user clicks to upate which devices are connected */
        private async void btn_connect_Click(object sender, EventArgs e)
        {
            /* '.Stop()' allows the program to check the connections again */
            if (_tScanner.dP.IsOpen)
            {
                _tScanner.dP.Stop();
            }

            if (_bs != null)
            {
                if (_bs.IsOpen)
                {
                    _bs.Stop();
                }
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

            /* Test if devices are still connected */
            if ((_tScanner.dP.IsOpen || Configuration.simulationMode) && _bs.IsOpen)
            {
                /*If true, go to Tube Rack screen */
                TubeRack form = new TubeRack(this, rack, _tScanner, _bs);
                form.ShowDialog();
            }
            else
            {
                _bs.Stop();
                /* if false, inform user of disconnection */
                MessageBox.Show("Device/s disconnected!\nReconnect Instrument and Barcode scanner and try again");
                devicesValid = ConnectDevices();
                readyToStart();
            }
        }

        /* Config button- opens configuration (settings) screen */
        private void btn_Config_Click(object sender, EventArgs e)
        {
            config.Show();
        }

        /* TODO: Send command to check connection */
        /* Checks whether USB connected devices are connected */
        public bool ConnectDevices()
        {
            bool connected = true;

            /* Update label based on whether barcode scanner is connected or disconnected */
            if (_tScanner.deviceConnectionMonitor._scannerComPortsList.Count > 0)
            {
                _bs = new OpticonScanner(_tScanner.deviceConnectionMonitor._scannerComPortsList[0]);
                lbl_BS.Text = "BARCODE SCANNER CONNECTED";
                lbl_BS.ForeColor = Color.Green;
                _bs.Start();
            }
            else if (Configuration.simulationMode)
            {
                lbl_TS.Text = "TUBE TRACKER SIMULATION";
                lbl_TS.ForeColor = Color.Green;

            }
            else
            {
                lbl_BS.Text = "BARCODE SCANNER NOT CONNECTED";
                lbl_BS.ForeColor = Color.Red;
                connected = false;
            }
                
             _tScanner.autoConnect();

            /* Update label based on whether tube scanner is connected or disconnected */
            if (_tScanner.dP.IsOpen) 
            {
                lbl_TS.Text = "TUBE TRACKER CONNECTED";
                lbl_TS.ForeColor = Color.Green;
            }
            else if (Configuration.simulationMode)
            {
                lbl_TS.Text = "TUBE TRACKER SIMULATION";
                lbl_TS.ForeColor = Color.Green;
            }
            else
            {
                lbl_TS.Text = "TUBE TRACKER NOT CONNECTED";
                lbl_TS.ForeColor = Color.Red;
                connected = false;
            }

            return connected;
        }

        /* Enables/diables the Start Run button if able to scan */
        public void readyToStart()
        {
            if (inputFileValid && devicesValid && Program.currentUser != "")
            {
                btn_runStart.Enabled = true;
            }
            else
            {
                btn_runStart.Enabled = false;
            }
        }

        /* Unloads the input file from the program */
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

        /* Login/Logout button */
        private void buttonLogin_Click(object sender, EventArgs e)
        {
            /* Login- opens login screen */
            if (buttonLogin.Text == "Login")
            {
                Login form = new Login();
                form.ShowDialog();

                /* If user has logged in, make button a "Logout" button */
                if (Program.currentUser != "")
                {
                    buttonLogin.Text = "Logout";
                    lbl_user.Text = "Current user: " + Program.currentUser;
                    lbl_user.ForeColor = Color.Green;
                }
            }
            /* If button is a logout button, log user out */
            else if (buttonLogin.Text == "Logout")
            {
                DialogResult dialogResult = MessageBox.Show("Are you sure you wish to log out?", "Logging out...", MessageBoxButtons.OKCancel);
                if (dialogResult == DialogResult.OK)
                {
                    Program.currentUser = "";
                    buttonLogin.Text = "Login";
                    lbl_user.Text = "Current user: " + Program.currentUser;
                    lbl_user.ForeColor = Color.Red;
                }
            }
            readyToStart();
        }

        /* Users button- opens users page and set the current user */
        private void usersButton_Click(object sender, EventArgs e)
        {
            UsersPage form = new UsersPage();
            form.ShowDialog();
            lbl_user.Text = "Current user: " + Program.currentUser;
            if (Program.currentUser == "")
            {
                buttonLogin.Text = "Login";
            }
        }

        private async void button_test_LED_Click(object sender, EventArgs e)
        {
            if (!LEDon)
            {
                await _tScanner.DleCommands.selectLED(1, 1, DleCommands.LedColour.LED_GREEN, DleCommands.LedState.LED_STATE_FLASHING);
                Console.WriteLine("ON");
            }
            else
            {
                await _tScanner.DleCommands.selectLED(1, 1, DleCommands.LedColour.LED_OFF, DleCommands.LedState.LED_STATE_OFF);
                Console.WriteLine("OFF");
            }

            LEDon = !LEDon;

            Byte[] tubeData = null;
            tubeData = await _tScanner.DleCommands.scanAllTubes();

            int row = 0;

            for (int col = 0; col < tubeData.Length; col++)
            {
                Console.WriteLine(tubeData[col]);

                

               

                //int col = 0;

                //if (by != 0)
                //{
                //    if (by == 1)
                //    {
                //        col = 0;
                //    }
                //    else
                //    {
                //        col = by / 4;
                //    }
                //}
                //else 
                //{
                   
                //}
            }
            Console.WriteLine("");

        }
    }
}
