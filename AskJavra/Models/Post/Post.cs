﻿using AskJavra.Enums;
using AskJavra.Models.Root;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

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
        public virtual ICollection<PostThread> Threads { get; set; }= new List<PostThread>();
        public virtual ICollection<PostTag> Tags { get; set; } = new List<PostTag>();
        public Post()
        {

        }
        public Post(string title, string? description, PostType postType, ICollection<PostThread> threads, ICollection<PostTag> tags)
        {
            Title = title;
            Description = description;
            PostType = postType;
            Threads = threads;
            Tags = tags;
        }
        public Post(string title, string? description, PostType postType)
        {
            Title = title;
            Description = description;
            PostType = postType;
        }
    }
}
