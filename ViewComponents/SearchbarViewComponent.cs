using Capstone.Models;
using Capstone.Models.ViewModels.SubModels;
using Microsoft.AspNetCore.Mvc;

namespace Capstone.ViewComponents
{
    public class SearchbarViewComponent : ViewComponent
    {
        AniDbContext db = new AniDbContext();

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var model = new SearchQuery();
            return View(model);
        }
    }
}
