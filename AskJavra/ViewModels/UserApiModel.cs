using AskJavra.ViewModels.Dto;

namespace AskJavra.ViewModels
{
    public class UserApiModel
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public string UserName { get; set; }
        public string PhoneNumber { get; set; }
        public bool Active { get; set; }
        public string Email { get; set; }
        public string Department { get; set; }
        public UserRankDetails UserRank { get; set; } = new UserRankDetails();
    }
    public class UserApiViewDto { 
         public List<UserApiModel> userApi { get; set; }        
        public List<RankDetails> RankDetails { get; set; }
    }
}
