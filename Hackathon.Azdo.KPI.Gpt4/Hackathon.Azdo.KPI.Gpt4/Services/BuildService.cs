using Azure.Storage.Blobs;
using Hackathon.Azdo.KPI.Gpt4.Services.Interface;
using Hackathon.Azdo.KPI.Gpt4.Services.model;
using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using System.Text;
using Hackathon.Azdo.KPI.Gpt4;
namespace Hackathon.Azdo.KPI.Gpt4.Services
{

    public class BuildService: IBuildService
    {
        private readonly BuildHttpClient _buildClient;
        private readonly BlobServiceClient _blobServiceClient;

        // Constructor initializes the Azure DevOps connection and Blob Storage client
        public BuildService()
        {
           
            var basicCredential = new VssBasicCredential(string.Empty, AppConstants.pat);
            var connection = new VssConnection(new Uri(AppConstants.url), basicCredential);
            _buildClient = connection.GetClient<BuildHttpClient>();

            // Initialize the Blob Service client for Azure Storage
            _blobServiceClient = new BlobServiceClient(AppConstants.connectionString);
        }

        public async Task<List<BuildModel>> RetrieveBuildsAsync(string projectName)
        {
            var buildList = await _buildClient.GetBuildsAsync(projectName);
            var buildModels = new List<BuildModel>();

         
            foreach (var build in buildList)
            {
                var logContent = await GetBuildLogContentAsync(build.Id, projectName);

                var buildModel = new BuildModel
                {
                    Id = build.Id,
                    BuildNumber = build.BuildNumber,
                    Status = build.Status.ToString()!,
                    Result = build.Result.ToString()!,
                    DefinitionName = build.Definition.Name,
                    ProjectName = projectName,
                    SourceBranch = build.SourceBranch,
                    RequestedBy = build.RequestedBy?.DisplayName ?? "Unknown",
                    StartTime = build.StartTime,
                    FinishTime = build.FinishTime,
                    Duration = (build.FinishTime - build.StartTime)?.TotalMinutes ?? 0,

                    LogUrl = $"https://dev.azure.com/{projectName}/_build/results?buildId={build.Id}&view=logs", // Constructed log URL
                    CreatedBy = build.RequestedBy?.DisplayName ?? "Unknown", 
                    CreatedDate = build.QueueTime,
                    LogContent = logContent
                };

                buildModels.Add(buildModel);
                Console.WriteLine(buildModels);
            }

            await SaveBuildsAsync(buildModels);
            Console.WriteLine(buildModels);
            return buildModels;
        }
        public async Task<string> GetBuildLogContentAsync(int buildId, string projectName)
        {
            try
            {
                // Retrieve all log entries for the build
                var logs = await _buildClient.GetBuildLogsAsync(projectName, buildId);

                var logContent = new StringBuilder();

                // Iterate through each log entry and concatenate content
                foreach (var log in logs)
                {
                    var logStream = await _buildClient.GetBuildLogAsync(projectName, buildId, log.Id);
                    using (var reader = new StreamReader(logStream))
                    {
                        logContent.AppendLine(await reader.ReadToEndAsync());
                    }
                }

                return logContent.ToString();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to retrieve logs for build {buildId}: {ex.Message}");
                return "Log retrieval failed.";
            }
        }


        // Saves a list of build details to Azure Blob Storage
        public async Task SaveBuildsAsync(List<BuildModel> builds)
        {
            foreach (var build in builds)
            {
                try
                {
                    string json = JsonConvert.SerializeObject(build, Formatting.Indented);
                    string fileName = $"build_{build.ProjectName}_{build.BuildNumber}.txt";

                    using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(json)))
                    {
                        await SaveToAzureBlobAsync(fileName, "devopsai1", stream);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to save build '{build.BuildNumber}': {ex.Message}");
                }
            }
        }

        // Uploads a file stream to a specified Azure Blob Storage container
        public async Task SaveToAzureBlobAsync(string blobName, string containerName, Stream fileStream)
        {
            try
            {
                var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
                await containerClient.CreateIfNotExistsAsync();
                var blobClient = containerClient.GetBlobClient(blobName);

                await blobClient.UploadAsync(fileStream, overwrite: true);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to upload blob '{blobName}': {ex.Message}");
            }
        }
    }

}
