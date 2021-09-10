
using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using NeoCortexApi.Encoders;
using NeoCortexApi.Network;
using NeoCortexApi;
using NeoCortexApi.Entities;
using System.Diagnostics;
using NeoCortexEntities.NeuroVisualizer;
using System.IO;
namespace UnitTestsProject
{

    public class MiniColumnExperiment
    {
        /// <summary>
        /// This experiment checks for the Complex Sequence learning process in dependence on 3 parameters namely Cells Per Column(C), Input Bits(N) and number of MiniColumns(CD).

        /// What is the effect on Learning by varying these parameters. 

        /// The basis of this experiment is SimpleSequenceExperiment. This program has 2 loops (loop inside a loop), 
        /// the parent loop/outer loop is defined keeping in mind how many readings are required in the result.
        /// The child loop/inner loop has 400 cycle, but is ended as soon as we get 100% prediction match
        /// i.e. if there are 10 input values in the input sequence, so there should be 10 out 10 matches.
        /// Then the parent loop is incremented and it continues for the number of interations defined. In this case it is 30.
        /// First column in DataRow Represents the Number of Cells which we are varying to observe its effect on the learning cycles
        /// Second column represents Input Bits(N), third column represents the number of Mini Columns and forth column
        /// represent the number of times this whole experiment is run 

        /// </summary>





        public List<string> MiniColumn(int CD, int loop) //(int CD, int loop)
        {
            List<int> cycless = new List<int>();
            // string filename = "Complex" + C + N + CD + ".csv";
            // using (StreamWriter writer = new StreamWriter(filename))
            // {
            Debug.WriteLine($"Learning Cycles: {400}");
            Debug.WriteLine("Cycle;Similarity");

            // Parent Loop
            //This loop defines the number of times the experiment will run for the given data
            for (int j = 0; j < loop; j++)
            {
                int inputBits = 200;
                bool learn = true;
                Parameters p = Parameters.getAllDefaultParameters();
                p.Set(KEY.RANDOM, new ThreadSafeRandom(42));
                p.Set(KEY.INPUT_DIMENSIONS, new int[] { inputBits });

                p.Set(KEY.COLUMN_DIMENSIONS, new int[] { CD });
                p.Set(KEY.CELLS_PER_COLUMN, 5);

                CortexNetwork net = new CortexNetwork("my cortex");
                List<CortexRegion> regions = new List<CortexRegion>();
                CortexRegion region0 = new CortexRegion("1st Region");

                regions.Add(region0);

                SpatialPoolerMT sp1 = new SpatialPoolerMT();
                TemporalMemory tm1 = new TemporalMemory();
                var mem = new Connections();
                p.apply(mem);
                sp1.init(mem); //, UnitTestHelpers.GetMemory());
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

                //  List<double> lst = new List<double>() { 9, 8, 7, 6, 9, 8, 7, 5 };
                List<double> lst = new List<double>() { 9, 8, 7, 6, 5, 4, 3, 2 };


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
                        cycless.Add(cycle);

                        Console.WriteLine($"{cycle}");
                        break;
                    }

                }
            }
            List<string> l2 = cycless.ConvertAll<string>(delegate (int i) { return i.ToString(); });
            return l2;
        }
    }
}