
namespace ProyectoZetino.WebMVC.Models
{
    public class RolDto
    {
        public int IdRol { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public bool Estado { get; set; }
    }
}