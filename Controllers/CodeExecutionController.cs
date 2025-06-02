using Microsoft.AspNetCore.Mvc;
using UniCodeProject.API.Services;

namespace UniCodeProject.API.Controllers
{
    [ApiController]
    [Route("api/code")]
    public class CodeExecutionController : ControllerBase
    {
        private readonly DockerExecutionService _dockerService;

        public CodeExecutionController(DockerExecutionService dockerService)
        {
            _dockerService = dockerService;
        }

        [HttpPost("execute")]
        public async Task<IActionResult> ExecuteCode([FromBody] CodeExecutionRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Code))
            {
                return BadRequest("Code is required.");
            }

            string output = await _dockerService.ExecuteCodeAsync(request.Code);

            return Ok(new { result = output });
        }
    }

    public class CodeExecutionRequest
    {
        public string Code { get; set; } = null!;
    }
}
