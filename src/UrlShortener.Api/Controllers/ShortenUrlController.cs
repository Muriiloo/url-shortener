using Microsoft.AspNetCore.Mvc;
using UrlShortener.Application.UseCases;

namespace UrlShortener.Api.Controllers;

[Route("api/shorten")]
[ApiController]
public class ShortenUrlController : ControllerBase
{
    private readonly ShortenUrlUsecase _useCase;
    public ShortenUrlController(ShortenUrlUsecase useCase)
    {
        _useCase = useCase;
    }

    [HttpPost]
    public async Task<IActionResult> Handler([FromBody] ShortenUrlRequest request)
    {
        var result = await _useCase.Execute(request.longUrl);

        return Created("", result);
    }
}

public record ShortenUrlRequest(string longUrl);
