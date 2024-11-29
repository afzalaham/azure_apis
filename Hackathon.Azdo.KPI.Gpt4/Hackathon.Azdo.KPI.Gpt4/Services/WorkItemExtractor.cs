using Azure.Storage.Blobs;
using Hackathon.Azdo.KPI.Gpt4;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using System.Text;
using System.Text.RegularExpressions;
public class UpdateDetail
{
    public List<object>? Fields { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<RevisedBy>? RevisedBy { get; set; } = new List<RevisedBy>();
}

public class DevOpsTasksModel
{
    public int? TaskNumber { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? Link { get; set; }
    public string? State { get; set; }
    public DateTime CreatedOn { get; set; }
    public string? AssignTo { get; set; }
    public string? Type { get; set; }
    public long? ParentWorkItem { get; set; }
    public string? Priority { get; set; }
    public List<string> Comments { get; set; } = new List<string>();
    public List<RelatedWorkItem> RelatedWorkItems { get; set; } = new List<RelatedWorkItem>();
    public string? AreaPath { get; set; }
    public string? TeamProject { get; set; }
    public string? IterationPath { get; set; }
    public string? Sprints { get; set; }
    public string? Iterations { get; set; }
    public string? TargetDate { get; set; }

    // New fields for updates
    public List<UpdateDetail> UpdateContent { get; set; } = new List<UpdateDetail>();
    public int UpdateCount { get; set; }


}

public class RelatedWorkItem
{
    public int? Id { get; set; }
    public string? Title { get; set; }
    public string? State { get; set; }
    public string? Link { get; set; }
    public long? ParentWorkItem { get; set; }
}

public class RevisedBy
{
    public string? DisplayName { get; set; }
    public string? Email { get; set; }
}


public class CopilotIntegrationService : IDisposable
{
    private readonly VssConnection _connection;
    private readonly BlobServiceClient _blobServiceClient;
    private WorkItemTrackingHttpClient _witClient; // Keep the client instance

    public CopilotIntegrationService()
    {

        var basicCredential = new VssBasicCredential(string.Empty, AppConstants.pat);
        _connection = new VssConnection(new Uri(AppConstants.url), basicCredential);
        _blobServiceClient = new BlobServiceClient(AppConstants.connectionString);

        // Initialize the client here
        _witClient = _connection.GetClient<WorkItemTrackingHttpClient>();
    }

    private string ReplaceValue(WorkItem workItem, string fieldName)
    {
        return workItem.Fields.ContainsKey(fieldName) ?
            Regex.Replace((workItem.Fields[fieldName] as string)?.Replace("<br>", "\n")!, @"<.*?>", string.Empty) : "";
    }

