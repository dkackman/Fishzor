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
    public IActionResult AddFish()
    {
        _fishService.AddFish();
        return Ok(_fishService.FishComponents.Count);
    }

    [HttpPost("remove")]
    public IActionResult RemoveFish()
    {
        _fishService.RemoveFish();
        return Ok(_fishService.FishComponents.Count);
    }

    [HttpGet("count")]
    public IActionResult GetFishCount()
    {
        return Ok(_fishService.FishComponents.Count);
    }
}