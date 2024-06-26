﻿using AskJavra.Models.Root;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AskJavra.Models.Post
{
    public class PostTag : RootAuditEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int? TagId { get; set; }
        public Guid? PostId {get; set;}
        public Post? Post { get; set;}
        public Tag? Tag { get; set;}
        public PostTag() { }
        public PostTag(int? tagId, Guid? postId, Post? post)
        {
            Id = this.Id;
            TagId = tagId;
            PostId = postId;
            Post = post;
        }
        public PostTag(int id, int? tagId, Guid? postId, Post? post)
        {
            Id = id;
            TagId = tagId;
            PostId = postId;
            Post = post;
        }
    }
}
