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
        private readonly DbSet<Post> _postDBSet;

        public PostThreadService(ApplicationDBContext context)
        {
            _context = context;
            _dbSet = _context.Set<PostThread>();
            _postDBSet = _context.Set<Post>();
        }

        public async Task<List<PostThreadDto>> GetAllAsync()
        {
            var result = await _dbSet.Include(x=>x.Post).ThenInclude(x=>x.Tags).ThenInclude(x=>x.Tag).ToListAsync();
            return result.Select(x=> new PostThreadDto
            {
                PostId = x.PostId,
                ThreadTitle = x.ThreadTitle,
                ThreadDescription = x.ThreadDescription,                
                Post = new PostDto { Description = x.Post.Description,
                    PostType = (Enums.PostType)x.Post.PostType,
                    Title = x.Post.Title, 
                    Tags = x.Post.Tags.Select(t => new PostTagDto
                    {
                        PostId = t.PostId,
                        TagId = t.TagId,
                        TagDescription = t.Tag.TagDescription,
                        TagName = t.Tag.Name

                    }).ToList(),
                }
            }).ToList();
        }

        public async  Task<ResponseDto<PostThread>> GetByIdAsync(Guid id)
        {
            try
            {
                var postThread = await _dbSet.FindAsync(id);
                if (postThread == null) return new ResponseDto<PostThread>(false, "not found", postThread);
                else
                    return new ResponseDto<PostThread>(true, "Success", postThread);
            }
            catch (Exception ex)
            {
                return new ResponseDto<PostThread>(false, ex.Message, new PostThread());
            }
        }
        public async Task<ResponseDto<PostThread>> AddAsync(PostThreadDto entity)
        {
            try
            {
                Post post = new Post();
                var postFetched =await _postDBSet.FindAsync(entity.PostId);
                if (post != null)
                    post = postFetched;
                else
                    return new ResponseDto<PostThread>(false, "Invalid post id", new PostThread());

                var postThread = new PostThread(entity.ThreadTitle, entity.ThreadDescription, entity.PostId, post);
                await _dbSet.AddAsync(postThread);
                await _context.SaveChangesAsync();

                return new ResponseDto<PostThread>(true, "Record added successfully", postThread);
            }
            catch (Exception ex)
            {
                return new ResponseDto<PostThread>(false, ex.Message, new PostThread());
            }
        }

        public async Task<ResponseDto<PostThread>> UpdateAsync(PostThread entity)
        {
            try
            {
                var post =await _postDBSet.FindAsync(entity.PostId);
                if (post == null)
                   return new ResponseDto<PostThread>(false, "Invalid post id", new PostThread ());
                entity.Post = post;
                
                _dbSet.Attach(entity);
                _context.Entry(entity).State = EntityState.Modified;

                await _context.SaveChangesAsync();
                return new ResponseDto<PostThread>(true, "Record updated successfully", entity);
            }
            catch (Exception ex)
            {
                return new ResponseDto<PostThread>(false, ex.Message, new PostThread());
            }
        }

        public async Task<ResponseDto<PostThreadDto>> DeleteAsync(Guid id)
        {
            try
            {
                var entity = _dbSet.Find(id);
                if (entity != null)
                {
                    _dbSet.Remove(entity);
                    await _context.SaveChangesAsync();

                    return new ResponseDto<PostThreadDto>(true, "Record deleted successfully", new PostThreadDto(entity.ThreadTitle, entity.ThreadDescription, entity.PostId));
                }
                else
                    return new ResponseDto<PostThreadDto>(false, "not found", new PostThreadDto());
                
            }
            catch (Exception ex)
            {
                return new ResponseDto<PostThreadDto>(false, ex.Message, new PostThreadDto());
            }
        }
    }
}
