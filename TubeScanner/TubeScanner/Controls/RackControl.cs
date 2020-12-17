using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using TubeScanner.Classes;

namespace TubeScanner.Controls
{
    public partial class RackControl : UserControl
    {
        private Rack _rack = null;
        private TubeRack _tubeRack = null;
        private List<TubeButton> tubeButtons = new List<TubeButton>();
        public List<TubeButton> OutputTubeList = new List<TubeButton>();

        private char[] _letters = { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H' ,'I','J','K','L','M','N'};

        public RackControl(Rack rack, TubeRack tubeRack)
        {
            _rack = rack;
            _tubeRack = tubeRack;
            InitializeComponent();
        }

        private void RackControl_Load(object sender, EventArgs e)
        {
            ReLoad();
        }

        private void ReLoad()
        {
            int Border = 10;
            int width = this.Width- Border*2 -10;
            int height = this.Height- Border*2;
            int index = 0;
                        
            Controls.Clear();
            tubeButtons.Clear();


            TableLayoutPanel panel = new TableLayoutPanel();
            panel.ColumnCount = _rack.Columns+1;
            panel.RowCount = _rack.Rows+1;
            panel.AutoSize = false;
            
           
            panel.Width = width ;
            int panelSize = panel.Width / 13;

            panel.Height = panelSize * panel.RowCount +5;
            

            int btnSize = (int)panelSize - 8;
            int fontSize = btnSize / 2 -3;

            
            panel.Location = new Point(Border, Border);
            panel.Anchor = (AnchorStyles.Bottom | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Left);

            //     panel.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;


            for (int row = 0; row < panel.RowCount; row++)
            {
                panel.RowStyles.Add(new RowStyle(SizeType.Absolute));                
                panel.RowStyles[row].Height = panelSize;              
              
            }
            for (int col = 0; col < panel.ColumnCount; col++)
            {
                panel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute));                
                panel.ColumnStyles[col].Width = panelSize;                
            }
            

            for (int row = 0; row < _rack.Rows; row++)
            {
                Label label = new Label();
                label.Text = _letters[row].ToString();
                label.Font = new Font("Arial", fontSize, FontStyle.Regular);
                label.ForeColor = Color.Navy;
                label.TextAlign = ContentAlignment.MiddleCenter;
                label.AutoSize = false;
                label.Width = panelSize;
                label.Height = panelSize-2;
             //   label.BorderStyle = BorderStyle.FixedSingle;
                label.Location = new Point(0,0);
                panel.Controls.Add(label,0,row+1);
            }

            for (int col = 0; col < _rack.Columns; col++)
            {
                Label label = new Label();
                label.ForeColor = Color.Navy;
                label.Text = (col+1).ToString();   //"D2"
                label.Font = new Font("Arial", fontSize, FontStyle.Regular);
                label.TextAlign = ContentAlignment.MiddleCenter;
              //  label.BorderStyle = BorderStyle.FixedSingle;
                label.AutoSize = false;
                label.Width = panelSize;
                label.Height = panelSize-2;
                label.Location = new Point(0, 0);
                panel.Controls.Add(label,col+1,0);
            }

            for (int row = 0; row < _rack.Rows; row++)
            {
                for (int col = 0; col < _rack.Columns; col++)
                {
                    TubeButton button = new TubeButton();
                    button.Size = new Size(btnSize, btnSize);
                   
                    button.Name = "" + _rack.TubeList[index].ID + "";
                    button.Text = _rack.TubeList[index].Number.ToString();
                    button.Click += button_Click; // Button Click Event
                    button.DoubleClick += button_DoubleClick;
                    button.Location = new Point(4, 4);
                    button.BackGroundcolor = new SolidBrush(this.BackColor);

                    button.ID = _rack.TubeList[index].ID;
                    button.Barcode = _rack.TubeList[index].Barcode;
                    button.Status = _rack.TubeList[index].Status;                
                    tubeButtons.Add(button);
                    
                    index++;
                    panel.Controls.Add(button, col+1, row+1);
                }
            }

            Controls.Add(panel);
        }

        public void Display(Control parent) => Display(parent, Point.Empty);
        public void Display(Control parent, Point position)
        {
            this.Location = position;
            parent.Controls.Add(this);
            this.BringToFront();
        }

        // Update the status of a tube at a specified index in the tube list
        public void UpdateTubeStatus(int tubeNumber, Status status)
        {            
            _rack.TubeList[tubeNumber].Status = status;
            tubeButtons[tubeNumber].Status = status;
        }

        public void UpdateTubeStatusNoNumber(string ID, Status status)
        {
            for (int tubeNumber = 0; tubeNumber < _rack.TubeList.Count; tubeNumber++)
            {
                if (_rack.TubeList[tubeNumber].ID == ID)
                {
                    _rack.TubeList[tubeNumber].Status = status;
                    tubeButtons[tubeNumber].Status = status;
                }
            }
        }

