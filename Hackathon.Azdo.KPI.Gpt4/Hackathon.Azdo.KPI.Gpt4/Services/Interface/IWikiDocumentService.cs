using Hackathon.Azdo.KPI.Gpt4.Services.model;

namespace Hackathon.Azdo.KPI.Gpt4.Services.Interface
{
    public interface IWikiDocumentService
    {
        Task<List<WikiDocumentModel>> RetrieveWikiDocumentsAsync(string projectName);
        (string description, List<string> codeBlocks) ExtractCodeBlocks(string content);
        Task SaveWikiDocumentsAsync(List<WikiDocumentModel> wikiDocuments);
        Task SaveToAzureBlobAsync(string blobName, string containerName, Stream fileStream);
    }
}
