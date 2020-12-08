using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace TubeScanner.Classes
{
    public class FileManager
    {

        private string[] _inputFile;

        public string[] InputFile
        {
            get { return _inputFile; }
            set { _inputFile = value; }
        }

        public FileManager()
        {
            string[] input = _inputFile;
        }

        public async Task<bool> LoadInputFile(Rack rack)
        {
            if (File.Exists(rack.InputFilename))
            {
                var lines = File.ReadAllLines(rack.InputFilename);
                //InputFile = lines;

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

                                rack.InitialTubeList[index].Barcode = contents[1];
                                rack.InitialTubeList[index].Status = Status.READY_TO_LOAD;
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


        public void WriteOutputFile(string filename, List<TubeButton> tList, string plateID, string userID, string date)
        {
            List<string> outputContent = new List<string>();

            outputContent.Add("Plate ID\t" + plateID);
            outputContent.Add("User ID " + userID);
            outputContent.Add("Date " + date);
            outputContent.Add("Position\tLab Number\tErrors");

            for (int i = 0; i < tList.Count; i++)
            {
                /* if tube has no barcode, do not add to output file */
                //if (tList[i].Barcode != "")
                //{
                    if (tList[i].Status == Status.ERROR)
                    {
                        outputContent.Add(tList[i].ID + "\t" + tList[i].Barcode + "\t" + "Tube placed in wrong well but accepted");
                    }
                    if (tList[i].Status == Status.REMOVED)
                    {
                        outputContent.Add(tList[i].ID + "\t" + tList[i].Barcode + "\t" + "Tube removed");
                    }
                    if (tList[i].Status == Status.LOADED)
                    {
                        outputContent.Add(tList[i].ID + "\t" + tList[i].Barcode + "\t" + "Tube placed correctly");
                    }
                //}
            }

            File.WriteAllLines(filename, outputContent);
        }

    }
}
