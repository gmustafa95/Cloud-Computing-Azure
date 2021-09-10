using NeoCortexApi;
using NeoCortexApi.Encoders;
using NeoCortexApi.Entities;
using NeoCortexApi.Network;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
//using UnitTestsProject;

namespace ConsoleApp1


{
    public class CellPerColumnExperiment
    {
        //private List<int> cycless;
        
        

        //int c = 2;
        public List<string>  CellPerColumn (int cells, int iterations)
         
        {
            List<int> cycless = new List<int>();
            //* string filename = "Cells=" + c + ".csv";
            //*using (StreamWriter writer = new StreamWriter(filename))
            //*{
            Debug.WriteLine($"Learning Cycles: {400}");
            Debug.WriteLine("Cycle;Similarity");
            

            for (int j = 0; j < iterations; j++) //loop=5
            {
                int inputBits = 200;
                bool learn = true;
                Parameters p = Parameters.getAllDefaultParameters();
#pragma warning disable CS0436 // Type conflicts with imported type
                p.Set(KEY.RANDOM, new ThreadSafeRandom(42));
#pragma warning restore CS0436 // Type conflicts with imported type
                p.Set(KEY.INPUT_DIMENSIONS, new int[] { inputBits });

                p.Set(KEY.COLUMN_DIMENSIONS, new int[] { 500 });
                p.Set(KEY.CELLS_PER_COLUMN, cells);

                NeoCortexApi.Network.CortexNetwork net = new NeoCortexApi.Network.CortexNetwork("my cortex");
                List<CortexRegion> regions = new List<CortexRegion>();
                CortexRegion region0 = new CortexRegion("1st Region");

                regions.Add(region0);

                SpatialPoolerMT sp1 = new SpatialPoolerMT();
                TemporalMemory tm1 = new TemporalMemory();
                var mem = new Connections();
                p.apply(mem);
                //sp1.init(mem, UnitTestHelpers.GetMemory());
                sp1.init(mem);
                tm1.init(mem);

                Dictionary<string, object> settings = new Dictionary<string, object>()
            {
                { "W", 15},
                { "N", inputBits},
                { "Radius", -1.0},
                { "MinVal", 0.0},
               // { "MaxVal", 20.0 },
                { "Periodic", false},
                { "Name", "scalar"},
                { "ClipInput", false},
            };

                double max = 10;

                List<double> lst = new List<double>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };

                // Later we run the whole experiment for all the below input sequences

                // List<double> lst = new List<double>() { 1, 2, 2, 3, 4, 4, 5, 6, 6, 7, 8 };
                // List<double> lst = new List<double>() { 1, 2, 3, 1, 2, 4 };
                // List<double> lst = new List<double>() { 1, 2, 3, 4, 1, 2, 3, 5 };
                // List<double> lst = new List<double>() { 1, 2, 3, 4, 5, 1, 2, 3, 4, 5 };

                settings["MaxVal"] = max;

                EncoderBase encoder = new ScalarEncoder(settings);

                CortexLayer<object, object> layer1 = new CortexLayer<object, object>("L1");

                // NewBorn learning stage.
                region0.AddLayer(layer1);
                layer1.HtmModules.Add("encoder", encoder);
                layer1.HtmModules.Add("sp", sp1);

                HtmClassifier<double, ComputeCycle> cls = new HtmClassifier<double, ComputeCycle>();

                double[] inputs = lst.ToArray();

                //
                // This trains SP.
                foreach (var input in inputs)
                {
                    Debug.WriteLine($" ** {input} **");
                    for (int i = 0; i < 3; i++)
                    {
                        var lyrOut = layer1.Compute((object)input, learn) as ComputeCycle;
                    }
                }

                // Here we add TM module to the layer.
                layer1.HtmModules.Add("tm", tm1);

                int cycle = 0;
                int matches = 0;

                double lastPredictedValue = 0;
                //
                // Now, training with SP+TM. SP is pretrained on pattern.
                //Child loop / Inner loop

                for (int i = 0; i < 400; i++)
                {
                    matches = 0;
                    cycle++;
                    foreach (var input in inputs)
                    {
                        var lyrOut = layer1.Compute(input, learn) as ComputeCycle;

                        cls.Learn(input, lyrOut.ActiveCells.ToArray(), lyrOut.predictiveCells.ToArray());

                        Debug.WriteLine($"-------------- {input} ---------------");

                        if (learn == false)
                            Debug.WriteLine($"Inference mode");

                        Debug.WriteLine($"W: {Helpers.StringifyVector(lyrOut.WinnerCells.Select(c => c.Index).ToArray())}");
                        Debug.WriteLine($"P: {Helpers.StringifyVector(lyrOut.predictiveCells.Select(c => c.Index).ToArray())}");

                        var predictedValue = cls.GetPredictedInputValue(lyrOut.predictiveCells.ToArray());

                        Debug.WriteLine($"Current Input: {input} \t| - Predicted value in previous cycle: {lastPredictedValue} \t| Predicted Input for the next cycle: {predictedValue}");

                        if (input == lastPredictedValue)
                        {
                            matches++;
                            Debug.WriteLine($"Match {input}");
                        }
                        else
                            Debug.WriteLine($"Missmatch Actual value: {input} - Predicted value: {lastPredictedValue}");

                        lastPredictedValue = predictedValue;
                    }

                    if (i == 500)
                    {
                        Debug.WriteLine("Stop Learning From Here. Entering inference mode.");
                        learn = false;
                    }

                    //tm1.reset(mem);

                    Debug.WriteLine($"Cycle: {cycle}\tMatches={matches} of {inputs.Length}\t {(double)matches / (double)inputs.Length * 100.0}%");
                    if ((double)matches / (double)inputs.Length == 1)
                    {
                        //* writer.WriteLine($"{cycle}");
                        //var cycless = new List<int>();
                        //List<int> cycless = new List<int>();
                        cycless.Add(cycle);
                      
                        Console.WriteLine($"{cycle}");
                        
                        break;
                    }

                }
            }
            List<string> l2 = cycless.ConvertAll<string>(delegate (int i) { return i.ToString(); });
            return l2 ;//cycless;
            
            //* }
        }
        //cls.TraceState();
        // Debug.WriteLine("------------------------------------------------------------------------\n----------------------------------------------------------------------------");
    }
}