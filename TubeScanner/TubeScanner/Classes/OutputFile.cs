using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TubeScanner.Classes
{
    class OutputFile
    { 

        public OutputFile()
        {

        }

        public void WriteOutputFile(string filename, List<Tube> tList, string plateID, string userID, string date)
        {
            List<string> outputContent = new List<string>();

            outputContent.Add("Plate ID\t" + plateID);
            outputContent.Add("User ID " + userID);
            outputContent.Add("Date " + date);
            outputContent.Add("Position\tLab Number\tErrors");

            for (int i = 0; i < tList.Count; i++)
            {
                /* if tube has no barcode, do not add to output file */
                if (tList[i].Barcode != "")
                {
                    outputContent.Add(tList[i].ID + "\t" + tList[i].Barcode);
                    /* TODO add Error to each line */
                }
            }

            File.WriteAllLines(filename, outputContent);
        }

    }
}
