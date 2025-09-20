using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PracticoII_Web.Data.Context;
using PracticoII_Web.Data.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using System.Linq;

namespace PracticoII_Web.Controllers
{
    [AllowAnonymous]
    public class LoginController : Controller
    {
        private readonly Bug_Tracker_BDDContext _context;

        public LoginController(Bug_Tracker_BDDContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            var model = new LoginRegister
            {
                Login = new Login(),
                Register = new Register()
            };

            return View(model);
        }

        // POST: Login/LoginPost
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginPost(LoginRegister model)
        {
            ModelState.Remove("Register.Nombre");
            ModelState.Remove("Register.Email");
            ModelState.Remove("Register.Contraseña");
            ModelState.Remove("Register.Rol");

            if (!ModelState.IsValid) return View("Login", model);

            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Email == model.Login.Email && u.Contraseña == model.Login.Contraseña);

            if (usuario == null)
            {
                ModelState.AddModelError(string.Empty, "Credenciales inválidas");
                return View("Login", model);
            }

            if (string.IsNullOrEmpty(usuario.Rol))
            {
                ModelState.AddModelError(string.Empty, "El usuario no tiene un rol asignado.");
                return View("Login", model);
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, usuario.Nombre),
                new Claim(ClaimTypes.Email, usuario.Email),
                new Claim("IdUsuario", usuario.IdUsuario.ToString()),
                new Claim(ClaimTypes.Role, usuario.Rol)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = model.Login.Recordarme,
                ExpiresUtc = model.Login.Recordarme
                ? DateTime.UtcNow.AddDays(7)   // sesión de 1 semana
                : DateTime.UtcNow.AddMinutes(30)
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity), authProperties);

            return RedirectToAction("Index", "Home");
        }

        // POST: Login/RegisterPost
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterPost(LoginRegister model)
        {
            ModelState.Remove("Login.Email");
            ModelState.Remove("Login.Contraseña");

            if (!ModelState.IsValid) return View("Login", model);

            var existe = await _context.Usuarios
                .AnyAsync(u => u.Email == model.Register.Email);

            if (existe)
            {
                ModelState.AddModelError(string.Empty, "Ya existe un usuario con ese correo.");
                return View("Login", model);
            }

            var nuevoUsuario = new Usuario
            {
                Nombre = model.Register.Nombre,
                Email = model.Register.Email,
                Contraseña = model.Register.Contraseña,
                Rol = model.Register.Rol 
            };

            _context.Usuarios.Add(nuevoUsuario);
            await _context.SaveChangesAsync();

            TempData["Mensaje"] = "Registro exitoso. Ahora podés iniciar sesión.";
            return RedirectToAction("Login");
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
    }
}