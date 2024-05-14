using AskJavra.Models.Root;

namespace AskJavra.Models.Post
{
    public class PostTag
    {
        public int TagId { get; set; }
        public Guid PostId {get; set;}
        public Post Post { get; set;}
        public Tag Tag { get; set;}

    }
}
