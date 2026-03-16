using Microsoft.AspNetCore.Mvc;
using ProyectoRestauranteC_.Filters;
using ProyectoRestauranteC_.Models;
using ProyectoRestauranteC_.Repositories;
using System.Security.Claims;
using System.Text.Json;

namespace ProyectoRestauranteC_.Controllers
{
    public class MenuController : Controller
    {
        private readonly IMenuRepository menuRepository;
        private readonly RepositoryUsuarios repoUsuarios;
        private readonly RepositoryReservas repoReservas;

        public MenuController(IMenuRepository menuRepository, RepositoryUsuarios repoUsuarios, RepositoryReservas repoReservas)
        {
            this.menuRepository = menuRepository;
            this.repoUsuarios = repoUsuarios;
            this.repoReservas = repoReservas;
        }

        private List<ItemCarrito> ObtenerCarrito()
        {
            var json = HttpContext.Session.GetString("CARRITO");
            return json != null ? JsonSerializer.Deserialize<List<ItemCarrito>>(json)! : new List<ItemCarrito>();
        }

        private void GuardarCarrito(List<ItemCarrito> carrito)
        {
            HttpContext.Session.SetString("CARRITO", JsonSerializer.Serialize(carrito));
        }

        private Cupon? ObtenerCupon()
        {
            var json = HttpContext.Session.GetString("CUPON");
            return json != null ? JsonSerializer.Deserialize<Cupon>(json) : null;
        }

        public async Task<IActionResult> Index()
        {
            var categorias = await menuRepository.GetMenuCompletoAsync();
            return View(categorias);
        }

        [AuthorizeUsers]
        public async Task<IActionResult> RealizarPedido()
        {
            var categorias = await menuRepository.GetMenuCompletoAsync();
            ViewBag.Carrito = ObtenerCarrito();
            return View(categorias);
        }

        [HttpPost]
        [AuthorizeUsers]
        public IActionResult AgregarAlCarrito(int id, string nombre, string precio)
        {
            decimal precioDecimal = decimal.Parse(precio, System.Globalization.CultureInfo.InvariantCulture);
            var carrito = ObtenerCarrito();
            var item = carrito.FirstOrDefault(i => i.Id == id);
            if (item != null)
            {
                item.Cantidad++;
            }
            else
            {
                carrito.Add(new ItemCarrito { Id = id, Nombre = nombre, Precio = precioDecimal, Cantidad = 1 });
            }
            GuardarCarrito(carrito);
            return RedirectToAction("RealizarPedido");
        }

        [HttpPost]
        [AuthorizeUsers]
        public IActionResult EliminarDelCarrito(int id)
        {
            var carrito = ObtenerCarrito();
            var item = carrito.FirstOrDefault(i => i.Id == id);
            if (item != null)
            {
                if (item.Cantidad > 1) item.Cantidad--;
                else carrito.Remove(item);
            }
            GuardarCarrito(carrito);
            return RedirectToAction("RealizarPedido");
        }

        [HttpPost]
        [AuthorizeUsers]
        public IActionResult VaciarCarrito()
        {
            HttpContext.Session.Remove("CARRITO");
            return RedirectToAction("RealizarPedido");
        }

        [AuthorizeUsers]
        public async Task<IActionResult> Checkout()
        {
            var carrito = ObtenerCarrito();
            if (!carrito.Any()) return RedirectToAction("RealizarPedido");

            string idStr = User.FindFirstValue("ID_USUARIO")!;
            Usuario? usuario = await repoUsuarios.GetUsuarioByIdAsync(int.Parse(idStr));
            if (usuario == null) return RedirectToAction("Login", "Acceso");

            var cupon = ObtenerCupon();
            decimal subtotal = carrito.Sum(i => i.Subtotal);
            decimal descuento = 0;
            if (cupon != null)
            {
                descuento = cupon.TipoDescuento == "Porcentaje"
                    ? Math.Round(subtotal * cupon.ValorDescuento / 100, 2)
                    : cupon.ValorDescuento;
                descuento = Math.Min(descuento, subtotal);
            }

            var vm = new CheckoutViewModel
            {
                Usuario = usuario,
                Items = carrito,
                CuponAplicado = cupon,
                Descuento = descuento
            };
            return View(vm);
        }

