namespace Hackathon.Azdo.KPI.Gpt4.Services.model
{
    public class BuildModel
    {
        public int? Id { get; set; }
        public string? BuildNumber { get; set; }
        public string? Status { get; set; }
        public string? Result { get; set; }
        public string? DefinitionName { get; set; }
        public string? ProjectName { get; set; }
        public string? SourceBranch { get; set; }
        public string? RequestedBy { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? FinishTime { get; set; }
        public double Duration { get; set; } // in minutes

        // New properties for logs and creation details
        public string? LogUrl { get; set; } // URL to access the build logs
        public string? CreatedBy { get; set; } // User who created the build
        public DateTime? CreatedDate { get; set; } // Date the build was created

        public string? LogContent { get; set; } // Optional: Content of the build log (if stored directly)
    }
}
