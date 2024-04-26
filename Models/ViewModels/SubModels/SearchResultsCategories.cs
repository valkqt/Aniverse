namespace Capstone.Models.ViewModels.SubModels
{
    public class SearchResultsCategories
    {
        public List<Anime> Trending { get; set; } = new List<Anime>();
        public List<Anime> Popular { get; set; } = new List<Anime>();
        public List<Anime> Score { get; set; } = new List<Anime>();
        public List<Anime> CurrentSeason { get; set; } = new List<Anime>();
        public List<Anime> NextSeason { get; set; } = new List<Anime>();
        public List<Anime> Results { get; set; } = new List<Anime>();


    }
}
