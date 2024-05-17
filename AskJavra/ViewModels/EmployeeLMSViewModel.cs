using AskJavra.Enums;

namespace AskJavra.ViewModels
{
    public class UserInfos 
    {
        public List<EmployeeLMSViewModel> UserInfo { get; set; }
    }
    public class EmployeeLMSViewModel
    {
        public string FullName { get; set; }
        public string Designation { get; set; }
        public string Department { get; set; }
        public string Country { get; set; }
        public string Status { get; set; }
        public string Username { get; set; }
        public required string Email { get; set; }
        public required int EmployeeID { get; set; }
        public string Phone { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
    }
}
