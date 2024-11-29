using Azure.Storage.Blobs;
using Hackathon.Azdo.KPI.Gpt4.Services.Interface;
using Hackathon.Azdo.KPI.Gpt4.Services.model;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using System.Text;
using System.Text.RegularExpressions;
namespace Hackathon.Azdo.KPI.Gpt4.Services
{


    public class PullRequestService : IPullRequestService
    {
        private readonly GitHttpClient _gitClient;
        private readonly WorkItemTrackingHttpClient _witClient; // Added WorkItemTrackingHttpClient
        private readonly BlobServiceClient _blobServiceClient;
        private readonly VssConnection _connection; // Added VssConnection

        public PullRequestService()
        {
          
            var basicCredential = new VssBasicCredential(string.Empty, AppConstants.pat);
            _connection = new VssConnection(new Uri(AppConstants.url), basicCredential); // Initialize _connection
            _gitClient = _connection.GetClient<GitHttpClient>();
            _witClient = _connection.GetClient<WorkItemTrackingHttpClient>(); // Initialize _witClient
            _blobServiceClient = new BlobServiceClient(AppConstants.connectionString);
        }

        public async Task<List<PullRequestModel>> RetrievePullRequestsAsync(string projectName)
        {
            var pullRequests = new List<PullRequestModel>();

            var repositories = await _gitClient.GetRepositoriesAsync(projectName); // Fetch repositories


            foreach (var repository in repositories)
            {
                Console.WriteLine($"Repository: {repository!.Name}, ID: {repository.Id}");
                var prList = await _gitClient.GetPullRequestsAsync(repository.Id, new GitPullRequestSearchCriteria { Status = PullRequestStatus.All });
                Console.WriteLine($"PR Count: {prList.Count}");


                foreach (var pr in prList)
                {
                    var reviewersList = pr.Reviewers
                        .Select(reviewer => new ReviewerModel
                        {
                            DisplayName = reviewer.DisplayName,
                            Email = reviewer.UniqueName
                        })
                        .ToList();

                    Console.WriteLine(JsonConvert.SerializeObject(pr));
                    var prModel = new PullRequestModel
                    {
                        Project = "WealthCareNext",
                        PullRequestId = pr.PullRequestId,
                        Title = pr.Title,
                        Description = pr.Description,
                        CreatedBy = pr.CreatedBy.DisplayName,
                        State = pr.Status.ToString(),
                        CreationDate = pr.CreationDate,
                        CompletionDate = pr.ClosedDate,
                        RepositoryName = repository.Name,
                        RepositoryUrl = repository.RemoteUrl,
                        PullRequestUrl = $"https://dev.azure.com/Alegeus-Technologies/WealthCareNext/_git/{repository.Name}/pullrequest/{pr.PullRequestId}",
                        Comments = new List<string>(),
                        RelatedWorkItems = new List<RelatedWorkItem>(),
                        Sprints = string.Empty,
                        Iterations = string.Empty,
                        SourceBranch = pr.SourceRefName,
                        TargetBranch = pr.TargetRefName,
                        AssignTo = reviewersList

                    };

                    // Fetch comments
                    var prThreads = await _gitClient.GetThreadsAsync(repository.Id, pr.PullRequestId);
                    foreach (var thread in prThreads)
                    {

                        foreach (var comment in thread.Comments)
                        {
                            Console.WriteLine(JsonConvert.SerializeObject(comment));
                            if (!string.IsNullOrEmpty(comment.Content))
                            {
                                prModel.Comments.Add($"{comment.Author.DisplayName}: {Regex.Replace(comment.Content.Replace("<br>", "\n"), @"<.*?>", string.Empty)}");
                            }
                        }
                    }

                    // Fetch related work items
                    var prWorkItemRefs = await _gitClient.GetPullRequestWorkItemRefsAsync(repository.Id, pr.PullRequestId);
                    foreach (var workItemRef in prWorkItemRefs)
                    {

                        Console.WriteLine(JsonConvert.SerializeObject(workItemRef));
                        var workItemId = int.Parse(workItemRef.Id);
                        var workItem = await _witClient.GetWorkItemAsync(workItemId);

                        prModel.RelatedWorkItems.Add(new RelatedWorkItem
                        {
                            Id = workItem!.Id!.Value,
                            Title = workItem!.Fields["System.Title"]! as string,
                            State = workItem.Fields["System.State"] as string,
                            Link = $@"https://dev.azure.com/{projectName}/_workitems/edit/{workItem.Id}"
                        });

                        // Retrieve Sprints and Iterations from work item fields inside the loop
                        prModel.Sprints = workItem.Fields.ContainsKey("Custom.Sprints") ? workItem.Fields["Custom.Sprints"] as string : string.Empty;
                        prModel.Iterations = workItem.Fields.ContainsKey("System.IterationPath") ? workItem.Fields["System.IterationPath"] as string : string.Empty;
                    }
                    pullRequests.Add(prModel);

                }
            }

            await SavePullRequestsAsync(pullRequests); // Save PRs to Blob Storage
            return pullRequests;
        }

        public async Task SavePullRequestsAsync(List<PullRequestModel> pullRequests)
        {
            foreach (var pr in pullRequests)
            {
                string json = JsonConvert.SerializeObject(pr, Formatting.Indented);
                string fileName = $"pull_request_{pr.PullRequestId}.txt";

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
            catch (Exception)
            {
            }
        }
    }
}
