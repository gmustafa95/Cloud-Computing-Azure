using Microsoft.Azure.Storage.Queue;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MyCloudProject.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MyExperiment.Models;
using MyExperiment.Utilities;
using Newtonsoft.Json;
using ConsoleApp1;
using MyExperiment.AzureConnections;
using MaxMinValueExp;
using UnitTestsProject;




namespace MyExperiment
{
    public class Experiment : IExperiment
    {
        public static string DataFolder { get; private set; }
        private IStorageProvider storageProvider;
        private ILogger logger;
        private MyConfig config;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configSection"></param>
        /// <param name="storageProvider"></param>
        /// <param name="log"></param>
        public Experiment(IConfigurationSection configSection, IStorageProvider storageProvider, ILogger log)
        {
            this.storageProvider = storageProvider;
            logger = log;
            config = new MyConfig();
            configSection.Bind(config);

            //  Creates the directory where the input-data from the blob will be stored
            DataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                config.LocalPath);
            Directory.CreateDirectory(DataFolder);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="localFileName"></param>
        /// <returns></returns>
        public async Task<ExperimentResult> Run(string localFileName)
        {
            var InputDataList =
                JsonConvert.DeserializeObject<List<HtmProjectInputDataModel>>(FileUtilities.ReadFile(localFileName));

            var startTime = DateTime.UtcNow;

            // The program will run until all the input ends
            string uploadedDataURI = await RunHtmProjects(InputDataList);

            Thread.Sleep(5000);
            var endTime = DateTime.UtcNow;

            logger?.LogInformation(
                $"All the tests ran successfully as per input from the blob storage");

            long duration = endTime.Subtract(startTime).Seconds;

            var res = new ExperimentResult(this.config.GroupId, Guid.NewGuid().ToString());
            ResultUpdate(res, startTime, endTime, duration, localFileName, uploadedDataURI);
            return res;
        }

        private async Task<string> RunHtmProjects(
           List<HtmProjectInputDataModel> seProjectInputDataList)
        {
            // Running Experiment Cells Per Column
            var cellPerColumnExperiment = new CellPerColumnExperiment();

            // First: Run all test cases in a file
            foreach (var input in seProjectInputDataList)
            {

                // Creating an object and giving input from HtmProjectInputDataModel 
                List<string> outputCells = cellPerColumnExperiment.CellPerColumn(input.NumberOfCells, input.NumberOfIterations);
                var StringOutputCells = string.Join(",", outputCells.ToArray());
                var seProjectOutputModel = new HtmProjectOutputModel(StringOutputCells);
                var outputAsByte = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(seProjectOutputModel));

                // Writing output in a file
                FileUtilities.CellsFile("Cells_Per_Column_Experiment_Result.txt", StringOutputCells);

               
            }

            // Second: Uploading output to blob storage
            var uploadedUriCells =
                await storageProvider.UploadResultFile("Cells_Per_Column_Experiment_Result.txt",
                    null);
            logger?.LogInformation(
                $"Cells Per Column Experiment file successfully uploaded to the Blob. Blob URL: {Encoding.ASCII.GetString(uploadedUriCells)}");

            // ------------------------------------------------------------------------------------------------------//         

            // Running Max Min Value Experiment
            var maxMinValueExperiment = new MaxMinValueExperiment();

            // First: Run all test cases in a file
            foreach (var input in seProjectInputDataList)
            {

                // Creating an object and giving input from HtmProjectInputDataModel
                List<string> outputMaxMin = maxMinValueExperiment.MaxMinValue(input.MaxValue, input.MinValue, input.NumberOfIterations);
                var StringOutputMaxMin = string.Join(",", outputMaxMin.ToArray());

                FileUtilities.MaxMinFile("Max_Min_Experiment_Result.txt", StringOutputMaxMin);

            }

            // Second: Uploading output to blob storage
            var uploadedUriMaxMin =
                await storageProvider.UploadResultFile("Max_Min_Experiment_Result.txt",
                    null);
            logger?.LogInformation(
                $"Max Min Experiment file successfully uploaded to the Blob. Blob URL: {Encoding.ASCII.GetString(uploadedUriMaxMin)}");

            // ------------------------------------------------------------------------------------------------------//     

