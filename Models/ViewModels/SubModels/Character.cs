using Capstone.Models.ViewModels.SubModels;
using GraphQL.Client.Abstractions.Utilities;

namespace Capstone.Models.ViewModels.SubModels
{
    public class Character
    {
        private string? role;
        public string? Role
        {
            get { return role; }
            set
            {
                role = value?.ToLowerCase();
            }
        }
        public string? Name { get; set; }
        public string? Image {  get; set; }
        public Staff? VoiceActor { get; set; }
    }
}

/*characters { edges { role voiceActors } nodes { name {first middle last} image { large } } }
*/