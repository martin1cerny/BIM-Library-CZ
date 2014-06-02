using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace TSKP
{
    class Program
    {
        static void Main(string[] args)
        {
            var output = @"TSKP";
            if (!Directory.Exists(output))
                Directory.CreateDirectory(output);

            var input = "141_TSKP_CS-ÚRS.txt";
            if (!File.Exists(input))
                throw new Exception("Input file doesn't exist");

            var lines = File.ReadAllLines(input);

            //this is a cache for the nodes so that it is not nevessary to search for the parent
            var nodes = new string[10];
            nodes[0] = "";

            var tskp = File.CreateText(Path.Combine(output, "TSKP 1.4.1.csv"));
            StreamWriter tskpPart = null;
            var format = "\"{0}\",\"{1}\",\"{2}\"";
            var header = String.Format(format, "Code", "Description", "Parent");

            tskp.WriteLine(header);

            //skip header and parse all lines
            for (int i = 1; i < lines.Length; i++)
            {
                var line = lines[i];
                var fields = line.Split('\t');

                var level = int.Parse(fields[0]);
                var code = fields[1];
                var description = fields[2];

                //set actual node code on the level
                nodes[level] = code;

                //create part file
                if (level == 1)
                {
                    //close last part
                    if (tskpPart != null)
                        tskpPart.Close();
                    var partName = String.Format("TSKP 1.4.1 - {0} - {1}.csv", code, description);
                    tskpPart = File.CreateText(Path.Combine(output, partName));
                    tskpPart.WriteLine(header);
                }

                var parent = nodes[level - 1];
                var csvLine = String.Format(format, code, description, parent);

                tskp.WriteLine(csvLine);
                if (level != 1)
                    tskpPart.WriteLine(csvLine);
            }
            tskp.Close();
            tskpPart.Close();
        }
    }
}
