**Exercise 1**
Login and check Azure account infos

$ az login -u //to login with your credentials

$ az account list //list all the subscriptions in that account

$ az account set -s <Subscription-name/ID> //switch to a subscription

$ az logout //to logout

**Exercise 2**

Docker Hub image: docker pull gmustafa95/newapp:v1.0

Azure Container Registry image: docker pull newappc.azurecr.io/newapp:v1.0

Azure and Docker script: https://github.com/UniversityOfAppliedSciencesFrankfurt/se-cloud-2019-2020/tree/Ghulam-Mustafa/MyWork/CloudExercise-02

**Exercise 3**

URL of the Web App: https://newweba.azurewebsites.net/

Command to deploy source code to the Web App from zip file: 
$ az webapp deployment source config-zip --src  --resource-group  --name

The web app turns off automatically so contact me for validation.

**Exercise 4**

URL to the image (private Azure Container Registry): exercise4webapp.azurecr.io/exercise04:latest

URL to the Web App: https://exercise4webapp.azurewebsites.net/

The web app turns off automatically so contact me for validation.

**Exercise 5**

The blob used for this exercise is "inputfile.json"

URL to Visual Studio C# project demonstrating sample operation on Azure Blob Storage service: https://github.com/UniversityOfAppliedSciencesFrankfurt/se-cloud-2019-2020/tree/Ghulam-Mustafa/MyWork/CloudExercise-05/BlobQuickstartV12

Blob "inputfile.json" SAS token: sp=r&st=2020-10-08T14:19:15Z&se=2021-10-08T22:19:15Z&spr=https&sv=2019-12-12&sr=b&sig=PARX58pG4L7CDW7OwLqkRt9aIo0SvZaY1kdbKkW1UeM%3D

Blob "inputfile.json" SAS URL: https://cloud12.blob.core.windows.net/input-file/inputfile.json?sp=r&st=2020-10-08T14:19:15Z&se=2021-10-08T22:19:15Z&spr=https&sv=2019-12-12&sr=b&sig=PARX58pG4L7CDW7OwLqkRt9aIo0SvZaY1kdbKkW1UeM%3D

**Exercise 6**

URL to Visual Studio C# project demonstrating sample operation on Azure Table Storage service:

https://github.com/UniversityOfAppliedSciencesFrankfurt/se-cloud-2019-2020/tree/Ghulam-Mustafa/MyWork/CloudExercise-06

**Exercise 7**

URL to Visual Studio C# project demonstrating sample operation on Azure Queue Storage service: 

https://github.com/UniversityOfAppliedSciencesFrankfurt/se-cloud-2019-2020/tree/Ghulam-Mustafa/MyWork/CloudExercise-07

for further details about exercises please check:

https://github.com/UniversityOfAppliedSciencesFrankfurt/se-cloud-2019-2020/tree/Ghulam-Mustafa/MyWork
