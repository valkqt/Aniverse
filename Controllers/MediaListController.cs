using Capstone.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Capstone.Controllers
{
    [Authorize]
    public class MediaListController : Controller
    {
        AniDbContext db = new AniDbContext();

        [HttpPost]
        public IActionResult DeleteListItem(int id)
        {
            int user = int.Parse(User.Identity.Name);
            var item = db.MediaListItems.Where(i => i.AnimeId == id && i.UserId == user).FirstOrDefault();
            db.Entry(item).State = EntityState.Deleted;
            db.SaveChanges();

            return Redirect(Request.Headers.Referer.ToString());
        }



        [HttpPost]
        public IActionResult UpdateListItem(MediaListItem model)
        {
            int user = int.Parse(User.Identity.Name);
            MediaListItem entry = db.MediaListItems.Where(i => i.UserId == user && i.AnimeId == model.AnimeId).FirstOrDefault();

            model.UserId = user;
            model.LastUpdate = DateTime.Now;
            model.EpisodesWatched = (model.Completion == MediaListItem.CompletionState.Completed || model.EpisodesWatched > model.Episodes) ? model.Episodes : model.EpisodesWatched;

            if (entry == null)
            {
                db.MediaListItems.Add(model);
            }
            else
            {
                entry.Completion = model.Completion;
                entry.Rating = model.Rating;
                entry.EpisodesWatched = model.Completion == MediaListItem.CompletionState.Completed || model.EpisodesWatched > model.Episodes ? model.Episodes : model.EpisodesWatched;
                entry.Notes = model.Notes;
                entry.LastUpdate = DateTime.Now;

                db.Entry(entry).State = EntityState.Modified;
            }

            db.SaveChanges();
            return Redirect(Request.Headers.Referer.ToString());
        }


        [HttpGet("Users/{id?}/List/")]
        [AllowAnonymous]
        public IActionResult List(int id, string? sort)
        {
            IEnumerable<MediaListItem> mediaList = db.MediaListItems.Where(i => i.UserId == id).Include(item => item.User);
            switch (sort)
            {
                case "title":
                    return View(mediaList.OrderBy(i => i.Title).ToList());
                case "title_desc":
                    return View(mediaList.OrderByDescending(i => i.Title).ToList());
                case "rating":
                    return View(mediaList.OrderBy(i => i.Rating).ToList());
                case "rating_desc":
                    return View(mediaList.OrderByDescending(i => i.Rating).ToList());
                case "episodes":
                    return View(mediaList.OrderBy(i => i.Episodes).ToList());
                case "episodes_desc":
                    return View(mediaList.OrderByDescending(i => i.Episodes).ToList());
                case "format":
                    return View(mediaList.OrderBy(i => i.Format).ToList());
                case "format_desc":
                    return View(mediaList.OrderByDescending(i => i.Format).ToList());
                default:
                    return View(mediaList.OrderBy(i => i.AnimeId).ToList());

            }




        }

    }
}

