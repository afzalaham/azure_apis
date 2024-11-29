using Hackathon.Azdo.KPI.Gpt4.Services;
using Hackathon.Azdo.KPI.Gpt4.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace Hackathon.Azdo.KPI.Gpt4.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AzdoController : ControllerBase
    {


        private readonly IPullRequestService _pullRequestService;
        private readonly IWikiDocumentService _wikiDocumentService;
        private readonly IBuildService _buildService;
        private readonly SprintIterationService _sprintIterationService;
        private readonly ReleaseService _releaseService;
        public AzdoController(IPullRequestService pullRequestService, IWikiDocumentService wikiDocumentService, IBuildService buildService,SprintIterationService sprintIterationService, ReleaseService releaseService)
        {
            _pullRequestService = pullRequestService;
            _wikiDocumentService = wikiDocumentService;
            _buildService = buildService;
            _sprintIterationService = sprintIterationService;
            _releaseService = releaseService;
        }

        [HttpGet("storeworkitems")]
        public async Task<IActionResult> StoreWorkItemsAsync()
        {

            CopilotIntegrationService copilotIntegrationService = new CopilotIntegrationService();
            await copilotIntegrationService.RetrieveTasksWithCommentsFromDevOps();
            return Ok("successfully strored.");
        }

        [HttpGet("process-pullrequests")]
        public async Task<IActionResult> ProcessAllPullRequestsAsync()
        {
            string projectName =AppConstants.WealthCareNextprojectName;  // Replace with your project name

            CopilotIntegrationService copilotIntegrationService = new CopilotIntegrationService();
            await _pullRequestService.RetrievePullRequestsAsync(projectName);

            return Ok("Pull requests processed and stored successfully.");
        }


        [HttpGet("Wiki")]
        public async Task<IActionResult> RetrieveWikiDocuments()
        {
            try
            {
                string projectName = AppConstants.WealthCareNextprojectName;
                CopilotIntegrationService copilotIntegrationService = new CopilotIntegrationService();
                var wikiDocuments = await _wikiDocumentService.RetrieveWikiDocumentsAsync(projectName);
                return Ok(new { message = "Wiki documents retrieved and saved successfully.", data = wikiDocuments });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving wiki documents.", error = ex.Message });
            }
        }
        [HttpGet("Build")]
        public async Task<IActionResult> GetBuilds()
        {
            try
            {
                string projectName = AppConstants.WealthCareNextprojectName;
                CopilotIntegrationService copilotIntegrationService = new CopilotIntegrationService();
                var builds = await _buildService.RetrieveBuildsAsync(projectName);
                return Ok(builds); // Return 200 OK with build data
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("sprint_and_iteration")]
        public async Task<IActionResult> Getsprint_and_iteration()
        {
            try
            {
                CopilotIntegrationService copilotIntegrationService = new CopilotIntegrationService();
                var sprintandinteration = await _sprintIterationService.RetrieveSprintsAndIterationsAsync();
                return Ok(sprintandinteration); // Return 200 OK with build data
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("ReleaseService")]
        public async Task<IActionResult> GetReleaseService()
        {
            try
            {
                ReleaseService copilotIntegrationService = new ReleaseService();
                string projectName = AppConstants.WealthCareNextprojectName;
                var sprintandinteration = await _releaseService.RetrieveReleasesAsync(projectName);
                return Ok(sprintandinteration); // Return 200 OK with build data
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
