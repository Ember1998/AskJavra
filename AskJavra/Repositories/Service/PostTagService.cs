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
        private readonly PostService _postService;
        private readonly PostThreadService _postThreadService;

        public PostTagService(
            PostService postService, 
            ApplicationDBContext context,
            PostThreadService postThreadService
            )
        {
            _context = context;
            _dbSet = _context.Set<PostTag>();
            _postService = postService;
            _postThreadService = postThreadService;
        }

        public IEnumerable<PostTag> GetAllAsync()
        {
            return _dbSet.ToList();
        }

        public ResponseDto<PostTag> GetByIdAsync(int id)
        {
            try
            {
                var postTag = _dbSet.Find(id);
                return new ResponseDto<PostTag>(true, "Success", postTag);
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

        public async Task<ResponseDto<List<PostTag>>> AddPostTagAsync(List<PostTagDto> entity, Post post)
        {
            try
            {
                List<PostTag> postTags = new List<PostTag>();
                foreach (var item in entity)
                    postTags.Add(new PostTag(item.TagId, item.PostId, post));    
                await _dbSet.AddRangeAsync(postTags);
                await _context.SaveChangesAsync();

                return new ResponseDto<List<PostTag>>(true, "Thread tag added successfully", postTags);
            }
            catch (Exception ex)
            {
                return new ResponseDto<List<PostTag>>(false, ex.Message, new List<PostTag>()); ;
            }
        }
        public async Task<ResponseDto<List<PostTag>>> AddPostThreadTagAsync(List<ThreadTagDto> entity, PostThread thred)
        {
            try
            {
                List<PostTag> threadtags = new List<PostTag>();
                foreach(var item in entity)
                    threadtags.Add(new PostTag(item.TagId, item.PostThreadId, thred));

                await _dbSet.AddRangeAsync(threadtags);
                await _context.SaveChangesAsync();

                return new ResponseDto<List<PostTag>>(true, "Thread tag added successfully", threadtags);
            }
            catch (Exception ex)
            {
                return new ResponseDto<List<PostTag>>(true, ex.Message, new List<PostTag>());
            }
        }

        //public ResponseDto<PostThread> UpdateAsync(PostTag entity)
        //{
        //    try
        //    {
        //        Post post = new Post();
        //        var postResult = _postService.GetByIdAsync(entity.PostId);
        //        if (postResult.Success)
        //            post = postResult.Data;
        //        entity.Post = postResult.Data;
                
        //        _dbSet.Attach(entity);
        //        _context.Entry(entity).State = EntityState.Modified;
        //        _context.SaveChanges();
        //        return new ResponseDto<PostThread>(true, "Record updated successfully", entity);
        //    }
        //    catch (Exception ex)
        //    {
        //        return new ResponseDto<PostThread>(true, ex.Message, new PostThread());
        //    }
        //}

        public ResponseDto<PostTagDto> DeleteAsync(int id)
        {
            try
            {
                var entity = _dbSet.Find(id);
                if (entity != null)
                {
                    _dbSet.Remove(entity);
                    _context.SaveChanges();
                }
                return new ResponseDto<PostTagDto>(true, "Record deleted successfully", new PostTagDto(entity.TagId, entity.PostId, entity.PostThreadId));
            }
            catch (Exception ex)
            {
                return new ResponseDto<PostTagDto>(true, "Error", new PostTagDto());
            }

        }
    }
}
