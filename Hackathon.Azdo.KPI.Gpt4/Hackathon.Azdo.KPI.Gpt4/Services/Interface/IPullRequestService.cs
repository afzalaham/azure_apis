using Hackathon.Azdo.KPI.Gpt4.Services.model;
namespace Hackathon.Azdo.KPI.Gpt4.Services.Interface
{
    public interface IPullRequestService
    {
         Task<List<PullRequestModel>> RetrievePullRequestsAsync(string projectName);
         Task SavePullRequestsAsync(List<PullRequestModel> pullRequests);
        Task SaveInAzureBlobStorageAsync(string blobName, string containerName, Stream fileStream);
    }
}
