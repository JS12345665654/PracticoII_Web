using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using PracticoII_Web.Data.Context;
using PracticoII_Web.Data.Models;
using OfficeOpenXml;
using System.IO;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Kernel.Font;
using iText.IO.Font.Constants;

namespace PracticoII_Web.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly Bug_Tracker_BDDContext _context;

        public UsuarioController(Bug_Tracker_BDDContext context)
        {
            _context = context;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index(string searchString)
        {
            var usuarios = _context.Usuarios.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                searchString = searchString.ToLower();
                usuarios = usuarios.Where(u =>
                    u.Nombre.ToLower().Contains(searchString) ||
                    u.Email.ToLower().Contains(searchString) ||
                    u.Rol.ToLower().Contains(searchString));
            }

            ViewData["CurrentFilter"] = searchString ?? TempData["SearchTerm"];
            return View(await usuarios.ToListAsync());
        }

        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(m => m.IdUsuario == id);
            if (usuario == null) return NotFound();
            return View(usuario);
        }

        [AllowAnonymous]
        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> Create([Bind("IdUsuario,Nombre,Email,Rol,Contraseña")] Usuario usuario)
        {
            if (ModelState.IsValid)
            {
                _context.Add(usuario);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(usuario);
        }

        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null) return NotFound();
            return View(usuario);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Edit(int id, [Bind("IdUsuario,Nombre,Email,Rol,Contraseña")] Usuario usuario)
        {
            if (id != usuario.IdUsuario) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(usuario);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UsuarioExists(usuario.IdUsuario)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(usuario);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Delete(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario != null)
            {
                _context.Usuarios.Remove(usuario);
                await _context.SaveChangesAsync();
                TempData["Mensaje"] = $"El usuario '{usuario.Nombre}' fue eliminado correctamente.";
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult ExportarExcel(string searchTerm)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            var usuarios = FiltrarUsuarios(searchTerm);

            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Usuarios");

            worksheet.Cells[1, 1].Value = "Nombre";
            worksheet.Cells[1, 2].Value = "Email";
            worksheet.Cells[1, 3].Value = "Rol";

            int row = 2;
            foreach (var usuario in usuarios)
            {
                worksheet.Cells[row, 1].Value = usuario.Nombre;
                worksheet.Cells[row, 2].Value = usuario.Email;
                worksheet.Cells[row, 3].Value = usuario.Rol;
                row++;
            }

            worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

            var stream = new MemoryStream();
            package.SaveAs(stream);
            stream.Position = 0;

            TempData["SearchTerm"] = searchTerm;
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Usuarios.xlsx");
        }

        [HttpPost]
        [AllowAnonymous]
        [HttpPost]
        public IActionResult ExportarPDF(string searchTerm)
        {
            var usuarios = FiltrarUsuarios(searchTerm);

            using var stream = new MemoryStream();
            var writer = new PdfWriter(stream);
            var pdf = new PdfDocument(writer);
            var document = new Document(pdf);

            var boldFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);
            var italicFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_OBLIQUE);

            document.Add(new Paragraph("Listado de Usuarios").SetFont(boldFont).SetFontSize(14));
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                document.Add(new Paragraph($"Filtrado por: \"{searchTerm}\"").SetFont(italicFont).SetFontSize(10));
            }

            var table = new Table(UnitValue.CreatePercentArray(new float[] { 3, 4, 3 })).UseAllAvailableWidth();
            table.AddHeaderCell("Nombre");
            table.AddHeaderCell("Email");
            table.AddHeaderCell("Rol");

            foreach (var usuario in usuarios)
            {
                table.AddCell(usuario.Nombre);
                table.AddCell(usuario.Email);
                table.AddCell(usuario.Rol);
            }

            document.Add(table);
            document.Close();

            TempData["SearchTerm"] = searchTerm;
            return File(stream.ToArray(), "application/pdf", "Usuarios.pdf");
        }


        private List<Usuario> FiltrarUsuarios(string searchTerm)
        {
            var query = _context.Usuarios.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = searchTerm.ToLower();
                query = query.Where(u =>
                    u.Nombre.ToLower().Contains(searchTerm) ||
                    u.Email.ToLower().Contains(searchTerm) ||
                    u.Rol.ToLower().Contains(searchTerm));
            }

            return query.ToList();
        }

        private bool UsuarioExists(int id)
        {
            return _context.Usuarios.Any(e => e.IdUsuario == id);
        }
    }
}
