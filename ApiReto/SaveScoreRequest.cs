using System.ComponentModel.DataAnnotations;

public class SaveScoreRequest
{
    [Required] public int? IdUsuario { get; set; }
    [Required] public int? IdJuego { get; set; }
    [Required] public int? Puntuacion { get; set; }
}