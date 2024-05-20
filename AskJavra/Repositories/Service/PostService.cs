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

        public PostService(ApplicationDBContext context)
        {
            _context = context;
            _dbSet = _context.Set<Post>();
        }

        public async Task<List<PostDto>> GetAllAsync()
        {
            var post = await _dbSet.Include(t => t.Tags).ThenInclude(x=>x.Tag).Include(p => p.Threads).ToListAsync();
            return post.Select(x=> new PostDto
            {
                Title = x.Title,
                Description = x.Description,
                PostType = x.PostType,
                Tags = x.Tags.Select(t=> new PostTagDto
                {
                    PostId = t.PostId,
                    TagId = t.TagId,
                    TagDescription = t.Tag.TagDescription,
                    TagName = t.Tag.Name

                }).ToList(),

            }).ToList();
        }

        public async Task<ResponseDto<Post>> GetByIdAsync(Guid id)
        {
            try
            {
                var post = await _dbSet.FindAsync(id);
                if (post != null)
                    return new ResponseDto<Post>(true, "Success", post);
                else
                    return new ResponseDto<Post>(false, "not found", new Post());
            }
            catch (Exception ex)
            {
                return new ResponseDto<Post>(false, ex.Message, new Post());
            }
        }

        public async  Task<ResponseDto<Post>> AddAsync(PostDto entity)
        {
            try
            {
                var post = new Post(entity.Title, entity.Description, entity.PostType, new List<PostThread>(), new List<PostTag>());
                
                await _dbSet.AddAsync(post);
                await _context.SaveChangesAsync();

                return new ResponseDto<Post>(true, "Record added successfully", post);
            }
            catch (Exception ex)
            {
                return new ResponseDto<Post>(false, ex.Message, new Post());
            }
        }

        public async Task<ResponseDto<Post>> UpdateAsync(Post entity)
        {
            try
            {
                if(await _dbSet.FindAsync(entity.Id) == null)
                    return new ResponseDto<Post>(false, "not found", entity);

                _dbSet.Attach(entity);
                _context.Entry(entity).State = EntityState.Modified;
               
                await _context.SaveChangesAsync();
                return new ResponseDto<Post>(true, "Record updated successfully", entity);
            }
            catch (Exception ex)
            {
                return new ResponseDto<Post>(false, ex.Message, new Post());
            }
        }

        public async Task<ResponseDto<PostDto>> DeleteAsync(Guid id)
        {
            try
            {
                var entity = await _dbSet.FindAsync(id);
                if (entity != null)
                {
                    _dbSet.Remove(entity);
                    await _context.SaveChangesAsync();
                    return new ResponseDto<PostDto>(true, "Record deleted successfully", new PostDto());
                }
                else
                    return new ResponseDto<PostDto>(false, "not found", new PostDto());
            }
            catch (Exception ex)
            {
                return new ResponseDto<PostDto>(false, ex.Message, new PostDto());
            }

        }
    }
}
