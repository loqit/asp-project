using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using MovieHub.Models.CrossRefModels;

namespace MovieHub.Models
{
    public class Movie
    {
        [HiddenInput(DisplayValue=false)]
        public int Id{ get; set; }
        [Required]
        public string Title { get; set; }
        [DisplayName("Date of creation")]
        [DataType(DataType.Date)]
        public int CreateDate { get; set; }
        
        [DisplayName("Description")]
        public string Desc { get; set; }
        [Url]
        public string Poster { get; set; }
        [Range(0,10)]
        public float Rating { get; set; }
        public bool IsFavorite { get; set; }
        public bool InWathlist { get; set; }

        public IList<MovieCategory> MovieCategories { get; set; } = new List<MovieCategory>();
        public IList<Review> Reviews { get; set; }
        public IList<MoviePerson> MoviePersons { get; set; } = new List<MoviePerson>();
/*
        public Movie()
        {
            MoviePersons = new List<MoviePerson>();
            MovieReviews = new List<MovieReview>();
            MovieCategories = new List<MovieCategory>();
        }*/
        
    }
}
