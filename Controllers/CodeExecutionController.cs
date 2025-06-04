using Microsoft.AspNetCore.Mvc;
using UniCodeProject.API.DTOs;
using UniCodeProject.API.Services;

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
    public async Task<IActionResult> ExecuteCode([FromBody] CodeRequest request)
    {
        var output = await _dockerService.ExecuteCodeAsync(request.Code, request.Language);
        return Ok(new { result = output });
    }
}