    public async Task<List<DevOpsTasksModel>> RetrieveTasksWithCommentsFromDevOps()
    {
        List<DevOpsTasksModel> tasks = new List<DevOpsTasksModel>();

        var wiql = new Wiql()
        {
            Query = "SELECT [System.Id], [System.Title], [System.State] FROM WorkItems WHERE [System.TeamProject] = 'WealthCareNext'"
        };
        var queryByWiqlResult = await _witClient.QueryByWiqlAsync(wiql);
        var workItemLists = queryByWiqlResult.WorkItems
            .Select((x, i) => new { Index = i, Value = x.Id })
            .GroupBy(x => x.Index / 200)
            .Select(g => g.Select(x => x.Value).ToList())
            .ToList();

        foreach (var workItemList in workItemLists)
        {
            var workItems = await _witClient.GetWorkItemsAsync(workItemList, expand: WorkItemExpand.All);

            foreach (var workItem in workItems)
            {

                var devOpsTask = new DevOpsTasksModel()
                {
                    TaskNumber = workItem.Id,
                    Title = workItem.Fields.ContainsKey("System.Title") ? workItem.Fields["System.Title"] as string : "",
                    Description = $"{ReplaceValue(workItem, "System.Description")}\n" +
                                  $"{ReplaceValue(workItem, "Custom.UserStory")}\n" +
                                  $"{ReplaceValue(workItem, "Microsoft.VSTS.Common.AcceptanceCriteria")}",
                    Link = $@"https://dev.azure.com/Alegeus-Technologies/WealthCareNext/_workitems/edit/{workItem.Id}",
                    AssignTo = workItem.Fields.ContainsKey("System.AssignedTo") ? (workItem.Fields["System.AssignedTo"] as IdentityRef)?.DisplayName : "",
                    CreatedOn = workItem.Fields.ContainsKey("System.CreatedDate") ? (DateTime)workItem.Fields["System.CreatedDate"] : DateTime.MinValue,
                    State = workItem.Fields.ContainsKey("System.State") ? workItem.Fields["System.State"] as string : "",
                    Type = workItem.Fields.ContainsKey("System.WorkItemType") ? workItem.Fields["System.WorkItemType"] as string : "",
                    ParentWorkItem = workItem.Fields.ContainsKey("System.Parent") ? workItem.Fields["System.Parent"] as long? : null,
                    Priority = workItem.Fields.ContainsKey("Microsoft.VSTS.Common.BacklogPriority") ? workItem.Fields["Microsoft.VSTS.Common.BacklogPriority"] as string : "",
                    AreaPath = workItem.Fields.ContainsKey("System.AreaPath") ? workItem.Fields["System.AreaPath"] as string : "",
                    TeamProject = workItem.Fields.ContainsKey("System.TeamProject") ? workItem.Fields["System.TeamProject"] as string : "",
                    IterationPath = workItem.Fields.ContainsKey("System.IterationPath") ? workItem.Fields["System.IterationPath"] as string : "",
                    Sprints = workItem.Fields.ContainsKey("Custom.Sprints") ? workItem.Fields["Custom.Sprints"] as string : "",
                    Iterations = workItem.Fields.ContainsKey("System.IterationPath") ? workItem.Fields["System.IterationPath"] as string : "",
                    TargetDate = workItem.Fields.ContainsKey("Microsoft.VSTS.Scheduling.TargetDate") ? workItem.Fields["Microsoft.VSTS.Scheduling.TargetDate"] as string : "",
                };

                // Fetch and set updates
                //devOpsTask.UpdateContent = await GetWorkItemUpdatesAsync(workItem.Id!.Value);
                //devOpsTask.UpdateCount = devOpsTask.UpdateContent.Count;


                // Fetch and populate the revisedBy details into the list

                tasks.Add(devOpsTask);
            }
        }
        SaveDevOpsTasks(tasks);
        return tasks;
    }

    private async Task<List<UpdateDetail>> GetWorkItemUpdatesAsync(int workItemId)
    {
        var updateDetails = new List<UpdateDetail>();

        try
        {
            // Get updates for the work item
            var updates = await _witClient.GetUpdatesAsync(workItemId);

            if (updates != null)
            {
                foreach (var update in updates)
                {
                    if (update?.Fields != null)
                    {
                        foreach (var fieldChange in update.Fields)
                        {
                            if (fieldChange.Value?.OldValue != null || fieldChange.Value?.NewValue != null)
                            {
                                updateDetails.Add(new UpdateDetail
                                {
                                    Fields = new List<object>
                                    {
                                        new
                                        {
                                            FieldName = fieldChange.Key,
                                            OldValue = fieldChange.Value?.OldValue?.ToString(),
                                            NewValue = fieldChange.Value?.NewValue?.ToString()
                                        }
                                    },
                                    UpdatedAt = update.RevisedDate,
                                    RevisedBy = update.RevisedBy != null ? new List<RevisedBy>
                                    {
                                        new RevisedBy
                                        {
                                            DisplayName = update.RevisedBy.DisplayName,
                                            Email = update.RevisedBy.UniqueName
                                        }
                                    } : new List<RevisedBy>(), // If no RevisedBy, return an empty list
                                });
                            }
                            Console.WriteLine(JsonConvert.SerializeObject(fieldChange));
                        }
                    }
                    else
                    {
                        Console.WriteLine($"No fields found in update for work item {workItemId}");
                    }
                }
            }
            else
            {
                Console.WriteLine($"No updates found for work item {workItemId}");
            }
        }
        catch (Exception ex)
        {
            // Log the exception details to help in debugging
            Console.WriteLine($"Error while retrieving updates for work item {workItemId}: {ex.Message}");
        }

        return updateDetails;
    }

    private async void SaveDevOpsTasks(List<DevOpsTasksModel> tasks)
    {
        foreach (var task in tasks)
        {
            string jsonTask = JsonConvert.SerializeObject(task, Formatting.Indented);
            var fileName = $"WorkItem{task.TaskNumber}.txt";
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(jsonTask)))
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
    public void Dispose()
    {
        _witClient?.Dispose();
    }
}
