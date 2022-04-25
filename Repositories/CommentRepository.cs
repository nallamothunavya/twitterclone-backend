using Twittertask.Models;
using Dapper;
using Twittertask.Utilities;


namespace Twittertask.Repositories;

public interface ICommentRepository
{
    Task<Comment> Create(Comment Item);

    Task Delete(long Id);
    Task<List<Comment>> GetAll();

    Task<Comment> GetById(long Id);

    Task<List<Comment>> GetCommentsByPostId(int PostId);

    Task<List<Comment>> GetAll(CommentParameters commentParameters);

}

public class CommentRepository : BaseRepository, ICommentRepository
{
    public CommentRepository(IConfiguration config) : base(config)
    {

    }

    public async Task<Comment> Create(Comment Item)
    {
        var query = $@"INSERT INTO {TableNames.comment} (post_id, user_id, text, created_at, updated_at) 
        VALUES (@PostId, @UserId, @Text, @CreatedAt, @UpdatedAt) RETURNING *";

        using (var con = NewConnection)
            return await con.QuerySingleOrDefaultAsync<Comment>(query, Item);
    }

    public async Task Delete(long Id)
    {
        var query = $@"DELETE FROM {TableNames.comment} WHERE id = @Id";

        using (var con = NewConnection)
            await con.ExecuteAsync(query, new { Id });
    }

    public async Task<List<Comment>> GetAll()
    {
        var query = $@"SELECT * FROM {TableNames.comment} ORDER BY created_at DESC";

        using (var con = NewConnection)
            return (await con.QueryAsync<Comment>(query)).AsList();
    }

    public async Task<List<Comment>> GetAll(CommentParameters commentParameters)
    {
        var query = $@"SELECT * FROM ""{TableNames.comment}"" LIMIT @Limit OFFSET @Offset";

        List<Comment> res;
        using (var con = NewConnection)
            res = (await con.QueryAsync<Comment>(query, new { Limit = commentParameters.PageSize, Offset = (commentParameters.PageNumber - 1) * commentParameters.PageSize }))
            // .Skip((employeeParameters.PageNumber - 1) * employeeParameters.PageSize)
            // .Take(employeeParameters.PageSize)
            .AsList();

        return res;
    }


    public async Task<Comment> GetById(long Id)
    {
        var query = $@"SELECT * FROM {TableNames.comment} WHERE id = @Id";

        using (var con = NewConnection)
            return await con.QuerySingleOrDefaultAsync<Comment>(query, new { Id });
    }

    public async Task<List<Comment>> GetCommentsByPostId(int PostId)
    {
        var query = $@"SELECT * FROM {TableNames.comment} WHERE Post_id = @PostId";

        using (var con = NewConnection)
            return (await con.QueryAsync<Comment>(query, new { PostId })).AsList();
    }

}
