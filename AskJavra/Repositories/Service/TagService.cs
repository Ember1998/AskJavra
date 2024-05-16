using AskJavra.DataContext;
using AskJavra.Models.Root;
using AskJavra.ViewModels.Dto;
using Microsoft.EntityFrameworkCore;

namespace AskJavra.Repositories.Service
{
    public class TagService
    {
        private readonly ApplicationDBContext _context;
        private readonly DbSet<Tag> _dbSet;

        public TagService(ApplicationDBContext context)
        {
            _context = context;
            _dbSet = _context.Set<Tag>();
        }

        public IEnumerable<Tag> GetAllAsync()
        {
            return _dbSet.ToList();
        }

        public ResponseDto<Tag> GetByIdAsync(int id)
        {
            try
            {
                var tag = _dbSet.Find(id);
                return new ResponseDto<Tag>(true, "Success", tag);
            }
            catch (Exception ex)
            {
                return new ResponseDto<Tag>(true, "Error", new Tag());
            }
        }

        public ResponseDto<Tag> AddAsync(TagDto entity)
        {
            try
            {
                var tag = new Tag(entity.Name, entity.TagDescription);
                _dbSet.Add(tag);
                _context.SaveChanges();

                return new ResponseDto<Tag>(true, "Record added successfully", tag);
            }
            catch (Exception ex)
            {
                return new ResponseDto<Tag>(true, "Error", new Tag());
            }
        }

        public ResponseDto<Tag> UpdateAsync(Tag entity)
        {
            try
            {
                _dbSet.Attach(entity);
                _context.Entry(entity).State = EntityState.Modified;
                _context.SaveChanges();
                return new ResponseDto<Tag>(true, "Record updated successfully", entity);
            }
            catch (Exception ex)
            {
                return new ResponseDto<Tag>(true, ex.Message, new Tag());
            }
        }

        public ResponseDto<TagDto> DeleteAsync(int id)
        {
            try
            {
                var entity = _dbSet.Find(id);
                if (entity != null)
                {
                    _dbSet.Remove(entity);
                    _context.SaveChanges();
                }
                return new ResponseDto<TagDto>(true, "Record deleted successfully", new TagDto(entity.Name, entity.TagDescription));
            }
            catch (Exception ex)
            {
                return new ResponseDto<TagDto>(true, "Error", new TagDto());
            }

        }
    }
}
