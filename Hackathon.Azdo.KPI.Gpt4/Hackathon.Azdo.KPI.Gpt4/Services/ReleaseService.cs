using Azure.Storage.Blobs;
using Hackathon.Azdo.KPI.Gpt4.Services.Interface;
using Hackathon.Azdo.KPI.Gpt4.Services.model;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Clients;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using System.Text;


namespace Hackathon.Azdo.KPI.Gpt4.Services
{
    public class ReleaseService //: IReleaseService
    {
        private readonly ReleaseHttpClient _releaseClient;
        private readonly BlobServiceClient _blobServiceClient;

        // Constructor initializes the Azure DevOps connection and Blob Storage client
        public ReleaseService()
        {
            
          
            var basicCredential = new VssBasicCredential(string.Empty, AppConstants.pat);
            var connection = new VssConnection(new Uri(AppConstants.url), basicCredential);
            _releaseClient = connection.GetClient<ReleaseHttpClient>();

            // Initialize the Blob Service client for Azure Storage
            _blobServiceClient = new BlobServiceClient(AppConstants.connectionString);
        }

        public async Task<List<ReleaseModel>> RetrieveReleasesAsync(string projectName)
        {
            // Fetch all releases in the specified project
            var releaseList = await _releaseClient.GetReleasesAsync(projectName);
            var releaseModels = new List<ReleaseModel>();

            foreach (var release in releaseList)
            {
                var releaseModel = new ReleaseModel
                {
                    Id = release.Id,
                    ReleaseName = release.Name,
                    Status = release.Status.ToString()!,
                    Environment = release.Environments.FirstOrDefault()?.Name, // Get the first environment's name
                  //  RequestedBy = release.RequestedBy?.DisplayName ?? "Unknown",
                    StartTime = release.CreatedOn,
                    FinishTime = release.ModifiedOn,
                   // Duration = (release.FinishTime - release.StartTime)?.TotalMinutes ?? 0

                };

                releaseModels.Add(releaseModel);
                Console.WriteLine(releaseModels);
            }

            // Save release data to Azure Blob Storage
            await SaveReleasesAsync(releaseModels);
            Console.WriteLine(releaseModels);
            return releaseModels;
        }

        // Saves a list of release details to Azure Blob Storage
        public async Task SaveReleasesAsync(List<ReleaseModel> releases)
        {
            foreach (var release in releases)
            {
                try
                {
                    string json = JsonConvert.SerializeObject(release, Formatting.Indented);
                    string fileName = $"release_{release.ReleaseName}_{release.Id}.json";

                    using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(json)))
                    {
                        await SaveToAzureBlobAsync(fileName, "devops-releases", stream);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to save release '{release.ReleaseName}': {ex.Message}");
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
