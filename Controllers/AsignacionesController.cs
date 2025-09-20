using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PracticoII_Web.Data.Context;
using PracticoII_Web.Data.Models;
using System.Linq;

namespace PracticoII_Web.Controllers
{
    public class AsignacionesController : Controller
    {
        private readonly Bug_Tracker_BDDContext _context;

        public AsignacionesController(Bug_Tracker_BDDContext context)
        {
            _context = context;
        }

        // GET: Asignaciones
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var bug_Tracker_BDDContext = _context.Asignaciones.Include(a => a.IdUsuarioAsignadoNavigation);
            return View(await bug_Tracker_BDDContext.ToListAsync());
        }

        // GET: Asignaciones/Details
        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var asignaciones = await _context.Asignaciones
                .Include(a => a.IdUsuarioAsignadoNavigation)
                .FirstOrDefaultAsync(m => m.IdAsignacion == id);
            if (asignaciones == null)
            {
                return NotFound();
            }

            return View(asignaciones);
        }

        // GET: Asignaciones/Create
        [Authorize(Roles = "Administrador")]
        public IActionResult Create()
        {
            ViewData["IdUsuarioAsignado"] = new SelectList(_context.Usuarios, "IdUsuario", "IdUsuario");
            return View();
        }

        // POST: Asignaciones/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Create([Bind("IdAsignacion,IdUsuarioAsignado,FechaAsignacion")] Asignacione asignaciones)
        {
            if (ModelState.IsValid)
            {
                _context.Add(asignaciones);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdUsuarioAsignado"] = new SelectList(_context.Usuarios, "IdUsuario", "IdUsuario", asignaciones.IdUsuarioAsignado);
            return View(asignaciones);
        }

        // GET: Asignaciones/Edit
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var asignaciones = await _context.Asignaciones.FindAsync(id);
            if (asignaciones == null)
            {
                return NotFound();
            }
            ViewData["IdUsuarioAsignado"] = new SelectList(_context.Usuarios, "IdUsuario", "IdUsuario", asignaciones.IdUsuarioAsignado);
            return View(asignaciones);
        }

        // POST: Asignaciones/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Edit(int id, [Bind("IdAsignacion,IdUsuarioAsignado,FechaAsignacion")] Asignacione asignaciones)
        {
            if (id != asignaciones.IdAsignacion)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(asignaciones);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AsignacionesExists(asignaciones.IdAsignacion))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdUsuarioAsignado"] = new SelectList(_context.Usuarios, "IdUsuario", "IdUsuario", asignaciones.IdUsuarioAsignado);
            return View(asignaciones);
        }

        // GET: Asignaciones/Delete
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var asignaciones = await _context.Asignaciones
                .Include(a => a.IdUsuarioAsignadoNavigation)
                .FirstOrDefaultAsync(m => m.IdAsignacion == id);
            if (asignaciones == null)
            {
                return NotFound();
            }

            return View(asignaciones);
        }

        // POST: Asignaciones/Delete
        [Authorize(Roles = "Administrador")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var asignaciones = await _context.Asignaciones.FindAsync(id);
            if (asignaciones != null)
            {
                _context.Asignaciones.Remove(asignaciones);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AsignacionesExists(int id)
        {
            return _context.Asignaciones.Any(e => e.IdAsignacion == id);
        }
    }
}
