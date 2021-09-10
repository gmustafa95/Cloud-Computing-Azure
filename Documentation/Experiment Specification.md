**ML19/20-5.4. Investigation of Sequence Learning of SP/TM Layer in Microsoft Azure Cloud - Group GT4**

**How to run the experiment:**

First, we need to change the “StorageConnectionString” and “StorageConnectionStringCosmosTable” in the appsetting.json file which can be found under MyCloudProject.

![1](https://github.com/UniversityOfAppliedSciencesFrankfurt/se-cloud-2019-2020/blob/GT4/MyProject/Cloud%20Computing%20Project/img/1.png)

![2](https://github.com/UniversityOfAppliedSciencesFrankfurt/se-cloud-2019-2020/blob/GT4/MyProject/Cloud%20Computing%20Project/img/2.png)

These connection strings can be found in the setting of Storage Account and Cosmos DB respectively. 
After making these changes make two containers in the storage account one for input “training-files” and other for output “result-files”.

After running the project, it will create a queue and wait for our message.

![3](https://github.com/UniversityOfAppliedSciencesFrankfurt/se-cloud-2019-2020/blob/GT4/MyProject/Cloud%20Computing%20Project/img/3.png)

The queue can be found in the storage account.

![4](https://github.com/UniversityOfAppliedSciencesFrankfurt/se-cloud-2019-2020/blob/GT4/MyProject/Cloud%20Computing%20Project/img/4.png)

Now we must add message in the queue. The queue message needs a link of input file in it. So, first we need to upload input file in the “training-files” container we made earlier. 
Input file can be found under “MyExperiment/Data/inputfile.json”. The input file can be varied for different experiment specifications. 
For HTM Sparsity we can enter the value of Width, Input Bits, and Number of Iterations(Experiment Repetitions).

![5](https://github.com/UniversityOfAppliedSciencesFrankfurt/se-cloud-2019-2020/blob/GT4/MyProject/Cloud%20Computing%20Project/img/5.png)
 
After uploading the input file to the container, we need to copy and paste the link in the queue message which can found under “MyExperiment/ProcessingData/QueueMessage.txt”. 
Now add this message to the queue.

![6](https://github.com/UniversityOfAppliedSciencesFrankfurt/se-cloud-2019-2020/blob/GT4/MyProject/Cloud%20Computing%20Project/img/6.png)
 
Now, it will run the experiment as per specifications and save the result files in the result file container.

![7](https://github.com/UniversityOfAppliedSciencesFrankfurt/se-cloud-2019-2020/blob/GT4/MyProject/Cloud%20Computing%20Project/img/7.png)

The uploaded files can be found in the result-files folder.

![8](https://github.com/UniversityOfAppliedSciencesFrankfurt/se-cloud-2019-2020/blob/GT4/MyProject/Cloud%20Computing%20Project/img/8.png)
 
The first file HTMoutput.txt is the main result file for the experiment it has the result in terms of average cycles that the experiment will take to get 100 % prediction 
for given width, input bits, and iterations.

![9](https://github.com/UniversityOfAppliedSciencesFrankfurt/se-cloud-2019-2020/blob/GT4/MyProject/Cloud%20Computing%20Project/img/9.png)
 
The other file “ResultFile-……” will have different information like the name of experiment, start time, end time, duration etc.
Most of this data and the links to output file and blob are also uploaded to Cosmos DB. It can be seen in the data explorer.

![10](https://github.com/UniversityOfAppliedSciencesFrankfurt/se-cloud-2019-2020/blob/GT4/MyProject/Cloud%20Computing%20Project/img/10.png)
 
For better understanding of the results please read the main project report which can be found at: 
https://github.com/UniversityOfAppliedSciencesFrankfurt/se-cloud-2019-2020/tree/GT4/MyProject/HTM/Documentation/Final%20Report
