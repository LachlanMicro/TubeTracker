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

namespace TubeScanner.Controls
{
    public partial class RackControl : UserControl
    {
        private Rack _rack = null;
        private List<TubeButton> tubeButtons = new List<TubeButton>();

        private char[] _letters = { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H' ,'I','J','K','L','M','N'};

        public RackControl(Rack rack)
        {
            _rack = rack;
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

        public void UpdateTubeStatus(int tubeNumber, Status status)
        {            
            _rack.TubeList[tubeNumber].Status = status;
            tubeButtons[tubeNumber].Status = status;
        }

        public void UpdateTextContent(int tubeNumber, eShowText showText)
        {            
            tubeButtons[tubeNumber].ShowText = showText;
        }

        private void RackControl_Resize(object sender, EventArgs e)
        {
            ReLoad();
        }

        private async void button_Click(object sender, EventArgs e)
        {
            TubeButton TB = sender as TubeButton;
            
            if (TB != null)
            {
                TB.Status = Status.NOT_USED;
            }
        }

        private async void button_DoubleClick(object sender, EventArgs e)
        {
            TubeButton TB = sender as TubeButton;

            if (TB != null)
            {
                MessageBox.Show("Tube ID: " + TB.Name + '\n' + "Tube Number: " + TB.Text);
            }
        }
        
    }
}
