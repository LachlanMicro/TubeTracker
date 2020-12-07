using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TubeScanner.Classes;

namespace TubeScanner
{

    public partial class Startup : Form
    {
        Rack rack;
        public string plateID;

        public Startup()
        {
            InitializeComponent();

            rack = new Rack(8, 12);
        }

        private void Startup_Load(object sender, EventArgs e)
        {
            ConnectDevices();
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
                            // _testSequence.ImportInputFileName = dialog.FileName;
                            //  txtImportInputFilename.Text = System.IO.Path.GetFileName(dialog.FileName);
                            txtImportInputFilename.Text = dialog.FileName;

                            rack = new Rack(8, 12);
                            rack.InputFilename = dialog.FileName;

                            FileManager inputFile = new FileManager();
                            await inputFile.LoadInputFile(rack);
                        }
                    }
                }

            }
        }

        private async void button2_ClickAsync(object sender, EventArgs e)
        {
            if (ConnectDevices())
            {
                TScanner _tScanner = new TScanner();
                OpticonScanner _bs = new OpticonScanner(_tScanner.deviceConnectionMonitor._scannerComPortsList[0]);

                _tScanner.autoConnect();

                if (_tScanner.deviceConnectionMonitor._scannerConnected)
                {
                    _bs.Start();
                }

                Form1 form = new Form1(rack, _tScanner, _bs);
                form.ShowDialog();

                _tScanner.dP.Stop();
                _bs.Stop();
            }
            else
            {
                MessageBox.Show("Device/s not connected! Try reconnecting.");
            }
        }

        private async void btn_connect_Click(object sender, EventArgs e)
        {
            ConnectDevices();
        }


        private bool ConnectDevices()
        { 
            TScanner _tScanner = new TScanner();

            bool connected = true;

            try
            {
                OpticonScanner _bs = new OpticonScanner(_tScanner.deviceConnectionMonitor._scannerComPortsList[0]);
                lbl_BS.ForeColor = Color.Green;
                lbl_BS.Text = "BARCODE ONLINE ON " + _bs.PortName;
            }
            catch (ArgumentOutOfRangeException)
            {
                lbl_BS.ForeColor = Color.Red;
                lbl_BS.Text = "BARCODE OFFLINE";
                connected = false;
            }
           
             _tScanner.autoConnect();
            //await _tScanner.DleCommands.sendNullCommand();

            if (_tScanner.dP.IsOpen)
            {
                lbl_TS.ForeColor = Color.Green;
                lbl_TS.Text = "INSTRUMENT ONLINE ON " + _tScanner.dP.PortName;
            }
            else
            {
                lbl_TS.ForeColor = Color.Red;
                lbl_TS.Text = "INSTRUMENT OFFLINE";
                connected = false;
            }

            _tScanner.dP.Stop();
            //_bs.Stop();

            return connected;
        }
    }
}
