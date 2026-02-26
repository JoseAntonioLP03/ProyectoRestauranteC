using Microsoft.AspNetCore.Mvc;
using ProyectoRestauranteC_.Repositories;

namespace ProyectoRestauranteC_.Controllers
{
    public class MenuController : Controller
    {
        private readonly IMenuRepository menuRepository;

        public MenuController(IMenuRepository menuRepository)
        {
            this.menuRepository = menuRepository;
        }

        public async Task<IActionResult> Index()
        {
            var categorias = await menuRepository.GetMenuCompletoAsync();
            return View(categorias);
        }
    }
}
