using Twittertask.Models;
using Twittertask.Repositories;
using Microsoft.AspNetCore.Mvc;
using Twittertask.DTOs;
using Microsoft.AspNetCore.Authorization;
using Twittertask.Utilities;
using System.Security.Claims;
using Microsoft.Extensions.Caching.Memory;

namespace Todo.Controllers;

[ApiController]
[Authorize]
[Route("api/post")]
public class PostController : ControllerBase
{
    private readonly ILogger<PostController> _logger;
    private readonly IPostRepository _post;

    private readonly IMemoryCache _memoryCache;



    public PostController(ILogger<PostController> logger,
    IPostRepository post, IMemoryCache memoryCache)
    {
        _logger = logger;
        _post = post;
        _memoryCache = memoryCache;
    }

    private int GetUserIdFromClaims(IEnumerable<Claim> claims)
    {
        return Convert.ToInt32(claims.Where(x => x.Type == TwitterConstants.Id).First().Value);
    }

    [HttpPost]
    public async Task<ActionResult<Post>> CreateTweet([FromBody] PostCreateDTO Data)
    {
        var userId = GetUserIdFromClaims(User.Claims);



        List<Post> usertweets = await _post.GetPostByUserId(userId);
        if (usertweets != null && usertweets.Count >= 5)
        {
            return BadRequest("Limit exceeded");
        }

        var toCreateItem = new Post
        {
            Title = Data.Title.Trim(),
            UserId = userId,

        };


        var createdItem = await _post.Create(toCreateItem);


        return StatusCode(201, createdItem);
    }


    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateTodo([FromRoute] long id,
    [FromBody] PostUpdateDTO Data)
    {
        var userId = GetUserIdFromClaims(User.Claims);

        var existingItem = await _post.GetById(id);

        if (existingItem is null)
            return NotFound();

        if (existingItem.UserId != userId)
            return StatusCode(404, "You cannot update other's POST");

        var toUpdateItem = existingItem with
        {
            Title = Data.Title is null ? existingItem.Title : Data.Title.Trim(),

        };

        await _post.Update(toUpdateItem);

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteTodo([FromRoute] long id)
    {
        var userId = GetUserIdFromClaims(User.Claims);

        var existingItem = await _post.GetById(id);

        if (existingItem is null)
            return NotFound();

        if (existingItem.UserId != userId)
            return StatusCode(404, "You cannot delete other's POST");

        await _post.Delete(id);

        return NoContent();
    }



    [HttpGet]
    public async Task<ActionResult<List<Post>>> GetAllPosts([FromQuery] PostParameters postParameters)
    {
        var allPosts = await _post.GetAll(postParameters);
        var cacheKey = "postList";
        //checks if cache entries exists
        if (!_memoryCache.TryGetValue(cacheKey, out List<Post> postList))
        {
            //calling the server
            postList = await _post.ToListAsync();

            //setting up cache options
            var cacheExpiryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.Now.AddMinutes(5),
                Priority = CacheItemPriority.High,
                SlidingExpiration = TimeSpan.FromMinutes(2)
            };
            //setting cache entries
            _memoryCache.Set(cacheKey, postList, cacheExpiryOptions);
        }

        return Ok(allPosts);
    }

    [HttpGet("{id}")]

    public async Task<ActionResult<Post>> GetUserById([FromRoute] long id)
    {
        var user = await _post.GetById(id);

        if (user is null)
            return NotFound("No user found with given idS");

        return Ok(user);
    }

}