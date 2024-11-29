namespace Hackathon.Azdo.KPI.Gpt4.Services.model
{
    public class SprintAndIterationmodel
    {
        public class Sprintmodel
        {
            public string? SprintId { get; set; }          // Unique identifier for the sprint
            public string? Name { get; set; }              // Sprint name
            public string? State { get; set; }             // Sprint state (e.g., "Active", "Completed")
            public DateTime? StartDate { get; set; }       // Sprint start date
            public DateTime? EndDate { get; set; }         // Sprint end date
            public string? Goal { get; set; }              // Sprint goal or objective
            public string? Project { get; set; }           // Associated project name
        }

        public class IterationModel
        {
            public string? IterationId { get; set; }       // Unique identifier for the iteration
            public string? Name { get; set; }              // Iteration name
            public DateTime? StartDate { get; set; }       // Iteration start date
            public DateTime? EndDate { get; set; }         // Iteration end date
            public string? Path { get; set; }              // Path within the project structure (e.g., "Project/Sprint 1")
            public string? Description { get; set; }       // Detailed iteration description or notes
        }

    }
}
