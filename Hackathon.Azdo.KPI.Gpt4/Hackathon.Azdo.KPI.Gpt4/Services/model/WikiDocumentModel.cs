namespace Hackathon.Azdo.KPI.Gpt4.Services.model
{
    public class WikiDocumentModel
    {
        public string? Project { get; set; }
        public string? WikiName { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public List<string?>? CodeBlocks { get; set; }
        public string? ContentUrl { get; set; }  // URL to the page for better reference
        public List<WikiDocumentModel>? SubPages { get; set; }
    }

}
