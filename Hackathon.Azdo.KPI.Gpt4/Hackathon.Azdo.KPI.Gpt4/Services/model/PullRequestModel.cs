namespace Hackathon.Azdo.KPI.Gpt4.Services.model
{
    public class PullRequestModel
    {
        public string? Project { get; set; }
        public int PullRequestId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? CreatedBy { get; set; }
        public string? State { get; set; }
        public DateTime? CreationDate { get; set; }
        public DateTime? CompletionDate { get; set; }
        public string? RepositoryName { get; set; }
        public string? RepositoryUrl { get; set; }
        public string? PullRequestUrl { get; set; }
        public List<string>? Comments { get; set; }
        public List<RelatedWorkItem>? RelatedWorkItems { get; set; }
        public string? Sprints { get; set; }
        public string? Iterations { get; set; }
        public List<ReviewerModel>? AssignTo { get; set; }
        public string? SourceBranch { get; set; }
        public string? TargetBranch { get; set; }
    }
    public class ReviewerModel
    {
        public string? DisplayName { get; set; }
        public string? Email { get; set; }
    }

}
