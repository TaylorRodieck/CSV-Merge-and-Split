using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Net;
using System.Net.Http;
using ChoETL;

namespace CSV_Merge
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ChoCSVRecordConfiguration config = null;

            List<object> items = new List<object>();

            foreach (var file in Directory.GetFiles(@"X:\VisualStudioProjectsTesting\CSV_Operations", "*.csv", SearchOption.TopDirectoryOnly))
            {
                using (var r1 = new ChoCSVReader(Path.GetFullPath(file)).WithFirstLineHeader())
                {
                    items.Add(r1.First());
                }
            }

            StringBuilder csv = new StringBuilder();
            using (var w = new ChoCSVWriter(csv).WithFirstLineHeader().WithMaxScanRows(5).ThrowAndStopOnMissingField(false))
            {
                w.Write(items);
                //Capture configuration for later use to merge CSV files
                config = w.Configuration;
            }

            using (var r1 = new ChoCSVReader(@"X:\VisualStudioProjectsTesting\CSV_Operations\test1.csv").WithFirstLineHeader())
            {
                using (var r2 = new ChoCSVReader(@"X:\VisualStudioProjectsTesting\CSV_Operations\test2.csv").WithFirstLineHeader())
                {
                    using (var w1 = new ChoCSVWriter(@"X:\VisualStudioProjectsTesting\CSV_Operations\combined.csv", config).WithFirstLineHeader())
                    {
                        w1.Write(r1.Concat(r2));
                        //Console.WriteLine(w1.ToString());


                    }
                }
            }

            Console.WriteLine("Done...");
            Console.ReadKey();
        }
    }
}
