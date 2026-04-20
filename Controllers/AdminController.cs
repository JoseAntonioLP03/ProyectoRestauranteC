using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoRestauranteC_.Data;
using ProyectoRestauranteC_.Models;
using ProyectoRestauranteC_.Repositories;

namespace ProyectoRestauranteC_.Controllers
{
    public class AdminController : Controller
    {
        private readonly RestauranteContext context;
        private readonly RepositoryAdmin repoAdmin;

        public AdminController(RestauranteContext context, RepositoryAdmin repoAdmin)
        {
            this.context = context;
            this.repoAdmin = repoAdmin;
        }

        private bool EsAdmin()
        {
            var rol = User.FindFirst("http://schemas.microsoft.com/ws/2008/06/identity/claims/role")?.Value 
                      ?? User.FindFirst("Role")?.Value;
            return rol == "Admin";
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            if (!EsAdmin())
            {
                return RedirectToAction("Index", "Home");
            }

            var stats = await repoAdmin.GetStatsAsync();

            return View(stats);
        }

        // PRODUCTOS
        [Authorize]
        public async Task<IActionResult> Productos()
        {
            if (!EsAdmin()) return RedirectToAction("Index", "Home");

            ViewBag.Categorias = await repoAdmin.GetCategoriasAsync();
            var productos = await repoAdmin.GetProductosAsync();
            return View(productos);
        }

