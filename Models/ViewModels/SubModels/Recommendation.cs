namespace Capstone.Models.ViewModels.SubModels
{
    public class Recommendation
    {
        public int? Id { get; set; }
        public int? Rating { get; set; }
        public Anime? Media { get; set; }
        public Anime? RecommendedMedia { get; set; }


    }
}
