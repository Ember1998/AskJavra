using AskJavra.Enums;
using AskJavra.Models.Root;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace AskJavra.Models.Post
{
    public class Post : RootAuditEntity
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public  string Title { get; set; }
        public string? Description { get; set; }
        public  PostType PostType { get; set; }
        public FeedStatus FeedStatus { get; set; }
        public bool IsAnonymous { get; set; } = false;
        public string ScreenshotPath { get; set; }
        public virtual List<PostThread> Threads { get; set; }= new List<PostThread>();
        public virtual List<PostTag> Tags { get; set; } = new List<PostTag>();
        public virtual List<PostUpVote> UpVotes { get; set; } = new List<PostUpVote>();
        public Post()
        {

        }
        public Post(string title, string? description, PostType postType,FeedStatus feedStatus, List<PostThread> threads, List<PostTag> tags, string createdBy, bool isAnonymous)
        {
            Title = title;
            Description = description;
            PostType = postType;
            Threads = threads;
            Tags = tags;
            FeedStatus = feedStatus;
            CreatedBy = createdBy;
            IsAnonymous = isAnonymous;
        }
        public Post(Guid id,string title, string? description, PostType postType, string createdBy, bool isAnonymous)
        {
            Id = id;
            Title = title;
            Description = description;
            PostType = postType;
            CreatedBy = createdBy;
            IsAnonymous=isAnonymous;
        }
    }
}
