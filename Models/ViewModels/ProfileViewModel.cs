using Capstone.Models.ViewModels.SubModels;

namespace Capstone.Models.ViewModels
{
    public class ProfileViewModel
    {

        public User User { get; set; }
        public List<User>? Friends { get; set; }
        public List<Notification> Notifications { get; set; }
        public List<Favourite> Favourites { get; set; }
        public List<ProfileComment> ProfileComments { get; set; }
        public ProfileComment? NewComment { get; set; }
        public List<MediaListItem> MediaList { get; set; }
        public Stats ProfileStats { get; set; }


    }
}
