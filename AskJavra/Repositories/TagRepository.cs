using AskJavra.Models.Post;
using AskJavra.Repositories.Interface;

namespace AskJavra.Repositories
{
    public class TagRepository
    {
        private readonly IRepository<Post> _postRepository;
        public TagRepository(IRepository<Post> postRepository)
        {
           _postRepository = postRepository;
        }
        public async Task<IEnumerable<Post>> GetAllPostAsync()
        {
            return await _postRepository.GetAllAsync();
        }
    }
}
