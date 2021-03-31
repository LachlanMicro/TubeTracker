using System;
using System.IO;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using TubeScanner.Classes;
using TubeScanner.Controls;
using System.Timers;
using System.Diagnostics;

namespace TubeScanner
{
    public partial class TubeRack : Form
    {
        private Rack _rack = null;
        RackControl rackControl = null;
        public TScanner _tScanner;
        public OpticonScanner _bs;
        public bool scanning = false;
        private bool correctBarcode = false;
        private bool manualEntry = false;
        private static System.Timers.Timer scanTimer = null;
        private int fillOrder;
        string wellNumber;
        string rackBarcode = "";
        string barcode = "";
        Startup _startupForm;

        private static int scannedTube;

        public TubeRack(Startup startupForm, Rack rack, TScanner tScanner, OpticonScanner bs)
        {
            _startupForm = startupForm;
            _rack = rack;
            _tScanner = tScanner;
            _bs = bs;
            InitializeComponent();
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            correctBarcode = false;

            rackControl = new RackControl(_rack, this);
            rackControl.Display(this, new Point(10, 75));
            rackControl.Anchor = (AnchorStyles.Bottom | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Left);

            fillOrder = 1;

            await _tScanner.DleCommands.runStatus(DleCommands.RunState.RUNNING);

            rackBarcode = "";
            
            lbl_Status.Text = "SCAN OR ENTER PLATE BARCODE";

            /* For scanning tubes */

            ScanTube();

            /* Do not proceed until first scan matches rack barcode in input file */
            while (!correctBarcode)
            {
                
                if (_tScanner.deviceConnectionMonitor._scannerComPortsList.Count > 0)
                {
                    try
                    {
                        do
                        {
                            rackBarcode = await _bs.startScan();
                        }
                        while (String.IsNullOrEmpty(rackBarcode));
                    }
                    catch
                    {
                        _bs.Stop();
                        await quitToStartupAsync();
                        break;
                    }
                }
                else
                {
                    _bs.Stop();
                    await quitToStartupAsync();
                    break;
                }

                if (manualEntry)
                {
                    correctBarcode = true;
                    break;
                }

                ScanTubeRack();
            }

            _tScanner.DleCommands.OnFootSwitchEvent += FootSwitchEvent;

            //if (_tScanner.dP.IsOpen || Configuration.simulationMode)
            //{
            //    await _tScanner.DleCommands.sendNullCommand();
            //}
            //else
            //{
            //    await quitToStartupAsync();
            //}
        }

        /* Prepares for tube scanning, returning a bool to indicate the program can progress */
        private bool ReadyToScan(string rackBC)
        {
            /* Write Output file headers */
            string[] currDateTime = DateTime.Today.ToString().Split(' ');

            lbl_PlateID.Text = rackBC;

            MessageBox.Show("Place rack ready for sample scanning", "Rack Ready");

            lbl_Status.Text = "SCAN OR ENTER TUBE BARCODE";

            /* Write the header for the output file */
            string fileName = "../../IO Files/" + _rack.PlateID + " Output Log.txt";
            FileManager.WriteOutputFileHeaders(fileName, _rack.PlateID, currDateTime[0]);

            return true;
        }

