using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Threading;
using System.Data.SqlClient;
using System.Net;
using System.Net.Http;
using ChoETL;
using System.Windows.Forms;

namespace CSV_Merge
{
    internal class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Console.WriteLine("Select First CSV File...");
            Thread.Sleep(750);
            OpenFileDialog oFD_1 = new OpenFileDialog();
            oFD_1.ShowDialog();
            string pathOne = Path.GetFullPath(oFD_1.FileName);
            Console.Write("Path One: " + pathOne);
            Console.WriteLine();
            Console.WriteLine();

            Console.WriteLine("Select Second CSV File...");
            Thread.Sleep(750);
            OpenFileDialog oFD_2 = new OpenFileDialog();
            oFD_2.ShowDialog();
            string pathTwo = Path.GetFullPath(oFD_2.FileName);
            Console.Write("Path Two: " + pathTwo);
            Console.WriteLine();
            Console.WriteLine();

            Console.WriteLine("Enter the name of the result file...");
            string resultFileName = Path.GetDirectoryName(pathTwo) + "\\" + Console.ReadLine() + ".csv";
            Console.Write("Result Path: " + resultFileName);
            Console.WriteLine();
            Console.WriteLine();
            //Console.WriteLine("pathOne: " + pathOne); // Debugging Purposes Only
            //Console.WriteLine("pathTwo" + pathTwo); // Debugging Purposes Only
            //Console.WriteLine("resultFileName" + resultFileName); // Debugging Purposes Only

            Csv_Merger(pathOne, pathTwo, resultFileName); // Invoke Merger Method

            Console.WriteLine("Done...");
            Console.ReadKey();
        }

        static void Csv_Merger(string pathOne, string pathTwo, string resultFileName)
        {
            ChoCSVRecordConfiguration config = null; // Initialize config settings

            List<object> items = new List<object>();

            foreach (var file in Directory.GetFiles(Path.GetDirectoryName(pathOne), "*.csv", SearchOption.TopDirectoryOnly))
            {
                using (var r1 = new ChoCSVReader(Path.GetFullPath(file)).WithFirstLineHeader())
                {
                    items.Add(r1.First()); // Adds CSV as object to list
                }
            }

            StringBuilder csv = new StringBuilder();
            using (var w = new ChoCSVWriter(csv).WithFirstLineHeader().WithMaxScanRows(5).ThrowAndStopOnMissingField(false))
            {
                w.Write(items);
                // Only used to get configuration for these files
                config = w.Configuration;
            }

            using (var r1 = new ChoCSVReader(pathOne).WithFirstLineHeader())
            {
                using (var r2 = new ChoCSVReader(pathTwo).WithFirstLineHeader())
                {
                    using (var w1 = new ChoCSVWriter(resultFileName, config).WithFirstLineHeader())
                    {
                        w1.Write(r1.Concat(r2)); // Writes the result of concatenating 
                    }
                }
            }
        }
    }
}
