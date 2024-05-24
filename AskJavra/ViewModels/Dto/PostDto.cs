using AskJavra.DataContext;
using AskJavra.Enums;
using System.ComponentModel.DataAnnotations;

namespace AskJavra.ViewModels.Dto
{
    public class PostDto
    {
        [Required]
        public string Title { get; set; }
        public string? Description { get; set; }
        public PostType PostType { get; set; }
        public FeedStatus FeedStatus { get; set; }       
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public bool IsAnonymous { get; set; }
        public int[] TagIds { get; set; }

        public IFormFile ScreenShot { get; set; }
        //public List<PostTagDto>? Tags { get; set; }

        public PostDto(string title, string? description, PostType postType, int[] tags)
        {
            Title = title;
            Description = description;
            PostType = postType;
            TagIds = tags;
        }
        public PostDto() {  }
    }
    public class PostAndIdDto : TagDto
    {
        public Guid Id { get; set; }
    }
    public class PostViewDto {
        public string Title { get; set; }
        public string? Description { get; set; }
        public PostType PostType { get; set; }
        public FeedStatus FeedStatus { get; set; }
        public List<PostTagDto>? Tags { get; set; }
        public Guid PostId { get; set; }
        public string PostTypeName { get; set; }
        public string FeedStatusName { get; set; }
        public DateTime CreationAt { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public List<UpvoteCountViewMode> UpVotes { get; set; }
        public List<PostThreadViewDto> postThreads { get; set; }
        public int TotalUpvoteCount { get; set; }
        public int TotalFeeds { get; set; } 
        public bool IsAnonymous { get; set; }

        public ApplicationUserViewDtocs CreatedByUser { get; set; }
    }
    public class  FeedRequestDto
    {
        public string? SearchTerm { get; set; } = string.Empty;
        public string? SortBy { get; set; }= string.Empty;
        public string? SortOrder { get; set; } = string.Empty;
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int[]? TagIds { get; set; }
        public FeedStatus[]? Filters { get; set; }
        public string? UserId { get; set; }

    }
    public class ResponseFeedDto
    {
        public List<PostViewDto> Feeds { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalRecords { get; set; }
        public int TotalPages { get; set; }
    }
    public class SortDto
    {
        public string SortBy { get; set; }
        public string SordOrder { get; set; }
    }
    public class PostUpvoteResponseDto
    {
        public string UpvoteBy { get; set; }
        public string PostTitle { get; set; }
        public string PostDescription { get; set; }
        public bool NeedPointRevoke { get; set; }
        public string PointUserId { get; set; }
    }

}
