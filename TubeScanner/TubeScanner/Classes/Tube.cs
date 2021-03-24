using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TubeScanner.Classes
{
   public enum Status
    {
        NOT_USED        = 0,
        READY_TO_LOAD   = 1,
        LOADED          = 2,
        ERROR           = 3,
        REMOVED         = 4,
        SELECTED        = 5,
    }

    public class Tube
    {
        private string _id;
        private int _number;

        private string _barcode;
        public int _fillOrder;
        public Status Status;

        public int Number 
        {
            get { return _number; }
            set { _number = value; }
        }  
        
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

        public int FillOrder
        { 
            get { return _fillOrder; }
            set { _fillOrder = value; }
        }

        public Tube(string ID, int number)
        {
            _id = ID;
            _number = number;
            _barcode = "";
            Status = Status.NOT_USED;
        }
    }
}
