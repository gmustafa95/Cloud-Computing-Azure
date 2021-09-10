using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;

namespace MyExperiment.Utilities
{
    public static class FileUtilities
    {
        /// <summary>
        /// Reading the text 
        /// </summary>
        /// <param name="localfilePath"></param>
        /// <returns></returns>
        public static string ReadFile(string localFilePath)
        {
            string jsonString = File.ReadAllText(localFilePath);
            return jsonString;
        }

       
        public static void CellsFile(string fileName, string data)
        {
            // Create a local file in the ./data/ directory for uploading and downloading
            string localfilePath = Path.Combine(Experiment.DataFolder, fileName);

            if (!File.Exists(localfilePath))
            {
                File.Create(localfilePath);
            }

            StreamWriter sw = File.AppendText(localfilePath);
            string[] split = data.Split(',');
            int[] intArray = Array.ConvertAll(split, int.Parse);
            double avg = Queryable.Average(intArray.AsQueryable());

            try
            {
                sw.WriteLine();
                sw.WriteLine("===========================");
                sw.WriteLine("Cells Per Column Experiment");
                sw.WriteLine("===========================");
               var count = 1;
                int summ = 0;
                // sw.Write("Output for nummber of cycles : " + data);
                foreach (int s in intArray)
                {
                   summ = s+ summ ;
                    sw.Write("Number of Cycles in Iteration No: " + count+ " : " + s + " \n ");
                    count++;
                }
                sw.WriteLine();
                sw.WriteLine("Average Cycles: " + avg);

            }
            finally
            {
                sw.WriteLine();
                sw.WriteLine("================");
                sw.WriteLine("Experiment End");
                sw.WriteLine("================");
                sw.WriteLine();
                sw.Flush();
                sw.Close();
            }
        }

// ------------------------------------------------------------------------------------------------------//  
        public static void MaxMinFile(string fileName, string data) 
        {
            // Create a local file in the ./data/ directory for uploading and downloading
            string localfilePath = Path.Combine(Experiment.DataFolder, fileName);

            if (!File.Exists(localfilePath))
            {
                File.Create(localfilePath);
            }

            StreamWriter sw = File.AppendText(localfilePath);
            string[] split = data.Split(',');
            int[] intArray = Array.ConvertAll(split, int.Parse);
            double avg = Queryable.Average(intArray.AsQueryable());

            try
            {
                sw.WriteLine();
                sw.WriteLine("===========================");
                sw.WriteLine("Max Min Value Experiment");
                sw.WriteLine("===========================");
                var count = 1;
                int summ = 0;
                // sw.Write("Output for nummber of cycles : " + data);
                foreach (int s in intArray)
                {
                    summ = s + summ;
                    sw.Write("Number of Cycles in Iteration No: " + count + " : " + s + " \n ");
                    count++;
                }
                sw.WriteLine();
                sw.WriteLine("Average Cycles: " + avg);

            }
            finally
            {
                sw.WriteLine();
                sw.WriteLine("================");
                sw.WriteLine("Experiment End");
                sw.WriteLine("================");
                sw.WriteLine();
                sw.Flush();
                sw.Close();
            }
        }
// ------------------------------------------------------------------------------------------------------//  
        public static void MiniColumnsFile (string fileName, string data)
        {
            // Create a local file in the ./data/ directory for uploading and downloading
            string localfilePath = Path.Combine(Experiment.DataFolder, fileName);

            if (!File.Exists(localfilePath))
            {
                File.Create(localfilePath);
            }

            StreamWriter sw = File.AppendText(localfilePath);
            string[] split = data.Split(',');
            int[] intArray = Array.ConvertAll(split, int.Parse);
            double avg = Queryable.Average(intArray.AsQueryable());

            try
            {
                sw.WriteLine();
                sw.WriteLine("===========================");
                sw.WriteLine("Mini Columns Experiment");
                sw.WriteLine("===========================");
                var count = 1;
                int summ = 0;
                // sw.Write("Output for nummber of cycles : " + data);
                foreach (int s in intArray)
                {
                    summ = s + summ;
                    sw.Write("Number of Cycles in Iteration No: " + count + " : " + s + " \n ");
                    count++;
                }
                sw.WriteLine();
                sw.WriteLine("Average Cycles: " + avg);

            }
            finally
            {
                sw.WriteLine();
                sw.WriteLine("================");
                sw.WriteLine("Experiment End");
                sw.WriteLine("================");
                sw.WriteLine();
                sw.Flush();
                sw.Close();
            }
        }
    }
}


