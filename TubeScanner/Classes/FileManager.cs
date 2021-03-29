using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TubeScanner.Classes
{
    class FileManager
    { 
        /* Loads in valid input file */
        public static async Task<bool> LoadInputFile(Rack rack)
        {
            bool valid = true;

            if (File.Exists(rack.InputFilename))
            {
                string[] inputLines = File.ReadAllLines(rack.InputFilename);
                
                /* First, we want to check no lines are empty, duplicated or exceed the list length over 96 */
                List<string> usedLines = new List<string>();

                for (int i = 0; i < inputLines.Length; i++)
                {
                    /* Check if empty */
                    if (inputLines[i] != "")
                    {
                        bool isUnique = true;

                        /* check duplicates */
                        if (usedLines.Count() > 0)
                        {
                            for (int j = 0; j < usedLines.Count(); j++)
                            {
                                if (usedLines[j] == inputLines[i])
                                {
                                    isUnique = false;
                                }
                            }
                        }
                        if (isUnique)
                        {
                            usedLines.Add(inputLines[i]);
                        }
                    }

                    if (usedLines.Count() >= rack.TubeList.Count())
                    {
                        break;
                    }
                }

                /* Read header- Plate ID */
                var header = inputLines[0].Split('\t');

                if (header[0] == "Plate ID" & header.Length == 2)
                {
                    rack.PlateID = header[1];
                }
                else
                {
                    MessageBox.Show("Plate ID not found!");
                    valid = false;
                }

                /* Check subsequent 2 lines before rack info */
                var line2 = inputLines[1];
                var line3 = inputLines[2].Split('\t');

                if (line2 == "")
                {
                    if (line3[0] != "Position" || line3.Length != 2)
                    {
                        valid = false;
                    }
                    else
                    {
                        if (line3[1] != "Lab number")
                        {
                            valid = false;
                        }
                    }
                }
                else
                {
                    valid = false;
                }

                if (valid)
                {
                    /* Tube data lines */
                    for (int lineNumber = 2; lineNumber < usedLines.Count(); lineNumber++)
                    {
                        if (usedLines[lineNumber].Trim() == "End of File")
                            break;

                        string[] contents = usedLines[lineNumber].Split('\t');

                        if (contents.Length == 2)
                        {
                            /* Check if position valid (format: A01) */
                            if (InputValid(contents[0]))
                            {
                                for (int index = 0; index < rack.TubeList.Count; index++)
                                {
                                    if (rack.TubeList[index].ID.Equals(contents[0]) && rack.TubeList[index].Barcode.Length < 1)
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
                                MessageBox.Show("Line" + (lineNumber + 1) + ": Tube position invalid, use format [row],[0],[column] \n e.g. A01");
                                valid = false;
                            }
                        }
                        else
                        {
                            valid = false;
                        }
                    }
                }
            }
            return valid;
        }

        /* Computation to check whether the input file is valid for use within program */
        private static bool InputValid(string line)
        {
            int rackLength = 12;
            List<char> rackLetters = new List<char> { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H' };
            char[] TPos = line.ToCharArray();

            if (line != "" && TPos.Length == 3)
            {
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
            }
            else
            {
                return false;
            }

            return true;
        }

        // Write all tube placements and removals to the output file
        public static void WriteOutputFileHeaders(string filename, string plateID, string date)
        {   
            List<string> outputContent = new List<string>();

            /* Add file header */
            outputContent.Add("BSD Tracker," + Program.currentUser + "," + plateID  + ',' + date);

            /* Add a heading for the tube results */
            outputContent.Add("Grid Ref,Sample Barcode,Fill Order,Comment");

            File.WriteAllLines(filename, outputContent);
        }

        public static void WriteTubeToOutputFile(string filename, string plateID, Tube currTube)
        {
            List<string> outputContent = new List<string>();

            /* For each tube on tube rack, log the ID, barcode, fill order and a message based on the status at that time */
            
            if (currTube.Status == Status.ERROR)
            {
                outputContent.Add(currTube.ID + "," + currTube.Barcode + "," + currTube.FillOrder + "," + "Tube placed incorrectly");
            }
            if (currTube.Status == Status.REMOVED)
            {
                outputContent.Add(currTube.ID + "," + currTube.Barcode + "," + currTube.FillOrder + "," + "Tube removed");
            }
            if (currTube.Status == Status.LOADED)
            {
                outputContent.Add(currTube.ID + "," + currTube.Barcode + "," + currTube.FillOrder + "," + "Tube placed correctly");
            }

            File.AppendAllLines(filename, outputContent);
        }
    }
}
