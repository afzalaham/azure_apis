namespace Hackathon.Azdo.KPI.Gpt4.Services.model
{
    public class ReleaseModel
    {
        public int? Id { get; set; }
        public string? ReleaseName { get; set; }
        public string? Status { get; set; }
        public string? Environment { get; set; }
        public string? RequestedBy { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? FinishTime { get; set; }
        public double Duration { get; set; }
    }
}
