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
    class FileManager
    { 

        public static async Task<bool> LoadInputFile(Rack rack)
        {
            bool valid = true;

            if (File.Exists(rack.InputFilename))
            {
                string[] inputLines = File.ReadAllLines(rack.InputFilename);
                
                /* First, we want to check no lines are empty, duplicated or exceed the list length over 96 */
                List<string> usedLines = new List<string>();

                string[] splitLine;

                for (int i = 0; i < inputLines.Length; i++)
                {
                    /* Check empty */
                    if (inputLines[i] != "")
                    {
                        bool isUnique = true;
                        splitLine = inputLines[i].Split('\t');

                        /* check duplicates */
                        if (usedLines.Count() > 0)
                        {
                            for (int u = 0; u < usedLines.Count(); u++)
                            {
                                string[] splitUsed = usedLines[u].Split('\t');
                                if (splitUsed[0] == splitLine[0] || splitUsed[1] == splitLine[1])
                                {
                                    isUnique = false;
                                }
                            }
                        }
                        if (isUnique)
                            usedLines.Add(inputLines[i]);
                    }

                    if (usedLines.Count() >= rack.TubeList.Count())
                    {
                        break;
                    }
                }

                /* Read header- Plate ID */
                bool hFound = false;
                for (int index = 0; index < usedLines.Count(); index++)
                {
                    var header = usedLines[index].Split('\t');

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

                /* Tube data lines */
                for (int lineNumber = 3; lineNumber < usedLines.Count(); lineNumber++)
                {
                    if (usedLines[lineNumber].Trim() == "End of File")
                        break;

                    string[] contents = usedLines[lineNumber].Split('\t');

                    /* Check if position valid (format: A01) */
                    if (InputValid(contents[0]))
                    {
                        for (int index = 0; index < rack.TubeList.Count; index++)
                        {
                            if (rack.TubeList[index].ID.Equals(contents[0]))
                            {
                                rack.TubeList[index].Barcode = contents[1];
                                rack.TubeList[index].Status = Status.READY_TO_LOAD;
                            }
                        }
                    }
                    else
                    {
                        //MessageBox.Show("Line" + (lineNumber + 1) + ": Tube position invalid, use format [row],[0],[column] \n e.g. A01");
                        valid = false;
                    }
                }
            }
            return valid;
        }

        private static bool InputValid(string line)
        {
            int rackLength = 13;
            List<char> rackLetters = new List<char> { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H' };
            char[] TPos = line.ToCharArray();

            /* Position (e.g. A01) */
            if (!rackLetters.Contains(TPos[0]))
            {
                return false;
            }

            int tubePosition = (Int32.Parse(TPos[1].ToString()) * 10) + Int32.Parse(TPos[2].ToString());
            if (tubePosition <= 0 || tubePosition > rackLength)
            {
                return false;
            }

            return true;
        }


        public static void WriteOutputFile(string filename, List<Tube> tList, string plateID, string userID, string date)
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
