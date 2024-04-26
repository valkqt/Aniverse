using Capstone.Models;
using Microsoft.AspNetCore.Mvc;

namespace Capstone.ViewComponents
{
    public class ProfileIconViewComponent : ViewComponent
    {
        AniDbContext db = new AniDbContext();

        public async Task<IViewComponentResult> InvokeAsync(int id)
        {
            var profile = db.Users.Where(u => u.Id == id).FirstOrDefault();
            return View(profile);
        }
    }
}
