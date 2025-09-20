using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PracticoII_Web.Data.Context;
using PracticoII_Web.Data.Models;

namespace PracticoII_Web.Controllers
{
    public class IncidenteController : Controller
    {
        private readonly Bug_Tracker_BDDContext _context;

        public IncidenteController(Bug_Tracker_BDDContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var incidentes = _context.Incidentes
                .Include(i => i.IdUsuarioReportaNavigation)
                .Include(i => i.IdProyectoNavigation);
            return View(await incidentes.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var incidente = await _context.Incidentes
                .Include(i => i.IdUsuarioReportaNavigation)
                .Include(i => i.IdProyectoNavigation)
                .FirstOrDefaultAsync(m => m.IdIncidente == id);

            if (incidente == null) return NotFound();

            return View(incidente);
        }

        public IActionResult Create()
        {
            ViewData["IdUsuarioReporta"] = new SelectList(_context.Usuarios, "IdUsuario", "Nombre");
            ViewData["IdProyecto"] = new SelectList(_context.Proyectos, "IdProyecto", "Nombre");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdIncidente,Titulo,Descripcion,IdUsuarioReporta,IdProyecto")] Incidente incidente)
        {
            if (ModelState.IsValid)
            {
                _context.Add(incidente);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["IdUsuarioReporta"] = new SelectList(_context.Usuarios, "IdUsuario", "Nombre", incidente.IdUsuarioReporta);
            ViewData["IdProyecto"] = new SelectList(_context.Proyectos, "IdProyecto", "Nombre", incidente.IdProyecto);
            return View(incidente);
        }

        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var incidente = await _context.Incidentes.FindAsync(id);
            if (incidente == null) return NotFound();

            ViewData["IdUsuarioReporta"] = new SelectList(_context.Usuarios, "IdUsuario", "Nombre", incidente.IdUsuarioReporta);
            ViewData["IdProyecto"] = new SelectList(_context.Proyectos, "IdProyecto", "Nombre", incidente.IdProyecto);
            return View(incidente);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Edit(int id, [Bind("IdIncidente,Titulo,Descripcion,IdUsuarioReporta,IdProyecto")] Incidente incidente)
        {
            if (id != incidente.IdIncidente) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(incidente);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!IncidenteExists(incidente.IdIncidente)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["IdUsuarioReporta"] = new SelectList(_context.Usuarios, "IdUsuario", "Nombre", incidente.IdUsuarioReporta);
            ViewData["IdProyecto"] = new SelectList(_context.Proyectos, "IdProyecto", "Nombre", incidente.IdProyecto);
            return View(incidente);
        }

        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var incidente = await _context.Incidentes
                .Include(i => i.IdUsuarioReportaNavigation)
                .Include(i => i.IdProyectoNavigation)
                .FirstOrDefaultAsync(m => m.IdIncidente == id);

            if (incidente == null) return NotFound();

            return View(incidente);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var incidente = await _context.Incidentes.FindAsync(id);
            if (incidente != null)
            {
                _context.Incidentes.Remove(incidente);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool IncidenteExists(int id)
        {
            return _context.Incidentes.Any(e => e.IdIncidente == id);
        }
    }
}