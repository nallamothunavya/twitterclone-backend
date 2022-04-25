using Twittertask.Models;
using Dapper;
using Twittertask.Utilities;

namespace Twittertask.Repositories;

public interface IPostRepository
{
    Task<Post> Create(Post Item);
    Task Update(Post Item);
    Task Delete(long Id);
    Task<List<Post>> GetAll();
    Task<Post> GetById(long Id);

    Task<List<Post>> GetPostByUserId(long UserId);

    Task<List<Post>> GetAll(PostParameters postParameters);

    Task<List<Post>> ToListAsync();
}

public class PostRepository : BaseRepository, IPostRepository
{
    public PostRepository(IConfiguration config) : base(config)
    {

    }

    public async Task<Post> Create(Post Item)
    {
        var query = $@"INSERT INTO {TableNames.post} (title, user_id, created_at, updated_at) 
        VALUES (@Title, @UserId, @CreatedAt, @UpdatedAt) RETURNING *";

        using (var con = NewConnection)
            return await con.QuerySingleOrDefaultAsync<Post>(query, Item);
    }

    public async Task Delete(long Id)
    {
        var query = $@"DELETE FROM {TableNames.post} WHERE id = @Id";

        using (var con = NewConnection)
            await con.ExecuteAsync(query, new { Id });
    }

    public async Task<List<Post>> GetAll()
    {
        var query = $@"SELECT * FROM {TableNames.post} ORDER BY created_at DESC";

        using (var con = NewConnection)
            return (await con.QueryAsync<Post>(query)).AsList();
    }

    public async Task<Post> GetById(long Id)
    {
        var query = $@"SELECT * FROM {TableNames.post} WHERE id = @Id";

        using (var con = NewConnection)
            return await con.QuerySingleOrDefaultAsync<Post>(query, new { Id });
    }

    public async Task Update(Post Item)
    {
        var query = $@"UPDATE {TableNames.post} SET title = @Title, 
        user_id = @UserId WHERE id = @Id";

        using (var con = NewConnection)
            await con.ExecuteAsync(query, Item);
    }

    public async Task<List<Post>> GetPostByUserId(long UserId)
    {
        var query = $@"SELECT * FROM {TableNames.post} WHERE user_id = @UserId";

        using (var con = NewConnection)
            return (await con.QueryAsync<Post>(query, new { UserId })).AsList();
    }

    public async Task<List<Post>> GetAll(PostParameters postParameters)
    {
        var query = $@"SELECT * FROM ""{TableNames.post}"" LIMIT @Limit OFFSET @Offset";

        List<Post> res;
        using (var con = NewConnection)
            res = (await con.QueryAsync<Post>(query, new { Limit = postParameters.PageSize, Offset = (postParameters.PageNumber - 1) * postParameters.PageSize }))
            // .Skip((employeeParameters.PageNumber - 1) * employeeParameters.PageSize)
            // .Take(employeeParameters.PageSize)
            .AsList();

        return res;
    }

    public async Task<List<Post>> ToListAsync()
    {
        var query = $@"SELECT * FROM ""{TableNames.post}""";

        List<Post> res;
        using (var con = NewConnection)
            res = (await con.QueryAsync<Post>(query))

            .AsList();

        return res;
    }
}