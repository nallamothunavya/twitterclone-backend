using Twittertask.Models;
using Twittertask.Repositories;
using Microsoft.AspNetCore.Mvc;
using Twittertask.DTOs;
using Microsoft.AspNetCore.Authorization;
using Twittertask.Utilities;
using System.Security.Claims;
using Microsoft.Extensions.Caching.Memory;

namespace Twitter.Controllers;

[ApiController]
[Authorize]
[Route("api/comment")]
public class CommentController : ControllerBase
{
    private readonly ILogger<CommentController> _logger;
    private readonly ICommentRepository _comment;

    private readonly IMemoryCache _memoryCache;

    public CommentController(ILogger<CommentController> logger,
    ICommentRepository comment, IMemoryCache memoryCache)
    {
        _logger = logger;
        _comment = comment;
        _memoryCache = memoryCache;
    }

    private int GetUserIdFromClaims(IEnumerable<Claim> claims)
    {
        return Convert.ToInt32(claims.Where(x => x.Type == TwitterConstants.Id).First().Value);
    }

    [HttpPost]
    public async Task<ActionResult<Comment>> CreateComment([FromBody] CommentCreateDTO Data)
    {
        var userId = GetUserIdFromClaims(User.Claims);

        var toCreateItem = new Comment
        {
            Text = Data.Text.Trim(),
            UserId = userId,
            PostId = Data.PostId,


        };


        var createdItem = await _comment.Create(toCreateItem);


        return StatusCode(201, createdItem);
    }



    [HttpDelete("{Comment_id}")]
    public async Task<ActionResult> DeleteComment([FromRoute] int comment_id)
    {
        var userId = GetUserIdFromClaims(User.Claims);

        var existingItem = await _comment.GetById(comment_id);

        if (existingItem is null)
            return NotFound();

        if (existingItem.UserId != userId)
            return StatusCode(403, "You cannot delete other's Comment");

        await _comment.Delete(comment_id);

        return NoContent();
    }

    [HttpGet]
    public async Task<ActionResult<List<Comment>>> GetAllComments([FromQuery] CommentParameters commentParameters)
    {
        var commentCache = _memoryCache.Get<List<Comment>>(key: "comments");
        if (commentCache is null)
        {
            commentCache = await _comment.GetAll(commentParameters);
            _memoryCache.Set(key: "comments", commentCache, TimeSpan.FromMinutes(value: 1));
        }
        // var cacheKey = "postList";
        // //checks if cache entries exists
        // if (!_memoryCache.TryGetValue(cacheKey, out List<Post> postList))
        // {
        //     //calling the server
        //     postList = await _post.ToListAsync();

        //     //setting up cache options
        //     var cacheExpiryOptions = new MemoryCacheEntryOptions
        //     {
        //         AbsoluteExpiration = DateTime.Now.AddMinutes(5),
        //         Priority = CacheItemPriority.High,
        //         SlidingExpiration = TimeSpan.FromMinutes(2)
        //     };
        //     //setting cache entries
        //     _memoryCache.Set(cacheKey, postList, cacheExpiryOptions);
        // }

        return Ok(commentCache);
    }

    [HttpGet("{post_id}")]
    public async Task<ActionResult<List<Comment>>> GetAllCommentsByPostId([FromRoute] int post_id)
    {
        var allComment = await _comment.GetCommentsByPostId(post_id);
        return Ok(allComment);
    }
}