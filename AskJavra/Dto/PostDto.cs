namespace AskJavra.Dto
{
    public class PostDto
    {
        public Guid Id { get; set; }
    }
    public class RequestDto{
        public Guid? PostId { get; set; }
        public string? SearchText { get; set; }
        public int? TagId { get; set;}
    
    }

}