            // Running Mini Column Experiment
            var miniColumnExperiment = new MiniColumnExperiment();
            // First: Run all test cases in a file
            foreach (var input in seProjectInputDataList)
            {

                // Creating an object and giving input from HtmProjectInputDataModel
                List<string> outputMiniColumns = miniColumnExperiment.MiniColumn(input.ColumnDimensions, input.NumberOfIterations);
                var StringOutputMiniColumns = string.Join(",", outputMiniColumns.ToArray());

                FileUtilities.MiniColumnsFile ("Mini_Columns_Experiment_Result.txt", StringOutputMiniColumns);

            }

            // Second: Uploading output to blob storage
            var uploadedUriMiniColumns =
                await storageProvider.UploadResultFile("Mini_Columns_Experiment_Result.txt",
                    null);
            logger?.LogInformation(
                $"Mini Columns Experiment file successfully uploaded to the Blob. Blob URL: {Encoding.ASCII.GetString(uploadedUriMiniColumns)}");

            // ------------------------------------------------------------------------------------------------------//  



            // ------------------------------------------------------------------------------------------------------//  



             // ------------------------------------------------------------------------------------------------------//  



            // return a string and delete the combined file if possible
            return Encoding.ASCII.GetString(uploadedUriMaxMin);
        }

        /// <inheritdoc/>
        public async Task RunQueueListener (CancellationToken cancelToken)
        {
            //CloudQueue queue = await StorageQueue .CreateQueueAsync(config, logger);
            CloudQueue queue = await AzureQueueOperations.CreateQueueAsync(config, logger);

            while (cancelToken.IsCancellationRequested == false)
            {
                var message = await queue.GetMessageAsync(cancelToken);
                try
                {
                    if (message != null)
                    {
                        // First: Reading and deserializing message from the Queue 
                        var experimentRequestMessage =
                            JsonConvert.DeserializeObject<ExerimentRequestMessage>(message.AsString);
                        logger?.LogInformation(
                            $"Received message from the queue with experimentID: " +
                            $"{experimentRequestMessage.ExperimentId}, " +
                            $"description: {experimentRequestMessage.Description}, " +
                            $"name: {experimentRequestMessage.Name}");

                        // Second: Downloading the input file from the blob storage and saving locally
                        var fileToDownload = experimentRequestMessage.InputFile;
                        var localStorageFilePath = await storageProvider.DownloadInputFile(fileToDownload);

                        logger?.LogInformation(
                            $"File download successful. Downloaded file link: {localStorageFilePath}");

                        // Third: Running HtmProjects experiment with inputs from the input file
                        var result = await Run(localStorageFilePath);
                        result.Description = experimentRequestMessage.Description;
                        var resultAsByte = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(result));

                        // Forth: Uploading result file to blob storage
                        var uploadedUri =
                            await storageProvider.UploadResultFile("ResultFile-" + Guid.NewGuid() + ".txt",
                                resultAsByte);
                        logger?.LogInformation($"Uploaded result file to the Blob");
                        result.SeExperimentOutputBlobUrl = Encoding.ASCII.GetString(uploadedUri);

                        // Fifth: Uploading result file to table storage in CosmosDB
                        await storageProvider.UploadExperimentResult(result);

                        // Sixth: Deleting the message from the queue
                        await queue.DeleteMessageAsync(message, cancelToken);
                    }
                }
                catch (Exception ex)
                {
                    logger?.LogError(ex, "Caught an exception: {0}", ex.Message);
                    await queue.DeleteMessageAsync(message, cancelToken);
                }

                // pause
                await Task.Delay(500, cancelToken);
            }

            logger?.LogInformation("Cancel pressed. Exiting the listener loop.");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="experimentResult"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="duration"></param>
        /// <param name="downloadFileUrl"></param>
        /// <param name="testCaseOutputUri"></param>
        private static void ResultUpdate(ExperimentResult experimentResult, DateTime startTime,
            DateTime endTime, long duration, string downloadFileUrl, string testCaseOutputUri)
        {
            experimentResult.StartTimeUtc = startTime;
            experimentResult.EndTimeUtc = endTime;
            experimentResult.DurationSec = duration;
            experimentResult.Name = "HTM Projects";
            experimentResult.InputFileUrl = downloadFileUrl;  // blob url 
            experimentResult.SeExperimentOutputFileUrl = testCaseOutputUri;
        }

    }

}