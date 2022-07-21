using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using ChoETL;
using System.Windows.Forms;

namespace CSV_MergeAndSplit
{
    internal class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            InitialSetup();
            Console.WriteLine("To Process Another File, Press 'F'...");
            string reProcess = Console.ReadLine();
            if (reProcess == "F")
            {
                InitialSetup();
            }

            Console.WriteLine("Done...");
            Console.ReadKey();
        }

        static void InitialSetup()
        {
            Console.WriteLine("Enter 'SPLIT' to split a csv and 'MERGE' to merge two CSV's...");
            string choice = Console.ReadLine();
            Console.WriteLine();
            Console.WriteLine();

            if (choice.ToLower() == "split")
            {
                csvDocument.splitCSV();
            }
            else if (choice.ToLower() == "merge")
            {
                CSV_MergerSetUp();
            }
        }

        static void CSV_MergerSetUp()
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
        }

        [STAThread]
        static void Csv_Merger(string pathOne, string pathTwo, string resultFileName)
        {
            //ChoCSVRecordConfiguration config = null; // Initialize config settings
            ChoCSVRecordConfiguration config = new ChoCSVRecordConfiguration();
            

            List<object> items = new List<object>();

            foreach (var file in Directory.GetFiles(Path.GetDirectoryName(pathOne), "*.csv", SearchOption.TopDirectoryOnly))
            {
                using (var r1 = new ChoCSVReader(Path.GetFullPath(file)).WithFirstLineHeader().ThrowAndStopOnMissingField(false))
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

        public class csvDocument
        {
            public string FilePath;
            public bool HasHeaders;
            public int RowSplitCount;
            private int TotalRowCount;

            public csvDocument(string filePath, bool hasHeaders, int rowSplitCount, int totalRowCount = 0)
            {
                FilePath = filePath;
                HasHeaders = hasHeaders;
                RowSplitCount = rowSplitCount;
                TotalRowCount = totalRowCount;
            }

            public static void splitCSV()
            {
                OpenFileDialog ofd_1 = new OpenFileDialog();
                ofd_1.ShowDialog();
                string filePath = ofd_1.FileName;
                Console.WriteLine("Path of Selected File to be Split :: " + filePath);

                Console.WriteLine();
                Console.Write("Row Limit Per File :: ");
                int rowLimit = int.Parse(Console.ReadLine());

                //string filePath = csvDoc.FilePath;
                //int rowLimit = csvDoc.RowSplitCount;
                using (System.IO.StreamReader sr = new System.IO.StreamReader(filePath))
                {
                    int fileNumber = 0;

                    while (!sr.EndOfStream)
                    {
                        int count = 0;
                        //Console.WriteLine(Directory.GetParent(filePath) + "\\" + Path.GetFileNameWithoutExtension(filePath) + "_Split_" + ++fileNumber + ".csv");

                        using (System.IO.StreamWriter sw = new System.IO.StreamWriter(Directory.GetParent(filePath) + "\\" + Path.GetFileNameWithoutExtension(filePath) + "_Split_" + ++fileNumber + ".csv"))
                        {
                            sw.AutoFlush = true;

                            while (!sr.EndOfStream && ++count < rowLimit)
                            {
                                sw.WriteLine(sr.ReadLine());

                            }
                        }
                    }
                }
            }
        }
    }
}
