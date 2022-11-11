
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PruebaWebMaster.Models;
using PruebaWebMaster.Services;

namespace PruebaWebMaster.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly AppDbContext context;
        

        public UsuariosController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, AppDbContext context)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.context = context;
            
        }

        [AllowAnonymous]
        public IActionResult Registro()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Registro(Registro modelo)
        {
            if (!ModelState.IsValid)
            {
                return View(modelo);
            }

            var usuario = new IdentityUser() { Email = modelo.Email, UserName = modelo.Email };
            var resultado = await userManager.CreateAsync(usuario, password: modelo.Password);

            if (resultado.Succeeded)
            {
                await signInManager.SignInAsync(usuario, isPersistent: true);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                foreach (var error in resultado.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return View(modelo);
            }
        }

        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(Login modelo)
        {
            if (!ModelState.IsValid)
            {
                return View(modelo);
            }

            var resultado = await signInManager.PasswordSignInAsync(modelo.Email, 
                                                                    modelo.Password, 
                                                                    modelo.Recuerdame, 
                                                                    lockoutOnFailure: false);

            if (resultado.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Nombre de usuario o password incorrecto.");
                return View(modelo);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
            return RedirectToAction("Login", "Usuarios");
        }

        [HttpGet]
        [Authorize(Roles = Constantes.RolAdmin)]
        public async Task<IActionResult> Index(string mensaje = null)
        {
            var usuarios = await context.Users.Select(u => new Usuario
            {
                Id = u.Id,
                Nombre = u.UserName,
                Email = u.Email,
                Password = u.PasswordHash,
                Telefono = u.PhoneNumber
            }).ToListAsync();

            var modelo = new UsuariosListado();
            modelo.Usuarios = usuarios;
            modelo.Mensaje = mensaje;

            return View(modelo);
        }

        [HttpPost]
        [Authorize(Roles = Constantes.RolAdmin)]
        public async Task<IActionResult> AsignarAdmin(string email)
        {
            var usuario = await context.Users.Where(u => u.Email == email).FirstOrDefaultAsync();
            if (usuario is null)
            {
                return NotFound();
            }

            await userManager.AddToRoleAsync(usuario, Constantes.RolAdmin);

            return RedirectToAction("Index", 
                                     routeValues: new { mensaje = "Rol asignado correctamente a " + email });
        }

        [HttpPost]
        [Authorize(Roles = Constantes.RolAdmin)]
        public async Task<IActionResult> RemoverAdmin(string email)
        {
            var usuario = await context.Users.Where(u => u.Email == email).FirstOrDefaultAsync();
            if (usuario is null)
            {
                return NotFound();
            }

            await userManager.RemoveFromRoleAsync(usuario, Constantes.RolAdmin);

            return RedirectToAction("Index",
                                     routeValues: new { mensaje = "Rol removido correctamente a " + email });
        }

        public IActionResult Crear()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Crear(Usuario modelo)
        {
            if (!ModelState.IsValid)
            {
                return View(modelo);
            }

            var usuario = new IdentityUser() { Email = modelo.Email, UserName = modelo.Nombre, PasswordHash = modelo.Password,PhoneNumber = modelo.Telefono };

                context.Add(usuario);
                await context.SaveChangesAsync();
                return RedirectToAction("Index", "Usuarios");
        }

        public async Task<IActionResult> Editar(string Id)
        {
            IdentityUser user = await context.Users.FirstOrDefaultAsync(u => u.Id == Id);
            var usuario = new Usuario() { Nombre = user.UserName, Email = user.Email, Password = user.PasswordHash, Telefono = user.PhoneNumber};
            await context.Users.FindAsync(Id);
            return View(usuario);
        }

        [HttpPost]
        public async Task<IActionResult> Editar(Usuario modelo)
        {   
            if (!ModelState.IsValid)
            {
                return View(modelo);
            }

            //var usuario = new IdentityUser() { Email = modelo.Email, UserName = modelo.Nombre, PasswordHash = modelo.Password, PhoneNumber = modelo.Telefono };
            //var usuario = context.Database.ExecuteSqlRaw(@"UPDATE AspNetUsers SET UserName = '"+modelo.Nombre+"', Email = '"+modelo.Email+"', PasswordHash = '"+modelo.Password+"', PhoneNumber = '"+modelo.Telefono+"' WHERE Id = '"+modelo.Id+"'");

            IdentityUser usuario = await userManager.FindByIdAsync(modelo.Id);
            usuario.UserName = modelo.Nombre;
            usuario.Email = modelo.Email;
            usuario.PasswordHash = modelo.Password;
            usuario.PhoneNumber = modelo.Telefono;
            context.Update(usuario);
            await context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Eliminar(string Id)
        {
            IdentityUser user = await context.Users.FirstOrDefaultAsync(u => u.Id == Id);
            var usuario = new Usuario() { Nombre = user.UserName, Email = user.Email, Password = user.PasswordHash, Telefono = user.PhoneNumber };
            await context.Users.FindAsync(Id);
            return View(usuario);
        }

        [HttpPost]
        public async Task<IActionResult> Eliminar(Usuario modelo)
        {
            //var usuario = new IdentityUser() { Email = modelo.Email, UserName = modelo.Nombre, PasswordHash = modelo.Password, PhoneNumber = modelo.Telefono };
            IdentityUser usuario = await userManager.FindByIdAsync(modelo.Id);
            usuario.Id = modelo.Id;

            context.Remove(usuario);
            await context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Detalle(string Id)
        {
            IdentityUser user = await context.Users.FirstOrDefaultAsync(u => u.Id == Id);
            var usuario = new Usuario() { Id = user.Id, Nombre = user.UserName, Email = user.Email, Password = user.PasswordHash, Telefono = user.PhoneNumber };
            await context.Users.FindAsync(Id);
            return View(usuario);
        }

    }
}
