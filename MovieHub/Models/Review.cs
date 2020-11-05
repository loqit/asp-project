using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MovieHub.Areas.Identity;
using MovieHub.Models.CrossRefModels;

namespace MovieHub.Models
{
    public class Review
    {
        [HiddenInput(DisplayValue=false)]
        public int Id { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        
        public DateTime DoC { get; set; }
        public MHUser Author { get; set; }
        public Movie Movie { get; set; }
        public int MovieId { get; set; }
        public string FormatedText { get; set; }
        [Range(0,10)]
        public int Mark { get; set; }
        /*
        public Review()
        {
            MovieReviews = new List<MovieReview>();
        }*/
        
    }
}