        // Find the number of a tube in tube list from the well ID
        public int GetTubeNum(string ID)
        {
            int num = 0;
            for (int tubeNumber = 0; tubeNumber < _rack.TubeList.Count; tubeNumber++)
            {
                if (_rack.TubeList[tubeNumber].ID == ID)
                {
                    num = tubeNumber;
                    break;
                }
            }
            return num;
        }

        public void UpdateTextContent(int tubeNumber, eShowText showText)
        {            
            tubeButtons[tubeNumber].ShowText = showText;
        }

        private void RackControl_Resize(object sender, EventArgs e)
        {
            ReLoad();
        }

        // Update well status on click for testing purposes only
        private void button_Click(object sender, EventArgs e)
        {
            TubeButton TB = sender as TubeButton;
            TB.Enabled = false;

            TubeButton dummy = new TubeButton();

            bool wasSelected = false;

            if (TB != null)
            {
                dummy.ID = TB.ID;
                dummy.Barcode = "N/A"; // Default N/A if no barcode has been scanned

                if (TB.Status == Status.SELECTED)
                {
                    wasSelected = true;
                    TB.Status = Status.LOADED;
                    dummy.Status = Status.LOADED;
                    dummy.Barcode = TB.Barcode;
                    _rack.BarcodesScanned.Add(TB.Barcode);
                    _rack.WellsUsed.Add(TB.ID);
                    UpdateTubeStatus(int.Parse(TB.Text)-1, Status.LOADED);
                }
                else if (TB.Status == Status.READY_TO_LOAD)
                {
                    TB.Status = Status.ERROR;
                    dummy.Status = Status.ERROR;
                    UpdateTubeStatus(int.Parse(TB.Text)-1, Status.ERROR);
                    MessageBox.Show("Warning: Tube placement does not match input file.");
                }
                else if (TB.Status == Status.LOADED)
                {
                    TB.Status = Status.REMOVED;
                    dummy.Status = Status.REMOVED;
                    dummy.Barcode = TB.Barcode;
                    MessageBox.Show("Warning: Tube at " + TB.ID + " has been removed.");
                    _rack.BarcodesScanned.Remove(TB.Barcode);
                    _rack.WellsUsed.Remove(TB.ID);
                    UpdateTubeStatus(int.Parse(TB.Text)-1, Status.REMOVED);
                }
                else if (TB.Status == Status.REMOVED)
                {
                    TB.Status = Status.ERROR;
                    dummy.Status = Status.ERROR;
                    UpdateTubeStatus(int.Parse(TB.Text) - 1, Status.ERROR);
                    MessageBox.Show("Warning: Tube placement does not match input file.");
                }
                else if (TB.Status == Status.NOT_USED)
                {
                    TB.Status = Status.ERROR;
                    dummy.Status = Status.ERROR;
                    UpdateTubeStatus(int.Parse(TB.Text)-1, Status.ERROR);
                    MessageBox.Show("Warning: Tube placement does not match input file.");
                }
                else if (TB.Status == Status.ERROR)
                {
                    if (_rack.InitialTubeList[int.Parse(TB.Text) - 1].Status == Status.NOT_USED)
                    {
                        TB.Status = Status.NOT_USED;
                        UpdateTubeStatus(int.Parse(TB.Text) - 1, Status.NOT_USED);
                    }
                    else if (_rack.InitialTubeList[int.Parse(TB.Text) - 1].Status == Status.READY_TO_LOAD)
                    {
                        TB.Status = Status.READY_TO_LOAD;
                        UpdateTubeStatus(int.Parse(TB.Text) - 1, Status.READY_TO_LOAD);
                    }
                    dummy.Status = Status.REMOVED;
                    MessageBox.Show("Warning: Tube at " + TB.ID + " has been removed.");
                }

                if (!wasSelected)
                {
                    for (int i = 0; i < _rack.TubeList.Count; i++)
                    {
                        if (_rack.TubeList[i].Status == Status.SELECTED)
                        {
                            UpdateTubeStatus(i, Status.READY_TO_LOAD);
                            dummy.Barcode = _rack.TubeList[i].Barcode;
                            break;
                        }
                    }
                }

                OutputTubeList.Add(dummy);

            }

            TB.Enabled = true;
        }

        // Show well info when it is double clicked
        private void button_DoubleClick(object sender, EventArgs e)
        {
            TubeButton TB = sender as TubeButton;

            if (TB != null)
            {
                //MessageBox.Show("Tube ID: " + TB.Name + '\n' + "Tube Number: " + TB.Text);
                _tubeRack.tubeInfoDisplay(TB);
            }
        }
        
    }
}
