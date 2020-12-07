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
            TScanner _tScanner = new TScanner();

            if (_tScanner.deviceConnectionMonitor._scannerComPortsList.Count > 0)
            {
                OpticonScanner _bs = new OpticonScanner(_tScanner.deviceConnectionMonitor._scannerComPortsList[0]);


                _tScanner.autoConnect();
                if (_tScanner.dP.IsOpen)
                {
                    if (await _tScanner.DleCommands.sendNullCommand())
                    {
                        Text = "Scanner connected on " + _tScanner.dP.PortName;
                    }
                    else
                    {
                        Text = "Failed to connect";
                    }

                }
                else
                {
                    Text = "Scanner not found";
                }

                if (_tScanner.deviceConnectionMonitor._scannerConnected)
                {
                    _bs.Start();
                }


                if (_tScanner.dP.IsOpen && _bs.IsOpen)
                {
                    Form1 form = new Form1(rack, _tScanner, _bs);
                    form.ShowDialog();

                    _tScanner.dP.Stop();
                    _bs.Stop();
                }
                else
                {
                    if (!_tScanner.dP.IsOpen && !_bs.IsOpen)
                    {
                        MessageBox.Show("Barcode Scanner and Tube Scanner Not Found!");
                    }
                    else if (!_tScanner.dP.IsOpen)
                    {
                        MessageBox.Show("Tube Scanner Not Found!");
                    }
                    else if (!_bs.IsOpen)
                    {
                        MessageBox.Show("Barcode Scanner Not Found!");
                    }
                }

            }
            else
            {
                OpticonScanner _bs = new OpticonScanner("COM0");

                /*TODO error handling, for now open form for testing purposes */
                Form1 form = new Form1(rack, _tScanner, _bs);
                form.ShowDialog();
                _tScanner.dP.Stop();
                _bs.Stop();
            }
        }
    }
}
