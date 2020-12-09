using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TubeScanner.Classes;
using TubeScanner.Controls;
using System.Timers;

namespace TubeScanner
{
    public partial class Form1 : Form
    {
        private Form1 form;
        private Rack _rack = null;
        RackControl rackControl = null;
        public TScanner _tScanner;
        public OpticonScanner _bs;
        public bool scanning = false;
        private static System.Timers.Timer scanTimer;
        string wellNumber;

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

            _tScanner.DleCommands.OnFootSwitchEvent += FootSwitchEvent;

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

        }

        // Function to scan all tube positions
        private async void FootSwitchEvent(object sender, EventArgs e)
        {
            if (_tScanner.dP.IsOpen)
            {

                //Byte[] tubeData = await _tScanner.DleCommands.scanAllTubes();
                Byte[] tubeData = null;

                if (tubeData != null)
                {
                    int bitNum = 0;
                    char[,] bitMap = new char[8, 12];

                    // Loop through the 12 column byte values
                    for (int col = 0; col < 12; col++)
                    {
                        // Convert column byte from decimal to 8-bit binary string 
                        string binary_column = Convert.ToString(tubeData[col], 2).PadLeft(8, '0');
                        // Convert string to a character array
                        char[] column_array = binary_column.ToCharArray(0, 8);

                        // Loop through each row bit within the array
                        for (int row = 0; row < 8; row++)
                        {
                            // Assign the value to the matrix 
                            bitMap[row, col] = column_array[row];
                        }
                        //Console.WriteLine(binary_column);
                    }

                    // Update the tube status based on the bit map matrix
                    for (int row = 0; row < 8; row++)
                    {
                        for (int col = 0; col < 12; col++)
                        {
                            if (bitMap[row, col] == '1')
                            {
                                if (_rack.InitialTubeList[bitNum].Status == Status.READY_TO_LOAD)
                                {
                                    rackControl.UpdateTubeStatus(bitNum, Status.LOADED);
                                }
                                else if (_rack.InitialTubeList[bitNum].Status == Status.NOT_USED)
                                {
                                    rackControl.UpdateTubeStatus(bitNum, Status.ERROR);
                                }
                            }

                            if (bitMap[row, col] == '0')
                            {
                                if (_rack.InitialTubeList[bitNum].Status == Status.READY_TO_LOAD)
                                {
                                    rackControl.UpdateTubeStatus(bitNum, Status.READY_TO_LOAD);
                                }
                                if (_rack.InitialTubeList[bitNum].Status == Status.NOT_USED)
                                {
                                    rackControl.UpdateTubeStatus(bitNum, Status.NOT_USED);
                                }
                                if (_rack.InitialTubeList[bitNum].Status == Status.LOADED)
                                {
                                    rackControl.UpdateTubeStatus(bitNum, Status.REMOVED);
                                }
                            }
                            bitNum++;
                        }
                    }
                }
            }
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

        // Button to activate serial communication and barcode scanner
        private async void button1_Click(object sender, EventArgs e)
        {
            // Update tube scanner state to running
            if (_tScanner.dP.IsOpen)
            {
                await _tScanner.DleCommands.runStatus(DleCommands.RunState.RUNNING);
                scanning = true;
            }

            while (scanning)
            {
                string barcode = "";
                bool found = false;

                if (_bs.IsOpen)
                {
                    barcode = await _bs.startScan();
                }

                lbl_Barcode.Text = barcode;

                //await _bs.stopScan();

                var lines = File.ReadAllLines(_rack.InputFilename);

                for (int line = 3; line < lines.Length; line++) 
                {
                    string[] splitLine = lines[line].Split('\t');
                    if (splitLine[1].Equals(barcode))
                    {
                        // If barcode has not already been used, update it to selected
                        if (!_rack.BarcodesScanned.Contains(barcode))
                        {
                            // If any well is already selected, cancel it
                            for (int i = 0; i < _rack.TubeList.Count; i++)
                            {
                                if (_rack.TubeList[i].Status == Status.SELECTED)
                                {
                                    rackControl.UpdateTubeStatus(i, Status.READY_TO_LOAD);
                                }
                            }
                            wellNumber = splitLine[0];
                            rackControl.UpdateTubeStatusNoNumber(wellNumber, Status.SELECTED);
                            found = true;
                            break;
                        }
                    }
                }

                if (!found)
                {
                    MessageBox.Show("Scanned barcode was not found in input file or has already been scanned");
                }

                SetTimer();
                System.Threading.Thread.Sleep(1000);
            }
        }

        // Create a timer with a 10 second interval.
        private void SetTimer()
        {
            scanTimer = new System.Timers.Timer(10000); 
            scanTimer.Elapsed += OnTimedEvent;
            scanTimer.AutoReset = false;
            scanTimer.Enabled = true;
        }

        // When 10 second timer has expired, set selected tube back to normal if it is still unloaded
        private void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            int num = rackControl.GetTubeNum(wellNumber);
            if (_rack.TubeList[num].Status == Status.SELECTED)
            {
                rackControl.UpdateTubeStatus(num, Status.READY_TO_LOAD);
                MessageBox.Show("10 second window to place scanned tube has expired. Please rescan and try again.");
            }
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            if (_tScanner.dP.IsOpen)
            {
                await _tScanner.DleCommands.runStatus(DleCommands.RunState.PAUSED);
                scanning = false;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            for (int x = 0; x < _rack.TubeList.Count; x++)
            {
                rackControl.UpdateTextContent(x, eShowText.NO_SHOW);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            for (int x = 0; x < _rack.TubeList.Count; x++)
            {
                rackControl.UpdateTextContent(x, eShowText.SHOW_ID);
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            for (int x = 0; x < _rack.TubeList.Count; x++)
            {
                rackControl.UpdateTextContent(x, eShowText.SHOW_NUMBER);
            }
        }


        /* End run- save output file, clear display, return to startup */
        private async void btn_endRun_Click(object sender, EventArgs e)
        {
            bool loadError = false;
            bool placeError = false;
            bool removeError = false;
            for (int x = 0; x < _rack.TubeList.Count; x++)
            {
                if (_rack.TubeList[x].Status == Status.READY_TO_LOAD)
                {
                    if (!loadError)
                    {
                        MessageBox.Show("Warning: The rack contains unloaded wells, please ensure that you wish to continue.");
                        loadError = true;
                    }
                }
                if (_rack.TubeList[x].Status == Status.ERROR)
                {
                    if (!placeError)
                    {
                        MessageBox.Show("Warning: The rack contains misplaced tubes, please ensure that you wish to continue.");
                        placeError = true;
                    }
                }
                if (_rack.TubeList[x].Status == Status.REMOVED)
                {
                    if (!removeError)
                    {
                        MessageBox.Show("Warning: Tubes have been removed from the rack and not replaced, please ensure that you wish to continue.");
                        removeError = true;
                    }
                }
                if (loadError & placeError & removeError)
                {
                    break;
                }
            }

            DialogResult dialogResult = System.Windows.Forms.MessageBox.Show("Save Run?", "Ending Run", MessageBoxButtons.YesNoCancel);
            if (dialogResult == DialogResult.Yes)
            {
                /* Save output file */
                string[] currDateTime = DateTime.Today.ToString().Split(' ');
                //FileManager outfile = new FileManager();
                FileManager.WriteOutputFile("../../IO Files/output log 2.txt", rackControl.OutputTubeList, _rack.PlateID, "aaaa", currDateTime[0]);

                await quitToStartupAsync();
            }
            else if (dialogResult == DialogResult.No)
            {
                await quitToStartupAsync();
            }

        }

        private async Task quitToStartupAsync()
        {
            /* Clear tubes */
            if (_tScanner.dP.IsOpen)
            {
                await _tScanner.DleCommands.runStatus(DleCommands.RunState.STOPPED);
                for (int x = 0; x < _rack.TubeList.Count; x++)
                {
                    rackControl.UpdateTubeStatus(x, Status.NOT_USED);
                }
            }

            //for (int x = 0; x < _rack.TubeList.Count; x++)
            //{
            //_rack.TubeList[x].Status = Status.NOT_USED;
            //}
            //_rack.TubeList.Clear();

            /* Close test window */
            this.Hide();
        }


    }
        
}

