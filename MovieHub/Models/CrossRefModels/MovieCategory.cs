namespace MovieHub.Models.CrossRefModels
{
    
    public class MovieCategory
    {
        public int Id { get; set; }
        public int MovieId { get; set; }
        public Movie Movie { get; set; }

        public int CategoryId { get; set; }
        public Category Category { get; set; }
    }
}