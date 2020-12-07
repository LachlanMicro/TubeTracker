using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TubeScanner.Classes
{
    public enum eShowText
    {
        NO_SHOW,
        SHOW_ID,
        SHOW_NUMBER
    }



    public class TubeButton : System.Windows.Forms.Button
    {

        private Brush _backGroundcolor = Brushes.LightGray;
        private Color _foreGroundcolor = Color.Black;
        public Status _status = Status.NOT_USED;
        public eShowText _showText = eShowText.NO_SHOW;
        private string _id;
        private string _barcode;

        public string Barcode
        {
            get { return _barcode; }
            set { _barcode = value; }
        }

        public string ID
        {
            get { return _id; }
            set { _id = value; }
        }

        private SolidBrush[] brushes = new SolidBrush[] {
        new SolidBrush(Color.LightCyan),    //EMPTY
        new SolidBrush(Color.LightGreen),   //READY_TO_LOAD
        new SolidBrush(Color.Green),         //LOADED
        new SolidBrush(Color.Red),              //ERROR
        new SolidBrush(Color.OrangeRed) };        //REMOVED

        public eShowText ShowText
        {
            get { return _showText; }
            set { _showText = value; Invalidate(); }
        }

        public Status Status
        {
            get { return _status; }
            set { _status = value; Invalidate(); }
        }

        public Brush BackGroundcolor
        {
            get { return _backGroundcolor; }
            set { _backGroundcolor = value; Invalidate(); }
        }

        public Color ForeGroundcolor
        {
            get { return _foreGroundcolor; }
            set { _foreGroundcolor = value; Invalidate(); }
        }


        public TubeButton()
        {
            SetStyle(ControlStyles.StandardClick | ControlStyles.StandardDoubleClick, true);
            FlatAppearance.BorderSize = 0;
            FlatStyle = FlatStyle.Flat;
        }

     

        protected override void OnPaint(PaintEventArgs e)
        {
                   
            Graphics canvas = e.Graphics;            
            canvas.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            
            canvas.FillRectangle(_backGroundcolor, -1, -1, Height+2, Height+2);
            canvas.FillEllipse(Brushes.Gray, 0, 0, Height , Height);
            canvas.FillEllipse(brushes[(int)Status], 2, 2, Height - 4, Height - 4);


            //  StringFormat drawFormat = new StringFormat();
            //   drawFormat.Alignment = StringAlignment.Center;
            //   canvas.DrawString(Text, Font, new SolidBrush(ForeColor), 0,0, drawFormat);
            if (ShowText != eShowText.NO_SHOW)
            {
                RectangleF drawRect = new RectangleF(0, 0, Width, Height);

                // Draw rectangle to screen.
                Pen blackPen = new Pen(Color.Black);
                //e.Graphics.DrawRectangle(blackPen, x, y, width, height);

                // Draw string to screen.
                //   canvas.DrawString(Text, Font, Brushes.Gray, drawRect);

                Font drawFont = new Font("Arial", Height / 5);
                SolidBrush drawBrush = new SolidBrush(Color.Navy);

                // Create point for upper-left corner of drawing.
                Point drawPoint = new Point(Width / 2, Height / 2);

                // Set format of string.
                StringFormat drawFormat = new StringFormat();
                drawFormat.Alignment = StringAlignment.Center;
                drawFormat.LineAlignment = StringAlignment.Center;

                // Draw string to screen.
                if (ShowText == eShowText.SHOW_ID)
                {
                    canvas.DrawString(Name, drawFont, drawBrush, drawPoint, drawFormat);
                }
                else
                {
                    canvas.DrawString(Text, drawFont, drawBrush, drawPoint, drawFormat);
                }
            }
        }
    }
}
