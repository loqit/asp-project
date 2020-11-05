using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using MovieHub.Models.CrossRefModels;

namespace MovieHub.Models
{
    public class Person
    {
        [HiddenInput(DisplayValue=false)]
        public int Id { get; set; }
        public string Name { get; set; }
        [DataType(DataType.Date)]
        public DateTime BirthDate { get; set; }
        public string Biog { get; set; }
        public string Position { get; set; }
        [Url]
        public string Img { get; set; }
        
        public IList<MoviePerson> MoviePersons { get; set; } = new List<MoviePerson>();

    }
}