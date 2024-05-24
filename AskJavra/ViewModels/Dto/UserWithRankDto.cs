using AskJavra.DataContext;

namespace AskJavra.ViewModels.Dto
{
    public class UserWithRankDto
    {
        public ApplicationUser User { get; set; }
        public UserRankDetails UserRank { get; set; }
        public List<RankDetails> RankDetails { get; set; }
    }
    public class UserRankDetails
    {
        public int TotalPoint { get; set; }
        public string  RankName { get; set; }
    }
    public class RankDetails
    {
        public int MinPoint { get; set; }
        public int MaxPoint { get; set; }
        public string RankName { get; set;}
    }

}
