using Microsoft.AspNetCore.Mvc;
using OtpModule.Abstractions;

namespace OtpModule.Controllers;

[ApiController]
[Route("api/otp")]
public class OtpController(IOtpService service) : ControllerBase
{
    private readonly IOtpService _service = service;

    [HttpPost("send")]
    public async Task<IActionResult> Send([FromQuery] string key)
    {
        await _service.SendAsync(key);
        return Ok();
    }

    [HttpPost("verify")]
    public async Task<IActionResult> Verify([FromQuery] string key, [FromQuery] string code)
    {
        var result = await _service.VerifyAsync(key, code);
        return result ? Ok() : BadRequest();
    }
}
