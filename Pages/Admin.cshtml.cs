using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

[Authorize(Policy = "AdminOnly")]
public class AdminModel : PageModel
{
    public string NombreUsuario { get; set; } = "Administrador";
    public DateTime FechaActual { get; set; }
    public string MensajeRecibido { get; set; } = string.Empty;

    public List<string> Elementos { get; set; } = new List<string>
    {
        "Elemento 1",
        "Elemento 2",
        "Elemento 3"
    };

    public void OnGet()
    {
        FechaActual = DateTime.Now;
    }

    public void OnPost(string Mensaje)
    {
        FechaActual = DateTime.Now;
        MensajeRecibido = Mensaje;
    }
}
