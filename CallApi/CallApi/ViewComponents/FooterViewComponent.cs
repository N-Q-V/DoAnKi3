using Microsoft.AspNetCore.Mvc;

namespace CallApi.ViewComponents
{
    public class FooterViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
