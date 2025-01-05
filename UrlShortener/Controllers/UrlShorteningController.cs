using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UrlShortener.Models;
using UrlShortener.Repository;
using UrlShortener.Services;

namespace UrlShortener.Controllers;

[ApiController]
[Route("api")]
public class UrlShorteningController : ControllerBase
{
    private readonly UrlShorteningService _urlShorteningService;

    public UrlShorteningController(UrlShorteningService urlShorteningService)
    {
        _urlShorteningService = urlShorteningService;
    }
    [Authorize(Roles = "admin")]
    [HttpPost("shorten")]
    public async Task<IActionResult> GetUrlShortening(ShortenUrlRequest request)
    {
        if (!Uri.TryCreate(request.Url, UriKind.Absolute, out _))
        {
            return BadRequest("The specified URL is not a valid URL");
        }
        
        var shortenedUrl = await _urlShorteningService.CreateShortenedUrl(request.Url, HttpContext);

        return Ok(shortenedUrl);
    }
    [Authorize]
    [HttpGet("{code}")]
    public async Task<IActionResult> GetLongUrl(string code)
    {
        var shortenedUrl = await _urlShorteningService.GetShortenedUrlByCode(code);

        if (shortenedUrl == null)
        {
            return NotFound();
        }
        
        return Redirect(shortenedUrl.LongUrl);
    }
}
