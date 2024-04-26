using Capstone.Models;
using Microsoft.AspNetCore.Mvc;

namespace Capstone.ViewComponents
{
    public class EditCommentModalViewComponent : ViewComponent
    {
        AniDbContext db = new AniDbContext();

        public async Task<IViewComponentResult> InvokeAsync(ProfileComment comment)
        {

            return View(comment);

        }
    }
}
