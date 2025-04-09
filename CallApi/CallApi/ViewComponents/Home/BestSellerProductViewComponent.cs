using Microsoft.AspNetCore.Mvc;

namespace CallApi.ViewComponents.Home
{
    public class BestSellerProductViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
