using AskJavra.DataContext;
using AskJavra.Models.Post;
using AskJavra.ViewModels.Dto;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AskJavra.Repositories.Service
{
    public class PostThreadService
    {
        private readonly ApplicationDBContext _context;
        private readonly DbSet<PostThread> _dbSet;
        private readonly DbSet<ThreadUpVote> _threadUpvotedbSet;
        private readonly DbSet<Post> _postDBSet;
        private readonly UserManager<ApplicationUser> _userManager;

        public PostThreadService(
            ApplicationDBContext context
            , UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _dbSet = _context.Set<PostThread>();
            _postDBSet = _context.Set<Post>();
            _threadUpvotedbSet = _context.Set<ThreadUpVote>();
            _userManager = userManager;
        }

        public async Task<List<PostThreadViewDto>> GetAllAsync()
        {
            var result = await _dbSet.Include(x=>x.Post).ThenInclude(x=>x.Tags).ThenInclude(x=>x.Tag).ToListAsync();
            return result.Select(x=> new PostThreadViewDto
            {
                PostId = x.PostId,
                ThreadTitle = x.ThreadTitle,
                ThreadDescription = x.ThreadDescription,                
                Post = new PostViewDto
                { Description = x.Post.Description,
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
        public async Task<ResponseDto<PostThreadViewDto>> AddAsync(PostThreadViewDto entity)
        {
            try
            {
                Post post = new Post();
                var postFetched =await _postDBSet.FindAsync(entity.PostId);

                if (post != null)
                    post = postFetched;
                else
                    return new ResponseDto<PostThreadViewDto>(false, "Invalid post id", new PostThreadViewDto());

                var postThread = new PostThread(entity.ThreadTitle, entity.ThreadDescription, entity.PostId, post);
                await _dbSet.AddAsync(postThread);
                await _context.SaveChangesAsync();
                var result = new PostThreadViewDto
                {
                    PostId = postThread.PostId,
                    ThreadTitle = postThread.ThreadTitle,
                    ThreadDescription = postThread.ThreadDescription,
                    Post = new PostViewDto
                    {
                        Description = postThread.Post.Description,
                        PostType = (Enums.PostType)postThread.Post.PostType,
                        Title = postThread.Post.Title,
                        Tags = postThread.Post.Tags.Select(t => new PostTagDto
                        {
                            PostId = t.PostId,
                            TagId = t.TagId,
                            TagDescription = t.Tag.TagDescription,
                            TagName = t.Tag.Name

                        }).ToList(),
                    }
                };
                return new ResponseDto<PostThreadViewDto>(true, "Record added successfully", result);
            }
            catch (Exception ex)
            {
                return new ResponseDto<PostThreadViewDto>(false, ex.Message, new PostThreadViewDto());
            }
        }

        public async Task<ResponseDto<PostThreadViewDto>> UpdateAsync(PostThread entity)
        {
            try
            {
                var post =await _postDBSet.FindAsync(entity.PostId);
                if (post == null)
                   return new ResponseDto<PostThreadViewDto>(false, "Invalid post id", new PostThreadViewDto());
                entity.Post = post;
                
                _dbSet.Attach(entity);
                _context.Entry(entity).State = EntityState.Modified;

                await _context.SaveChangesAsync();
                var result = new PostThreadViewDto
                {
                    PostId = entity.PostId,
                    ThreadTitle = entity.ThreadTitle,
                    ThreadDescription = entity.ThreadDescription,
                    Post = new PostViewDto
                    {
                        Description = entity.Post.Description,
                        PostType = (Enums.PostType)entity.Post.PostType,
                        Title = entity.Post.Title,
                        Tags = entity.Post.Tags.Select(t => new PostTagDto
                        {
                            PostId = t.PostId,
                            TagId = t.TagId,
                            TagDescription = t.Tag.TagDescription,
                            TagName = t.Tag.Name

                        }).ToList(),
                    }
                };
                return new ResponseDto<PostThreadViewDto>(true, "Record updated successfully", result);
            }
            catch (Exception ex)
            {
                return new ResponseDto<PostThreadViewDto>(false, ex.Message, new PostThreadViewDto());
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
        public async Task<ResponseDto<PostUpvoteResponseDto>> UpvoteThread(Guid threadId, string upvoteBy)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(upvoteBy);
                if (user == null)
                    return new ResponseDto<PostUpvoteResponseDto>(false, "User not found", new PostUpvoteResponseDto());
                var thread = await _dbSet.FindAsync(threadId);
                if (thread == null)
                    return new ResponseDto<PostUpvoteResponseDto>(false, "Invalid Post Id", new PostUpvoteResponseDto());
                var checkIfUpVoteAlreadyExist = await _threadUpvotedbSet.Where(x => x.UserId == upvoteBy && x.ThreadId == threadId).FirstOrDefaultAsync();
                if (checkIfUpVoteAlreadyExist == null)
                {
                    ThreadUpVote postUpVote = new ThreadUpVote
                    {
                        UserId = user.Id,
                        User = user,
                        ThreadId = threadId,
                        Thread = thread
                    };
                    await _threadUpvotedbSet.AddAsync(postUpVote);
                    await _context.SaveChangesAsync();
                    var response = new PostUpvoteResponseDto
                    {
                        PostDescription = thread.ThreadDescription,
                        PostTitle = thread.ThreadTitle,
                        UpvoteBy = user.FullName
                    };
                    return new ResponseDto<PostUpvoteResponseDto>(true, "Feed upvote raised succesfully.", response);
                }
                else
                {
                    return await RevokeUpvoteThread(checkIfUpVoteAlreadyExist);
                }


            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<ResponseDto<PostUpvoteResponseDto>> RevokeUpvoteThread(ThreadUpVote postUpVote)
        {
            try
            {
                if (postUpVote != null)
                {
                    _threadUpvotedbSet.Remove(postUpVote);
                    await _context.SaveChangesAsync();
                    return new ResponseDto<PostUpvoteResponseDto>(true, "Upvote revoked successfully", new PostUpvoteResponseDto());
                }
                else
                    return new ResponseDto<PostUpvoteResponseDto>(false, "not found", new PostUpvoteResponseDto());
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
