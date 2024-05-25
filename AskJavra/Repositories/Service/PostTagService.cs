using AskJavra.DataContext;
using AskJavra.Models.Post;
using AskJavra.ViewModels.Dto;
using Microsoft.EntityFrameworkCore;

namespace AskJavra.Repositories.Service
{
    public class PostTagService
    {
        private readonly ApplicationDBContext _context;
        private readonly DbSet<PostTag> _dbSet;

        public PostTagService(
            PostService postService, 
            ApplicationDBContext context
            )
        {
            _context = context;
            _dbSet = _context.Set<PostTag>();
        }

        public async Task<IEnumerable<PostTag>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<ResponseDto<PostTag>> GetByIdAsync(int id)
        {
            try
            {
                var postTag = await _dbSet.FindAsync(id);
                if (postTag != null)
                    return new ResponseDto<PostTag>(true, "Success", postTag);
                else
                    return new ResponseDto<PostTag>(false, "not found", new PostTag());
            }
            catch (Exception ex)
            {
                return new ResponseDto<PostTag>(true, "Error", new PostTag());
            }
        }
        //public List<PostThread> GetThreadByPostId(Guid postId)
        //{
        //    try
        //    {
        //        var result = _dbSet.Where(x=>x.PostId == postId).ToList();
        //        return result;
        //    }
        //    catch (Exception ex)
        //    {
        //        return new List<PostThread>();
        //    }
        //}

        public async Task<ResponseDto<List<PostTag>>> AddPostTagAsync(int[] tags, Post post)
        {
            try
            {
                List<PostTag> postTags = new List<PostTag>();
                foreach (var item in tags)
                    postTags.Add(new PostTag(item, post.Id, post));    
                await _dbSet.AddRangeAsync(postTags);
                await _context.SaveChangesAsync();

                return new ResponseDto<List<PostTag>>(true, "Thread tag added successfully", postTags);
            }
            catch (Exception ex)
            {
                return new ResponseDto<List<PostTag>>(false, ex.Message, new List<PostTag>()); ;
            }
        }
       
        public async Task<bool> deletePostTagByPostId(int[] tags, Guid postId)
        {
            try
            {
                List<PostTag> tagsTec = new List<PostTag>();

                foreach(var item in tags)
                {
                    tagsTec.AddRange(_dbSet.Where(x => x.TagId == item && x.PostId == postId).ToList());
                }

                _dbSet.RemoveRange(tagsTec);
                await _context.SaveChangesAsync();
                return true;
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        //public async Task<ResponseDto<List<PostTag>>> AddPostThreadTagAsync(List<ThreadTagDto> entity, PostThread thred)
        //{
        //    try
        //    {
        //        List<PostTag> threadtags = new List<PostTag>();
        //        foreach(var item in entity)
        //            threadtags.Add(new PostTag(item.TagId, item.PostThreadId, thred));

        //        await _dbSet.AddRangeAsync(threadtags);
        //        await _context.SaveChangesAsync();

        //        return new ResponseDto<List<PostTag>>(true, "Thread tag added successfully", threadtags);
        //    }
        //    catch (Exception ex)
        //    {
        //        return new ResponseDto<List<PostTag>>(true, ex.Message, new List<PostTag>());
        //    }
        //}

        public async Task<ResponseDto<PostTag>> UpdatePostTagAsync(int postTagId, PostTagDto entity, Post post)
        {
            try
            {
                PostTag postTag = new PostTag(postTagId, entity.TagId, entity.PostId, post);
               

                _dbSet.Attach(postTag);
                _context.Entry(postTag).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return new ResponseDto<PostTag>(true, "Record updated successfully", postTag);
            }
            catch (Exception ex)
            {
                return new ResponseDto<PostTag>(true, ex.Message, new PostTag());
            }
        }
        //public async  Task<ResponseDto<PostTag>> UpdatePostThreadAsync(int postTagId, ThreadTagDto entity, Post post)
        //{
        //    try
        //    {
        //        PostTag postTag = new PostTag(postTagId, entity.TagId, entity.PostThreadId, post);


        //        _dbSet.Attach(postTag);
        //        _context.Entry(postTag).State = EntityState.Modified;
        //        await _context.SaveChangesAsync();
        //        return new ResponseDto<PostTag>(true, "Record updated successfully", postTag);
        //    }
        //    catch (Exception ex)
        //    {
        //        return new ResponseDto<PostTag>(true, ex.Message, new PostTag());
        //    }
        //}

        public async Task<ResponseDto<PostTagDto>> DeleteAsync(int postTagId)
        {
            try
            {
                var entity = await _dbSet.FindAsync(postTagId);
                if (entity != null)
                {
                    _dbSet.Remove(entity);
                    await _context.SaveChangesAsync();
                }
                return new ResponseDto<PostTagDto>(true, "Record deleted successfully", new PostTagDto((int)entity.TagId,(Guid) entity.PostId));
            }
            catch (Exception ex)
            {
                return new ResponseDto<PostTagDto>(true, "Error", new PostTagDto());
            }

        }
    }
}
