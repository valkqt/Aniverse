using Capstone.Models;
using Capstone.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Capstone.ViewComponents
{
    public class SearchCardViewComponent : ViewComponent
    {
        AniDbContext db = new AniDbContext();

        public async Task<IViewComponentResult> InvokeAsync(List<Anime> list)
        {
            return View(list);
        }
    }
}
