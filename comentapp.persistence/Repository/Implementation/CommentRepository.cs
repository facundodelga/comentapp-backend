using comentapp.persistence.Models;

namespace comentapp.persistence.Repository.Implementation
{
    public class CommentRepository(ComentappDbContext context)
        : Repository<Comment>(context), ICommentRepository
    {
    }
}
