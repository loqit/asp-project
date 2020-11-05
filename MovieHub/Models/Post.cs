using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using MovieHub.Areas.Identity;

namespace MovieHub.Models
{
    public sealed class Post
    {
        [HiddenInput(DisplayValue=false)]
        public int Id { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public string FormatedText { get; set; }
        public int Rating { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime DOC { get; set; }
        [Url]
        public string Img { get; set; }
        public MHUser Author { get; set; }
        public ICollection<Comment> Comments { get; set; }

    }
}