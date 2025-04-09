using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System.Data;

namespace ApiReto.Controllers;

[ApiController]
[Route("[controller]")]
public class VideojuegoController : ControllerBase
{
    string connectionString = "Server=mysql-93cf659-tamtok2-09a8.b.aivencloud.com;Port=23481;Database=BDOxxo;Uid=avnadmin;Password=AVNS_-9SXvTjsy8x6dg2kaJR";

    [HttpGet]
    public List<object> GetCasos()
    {
        List<object> casos = new List<object>();

        using (MySqlConnection conexion = new MySqlConnection(connectionString))
        {
            conexion.Open();
            MySqlCommand cmd = new MySqlCommand("sp_get_casos_aleatorios", conexion);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Prepare();

            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    casos.Add(new
                    {
                        id_caso = reader.GetInt32("id_caso"),
                        titulo = reader["titulo"].ToString(),
                        descripcion = reader["descripcion"].ToString(),
                        orden_sprite = reader.GetInt32("orden_sprite")
                    });
                }
            }

            conexion.Close();
        }

        return casos;
    }

    // GET: /casos/opciones/1
    [HttpGet("opciones/{id}")]
    public List<object> GetOpcionesPorCaso(int id)
    {
        List<object> opciones = new List<object>();

        using (MySqlConnection conexion = new MySqlConnection(connectionString))
        {
            conexion.Open();
            MySqlCommand cmd = new MySqlCommand("sp_get_opciones_por_caso", conexion);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@casoId", id);
            cmd.Prepare();

            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    opciones.Add(new
                    {
                        id_opcion = reader.GetInt32("id_opcion"),
                        texto_opcion = reader["texto_opcion"].ToString(),
                        sprite_resultado = reader.GetInt32("sprite_resultado"),
                        id_tipo_opcion = reader.IsDBNull(reader.GetOrdinal("id_tipo_opcion")) ? (int?)null : reader.GetInt32("id_tipo_opcion")
                    });
                }
            }

            conexion.Close();
        }

        return opciones;
    }

    [HttpPost("instancia/{idJuego}")]
    public IActionResult CrearInstancia(int idJuego)
    {
        using (MySqlConnection conexion = new MySqlConnection(connectionString))
        {
            conexion.Open();
            MySqlCommand cmd = new MySqlCommand("sp_insert_instancia", conexion);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@p_id_juego", idJuego);
            cmd.Prepare();

            using (var reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    int id_instancia = Convert.ToInt32(reader["id_instancia"]);
                    return Ok(new { id_instancia });
                }
            }

            conexion.Close();
        }

        return BadRequest("No se pudo crear la instancia.");
    }


    [HttpPost("decision")]
    public IActionResult InsertarDecision([FromBody] DecisionJugador datos)
    {
        int instanciaId = datos.Id_Instancia;
        int opcionId = datos.Id_Opcion;
        int ordenCaso = datos.Orden_Caso;

        using (MySqlConnection conexion = new MySqlConnection(connectionString))
        {
            conexion.Open();
            MySqlCommand cmd = new MySqlCommand("sp_insert_decision", conexion);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@instanciaId", instanciaId);
            cmd.Parameters.AddWithValue("@opcionId", opcionId);
            cmd.Parameters.AddWithValue("@ordenCaso", ordenCaso);

            cmd.Prepare();
            cmd.ExecuteNonQuery();
            conexion.Close();
        }

        return Ok("Decisión registrada correctamente.");
    }



    [HttpGet("indicadores/{id}")]
    public IActionResult GetIndicadoresPorInstancia(int id)
    {
        List<object> indicadores = new List<object>();

        using (MySqlConnection conexion = new MySqlConnection(connectionString))
        {
            conexion.Open();
            MySqlCommand cmd = new MySqlCommand("sp_get_indicadores_por_instancia", conexion);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@instanciaId", id);
            cmd.Prepare();

            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    indicadores.Add(new
                    {
                        id_indicador = reader.GetInt32("id_indicador"),
                        nombre = reader["nombre"].ToString(),
                        unidad = reader["unidad"].ToString(),
                        categoria = reader["categoria"].ToString(),
                        impacto_total = reader.GetInt32("impacto_total")
                    });
                }
            }

            conexion.Close();
        }

        return Ok(indicadores);
    }

    [HttpGet("opcion/{id}/indicadores")]
    public IActionResult GetIndicadoresPorOpcion(int id)
    {
        List<object> indicadores = new List<object>();

        using (MySqlConnection conexion = new MySqlConnection(connectionString))
        {
            conexion.Open();
            MySqlCommand cmd = new MySqlCommand("sp_get_indicadores_por_opcion", conexion);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@opcionId", id);
            cmd.Prepare();

            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    indicadores.Add(new
                    {
                        id_indicador = reader.GetInt32("id_indicador"),
                        nombre = reader["nombre"].ToString(),
                        categoria = reader["categoria"].ToString(),
                        cambio_valor = Convert.ToInt32(reader["cambio_valor"])
                    });
                }
            }

            conexion.Close();
        }

        return Ok(indicadores);
    }




}
