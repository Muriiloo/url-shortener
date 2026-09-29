using Microsoft.AspNetCore.Mvc;
using UrlShortener.Application.Exceptions;
using UrlShortener.Application.UseCases;

namespace UrlShortener.Api.Controllers;

[Route("")]
[ApiController]
public class RedirectLongUrlController : ControllerBase
{
    private readonly RedirectLongUrlUseCase _useCase;

    public RedirectLongUrlController(RedirectLongUrlUseCase useCase)
    {
        _useCase = useCase;
    }

    [HttpGet("{shortCode}")]
    public async Task<IActionResult> RedirectLongUrl(string shortCode)
    {
        try
        {
            var result = await _useCase.ExecuteAsync(shortCode);

            if (result is null)
                return NotFound();

            return Redirect(result!);

        }
        catch(NotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }
}
