using AskJavra.DataContext;
using AskJavra.Models.Post;
using AskJavra.ViewModels.Dto;
using Microsoft.EntityFrameworkCore;

namespace AskJavra.Repositories.Service
{
    public class PostService
    {
        private readonly ApplicationDBContext _context;
        private readonly DbSet<Post> _dbSet;
        //private readonly PostThreadService _postThreadService;

        public PostService(ApplicationDBContext context /*,PostThreadService postThreadService*/)
        {
            _context = context;
            _dbSet = _context.Set<Post>();
            //_postThreadService = postThreadService;
        }

        public IEnumerable<Post> GetAllAsync()
        {
            return _dbSet.ToList();
        }

        public ResponseDto<Post> GetByIdAsync(Guid id)
        {
            try
            {
                var tag = _dbSet.Find(id);
                if (tag != null)
                    return new ResponseDto<Post>(true, "Success", tag);
                else
                    return new ResponseDto<Post>(false, "Error", new Post());
            }
            catch (Exception ex)
            {
                return new ResponseDto<Post>(false, "Error", new Post());
            }
        }

        public ResponseDto<Post> AddAsync(PostDto entity)
        {
            try
            {
                var post = new Post(entity.Title, entity.Description, entity.PostType, new List<PostThread>(), new List<PostTag>());
                _dbSet.Add(post);
                _context.SaveChanges();

                return new ResponseDto<Post>(true, "Record added successfully", post);
            }
            catch (Exception ex)
            {
                return new ResponseDto<Post>(false, "Error", new Post());
            }
        }

        public ResponseDto<Post> UpdateAsync(Post entity)
        {
            try
            {
                _dbSet.Attach(entity);
                _context.Entry(entity).State = EntityState.Modified;
                _context.SaveChanges();
                return new ResponseDto<Post>(true, "Record updated successfully", entity);
            }
            catch (Exception ex)
            {
                return new ResponseDto<Post>(false, ex.Message, new Post());
            }
        }

        public ResponseDto<PostDto> DeleteAsync(int id)
        {
            try
            {
                var entity = _dbSet.Find(id);
                if (entity != null)
                {
                    _dbSet.Remove(entity);
                    _context.SaveChanges();
                }
                return new ResponseDto<PostDto>(true, "Record deleted successfully", new PostDto(entity.Title, entity.Description, entity.PostType));
            }
            catch (Exception ex)
            {
                return new ResponseDto<PostDto>(false, "Error", new PostDto());
            }

        }
    }
}
