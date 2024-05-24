using AskJavra.DataContext;
using AskJavra.Models.Post;
using AskJavra.Models.Root;
using AskJavra.ViewModels.Dto;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata.Ecma335;

namespace AskJavra.Repositories.Service
{
    public class TagService
    {
        private readonly ApplicationDBContext _context;
        private readonly DbSet<Tag> _dbSet;
        private readonly DbSet<PostTag> _dbPostSet;

        public TagService(ApplicationDBContext context)
        {
            _context = context;
            _dbSet = _context.Set<Tag>();
            _dbPostSet = _context.Set<PostTag>();
        }

        public async Task<IEnumerable<TagViewDto>> GetAllAsync()
        {
            var result = await _dbSet.Select(x=> new TagViewDto
            {
                Id = x.Id,
                Name = x.Name,
                TagDescription = x.TagDescription,
                TotalFeedMentions = _dbPostSet.Where(y=>y.TagId == x.Id).Count()
            }).ToListAsync();
            return result;
        }

        public async Task<ResponseDto<Tag>> GetByIdAsync(int id)
        {
            try
            {
                var tag = await _dbSet.FindAsync(id);
                if (tag != null)
                    return new ResponseDto<Tag>(true, "Success", tag);
                else
                    return new ResponseDto<Tag>(false, "not found", new Tag());
            }
            catch (Exception ex)
            {
                return new ResponseDto<Tag>(false, ex.Message, new Tag());
            }
        }

        public async Task<ResponseDto<Tag>> AddAsync(TagDto entity)
        {
            try
            {
                var tag = new Tag(entity.Name, entity.TagDescription);
                _dbSet.Add(tag);
                await _context.SaveChangesAsync();
                var result = new ResponseDto<Tag>(true, "Record added successfully", tag);
                return result;
            }
            catch (Exception ex)
            {
                return new ResponseDto<Tag>(false, ex.Message, new Tag());
            }
        }

        public async Task<ResponseDto<Tag>> UpdateAsync(Tag entity)
        {
            try
            {
                if(await _dbSet.FindAsync(entity.Id) == null)
                    return new ResponseDto<Tag>(false, "not found", entity);
                
                _dbSet.Attach(entity);
                _context.Entry(entity).State = EntityState.Modified;
              
                await  _context.SaveChangesAsync();
                return new ResponseDto<Tag>(true, "Record updated successfully", entity);
            }
            catch (Exception ex)
            {
                return new ResponseDto<Tag>(false, ex.Message, null);
            }
        }

        public async Task<ResponseDto<TagDto>> DeleteAsync(int id)
        {
            try
            {
                var entity = await _dbSet.FindAsync(id);
                if (entity != null)
                {
                    _dbSet.Remove(entity);
                    await _context.SaveChangesAsync();

                    return new ResponseDto<TagDto>(true, "Record deleted successfully", new TagDto());
                }
                else
                    return new ResponseDto<TagDto>(false, "not found", new TagDto());
            }
            catch (Exception ex)
            {
                return new ResponseDto<TagDto>(false, ex.Message, new TagDto());
            }

        }
    }
}
