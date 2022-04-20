using Twittertask.Models;
using Twittertask.Repositories;
using Microsoft.AspNetCore.Mvc;
using Twittertask.DTOs;
using Microsoft.AspNetCore.Authorization;
using Twittertask.Utilities;
using System.Security.Claims;

namespace Todo.Controllers;

[ApiController]
[Authorize]
[Route("api/post")]
public class PostController : ControllerBase
{
    private readonly ILogger<PostController> _logger;
    private readonly IPostRepository _post;

    public PostController(ILogger<PostController> logger,
    IPostRepository post)
    {
        _logger = logger;
        _post = post;
    }

    private int GetUserIdFromClaims(IEnumerable<Claim> claims)
    {
        return Convert.ToInt32(claims.Where(x => x.Type == TwitterConstants.Id).First().Value);
    }

    [HttpPost]
    public async Task<ActionResult<Post>> CreateTweet([FromBody] PostCreateDTO Data)
    {
        var userId = GetUserIdFromClaims(User.Claims);



        List<Post> usertweets = await _post.GetTweetsByUserId(userId);
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
    public async Task<ActionResult<List<Post>>> GetAllPosts()
    {
        var allPosts = await _post.GetAll();
        return Ok(allPosts);
    }
}