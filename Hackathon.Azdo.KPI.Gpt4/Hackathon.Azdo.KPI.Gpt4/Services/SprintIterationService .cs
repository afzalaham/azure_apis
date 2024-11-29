using Azure.Storage.Blobs;
using Hackathon.Azdo.KPI.Gpt4.Services.Interface;
using Hackathon.Azdo.KPI.Gpt4.Services.model;
using Microsoft.TeamFoundation.Core.WebApi.Types;
using Microsoft.TeamFoundation.Work.WebApi;
using Microsoft.TeamFoundation.Work.WebApi.Contracts;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using System.Text;
using static Hackathon.Azdo.KPI.Gpt4.Services.model.SprintAndIterationmodel;

namespace Hackathon.Azdo.KPI.Gpt4.Services
{
    public class SprintIterationService// : ISprintIterationService
    {
        private readonly WorkHttpClient _workClient;
        private readonly BlobServiceClient _blobServiceClient;
        private readonly VssConnection _connection;
        private readonly WorkItemTrackingHttpClient _witClient;
        public SprintIterationService()
        {           
            var basicCredential = new VssBasicCredential(string.Empty, AppConstants.pat);
            _connection = new VssConnection(new Uri(AppConstants.url), basicCredential);
            _workClient = _connection.GetClient<WorkHttpClient>();
            _witClient = _connection.GetClient<WorkItemTrackingHttpClient>(); // Add this line
            _blobServiceClient = new BlobServiceClient(AppConstants.connectionString);
        }

        public async Task<List<IterationModel>> RetrieveSprintsAndIterationsAsync()
        {
            var iterations = new List<IterationModel>();

            string projectName = "WealthCareNext";
            // Retrieve classification nodes (iterations)
            var iterationTree = await _witClient.GetClassificationNodeAsync(projectName, TreeStructureGroup.Iterations, depth: 10);

            ExtractIterations(iterationTree, iterations);


            //// Retrieve sprint details
            //var teamSettings = await _workClient.GetTeamSettingsAsync(new TeamContext(projectName));
            //var sprints = await _workClient.GetTeamIterationsAsync(new TeamContext(projectName));


            //foreach (var sprint in sprints)
            //{
            //    iterations.Add(new IterationModel
            //    {
            //        IterationId = sprint.Id.ToString(),
            //        Name = sprint.Name,
            //        Path = sprint.Path,
            //        StartDate = sprint.Attributes.StartDate,
            //        EndDate = sprint.Attributes.FinishDate
            //    });
            //}

            // Save both iterations and sprints to Blob Storage
            await SaveIterationsAsync(iterations);

            return iterations;
        }

        private void ExtractIterations(WorkItemClassificationNode node, List<IterationModel> iterations)
        {
            if (node.Children == null) return;

            foreach (var child in node.Children)
            {
                var iterationModel = new IterationModel
                {
                    IterationId = child.Id.ToString(),
                    Name = child.Name,
                    Path = child.Path,
                    StartDate = child.Attributes.ContainsKey("startDate") ? Convert.ToDateTime(child.Attributes["startDate"]) : DateTime.MinValue,
                    EndDate = child.Attributes.ContainsKey("finishDate") ? Convert.ToDateTime(child.Attributes["finishDate"]) : DateTime.MinValue
                };

                iterations.Add(iterationModel);
             //   ExtractIterations(child, iterations);
            }
        }

        public async Task SaveIterationsAsync(List<IterationModel> iterations)
        {
            foreach (var iteration in iterations)
            {
                string json = JsonConvert.SerializeObject(iteration, Formatting.Indented);
                string fileName = $"{iteration.IterationId!.Replace(" ", "")}.txt";

                using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(json)))
                {
                    await SaveInAzureBlobStorageAsync(fileName, AppConstants.containerName, stream);
                }
            }
        }

        public async Task SaveInAzureBlobStorageAsync(string blobName, string containerName, Stream fileStream)
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
                Console.WriteLine($"Error saving to Blob Storage: {ex.Message}");
            }
        }
    }
}
