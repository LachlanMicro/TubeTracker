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
                            await LoadInputFile(rack);
                        }
                    }
                }
            }
        }

        public async Task<bool> LoadInputFile(Rack rack)
        {
            if (File.Exists(rack.InputFilename))
            {
                var lines = File.ReadAllLines(rack.InputFilename);

                /* Read header- Plate ID */
                bool hFound = false;
                for (int index = 0; index < rack.TubeList.Count; index++)
                {
                    var header = lines[index].Split('\t');

                    if (header[0] == "Plate ID")
                    {
                        rack.PlateID = header[1];
                        hFound = true;
                        break;
                    }
                }
                if (!hFound)
                {
                    MessageBox.Show("Plate ID not found!");
                }

                 // 2. Input file of old format
                bool isPlateFound = true;
                for (var lineNumber = 3; lineNumber < lines.Length; lineNumber++)
                {
                    if (lines[lineNumber].Trim() == "End of File")
                        break;

                    //string plateType = "";
                    var contents = lines[lineNumber].Split('\t');

                    char[] TPos = contents[0].ToCharArray();

                    /* Check if position valid (format: A01) */
                    if (Char.IsLetter(TPos[0]) && (Char.IsDigit(TPos[2])))
                    {

                        for (int index = 0; index < rack.TubeList.Count; index++)
                        {
                            if (rack.TubeList[index].ID.Equals(contents[0]))
                            {
                                rack.TubeList[index].Barcode = contents[1];
                                rack.TubeList[index].Status = Status.READY_TO_LOAD;
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("Line " + (lineNumber + 1) + ": Tube position invalid, use format [row],[0],[column] \n e.g. A01");
                    }
                
                }
            }
            return true;
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
