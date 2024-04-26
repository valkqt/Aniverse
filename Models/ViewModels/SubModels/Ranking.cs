using GraphQL.Client.Abstractions.Utilities;

namespace Capstone.Models.ViewModels.SubModels
{
    public class Ranking
    {
        public int? Rank { get; set; }
        private string? type;
        public string? Type
        {
            get { return type; }
            set
            {
                type = value?.ToLowerCase();
            }
        }
        public string? Format { get; set; }
        public int? Year { get; set; }
        public string? Season { get; set; }
        public bool? AllTime { get; set; }
        public string? Context { get; set; }
    }
}
