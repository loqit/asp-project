using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using MovieHub.Models.CrossRefModels;

namespace MovieHub.Models
{
    public class Category
    {
        [HiddenInput(DisplayValue=false)]
        public int Id { get; set; }
        public string Name { get; set; }
        [Url]
        public string Img { get; set; }
        public IList<MovieCategory> MovieCategories { get; set; } = new List<MovieCategory>();

        
        
    }
}