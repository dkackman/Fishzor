using Microsoft.AspNetCore.Mvc;
using Fishzor.Services;

namespace Fishzor.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FishController : ControllerBase
{
    private readonly FishService _fishService;

    public FishController(FishService fishService)
    {
        _fishService = fishService;
    }

    [HttpPost("add")]
    public async Task<IActionResult> AddFish()
    {
        int newCount = await _fishService.AddFish();
        return Ok(newCount);
    }

    [HttpPost("remove")]
    public async Task<IActionResult> RemoveFish()
    {
        int newCount = await _fishService.RemoveFish();
        return Ok(newCount);
    }

    [HttpGet("count")]
    public IActionResult GetFishCount()
    {
        int count = _fishService.GetFishCount();
        return Ok(count);
    }
}