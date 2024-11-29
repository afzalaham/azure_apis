using Azure.Storage.Blobs;
using Hackathon.Azdo.KPI.Gpt4.Services.Interface;
using Hackathon.Azdo.KPI.Gpt4.Services.model;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.TeamFoundation.Wiki.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using System.Text;
using System.Text.RegularExpressions;

namespace Hackathon.Azdo.KPI.Gpt4.Services
{
    public class WikiDocumentService : IWikiDocumentService
    {
        private readonly WikiHttpClient _wikiClient;
        private readonly BlobServiceClient _blobServiceClient;
        private readonly VssConnection _connection;

        // Constructor initializes the Azure DevOps connection and Blob Storage client
        public WikiDocumentService()
        {
            // Replace these with secure key management (e.g., Azure Key Vault) for production environments
           

            // Establish a connection to Azure DevOps using a PAT (Personal Access Token)
            var basicCredential = new VssBasicCredential(string.Empty, AppConstants.pat);
            _connection = new VssConnection(new Uri(AppConstants.url), basicCredential);
            _wikiClient = _connection.GetClient<WikiHttpClient>();

            // Initialize the Blob Service client for Azure Storage
            _blobServiceClient = new BlobServiceClient(AppConstants.connectionString);
        }

        public async Task<List<WikiDocumentModel>> RetrieveWikiDocumentsAsync(string projectName)
        {
            var wikis = await _wikiClient.GetAllWikisAsync(projectName);
            List<WikiDocumentModel> wikiDocuments = new List<WikiDocumentModel>();
            foreach (var wiki in wikis)
            {
                var pagesCollection = await _wikiClient.GetPageAsync(projectName, wiki.Id, path: "/", recursionLevel: VersionControlRecursionType.Full);

                var wikiDocumentModel = new WikiDocumentModel
                {
                    Project = projectName,
                    WikiName = wiki.Name,
                    Title = wiki.Name,
                    ContentUrl = wiki.RemoteUrl,
                    SubPages = await IteratePages(projectName, wiki, pagesCollection.Page.SubPages)
                };
                wikiDocuments.Add(wikiDocumentModel);
            }

            await SaveWikiDocumentsAsync(wikiDocuments);
            return wikiDocuments;
        }

        private async Task<List<WikiDocumentModel>> IteratePages(string projectName, WikiV2? wiki, IList<WikiPage> allPages)
        {
            List<WikiDocumentModel> wikiSubDocuments = new List<WikiDocumentModel>();
            foreach (var subPage in allPages)
            {
                var stream = await _wikiClient.GetPageTextAsync(projectName, wiki!.Id, subPage.Path);
                using var reader = new StreamReader(stream);
                string content = await reader.ReadToEndAsync();

                // Extract code blocks and description
                var (description, codeBlocks) = ExtractCodeBlocks(content);

                var wikiDocumentModel = new WikiDocumentModel
                {
                    Project = projectName,
                    WikiName = wiki.Name,
                    Title = subPage.Path,
                    Description = description,
                    CodeBlocks = codeBlocks!,
                    ContentUrl = subPage.RemoteUrl,
                    SubPages = await IteratePages(projectName, wiki, subPage.SubPages)
                };
                wikiSubDocuments.Add(wikiDocumentModel);
            }
            return wikiSubDocuments;
        }

        // Utility function to extract code blocks from content
        public (string description, List<string> codeBlocks) ExtractCodeBlocks(string content)
        {
            var codeBlocks = new List<string>();
            var descriptionBuilder = new StringBuilder();

            var lines = content.Split('\n');
            bool insideCodeBlock = false;
            StringBuilder codeBlockBuilder = new();

            foreach (var line in lines)
            {
                if (line.TrimStart().StartsWith("```")) // Check for code block delimiter
                {
                    if (insideCodeBlock)
                    {
                        // End of code block
                        insideCodeBlock = false;
                        codeBlocks.Add(codeBlockBuilder.ToString().Trim());
                        codeBlockBuilder.Clear();
                    }
                    else
                    {
                        // Start of code block
                        insideCodeBlock = true;
                    }
                }
                else if (insideCodeBlock)
                {
                    codeBlockBuilder.AppendLine(line);
                }
                else
                {
                    descriptionBuilder.AppendLine(line); // Regular content
                }
            }

            return (descriptionBuilder.ToString().Trim(), codeBlocks);
        }

        private string SanitizeBlobName(string name)
        {
            // Replace invalid characters with hyphens
            name = Regex.Replace(name, @"[^a-zA-Z0-9\-_.]", "-");

            // Truncate the name if it's too long
            const int maxLength = 200;
            if (name.Length > maxLength)
            {
                name = name.Substring(0, maxLength);
            }

            // Ensure the name doesn't start or end with a period or hyphen
            name = name.Trim('-').Trim('.');

            return name;
        }

        // Saves a list of wiki documents to Azure Blob Storage
        public async Task SaveWikiDocumentsAsync(List<WikiDocumentModel> wikiDocuments)
        {
            foreach (var document in wikiDocuments)
            {
                try
                {
                    string json = JsonConvert.SerializeObject(document, Formatting.Indented);
                    string fileName = SanitizeBlobName($"{document.WikiName!.Replace(" ", "")}.txt");

                    using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(json)))
                    {
                        await SaveToAzureBlobAsync(fileName, AppConstants.containerName, stream);
                    }
                }
                catch (Exception saveEx)
                {
                    Console.WriteLine($"Failed to save document '{document.WikiName}': {saveEx.Message}");
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
            catch (Exception blobEx)
            {
                Console.WriteLine($"Failed to upload blob '{blobName}': {blobEx.Message}");
            }
        }
    }
}
