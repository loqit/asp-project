using System.Collections;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using MovieHub.Models;

namespace MovieHub.Areas.Identity
{
    public class MHUser : IdentityUser
    {
        [PersonalData]
        public ICollection<Post> Posts { get; set; } = new List<Post>();
        
        [PersonalData]
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
        
        [PersonalData]
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    }
}