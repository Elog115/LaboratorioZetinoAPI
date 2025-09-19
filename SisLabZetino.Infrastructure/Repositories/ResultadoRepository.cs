using Microsoft.EntityFrameworkCore;
using SisLabZetino.Domain.Entities;
using SisLabZetino.Domain.Repositories;
using SisLabZetino.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SisLabZetino.Infrastructure.Repositories
{
    public class ResultadoRepository : IResultadoRepository
    {
        private readonly AppDBContext _context;

        public ResultadoRepository(AppDBContext context)
        {
            _context = context;
        }

        // Obtener todos los resultados
        public async Task<IEnumerable<Resultado>> GetResultadosAsync()
        {
            return await _context.Resultados.ToListAsync();
        }

        // Obtener un resultado por su Id
        public async Task<Resultado> GetResultadoByIdAsync(int id)
        {
            return await _context.Resultados
                                 .FirstOrDefaultAsync(r => r.IdResultado == id);
        }

        // Agregar un nuevo resultado
        public async Task<Resultado> AddResultadoAsync(Resultado resultado)
        {
            _context.Resultados.Add(resultado);
            await _context.SaveChangesAsync();
            return resultado;
        }

        // Actualizar un resultado existente
        public async Task<Resultado> UpdateResultadoAsync(Resultado resultado)
        {
            _context.Resultados.Update(resultado);
            await _context.SaveChangesAsync();
            return resultado;
        }

        // Eliminar un resultado por su Id
        public async Task<bool> DeleteResultadoAsync(int id)
        {
            var resultado = await _context.Resultados.FindAsync(id);
            if (resultado == null)
                return false;

            _context.Resultados.Remove(resultado);
            await _context.SaveChangesAsync();
            return true;
        }

        // Obtener resultados por IdExamen
        public async Task<IEnumerable<Resultado>> GetResultadosByExamenAsync(int idExamen)
        {
            return await _context.Resultados
                                 .Where(r => r.IdExamen == idExamen)
                                 .ToListAsync();
        }

        // Obtener resultados por fecha de entrega
        public async Task<IEnumerable<Resultado>> GetResultadosByFechaEntregaAsync(DateTime fechaEntrega)
        {
            // Se compara solo la parte de la fecha
            return await _context.Resultados
                                 .Where(r => r.FechaEntrega.Date == fechaEntrega.Date)
                                 .ToListAsync();
        }

        // Obtener resultados por estado
        public async Task<IEnumerable<Resultado>> GetResultadosByEstadoAsync(int estado)
        {
            return await _context.Resultados
                                 .Where(r => r.Estado == estado)
                                 .ToListAsync();
        }
    }
}
