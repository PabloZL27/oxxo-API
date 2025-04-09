public class Pregunta
{
    public int IdPregunta { get; set; }
    public string Texto { get; set; }
    public string Categoria { get; set; }
    public int Puntaje { get; set; }

    public int IdJuego { get; set; }
    public Juego Juego { get; set; }

    public ICollection<Respuesta> Respuestas { get; set; }
}
