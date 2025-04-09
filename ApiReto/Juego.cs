public class Juego
{
    public int IdJuego { get; set; }
    public string JuegoNombre { get; set; }
    public string Descripcion { get; set; }

    public ICollection<Pregunta> Preguntas { get; set; }
}
