namespace ProyectoZetino.WebMVC.Models // (Usa tu namespace)
{
    public class ErrorViewModel
    {
        public string? RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}