        /* Function to scan all tube positions */
        private async void FootSwitchEvent(object sender, EventArgs e)
        {
            string fileName = "../../IO Files/" + _rack.PlateID + " Output Log.txt";

            if (_tScanner.dP.IsOpen || Configuration.simulationMode)
            {
                Byte[] tubeData = null;

                if (!Configuration.simulationMode)
                {
                    tubeData = await _tScanner.DleCommands.scanAllTubes();
                }
                else
                {
                    tubeData = new Byte[12];

                    //tubeData[0] = 0x00000001;
                    tubeData[0] = Convert.ToByte(scannedTube);
                    Console.WriteLine("Tube Number: " + tubeData[0]);
                }

                if (tubeData != null)
                {
                    int bitNum = 0;
                    char[,] bitMap = new char[8, 12];

                    /* Loop through the 12 column byte values */
                    for (int col = 0; col < 12; col++)
                    {
                        /* Convert column byte from decimal to 8-bit binary string */
                        string binary_column = Convert.ToString(tubeData[col], 2).PadLeft(8, '0');
                        /* Convert string to a character array */
                        char[] column_array = binary_column.ToCharArray(0, 8);

                        /* Loop through each row bit within the array */
                        for (int row = 0; row < 8; row++)
                        {
                            /* Assign the value to the matrix */
                            bitMap[row, col] = column_array[row];
                        }
                    }

                    /* Update the tube status based on the bit map matrix */
                    for (int row = 0; row < 8; row++)
                    {
                        for (int col = 0; col < 12; col++)
                        {
                            if (bitMap[row, col] == '1')
                            {
                                /* TODO Initial or regular tube lis for selected */
                                if (_rack.TubeList[bitNum].Status == Status.SELECTED)
                                {
                                    rackControl.UpdateTubeStatus(bitNum, Status.LOADED);

                                    _rack.InitialTubeList[bitNum].Status = Status.LOADED;

                                    _rack.InitialTubeList[bitNum].FillOrder = fillOrder;
                                    fillOrder++;

                                    FileManager.WriteTubeToOutputFile(fileName, _rack.PlateID, _rack.InitialTubeList[bitNum]);

                                    /* turn on LED solid at position where tube is detected */
                                    await _tScanner.DleCommands.selectLED((UInt16)(row + 1), (UInt16)(col + 1), DleCommands.LedColour.LED_GREEN, DleCommands.LedState.LED_STATE_SOLID);
                                }
                                else if (_rack.InitialTubeList[bitNum].Status == Status.READY_TO_LOAD)
                                {
                                    rackControl.UpdateTubeStatus(bitNum, Status.ERROR);

                                    _rack.InitialTubeList[bitNum].Status = Status.ERROR;

                                    _rack.InitialTubeList[bitNum].FillOrder = fillOrder;
                                    fillOrder++;

                                    FileManager.WriteTubeToOutputFile(fileName, _rack.PlateID, _rack.InitialTubeList[bitNum]);

                                    await _tScanner.DleCommands.selectLED((UInt16)(row + 1), (UInt16)(col + 1), DleCommands.LedColour.LED_RED, DleCommands.LedState.LED_STATE_FLASHING);

                                    MessageBox.Show("Warning: Tube placement does not match input file.");
                                }
                                else if (_rack.InitialTubeList[bitNum].Status == Status.NOT_USED)
                                {
                                    rackControl.UpdateTubeStatus(bitNum, Status.ERROR);

                                    _rack.InitialTubeList[bitNum].Status = Status.ERROR;

                                    _rack.InitialTubeList[bitNum].FillOrder = fillOrder;
                                    fillOrder++;

                                    FileManager.WriteTubeToOutputFile(fileName, _rack.PlateID, _rack.InitialTubeList[bitNum]);

                                    await _tScanner.DleCommands.selectLED((UInt16)(row + 1), (UInt16)(col + 1), DleCommands.LedColour.LED_RED, DleCommands.LedState.LED_STATE_FLASHING);
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
                                if (_rack.InitialTubeList[bitNum].Status == Status.ERROR)
                                {
                                    rackControl.UpdateTubeStatus(bitNum, Status.READY_TO_LOAD);

                                    _rack.InitialTubeList[bitNum].Status = Status.READY_TO_LOAD;

                                    await _tScanner.DleCommands.selectLED((UInt16)(row + 1), (UInt16)(col + 1), DleCommands.LedColour.LED_RED, DleCommands.LedState.LED_STATE_OFF);
                                }
                                if (_rack.InitialTubeList[bitNum].Status == Status.LOADED)
                                {
                                    rackControl.UpdateTubeStatus(bitNum, Status.REMOVED);

                                    _rack.InitialTubeList[bitNum].Status = Status.REMOVED;

                                    await _tScanner.DleCommands.selectLED((UInt16)(row + 1), (UInt16)(col + 1), DleCommands.LedColour.LED_GREEN, DleCommands.LedState.LED_STATE_OFF);

                                    _rack.InitialTubeList[bitNum].FillOrder = fillOrder;
                                    fillOrder++;

                                    FileManager.WriteTubeToOutputFile(fileName, _rack.PlateID, _rack.InitialTubeList[bitNum]);

                                    MessageBox.Show("Warning: Tube at " + _rack.InitialTubeList[bitNum].ID + " has been removed.");
                                }
                            }



                            bitNum++;
                        }
                    }
                }
            }
            else
            {
                await quitToStartupAsync();
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

        public void ScanTubeRack()
        {
            /* If the barcodes match, enable scan button to begin run */
            if (rackBarcode == _rack.PlateID)
            {
                correctBarcode = ReadyToScan(rackBarcode);
            }
            else
            {
                /* If plate barcode not matching, give option to overwrite the barcode with new barcode */

                DialogResult dialogResult = MessageBox.Show("Plate barcode does not match input file. Would you like to use this instead?", "Incorrect Plate ID", MessageBoxButtons.YesNo);

                if (dialogResult == DialogResult.Yes)
                {
                    _rack.PlateID = rackBarcode;

                    correctBarcode = ReadyToScan(rackBarcode);
                }
                else if (dialogResult == DialogResult.No)
                {
                    MessageBox.Show("Ensure the rack is correct and try scan again.");
                }
            }
        }

        /* Waits for scanner to scan tube */
        private async void ScanTube()
        { 
            // Update tube scanner state to running
            if (_tScanner.dP.IsOpen || Configuration.simulationMode)
            {
                scanning = true;
            }
            else
            {
                await quitToStartupAsync();
            }

            while (scanning)
            {
                bool found = false;

                if (!_tScanner.dP.IsOpen && !Configuration.simulationMode)
                {
                    await quitToStartupAsync();
                }

                if (_tScanner.deviceConnectionMonitor._scannerComPortsList.Count > 0)
                {
                    try
                    {
                        barcode = await _bs.startScan();
                    }
                    catch
                    {
                        _bs.Stop();
                        await quitToStartupAsync();
                        break;
                    }
                }
                else
                {
                    _bs.Stop();
                    await quitToStartupAsync();
                    break;
                }

                if (correctBarcode)
                {
                    lbl_Barcode.Text = barcode;

                    var lines = File.ReadAllLines(_rack.InputFilename);

                    // If any well is already selected, cancel it so only one is active
                    for (int i = 0; i < _rack.TubeList.Count; i++)
                    {
                        if (_rack.TubeList[i].Status == Status.SELECTED)
                        {
                            rackControl.UpdateTubeStatus(i, Status.READY_TO_LOAD);

                            //_rack.InitialTubeList[i].Status = Status.SELECTED;
                        }
                    }

                    for (int line = 3; line < lines.Length; line++)
                    {
                        // Split file lines into well and barcode
                        string[] splitLine = lines[line].Split('\t');
                        if (splitLine[1].Equals(barcode))
                        {
                            // If barcode has not already been used, update well status to selected
                            if (!_rack.BarcodesScanned.Contains(barcode))
                            {
                                wellNumber = splitLine[0];
                                scannedTube = rackControl.GetTubeNum(wellNumber);

                                // If the scanned barcode well is currently used, give an error message
                                if (_rack.TubeList[scannedTube].Status == Status.ERROR)
                                {
                                    MessageBox.Show("Error: There is currently a tube in the desired well. Please remove the tube and try again.");
                                }
                                else
                                {
                                    rackControl.UpdateTubeStatus(scannedTube, Status.SELECTED);

                                }
                                found = true;
                                break;
                            }
                        }
                    }

                    if (!found)
                    {
                        MessageBox.Show("Scanned barcode was not found in input file or has already been scanned");
                    }


                    /* Flash LEDs to scanned well position */
                    int row = rackControl.GetTubeNum(wellNumber) / 12;
                    int col = rackControl.GetTubeNum(wellNumber) % 12;
                    _tScanner.DleCommands.selectLED((UInt16)(row + 1), (UInt16)(col + 1), DleCommands.LedColour.LED_GREEN, DleCommands.LedState.LED_STATE_FLASHING);
                }
                //FootSwitchEvent(this, null);

                // await _tScanner.DleCommands.selectLED(3, 4, DleCommands.LedColour.LED_GREEN, DleCommands.LedState.LED_STATE_FLASHING);

                //for (int x = 0; x <= 8; x++)
                //{
                //    for (int y = 0; y <= 8; y++)
                //    {


                //        await _tScanner.DleCommands.selectLED((UInt16)(x), (UInt16)(y), DleCommands.LedColour.LED_GREEN, DleCommands.LedState.LED_STATE_FLASHING);

                //        Debug.WriteLine("x=" + x + ", y=" + y);
                //        await Task.Delay(500);
                //    }
                //}

                /* Activate placement timer and 1 second delay after scanning barcode */
                if (scanTimer != null)
                {
                    scanTimer.Stop();
                    scanTimer.Dispose();
                }
                SetTimer();
                System.Threading.Thread.Sleep(1000);
            }
        }

        /* Create a timer with a 10 second interval */
        private void SetTimer()
        {
            /* Interval can be altered in the configuration */
            scanTimer = new System.Timers.Timer(Configuration.interval*1000); 

            scanTimer.Elapsed += OnTimedEvent;
            scanTimer.AutoReset = false;
            scanTimer.Enabled = true;
        }

        /* When 10-30 second timer has expired, set selected tube back to normal if it is still unloaded */
        private async void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            int num = rackControl.GetTubeNum(wellNumber);
            if (_rack.TubeList[num].Status == Status.SELECTED)
            {
                rackControl.UpdateTubeStatus(num, Status.READY_TO_LOAD);

                /* Turn off LEDs */
                for (int row = 0; row < 8; row++)
                {
                    for (int col = 0; col < 12; col++)
                    {
                        await _tScanner.DleCommands.selectLED((UInt16)(row + 1), (UInt16)(col + 1), DleCommands.LedColour.LED_GREEN, DleCommands.LedState.LED_STATE_OFF);
                    }
                }

                MessageBox.Show(String.Format("{0} second window to place scanned tube has expired. Please rescan and try again.", Configuration.interval));
            }
        }

        /* Hides all the tube labels */
        private void button3_Click(object sender, EventArgs e)
        {
            for (int x = 0; x < _rack.TubeList.Count; x++)
            {
                rackControl.UpdateTextContent(x, eShowText.NO_SHOW);
            }
        }

        /* Shows tube labels in A-H 1-12 format */
        private void button4_Click(object sender, EventArgs e)
        {
            for (int x = 0; x < _rack.TubeList.Count; x++)
            {
                rackControl.UpdateTextContent(x, eShowText.SHOW_ID);
            }
        }

        /* Shows tube numbers in 1-96 format */
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

            if (_tScanner.dP.IsOpen && _tScanner.deviceConnectionMonitor._scannerComPortsList.Count > 0)
            {
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

                DialogResult dialogResult = MessageBox.Show("End Run?", "Ending Run", MessageBoxButtons.YesNo);

                if (dialogResult == DialogResult.Yes)
                {
                    /* Turn off barcode scanner and tube scanner LEDs */
                    await _bs.stopScan();

                    await quitToStartupAsync();

                    for (int row = 0; row < 8; row++)
                    {
                        for (int col = 0; col < 12; col++)
                        {
                            await _tScanner.DleCommands.selectLED((UInt16)(row + 1), (UInt16)(col + 1), DleCommands.LedColour.LED_GREEN, DleCommands.LedState.LED_STATE_OFF);
                        }
                    }
                }

            }
        }

        public void tubeInfoDisplay(TubeButton tb)
        {
            lbl_Status.Text = "Tube ID: " + tb.Name + '\n' + "Tube Number: " + tb.Text;
        }

        /* Called when user ends run */
        private async Task quitToStartupAsync()
        {
            /* Clear tubes */
            if (_tScanner.dP.IsOpen || Configuration.simulationMode)
            {
                await _tScanner.DleCommands.runStatus(DleCommands.RunState.STOPPED);
                for (int x = 0; x < _rack.TubeList.Count; x++)
                {
                    rackControl.UpdateTubeStatus(x, Status.NOT_USED);
                }
            }
            else
            {
                await _tScanner.DleCommands.runStatus(DleCommands.RunState.STOPPED);
                for (int x = 0; x < _rack.TubeList.Count; x++)
                {
                    rackControl.UpdateTubeStatus(x, Status.NOT_USED);
                } 

                _startupForm.tubeNotConnected();
            }

            if (!_bs.IsOpen)
            {
                _startupForm.barcodeNotConnected();
            }

            /* Clear the loaded input file */
            _startupForm.EmptyInputFile();

            /* Close test window */
            this.Hide();
        }

        private void btn_enter_Click(object sender, EventArgs e)
        {
            /* For plate barcode */
            if (!correctBarcode)
            {
                rackBarcode = txt_barcodeEntry.Text;

                correctBarcode = true;

                manualEntry = true;

                ScanTubeRack();
            }
            /* For tube barcode */
            else
            {
                barcode = txt_barcodeEntry.Text;

                ScanTube();
            }
        }

        private async void btn_Pause_Click(object sender, EventArgs e)
        {
            if (_tScanner.dP.IsOpen || Configuration.simulationMode)
            {
                await _tScanner.DleCommands.runStatus(DleCommands.RunState.PAUSED);
                scanning = false;
            }
        }
    }   
}
