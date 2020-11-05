using System;
using Microsoft.AspNetCore.Mvc;
using MovieHub.Areas.Identity;

namespace MovieHub.Models
{
    public class Comment
    {
        public string Text { get; set; }
        [HiddenInput(DisplayValue=false)]
        public int Id { get; set; }
        public DateTime DOC { get; set; }
        public Post Post { get; set; }
        public int PostId { get; set; }
        public  MHUser Author { get; set; }
        /*
        public Comment()
        {
            Author = new MHUser();
            Post = new Post();
        }*/
    }
}