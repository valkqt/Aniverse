using System.ComponentModel.DataAnnotations;

namespace Capstone.Models
{
    public class ProfileComment
    {
        [Key]
        public int Id { get; set; }
        [Length(1, 2500)]
        public string Text { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.Now;
        public int ProfileId { get; set; }
        public int AuthorId { get; set; }
        public string? Image { get; set; }
        public User? Profile { get; set; }
        public User? Author { get; set; }
    }
}
