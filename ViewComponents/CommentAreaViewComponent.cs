using Capstone.Models;
using Microsoft.AspNetCore.Mvc;

namespace Capstone.ViewComponents
{
    public class CommentAreaViewComponent : ViewComponent
    {
        AniDbContext db = new AniDbContext();

        public async Task<IViewComponentResult> InvokeAsync(int id, int author, string areaType)
        {
            ProfileComment comment = new ProfileComment()
            {
                AuthorId = author,
                ProfileId = id,
            };

            ViewBag.area = areaType;

            return View(comment);

        }
    }
}
