﻿using AskJavra.Enums;
using AskJavra.Models.Post;
using AskJavra.Models.Root;
using System.ComponentModel.DataAnnotations;

namespace AskJavra.ViewModels.Dto
{
    public class PostTagDto
    {
        public int TagId { get; set; }
        public Guid PostId { get; set; }
        public PostTagDto(int tagId, Guid postId)
        {
            TagId = tagId;
            PostId = postId;
        }
        public PostTagDto() {  }
    }
    public class ThreadTagDto
    {
        public int TagId { get; set; }
        public Guid PostThreadId { get; set; }
        public ThreadTagDto(int tagId, Guid postThreadId)
        {
            TagId = tagId;
            PostThreadId = postThreadId;
        }
    }
    public class PostTagAndIdDto : PostThreadDto
    {
        public Guid Id { get; set; }
    }
}
