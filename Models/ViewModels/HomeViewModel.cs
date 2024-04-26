namespace Capstone.Models.ViewModels
{
    public class HomeViewModel
    {
        public List<Anime> Carousel { get; set; } = new List<Anime>();
        public List<Anime> Trending { get; set; } = new List<Anime>();
        public List<Anime> CurrentSeason { get; set; } = new List<Anime>();
        public List<Anime> NextSeason { get; set; } = new List<Anime>();
        public List<Anime> Score { get; set; } = new List<Anime>();

    }
}