        // PRODUCTOS - CREATE
        [HttpPost]
        public async Task<IActionResult> CreateProducto(string nombre, string descripcion, decimal precio, IFormFile? imagen, int categoriaId, bool disponible)
        {
            if (!EsAdmin()) return Unauthorized(new { success = false, message = "No autorizado" });

            try
            {
                if (string.IsNullOrWhiteSpace(nombre) || nombre.Length > 150)
                    return Json(new { success = false, message = "El nombre debe tener entre 1 y 150 caracteres" });

                if (precio <= 0)
                    return Json(new { success = false, message = "El precio debe ser mayor a 0" });

                var categoria = await repoAdmin.GetCategoriaByIdAsync(categoriaId);
                if (categoria == null)
                    return Json(new { success = false, message = "La categoría no existe" });

                string imagenUrl = "/images/menu/default.jpg";
                if (imagen != null)
                {
                    var uploadedPath = await repoAdmin.UploadImagenAsync(imagen);
                    if (uploadedPath != null) imagenUrl = uploadedPath;
                }

                var producto = new Productos
                {
                    Nombre = nombre,
                    Descripcion = descripcion,
                    Precio = precio,
                    ImagenUrl = imagenUrl,
                    CategoriaId = categoriaId,
                    Disponible = disponible,
                    FechaCreacion = DateTime.Now
                };

                await repoAdmin.CrearProductoAsync(producto);

                return Json(new { success = true, message = "Producto creado exitosamente", id = producto.Id });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        // PRODUCTOS - UPDATE
        [HttpPost]
        public async Task<IActionResult> UpdateProducto(int id, string nombre, string descripcion, decimal precio, IFormFile? imagen, int categoriaId, bool disponible)
        {
            if (!EsAdmin()) return Unauthorized(new { success = false, message = "No autorizado" });

            try
            {
                var producto = await repoAdmin.GetProductoByIdAsync(id);
                if (producto == null) return Json(new { success = false, message = "Producto no encontrado" });

                if (string.IsNullOrWhiteSpace(nombre) || nombre.Length > 150)
                    return Json(new { success = false, message = "El nombre debe tener entre 1 y 150 caracteres" });

                if (precio <= 0)
                    return Json(new { success = false, message = "El precio debe ser mayor a 0" });

                var categoria = await repoAdmin.GetCategoriaByIdAsync(categoriaId);
                if (categoria == null)
                    return Json(new { success = false, message = "La categoría no existe" });

                if (imagen != null)
                {
                    var uploadedPath = await repoAdmin.UploadImagenAsync(imagen);
                    if (uploadedPath != null) producto.ImagenUrl = uploadedPath;
                }

                producto.Nombre = nombre;
                producto.Descripcion = descripcion;
                producto.Precio = precio;
                producto.CategoriaId = categoriaId;
                producto.Disponible = disponible;

                await repoAdmin.UpdateProductoAsync(producto);

                return Json(new { success = true, message = "Producto actualizado exitosamente" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteProducto(int id)
        {
            if (!EsAdmin()) return Unauthorized(new { success = false, message = "No autorizado" });

            try
            {
                var producto = await repoAdmin.GetProductoByIdAsync(id);
                if (producto == null) return Json(new { success = false, message = "Producto no encontrado" });

                await repoAdmin.DeleteProductoAsync(producto);

                return Json(new { success = true, message = "Producto eliminado exitosamente" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // CATEGORIAS
        [Authorize]
        public async Task<IActionResult> Categorias()
        {
            if (!EsAdmin()) return RedirectToAction("Index", "Home");

            var categorias = await repoAdmin.GetCategoriasAsync();
            return View(categorias);
        }

        // CATEGORIAS - CREATE
        [HttpPost]
        public async Task<IActionResult> CreateCategoria(string nombre, string descripcion, bool activo = true)
        {
            if (!EsAdmin()) return Unauthorized(new { success = false, message = "No autorizado" });

            try
            {
                if (string.IsNullOrWhiteSpace(nombre) || nombre.Length > 100)
                    return Json(new { success = false, message = "El nombre debe tener entre 1 y 100 caracteres" });

                var categoria = new Categoria
                {
                    Nombre = nombre,
                    Descripcion = descripcion,
                    Activo = activo
                };

                await repoAdmin.CrearCategoriaAsync(categoria);

                return Json(new { success = true, message = "Categoría creada exitosamente", id = categoria.Id });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        // CATEGORIAS - UPDATE
        [HttpPost]
        public async Task<IActionResult> UpdateCategoria(int id, string nombre, string descripcion, bool activo)
        {
            if (!EsAdmin()) return Unauthorized(new { success = false, message = "No autorizado" });

            try
            {
                var categoria = await repoAdmin.GetCategoriaByIdAsync(id);
                if (categoria == null) return Json(new { success = false, message = "Categoría no encontrada" });

                if (string.IsNullOrWhiteSpace(nombre) || nombre.Length > 100)
                    return Json(new { success = false, message = "El nombre debe tener entre 1 y 100 caracteres" });

                categoria.Nombre = nombre;
                categoria.Descripcion = descripcion;
                categoria.Activo = activo;

                await repoAdmin.UpdateCategoriaAsync(categoria);

                return Json(new { success = true, message = "Categoría actualizada exitosamente" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCategoria(int id)
        {
            if (!EsAdmin()) return Unauthorized(new { success = false, message = "No autorizado" });

            try
            {
                var categoria = await repoAdmin.GetCategoriaByIdAsync(id);
                if (categoria == null) return Json(new { success = false, message = "Categoría no encontrada" });

                await repoAdmin.DeleteCategoriaAsync(categoria);

                return Json(new { success = true, message = "Categoría eliminada exitosamente" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // USUARIOS
        [Authorize]
        public async Task<IActionResult> Usuarios()
        {
            if (!EsAdmin()) return RedirectToAction("Index", "Home");

            var usuarios = await repoAdmin.GetUsuariosAsync();
            return View(usuarios);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateUsuario(int id, bool activo)
        {
            if (!EsAdmin()) return Unauthorized(new { success = false, message = "No autorizado" });

            try
            {
                var usuario = await repoAdmin.GetUsuarioByIdAsync(id);
                if (usuario == null) return Json(new { success = false, message = "Usuario no encontrado" });

                usuario.Activo = activo;

                await repoAdmin.UpdateUsuarioAsync(usuario);

                return Json(new { success = true, message = "Usuario actualizado exitosamente" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUsuario(int id)
        {
            if (!EsAdmin()) return Unauthorized(new { success = false, message = "No autorizado" });

            try
            {
                var usuario = await repoAdmin.GetUsuarioByIdAsync(id);
                if (usuario == null) return Json(new { success = false, message = "Usuario no encontrado" });

                await repoAdmin.DeleteUsuarioAsync(usuario);

                return Json(new { success = true, message = "Usuario eliminado exitosamente" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // PEDIDOS
        [Authorize]
        public async Task<IActionResult> Pedidos()
        {
            if (!EsAdmin()) return RedirectToAction("Index", "Home");

            var pedidos = await repoAdmin.GetPedidosAsync();
            return View(pedidos);
        }

        [HttpPost]
        public async Task<IActionResult> UpdatePedido(int id, string estado)
        {
            if (!EsAdmin()) return Unauthorized(new { success = false, message = "No autorizado" });

            try
            {
                var estadosValidos = new[] { "PendientePago", "EnPreparacion", "Listo", "Entregado", "Cancelado" };
                if (!estadosValidos.Contains(estado))
                    return Json(new { success = false, message = "Estado inválido. Debe ser uno de: PendientePago, EnPreparacion, Listo, Entregado, Cancelado" });

                var pedido = await repoAdmin.GetPedidoByIdAsync(id);
                if (pedido == null) return Json(new { success = false, message = "Pedido no encontrado" });

                pedido.Estado = estado;

                await repoAdmin.UpdatePedidoAsync(pedido);

                return Json(new { success = true, message = "Pedido actualizado exitosamente" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeletePedido(int id)
        {
            if (!EsAdmin()) return Unauthorized(new { success = false, message = "No autorizado" });

            try
            {
                var pedido = await repoAdmin.GetPedidoByIdAsync(id);
                if (pedido == null) return Json(new { success = false, message = "Pedido no encontrado" });

                await repoAdmin.DeletePedidoAsync(pedido, id);

                return Json(new { success = true, message = "Pedido eliminado exitosamente" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // RESERVAS
        [Authorize]
        public async Task<IActionResult> Reservas()
        {
            if (!EsAdmin()) return RedirectToAction("Index", "Home");

            var reservas = await context.Reservas
                .Include(r => r.Usuario)
                .Include(r => r.Mesa)
                .ToListAsync();
            return View(reservas);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateReserva(int id, string estado)
        {
            if (!EsAdmin()) return Unauthorized(new { success = false, message = "No autorizado" });

            try
            {
                var estadosValidos = new[] { "Pendiente", "Confirmada", "Cancelada", "Expirada" };
                if (!estadosValidos.Contains(estado))
                    return Json(new { success = false, message = "Estado inválido. Debe ser uno de: Pendiente, Confirmada, Cancelada, Expirada" });

                var reserva = await context.Reservas.FindAsync(id);
                if (reserva == null) return Json(new { success = false, message = "Reserva no encontrada" });

                reserva.Estado = estado;
                if (estado == "Confirmada") reserva.FechaConfirmacion = DateTime.Now;

                context.Reservas.Update(reserva);
                await context.SaveChangesAsync();

                return Json(new { success = true, message = "Reserva actualizada exitosamente" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteReserva(int id)
        {
            if (!EsAdmin()) return Unauthorized(new { success = false, message = "No autorizado" });

            try
            {
                var reserva = await context.Reservas.FindAsync(id);
                if (reserva == null) return Json(new { success = false, message = "Reserva no encontrada" });

                context.Reservas.Remove(reserva);
                await context.SaveChangesAsync();

                return Json(new { success = true, message = "Reserva eliminada exitosamente" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // VALORACIONES
        [Authorize]
        public async Task<IActionResult> Valoraciones()
        {
            if (!EsAdmin()) return RedirectToAction("Index", "Home");

            var valoraciones = await context.Valoraciones
                .Include(v => v.Usuario)
                .ToListAsync();
            return View(valoraciones);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateValoracion(int id, bool visible)
        {
            if (!EsAdmin()) return Unauthorized(new { success = false, message = "No autorizado" });

            try
            {
                var valoracion = await context.Valoraciones.FindAsync(id);
                if (valoracion == null) return Json(new { success = false, message = "Valoración no encontrada" });

                valoracion.Visible = visible;

                context.Valoraciones.Update(valoracion);
                await context.SaveChangesAsync();

                return Json(new { success = true, message = "Valoración actualizada exitosamente" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteValoracion(int id)
        {
            if (!EsAdmin()) return Unauthorized(new { success = false, message = "No autorizado" });

            try
            {
                var valoracion = await context.Valoraciones.FindAsync(id);
                if (valoracion == null) return Json(new { success = false, message = "Valoración no encontrada" });

                context.Valoraciones.Remove(valoracion);
                await context.SaveChangesAsync();

                return Json(new { success = true, message = "Valoración eliminada exitosamente" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // CUPONES
        [Authorize]
        public async Task<IActionResult> Cupones()
        {
            if (!EsAdmin()) return RedirectToAction("Index", "Home");

            var cupones = await context.Cupones.ToListAsync();
            return View(cupones);
        }

        // CUPONES - CREATE
        [HttpPost]
        public async Task<IActionResult> CreateCupon(string codigo, string tipoDescuento, decimal valorDescuento, DateTime fechaInicio, DateTime fechaFin, int usoMaximo, bool activo = true)
        {
            if (!EsAdmin()) return Unauthorized(new { success = false, message = "No autorizado" });

            try
            {
                if (string.IsNullOrWhiteSpace(codigo) || codigo.Length > 50)
                    return Json(new { success = false, message = "El código debe tener entre 1 y 50 caracteres" });

                if (tipoDescuento != "Porcentaje" && tipoDescuento != "Fijo")
                    return Json(new { success = false, message = "El tipo debe ser 'Porcentaje' o 'Fijo'" });

                if (valorDescuento <= 0)
                    return Json(new { success = false, message = "El valor debe ser mayor a 0" });

                if (fechaInicio >= fechaFin)
                    return Json(new { success = false, message = "La fecha de inicio debe ser menor a la de fin" });

                if (usoMaximo <= 0)
                    return Json(new { success = false, message = "El uso máximo debe ser mayor a 0" });

                // Verificar código único
                var codigoExiste = await context.Cupones.AnyAsync(c => c.Codigo == codigo);
                if (codigoExiste)
                    return Json(new { success = false, message = "El código de cupón ya existe" });

                var cupon = new Cupon
                {
                    Codigo = codigo,
                    TipoDescuento = tipoDescuento,
                    ValorDescuento = valorDescuento,
                    FechaInicio = fechaInicio,
                    FechaFin = fechaFin,
                    UsoMaximo = usoMaximo,
                    VecesUsado = 0,
                    Activo = activo
                };

                context.Cupones.Add(cupon);
                await context.SaveChangesAsync();

                return Json(new { success = true, message = "Cupón creado exitosamente", id = cupon.Id });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        // CUPONES - UPDATE
        [HttpPost]
        public async Task<IActionResult> UpdateCupon(int id, string codigo, string tipoDescuento, decimal valorDescuento, DateTime fechaInicio, DateTime fechaFin, int usoMaximo, bool activo)
        {
            if (!EsAdmin()) return Unauthorized(new { success = false, message = "No autorizado" });

            try
            {
                var cupon = await context.Cupones.FindAsync(id);
                if (cupon == null) return Json(new { success = false, message = "Cupón no encontrado" });

                if (string.IsNullOrWhiteSpace(codigo) || codigo.Length > 50)
                    return Json(new { success = false, message = "El código debe tener entre 1 y 50 caracteres" });

                if (tipoDescuento != "Porcentaje" && tipoDescuento != "Fijo")
                    return Json(new { success = false, message = "El tipo debe ser 'Porcentaje' o 'Fijo'" });

                if (valorDescuento <= 0)
                    return Json(new { success = false, message = "El valor debe ser mayor a 0" });

                if (fechaInicio >= fechaFin)
                    return Json(new { success = false, message = "La fecha de inicio debe ser menor a la de fin" });

                if (usoMaximo <= 0)
                    return Json(new { success = false, message = "El uso máximo debe ser mayor a 0" });

                // Verificar código único (si cambió)
                if (cupon.Codigo != codigo)
                {
                    var codigoExiste = await context.Cupones.AnyAsync(c => c.Codigo == codigo);
                    if (codigoExiste)
                        return Json(new { success = false, message = "El código de cupón ya existe" });
                }

                cupon.Codigo = codigo;
                cupon.TipoDescuento = tipoDescuento;
                cupon.ValorDescuento = valorDescuento;
                cupon.FechaInicio = fechaInicio;
                cupon.FechaFin = fechaFin;
                cupon.UsoMaximo = usoMaximo;
                cupon.Activo = activo;

                context.Cupones.Update(cupon);
                await context.SaveChangesAsync();

                return Json(new { success = true, message = "Cupón actualizado exitosamente" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCupon(int id)
        {
            if (!EsAdmin()) return Unauthorized(new { success = false, message = "No autorizado" });

            try
            {
                var cupon = await context.Cupones.FindAsync(id);
                if (cupon == null) return Json(new { success = false, message = "Cupón no encontrado" });

                context.Cupones.Remove(cupon);
                await context.SaveChangesAsync();

                return Json(new { success = true, message = "Cupón eliminado exitosamente" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // MESAS
        [Authorize]
        public async Task<IActionResult> Mesas()
        {
            if (!EsAdmin()) return RedirectToAction("Index", "Home");

            var mesas = await context.Mesas.ToListAsync();
            return View(mesas);
        }

        // MESAS - CREATE
        [HttpPost]
        public async Task<IActionResult> CreateMesa(int numeroMesa, int capacidad, bool activa = true)
        {
            if (!EsAdmin()) return Unauthorized(new { success = false, message = "No autorizado" });

            try
            {
                if (numeroMesa <= 0)
                    return Json(new { success = false, message = "El número de mesa debe ser mayor a 0" });

                if (capacidad <= 0 || capacidad > 20)
                    return Json(new { success = false, message = "La capacidad debe estar entre 1 y 20 personas" });

                // Verificar número de mesa único
                var mesaExiste = await context.Mesas.AnyAsync(m => m.NumeroMesa == numeroMesa);
                if (mesaExiste)
                    return Json(new { success = false, message = "El número de mesa ya existe" });

                var mesa = new Mesa
                {
                    NumeroMesa = numeroMesa,
                    Capacidad = capacidad,
                    Activa = activa
                };

                context.Mesas.Add(mesa);
                await context.SaveChangesAsync();

                return Json(new { success = true, message = "Mesa creada exitosamente", id = mesa.Id });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        // MESAS - UPDATE
        [HttpPost]
        public async Task<IActionResult> UpdateMesa(int id, int numeroMesa, int capacidad, bool activa)
        {
            if (!EsAdmin()) return Unauthorized(new { success = false, message = "No autorizado" });

            try
            {
                var mesa = await context.Mesas.FindAsync(id);
                if (mesa == null) return Json(new { success = false, message = "Mesa no encontrada" });

                if (numeroMesa <= 0)
                    return Json(new { success = false, message = "El número de mesa debe ser mayor a 0" });

                if (capacidad <= 0 || capacidad > 20)
                    return Json(new { success = false, message = "La capacidad debe estar entre 1 y 20 personas" });

                // Verificar número de mesa único (si cambió)
                if (mesa.NumeroMesa != numeroMesa)
                {
                    var mesaExiste = await context.Mesas.AnyAsync(m => m.NumeroMesa == numeroMesa);
                    if (mesaExiste)
                        return Json(new { success = false, message = "El número de mesa ya existe" });
                }

                mesa.NumeroMesa = numeroMesa;
                mesa.Capacidad = capacidad;
                mesa.Activa = activa;

                context.Mesas.Update(mesa);
                await context.SaveChangesAsync();

                return Json(new { success = true, message = "Mesa actualizada exitosamente" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteMesa(int id)
        {
            if (!EsAdmin()) return Unauthorized(new { success = false, message = "No autorizado" });

            try
            {
                var mesa = await context.Mesas.FindAsync(id);
                if (mesa == null) return Json(new { success = false, message = "Mesa no encontrada" });

                context.Mesas.Remove(mesa);
                await context.SaveChangesAsync();

                return Json(new { success = true, message = "Mesa eliminada exitosamente" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // GALERÍAS
        [Authorize]
        public async Task<IActionResult> Galeria()
        {
            if (!EsAdmin()) return RedirectToAction("Index", "Home");

            var imagenes = await context.Galeria.ToListAsync();
            return View(imagenes);
        }

        // GALERÍAS - CREATE
        [HttpPost]
        public async Task<IActionResult> CreateGaleria(IFormFile? imagenFile, string descripcion, string tipo, bool activa = true)
        {
            if (!EsAdmin()) return Unauthorized(new { success = false, message = "No autorizado" });

            try
            {
                string urlImagen = "/images/galeria/default.jpg";
                if (imagenFile != null)
                {
                    var uploadedPath = await repoAdmin.UploadImagenAsync(imagenFile, "galeria");
                    if (uploadedPath != null) urlImagen = uploadedPath;
                }

                if (tipo != "Local" && tipo != "Plato")
                    return Json(new { success = false, message = "El tipo debe ser 'Local' o 'Plato'" });

                var imagen = new Galeria
                {
                    UrlImagen = urlImagen,
                    Descripcion = descripcion,
                    Tipo = tipo,
                    Activa = activa
                };

                context.Galeria.Add(imagen);
                await context.SaveChangesAsync();

                return Json(new { success = true, message = "Imagen creada exitosamente", id = imagen.Id });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        // GALERÍAS - UPDATE
        [HttpPost]
        public async Task<IActionResult> UpdateGaleria(int id, IFormFile? imagenFile, string descripcion, string tipo, bool activa)
        {
            if (!EsAdmin()) return Unauthorized(new { success = false, message = "No autorizado" });

            try
            {
                var imagen = await context.Galeria.FindAsync(id);
                if (imagen == null) return Json(new { success = false, message = "Imagen no encontrada" });

                if (tipo != "Local" && tipo != "Plato")
                    return Json(new { success = false, message = "El tipo debe ser 'Local' o 'Plato'" });

                if (imagenFile != null)
                {
                    var uploadedPath = await repoAdmin.UploadImagenAsync(imagenFile, "galeria");
                    if (uploadedPath != null) imagen.UrlImagen = uploadedPath;
                }

                imagen.Descripcion = descripcion;
                imagen.Tipo = tipo;
                imagen.Activa = activa;

                context.Galeria.Update(imagen);
                await context.SaveChangesAsync();

                return Json(new { success = true, message = "Imagen actualizada exitosamente" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteGaleria(int id)
        {
            if (!EsAdmin()) return Unauthorized(new { success = false, message = "No autorizado" });

            try
            {
                var imagen = await context.Galeria.FindAsync(id);
                if (imagen == null) return Json(new { success = false, message = "Imagen no encontrada" });

                context.Galeria.Remove(imagen);
                await context.SaveChangesAsync();

                return Json(new { success = true, message = "Imagen eliminada exitosamente" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // HORARIOS
        [Authorize]
        public async Task<IActionResult> Horarios()
        {
            if (!EsAdmin()) return RedirectToAction("Index", "Home");

            var horarios = await context.Horarios.ToListAsync();
            return View(horarios);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateHorario(int id, TimeSpan horaApertura, TimeSpan horaCierre, bool activo)
        {
            if (!EsAdmin()) return Unauthorized(new { success = false, message = "No autorizado" });

            try
            {
                if (horaApertura >= horaCierre)
                    return Json(new { success = false, message = "La hora de apertura debe ser menor a la de cierre" });

                var horario = await context.Horarios.FindAsync(id);
                if (horario == null) return Json(new { success = false, message = "Horario no encontrado" });

                horario.HoraApertura = horaApertura;
                horario.HoraCierre = horaCierre;
                horario.Activo = activo;

                context.Horarios.Update(horario);
                await context.SaveChangesAsync();

                return Json(new { success = true, message = "Horario actualizado exitosamente" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }
    }
}

