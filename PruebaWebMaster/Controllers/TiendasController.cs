using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PruebaWebMaster.Models;

namespace PruebaWebMaster.Controllers
{
    public class TiendasController : Controller
    {
        private readonly AppDbContext context;

        public TiendasController(AppDbContext context)
        {
            this.context = context;
        }
        public async Task<IActionResult> Index()
        {
            return context.Tiendas != null ?
                View(await context.Tiendas.ToListAsync()) : Problem("Entity set is null");
        }

        public IActionResult Crear()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Crear(Tienda tienda)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    context.Add(tienda);
                    await context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("", "No se pueden guardar cambios. " + ex);
            }

            return View(tienda);
        }

        public async Task<IActionResult> Editar(int id)
        {
            var tienda = await context.Tiendas.FindAsync(id);
            return View(tienda);
        }

        [HttpPost]
        public async Task<IActionResult> Editar(Tienda tienda)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    context.Update(tienda);
                    await context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception)
            {
                throw;
            }
            return View(tienda);
        }

        public async Task<IActionResult> Eliminar(int id)
        {
            var tienda = await context.Tiendas.FindAsync(id);
            return View(tienda);
        }

        [HttpPost]
        public async Task<IActionResult> Eliminar(Tienda tienda)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    context.Remove(tienda);
                    await context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception)
            {
                throw;
            }
            return View(tienda);
        }

        public async Task<IActionResult> Detalle(int Id)
        {
            var tienda = await context.Tiendas.FirstOrDefaultAsync(u => u.Id == Id);
            await context.Tiendas.FindAsync(Id);
            return View(tienda);
        }
    }
}