        [HttpPost]
        [AuthorizeUsers]
        public async Task<IActionResult> AplicarCupon(string codigoCupon)
        {
            if (!string.IsNullOrWhiteSpace(codigoCupon))
            {
                var cupon = await repoUsuarios.ValidarCuponAsync(codigoCupon.Trim().ToUpper());
                if (cupon != null)
                {
                    HttpContext.Session.SetString("CUPON", JsonSerializer.Serialize(cupon));
                    TempData["CuponOk"] = $"Cupón \"{cupon.Codigo}\" aplicado correctamente";
                }
                else
                {
                    HttpContext.Session.Remove("CUPON");
                    TempData["CuponError"] = "El cupón no es válido, ha expirado o ya se agotó";
                }
            }
            return RedirectToAction("Checkout");
        }

        [HttpPost]
        [AuthorizeUsers]
        public IActionResult QuitarCupon()
        {
            HttpContext.Session.Remove("CUPON");
            return RedirectToAction("Checkout");
        }

        [HttpPost]
        [AuthorizeUsers]
        public async Task<IActionResult> ConfirmarPedido(bool esRecoger = false)
        {
            var carrito = ObtenerCarrito();
            if (!carrito.Any()) return RedirectToAction("RealizarPedido");

            string idStr = User.FindFirstValue("ID_USUARIO")!;
            int idUsuario = int.Parse(idStr);

            Usuario? usuario = await repoUsuarios.GetUsuarioByIdAsync(idUsuario);
            if (usuario == null) return RedirectToAction("Login", "Acceso");

            var cupon = ObtenerCupon();
            decimal subtotal = carrito.Sum(i => i.Subtotal);
            decimal descuento = 0;
            if (cupon != null)
            {
                descuento = cupon.TipoDescuento == "Porcentaje"
                    ? Math.Round(subtotal * cupon.ValorDescuento / 100, 2)
                    : cupon.ValorDescuento;
                descuento = Math.Min(descuento, subtotal);
            }
            decimal total = Math.Max(0, subtotal + 3.00m + 2.05m - descuento);

            string tipoPedido = esRecoger ? "Recogida" : "Domicilio";

            var pedido = await repoUsuarios.CrearPedidoAsync(
                idUsuario, carrito, subtotal, descuento, total,
                usuario.Direccion, cupon?.Id, tipoPedido);

            HttpContext.Session.Remove("CARRITO");
            HttpContext.Session.Remove("CUPON");

            return RedirectToAction("PedidoConfirmado", new { id = pedido.Id });
        }

        [AuthorizeUsers]
        public async Task<IActionResult> Perfil()
        {
            string idStr = User.FindFirstValue("ID_USUARIO")!;
            int idUsuario = int.Parse(idStr);

            Usuario? usuario = await repoUsuarios.GetUsuarioByIdAsync(idUsuario);
            if (usuario == null) return RedirectToAction("Login", "Acceso");

            var pedidos = await repoUsuarios.GetPedidosByUsuarioAsync(idUsuario);
            var reservas = await repoReservas.GetReservasByUsuarioAsync(idUsuario);

            var vm = new PerfilViewModel
            {
                Usuario = usuario,
                Pedidos = pedidos,
                Reservas = reservas
            };
            return View(vm);
        }

        [AuthorizeUsers]
        public async Task<IActionResult> PedidoConfirmado(int id)
        {
            string idStr = User.FindFirstValue("ID_USUARIO")!;
            int idUsuario = int.Parse(idStr);

            var pedido = await repoUsuarios.GetPedidoConDetallesAsync(id, idUsuario);
            if (pedido == null) return RedirectToAction("Index", "Home");

            Usuario? usuario = await repoUsuarios.GetUsuarioByIdAsync(idUsuario);
            if (usuario == null) return RedirectToAction("Login", "Acceso");

            var vm = new PedidoConfirmadoViewModel
            {
                Pedido = pedido,
                Usuario = usuario,
                Detalles = pedido.Detalles.ToList()
            };
            return View(vm);
        }
    }
}
