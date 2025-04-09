using Microsoft.AspNetCore.Mvc;

namespace CallApi.ViewComponents.Home
{
    public class FeatureViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
