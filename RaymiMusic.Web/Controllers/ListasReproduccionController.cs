﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaymiMusic.Api.Data;
using RaymiMusic.Modelos;

namespace RaymiMusic.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ListasReproduccionController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ListasReproduccionController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/ListasReproduccion
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ListaReproduccion>>> GetListas()
        {
            return await _context.ListasReproduccion
                .Include(l => l.Usuario)
                .Include(l => l.CancionesEnListas)
                    .ThenInclude(cl => cl.Cancion)
                .ToListAsync();
        }

        // GET: api/ListasReproduccion/{id}
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ListaReproduccion>> GetLista(Guid id)
        {
            var lista = await _context.ListasReproduccion
                .Include(l => l.Usuario)
                .Include(l => l.CancionesEnListas)
                    .ThenInclude(cl => cl.Cancion)
                .FirstOrDefaultAsync(l => l.Id == id);

            if (lista == null) return NotFound();
            return lista;
        }

        // POST: api/ListasReproduccion
        [HttpPost]
        public async Task<ActionResult<ListaReproduccion>> PostLista(ListaReproduccion lista)
        {
            lista.Id = Guid.NewGuid();
            _context.ListasReproduccion.Add(lista);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetLista), new { id = lista.Id }, lista);
        }

        // PUT: api/ListasReproduccion/{id}
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> PutLista(Guid id, ListaReproduccion lista)
        {
            if (id != lista.Id) return BadRequest();

            _context.Entry(lista).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                bool exists = await _context.ListasReproduccion.AnyAsync(l => l.Id == id);
                if (!exists) return NotFound();
                throw;
            }
            return NoContent();
        }

        // DELETE: api/ListasReproduccion/{id}
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteLista(Guid id)
        {
            var lista = await _context.ListasReproduccion.FindAsync(id);
            if (lista == null) return NotFound();

            _context.ListasReproduccion.Remove(lista);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // POST: api/ListasReproduccion/{listaId}/AgregarCancion/{cancionId}
        [HttpPost("{listaId:guid}/AgregarCancion/{cancionId:guid}")]
        public async Task<IActionResult> AgregarCancion(Guid listaId, Guid cancionId)
        {
            var lista = await _context.ListasReproduccion.FindAsync(listaId);
            var cancion = await _context.Canciones.FindAsync(cancionId);
            if (lista == null || cancion == null) return NotFound();

            var enlace = new CancionLista
            {
                Id = Guid.NewGuid(),
                ListaReproduccionId = listaId,
                CancionId = cancionId
            };
            _context.CancionesEnListas.Add(enlace);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/ListasReproduccion/{listaId}/QuitarCancion/{cancionId}
        [HttpDelete("{listaId:guid}/QuitarCancion/{cancionId:guid}")]
        public async Task<IActionResult> QuitarCancion(Guid listaId, Guid cancionId)
        {
            var enlace = await _context.CancionesEnListas
                .FirstOrDefaultAsync(cl => cl.ListaReproduccionId == listaId && cl.CancionId == cancionId);
            if (enlace == null) return NotFound();

            _context.CancionesEnListas.Remove(enlace);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
