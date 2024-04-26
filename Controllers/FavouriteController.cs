using Capstone.Models;
using Microsoft.AspNetCore.Mvc;

namespace Capstone.Controllers
{
    public class FavouriteController : Controller
    {
        AniDbContext db = new AniDbContext();
        [HttpPost]
        public IActionResult ToggleFavourite(int animeId, string cover, string name)
        {
            int user = int.Parse(User.Identity.Name);
            var favourite = db.Favourites.Where(f => f.UserId == user && f.Type == Favourite.FavouriteType.Anime && f.FavouriteId == animeId).FirstOrDefault();
            if (favourite == null)
            {
                db.Favourites.Add(new Favourite()
                {
                    Type = Favourite.FavouriteType.Anime,
                    FavouriteId = animeId,
                    UserId = user,
                    Image = cover,
                    Name = name,
                });
            }
            else
            {
                db.Favourites.Remove(favourite);
            }
            db.SaveChanges();

            return RedirectToAction("Anime", "Anime", new { Id = animeId });
        }
    }
}
