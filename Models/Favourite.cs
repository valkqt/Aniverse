using System.ComponentModel.DataAnnotations;

namespace Capstone.Models
{
    public class Favourite
    {
        public enum FavouriteType
        {
            Anime,
            Studio,
            Character,
            Staff,
        }
        [Key]
        public int Id { get; set; }
        public FavouriteType Type { get; set; }
        public int FavouriteId { get; set; }
        public int UserId { get; set; }
        public string? Image { get; set; }
        public string? Name { get; set; }

    }
}
