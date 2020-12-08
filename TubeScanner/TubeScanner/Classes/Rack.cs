using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TubeScanner.Classes
{
    public class Rack
    {
        private int _rows = 0;
        private int _columns = 0;
        private string _plateID;
        private string _inputFilename = string.Empty;

        public List<Tube> TubeList = new List<Tube>();
        public List<Tube> InitialTubeList = new List<Tube>();
        public List<string> BarcodesScanned = new List<string>();
        public List<string> WellsUsed = new List<string>();

        public int Rows { get { return _rows; } }
        public int Columns { get { return _columns; } }

        public string InputFilename
        {
            get { return _inputFilename; }
            set { _inputFilename = value; }
        }

        public string PlateID
        {
            get { return _plateID; }
            set { _plateID = value; }
        }

        public Rack(int rows,int columns)
        {
            _rows = rows;
            _columns = columns;
            int numTubes = _rows * _columns;

            char[] letters = { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H' };
            int tubeNumber = 1;
            for(int row = 0; row < _rows; row++)
            {
                for (int col = 0; col < _columns; col++)
                {
                    string id = letters[row].ToString();
                    id += (col + 1).ToString("D2");
                    Tube tube = new Tube(id, tubeNumber++);
                    TubeList.Add(tube);
                    Tube initialTube = new Tube(id, tubeNumber);
                    InitialTubeList.Add(initialTube);
                }
            }
        }
    }
}
