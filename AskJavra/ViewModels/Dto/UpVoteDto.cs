namespace AskJavra.ViewModels.Dto
{
    public class UpVoteDto
    {
    }
    public class UpvoteCountViewMode
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public Guid PostId { get; set; }
    }
}
