using Hackathon.Azdo.KPI.Gpt4.Services.model;

namespace Hackathon.Azdo.KPI.Gpt4.Services.Interface
{
    public interface IBuildService
    {
         Task<List<BuildModel>> RetrieveBuildsAsync(string projectName);
         Task SaveBuildsAsync(List<BuildModel> builds);
        Task SaveToAzureBlobAsync(string blobName, string containerName, Stream fileStream);
    }
}
