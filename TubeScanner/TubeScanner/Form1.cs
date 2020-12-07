using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
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
            //_rack = new Rack(8, 12); 

            rackControl = new RackControl(_rack);
            rackControl.Display(this, new Point(10, 75));
            rackControl.Anchor = (AnchorStyles.Bottom | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Left);

            lbl_PlateID.Text = _rack.PlateID;

            //_tScanner = new TScanner();
            _tScanner.DleCommands.OnFootSwitchEvent += FootSwitchEvent;
            //_tScanner.autoConnect();
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

                Byte[] tubeData = await _tScanner.DleCommands.scanAllTubes();

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
                        Console.WriteLine(binary_column);
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
                CreateParams myCp = base.CreateParams;
                myCp.ClassStyle = myCp.ClassStyle | CP_NOCLOSE_BUTTON;
                return myCp;
            }
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            if (_tScanner.dP.IsOpen)
            {
                await _tScanner.DleCommands.runStatus(DleCommands.RunState.RUNNING);
            }

            /*
            string barcode = String.Empty;

            if (_bs.IsOpen)
            {
                barcode = await _bs.startScan();
            }

            lbl_Barcode.Text = barcode;*/
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            if (_tScanner.dP.IsOpen)
            {
                await _tScanner.DleCommands.runStatus(DleCommands.RunState.PAUSED);
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
            /* Save output file */
            OutputFile outfile = new OutputFile();
            outfile.WriteOutputFile("Z:/ENGINEERING/BSD Tracker/John Tongue Supplied/30-11-20/output log 2.txt", _rack.TubeList, _rack.PlateID, "aaaa", DateTime.Today.ToString());

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
