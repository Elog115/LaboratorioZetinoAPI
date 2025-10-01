using SisLabZetino.Domain.Entities;
using SisLabZetino.Domain.Repositories;
using SisLabZetino.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SisLabZetino.Infrastructure.Repositories
{
    public class CitaRepository : ICitaRepository
    {
        private readonly AppDBContext _context;

        public CitaRepository(AppDBContext context)
        {
            _context = context;
        }

        // Obtener todas las citas
        public async Task<IEnumerable<Cita>> GetCitasAsync()
        {
            return await _context.Citas.ToListAsync();
        }

        // Obtener una cita por su Id
        public async Task<Cita> GetCitaByIdAsync(int id)
        {
            return await _context.Citas.FindAsync(id);
        }

        // Agregar una nueva cita
        public async Task<Cita> AddCitaAsync(Cita cita)
        {
            _context.Citas.Add(cita);
            await _context.SaveChangesAsync();
            return cita;
        }

        // Actualizar una cita existente
        public async Task<Cita> UpdateCitaAsync(Cita cita)
        {
            var existingCita = await _context.Citas.FindAsync(cita.IdCita);
            if (existingCita == null)
                return null;

            existingCita.IdUsuario = cita.IdUsuario;
            existingCita.FechaHora = cita.FechaHora;
            existingCita.Descripcion = cita.Descripcion;
            existingCita.Estado = cita.Estado;

            await _context.SaveChangesAsync();
            return existingCita;
        }

        // Eliminar una cita por su Id
        public async Task<bool> DeleteCitaAsync(int id)
        {
            var cita = await _context.Citas.FindAsync(id);
            if (cita == null)
                return false;

            _context.Citas.Remove(cita);
            await _context.SaveChangesAsync();
            return true;
        }

        // Obtener citas por IdUsuario
        public async Task<IEnumerable<Cita>> GetCitasByUsuarioAsync(int idUsuario)
        {
            return await _context.Citas
                                 .Where(c => c.IdUsuario == idUsuario)
                                 .ToListAsync();
        }

        // Obtener citas por estado (activas, canceladas, etc.)
        public async Task<IEnumerable<Cita>> GetCitasByEstadoAsync(bool estado)
        {
            return await _context.Citas
                                 .Where(c => c.Estado == estado)
                                 .ToListAsync();
        }
    }
}
