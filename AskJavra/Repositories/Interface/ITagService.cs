using AskJavra.Models.Root;
using System.Xml;

namespace AskJavra.Repositories.Interface
{
    public interface ITagService
    {
        Task<IEnumerable<Tag>> GetAllAsync();
        Task<Tag> GetByIdAsync(int id);
        Task AddAsync(Tag entity);
        Task UpdateAsync(Tag entity);
        Task DeleteAsync(int id);
    }
}
