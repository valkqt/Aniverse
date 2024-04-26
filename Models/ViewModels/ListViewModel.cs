namespace Capstone.Models.ViewModels
{
    public class ListViewModel
    {
        public List<MediaListItem>? List { get; set; }
        public MediaListItem? Entry { get; set; } = new MediaListItem();
    }
}
