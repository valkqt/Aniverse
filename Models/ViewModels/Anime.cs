using Capstone.Models.ViewModels.SubModels;
using GraphQL.Client.Abstractions.Utilities;


namespace Capstone.Models.ViewModels
{
    public class Anime
    {
        static Random rd = new Random();
        public static string CreateString(int stringLength)
        {
            const string allowedChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz0123456789!@$?_-";
            char[] chars = new char[stringLength];

            for (int i = 0; i < stringLength; i++)
            {
                chars[i] = allowedChars[rd.Next(0, allowedChars.Length)];
            }

            return new string(chars);
        }



        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Format { get; set; }
        public string? Description { get; set; }
        private string? status;
        public string? Status
        {
            get
            {
                return status;
            }
            set
            {
                status = value?.ToLowerCase();
            }
        }
        public FuzzyDate? StartDate { get; set; }
        public FuzzyDate? EndDate { get; set; }
        private string? relationType;
        public string? RelationType
        {
            get { return relationType; }
            set
            {
                relationType = value?.ToLowerCase();
            }
        }
        public string? CoverImage { get; set; }
        public string? ColorAvg { get; set; }
        private string? season;
        public string? Season
        {
            get { return season; }
            set
            {
                season = value?.ToLowerCase();
            }
        }
        public int? SeasonYear { get; set; }
        public int Episodes { get; set; }
        public int? Duration { get; set; }
        private string? source;
        public string? Source
        {
            get { return source; }
            set
            {
                source = value?.ToLowerCase();
            }
        }
        public string? TrailerLink { get; set; }
        public string? TrailerImage { get; set; }
        public string? BannerImage { get; set; }
        public List<string> Genres { get; set; } = new List<string>();
        public double? AverageScore { get; set; }
        public double? MeanScore { get; set; }
        public List<Anime> Relations { get; set; } = new List<Anime>();
        public List<Character> Characters { get; set; } = new List<Character>();
        public List<Studio> Studios { get; set; } = new List<Studio>();
        public List<Staff> Staff { get; set; } = new List<Staff>();
        public Stats? Stats { get; set; }
        public int? NextAiringTime { get; set; }
        public int? NextAiringNumber { get; set; }
        /*        public List<ExternalLink>? ExternalLinks {  get; set; } 
        */
        public List<StreamingEpisode> StreamingEpisodes { get; set; } = new List<StreamingEpisode>();
        public List<Ranking>? Rankings { get; set; } = new List<Ranking>();
        public List<Recommendation>? Recommendations { get; set; } = new List<Recommendation>();
        public Trailer? Trailer { get; set; }

    }
}
