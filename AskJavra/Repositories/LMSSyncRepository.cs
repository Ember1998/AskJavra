using AskJavra.DataContext;
using AskJavra.Enums;
using AskJavra.Models;
using AskJavra.Models.Employee;
using AskJavra.ViewModels;

namespace AskJavra.Repositories
{
    public class LMSSyncRepository
    {
        private readonly ApplicationDBContext _applicationDBContext;
        public LMSSyncRepository(ApplicationDBContext applicationDBContext)
        {
            _applicationDBContext = applicationDBContext;
        }
        public async Task SyncLMSdata(UserInfos userInfo)
        {
            var Employees  =userInfo.UserInfo.Select(x=> new Employee { 
                Username = x.Username,
                Status = (EmpStatus) Enum.Parse(typeof(EmpStatus),x.Status, true),
                Country = x.Country,
                Department = x.Department,
                Name = x.FullName,
                Designation = x.Designation
            });

            _applicationDBContext.AddRange(Employees);
            _applicationDBContext.SaveChanges();

        }

        public void CreateDemo(Demo demo)
        {
            _applicationDBContext.Add(demo);
            _applicationDBContext.SaveChanges();
        }
    }
}
