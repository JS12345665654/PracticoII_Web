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
    public class ProyectoController : Controller
    {
        private readonly Bug_Tracker_BDDContext _context;

        public ProyectoController(Bug_Tracker_BDDContext context)
        {
            _context = context;
        }

        // GET: Proyecto
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var bug_Tracker_BDDContext = _context.Proyectos.Include(p => p.IdProyectoNavigation);
            return View(await bug_Tracker_BDDContext.ToListAsync());
        }

        // GET: Proyecto/Details
        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var proyecto = await _context.Proyectos
                .Include(p => p.IdProyectoNavigation)
                .FirstOrDefaultAsync(m => m.IdProyecto == id);
            if (proyecto == null)
            {
                return NotFound();
            }

            return View(proyecto);
        }

        // GET: Proyecto/Create
        [Authorize(Roles = "Administrador")]
        public IActionResult Create()
        {
            ViewData["IdProyecto"] = new SelectList(_context.Asignaciones, "IdAsignacion", "IdAsignacion");
            return View();
        }

        // POST: Proyecto/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Create([Bind("IdProyecto,Nombre,Descripcion,FechaInicio,FechaFin,IdAsignacion")] Proyecto proyecto)
        {
            if (ModelState.IsValid)
            {
                _context.Add(proyecto);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdProyecto"] = new SelectList(_context.Asignaciones, "IdAsignacion", "IdAsignacion", proyecto.IdProyecto);
            return View(proyecto);
        }

        // GET: Proyecto/Edit
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var proyecto = await _context.Proyectos.FindAsync(id);
            if (proyecto == null)
            {
                return NotFound();
            }
            ViewData["IdProyecto"] = new SelectList(_context.Asignaciones, "IdAsignacion", "IdAsignacion", proyecto.IdProyecto);
            return View(proyecto);
        }

        // POST: Proyecto/Edit
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Edit(int id, [Bind("IdProyecto,Nombre,Descripcion,FechaInicio,FechaFin,IdAsignacion")] Proyecto proyecto)
        {
            if (id != proyecto.IdProyecto)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(proyecto);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProyectoExists(proyecto.IdProyecto))
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
            ViewData["IdProyecto"] = new SelectList(_context.Asignaciones, "IdAsignacion", "IdAsignacion", proyecto.IdProyecto);
            return View(proyecto);
        }

        // GET: Proyecto/Delete
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var proyecto = await _context.Proyectos
                .Include(p => p.IdProyectoNavigation)
                .FirstOrDefaultAsync(m => m.IdProyecto == id);
            if (proyecto == null)
            {
                return NotFound();
            }

            return View(proyecto);
        }

        // POST: Proyecto/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var proyecto = await _context.Proyectos.FindAsync(id);
            if (proyecto != null)
            {
                _context.Proyectos.Remove(proyecto);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProyectoExists(int id)
        {
            return _context.Proyectos.Any(e => e.IdProyecto == id);
        }
    }
}
