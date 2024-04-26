using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Capstone.Models
{
    public class MediaListItem
    {
        [Key]
        public int Id { get; set; }
        public enum CompletionState
        {
            Watching,
            Completed,
            OnHold,
            Dropped,
            Planned
        }
        public int AnimeId { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public int? Rating { get; set; }
        public int? EpisodesWatched { get; set; }
        public CompletionState Completion { get; set; }
        public string? Image { get; set; }
        public string? Title { get; set; }
        public string? Format { get; set; }
        public DateTime? StartedAt { get; set; }
        public DateTime? LastUpdate { get; set; }
        public DateTime? CompletedAt { get; set; }
        public int? Episodes { get; set; }
        public string? Notes { get; set; }

        [NotMapped]
        public bool? IsFavourite { get; set; } = false;
    }
}
