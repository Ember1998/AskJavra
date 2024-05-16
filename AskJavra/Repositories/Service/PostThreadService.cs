using AskJavra.DataContext;
using AskJavra.Models.Post;
using AskJavra.ViewModels.Dto;
using Microsoft.EntityFrameworkCore;

namespace AskJavra.Repositories.Service
{
    public class PostThreadService
    {
        private readonly ApplicationDBContext _context;
        private readonly DbSet<PostThread> _dbSet;
        private readonly PostService _postService;

        public PostThreadService(ApplicationDBContext context, PostService postService)
        {
            _context = context;
            _dbSet = _context.Set<PostThread>();
            _postService = postService;
        }

        public IEnumerable<PostThread> GetAllAsync()
        {
            return _dbSet.ToList();
        }

        public ResponseDto<PostThread> GetByIdAsync(Guid id)
        {
            try
            {
                var postThread = _dbSet.Find(id);
                return new ResponseDto<PostThread>(true, "Success", postThread);
            }
            catch (Exception ex)
            {
                return new ResponseDto<PostThread>(true, "Error", new PostThread());
            }
        }
        public List<PostThread> GetThreadByPostId(Guid postId)
        {
            try
            {
                var result = _dbSet.Where(x=>x.PostId == postId).ToList();
                return result;
            }
            catch (Exception ex)
            {
                return new List<PostThread>();
            }
        }

        public ResponseDto<PostThread> AddAsync(PostThreadDto entity)
        {
            try
            {
                Post post = new Post();
                var postResult = _postService.GetByIdAsync(entity.PostId);
                if (postResult.Success)
                    post = postResult.Data;
                var postThread = new PostThread(entity.ThreadTitle, entity.ThreadDescription, entity.PostId, post);
                _dbSet.Add(postThread);
                _context.SaveChanges();

                return new ResponseDto<PostThread>(true, "Record added successfully", postThread);
            }
            catch (Exception ex)
            {
                return new ResponseDto<PostThread>(true, "Error", new PostThread());
            }
        }

        public ResponseDto<PostThread> UpdateAsync(PostThread entity)
        {
            try
            {
                Post post = new Post();
                var postResult = _postService.GetByIdAsync(entity.PostId);
                if (postResult.Success)
                    post = postResult.Data;
                entity.Post = postResult.Data;
                
                _dbSet.Attach(entity);
                _context.Entry(entity).State = EntityState.Modified;
                _context.SaveChanges();
                return new ResponseDto<PostThread>(true, "Record updated successfully", entity);
            }
            catch (Exception ex)
            {
                return new ResponseDto<PostThread>(true, ex.Message, new PostThread());
            }
        }

        public ResponseDto<PostThreadDto> DeleteAsync(Guid id)
        {
            try
            {
                var entity = _dbSet.Find(id);
                if (entity != null)
                {
                    _dbSet.Remove(entity);
                    _context.SaveChanges();
                }
                return new ResponseDto<PostThreadDto>(true, "Record deleted successfully", new PostThreadDto(entity.ThreadTitle, entity.ThreadDescription, entity.PostId));
            }
            catch (Exception ex)
            {
                return new ResponseDto<PostThreadDto>(true, "Error", new PostThreadDto());
            }

        }
    }
}
