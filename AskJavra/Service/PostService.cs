using AskJavra.Models.Post;
using AskJavra.Repositories.Interface;

namespace AskJavra.Service
{
    public class PostService
    {
        private readonly IRepository<Post> _postRepository;
        public PostService(IRepository<Post> postRepository)
        {
            _postRepository = postRepository;
        }
        public async Task<IEnumerable<Post>> GetAllProductsAsync()
        {
            return await _postRepository.GetAllAsync();
        }
    }
}
