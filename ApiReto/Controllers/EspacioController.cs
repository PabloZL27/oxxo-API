using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System.Data;

namespace ApiReto.Controllers;

[ApiController]
[Route("[controller]")]
public class EspacioController : ControllerBase
{
    string connectionString = "Server=mysql-93cf659-tamtok2-09a8.b.aivencloud.com;Port=23481;Database=BDOxxo;Uid=avnadmin;Password=AVNS_-9SXvTjsy8x6dg2kaJR";

    // GET: /videojuego?juegoId=2
    [HttpGet("Espacio_Pregunta")]
    public List<object> GetPreguntas([FromQuery] int juegoId = 1)
    {
        var preguntas = new List<object>();

        using var conexion = new MySqlConnection(connectionString);
        conexion.Open();

        string query = "SELECT id_pregunta, texto, categoria, puntaje FROM preguntas WHERE id_juego = @juegoId ORDER BY RAND() LIMIT 6";
        using var cmd = new MySqlCommand(query, conexion);
        cmd.Parameters.AddWithValue("@juegoId", juegoId);

        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            preguntas.Add(new
            {
                id_pregunta = reader.GetInt32("id_pregunta"),
                texto = reader["texto"].ToString(),
                categoria = reader["categoria"].ToString(),
                puntaje = reader.GetInt32("puntaje")
            });
        }

        return preguntas;
    }

    // GET: /videojuego/respuestas/3
    [HttpGet("respuestas/{idPregunta}")]
    public List<object> GetRespuestas(int idPregunta)
    {
        var respuestas = new List<object>();

        using var conexion = new MySqlConnection(connectionString);
        conexion.Open();

        string query = "SELECT id_respuesta, texto, es_correcta FROM respuestas WHERE id_pregunta = @idPregunta";
        using var cmd = new MySqlCommand(query, conexion);
        cmd.Parameters.AddWithValue("@idPregunta", idPregunta);

        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            respuestas.Add(new
            {
                id_respuesta = reader.GetInt32("id_respuesta"),
                texto = reader["texto"].ToString(),
                es_correcta = reader.GetBoolean("es_correcta")
            });
        }

        return respuestas;
    }


}
