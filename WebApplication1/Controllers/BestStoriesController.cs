using MediatR;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Handlers.BestStoreiesController.Get;
using WebApplication1.Interfaces;

namespace WebApplication1.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BestStoriesController : Controller
{
    private readonly IMediator _mediator;

    public BestStoriesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> Get(int count)
    {
        return Ok((await _mediator.Send(new GetRequest { Count = count })).Select(x => new BestStoryViewModel
        {
            Title = x.Title,
            Uri = x.Url,
            PostedBy = x.By,
            Time = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(x.Time),
            Score = x.Score,
            CommentCount = x.Kids?.Count??0
        }));
    }
}