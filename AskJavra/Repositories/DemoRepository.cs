using AskJavra.DataContext;
using AskJavra.Models;

namespace AskJavra.Repositories
{
    public class DemoRepository
    {
        private readonly ApplicationDBContext _applicationDBContext;
        public DemoRepository(ApplicationDBContext applicationDBContext)
        {
            _applicationDBContext = applicationDBContext;
        }
        public string DemoMethod()
        {
            return "Hello World!!";
        }

        public void CreateDemo(Demo demo)
        {
            _applicationDBContext.Add(demo);
            _applicationDBContext.SaveChanges();
        }
    }
}
