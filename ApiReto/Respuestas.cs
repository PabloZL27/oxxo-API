public class Respuesta
{
    public int IdRespuesta { get; set; }
    public int IdPregunta { get; set; }

    public string Texto { get; set; }
    public bool EsCorrecta { get; set; }

    public Pregunta Pregunta { get; set; }
}
