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

   [HttpPost("inicializar_valores/{idInstancia}")]
    public IActionResult InicializarValoresTemporales(int idInstancia)
    {
        using (MySqlConnection conexion = new MySqlConnection(connectionString))
        {
            conexion.Open();
            MySqlCommand cmd = new MySqlCommand("InicializarValoresTemporales", conexion);
            cmd.CommandType = CommandType.StoredProcedure;

            // CORRECTO: Este debe coincidir con el nombre del parámetro en tu SP
            cmd.Parameters.AddWithValue("@instancia_id", idInstancia);

            try
            {
                cmd.ExecuteNonQuery();
                return Ok("Valores temporales inicializados");
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = "Error al inicializar valores: " + ex.Message });
            }
        }
    }

    [HttpGet("valores_actuales/{idInstancia}")]
    public IActionResult GetValoresActuales(int idInstancia)
    {
        List<object> valores = new List<object>();

        using (MySqlConnection conexion = new MySqlConnection(connectionString))
        {
            conexion.Open();

            MySqlCommand cmd = new MySqlCommand(@"
                SELECT i.id_indicador, i.nombre, v.valor_actual
                FROM valores_temporales_indicadores v
                JOIN indicadores i ON v.id_indicador = i.id_indicador
                WHERE v.id_instancia = @idInstancia
            ", conexion);

            cmd.Parameters.AddWithValue("@idInstancia", idInstancia);
            cmd.Prepare();

            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    valores.Add(new
                    {
                        id_indicador = reader.GetInt32("id_indicador"),
                        nombre = reader["nombre"].ToString(),
                        valor_actual = reader.GetInt32("valor_actual")
                    });
                }
            }

            conexion.Close();
        }

        return Ok(valores);
    }

        [HttpPost("aplicar_impacto")]
    public IActionResult AplicarImpacto([FromBody] AplicarImpactoRequest request)
    {
        // --- Validación Opcional (pero recomendada) ---
        if (request.id_instancia <= 0 || request.id_opcion <= 0)
        {
            return BadRequest(new { error = "ID de instancia y/o ID de opción inválido(s)." });
        }

        try
        {
            using (var conexion = new MySqlConnection(connectionString))
            {
                conexion.Open();

                // --- Configurar el comando para llamar al Stored Procedure ---
                var cmd = new MySqlCommand("aplicar_impactos_a_temporales", conexion); // Nombre del SP
                cmd.CommandType = CommandType.StoredProcedure; // Indicar que es un SP

                // --- Añadir los parámetros requeridos por el SP ---
                // Asegúrate que los nombres (@p_id_instancia, @p_id_opcion) coincidan con los del SP
                // AddWithValue a menudo funciona, pero especificar el tipo es más seguro:
                cmd.Parameters.Add("@p_id_instancia", MySqlDbType.Int32).Value = request.id_instancia;
                cmd.Parameters.Add("@p_id_opcion", MySqlDbType.Int32).Value = request.id_opcion;

                // --- Ejecutar el Stored Procedure ---
                // ExecuteNonQuery se usa porque el SP no devuelve un conjunto de resultados (solo hace UPDATEs)
                cmd.ExecuteNonQuery();

                // --- Ya no necesitas el bucle foreach ---

            } // La conexión se cierra automáticamente por el using

            return Ok("Impactos aplicados correctamente mediante SP."); // Mensaje de éxito
        }
        catch (MySqlException mysqlEx) // Captura errores específicos de MySQL si quieres
        {
            // Loggear el error completo para diagnóstico
            Console.WriteLine($"MySQL Error executing SP aplicar_impactos_a_temporales: {mysqlEx.ToString()}");
            return StatusCode(500, new { error = "Error en la base de datos al aplicar impactos.", details = mysqlEx.Message });
        }
        catch (Exception ex) // Captura cualquier otro error
        {
            // Loggear el error completo para diagnóstico
            Console.WriteLine($"General Error executing SP aplicar_impactos_a_temporales: {ex.ToString()}");
            return StatusCode(500, new { error = "Error interno del servidor al aplicar impactos.", details = ex.Message }); // 500 Internal Server Error es más apropiado aquí
        }
    }


    [HttpPost("UpdateGameResult")]
public IActionResult UpdateGameResult([FromBody] UpdateScoreRequest request)
{
    string query = @"
        UPDATE instanciajuego
        SET puntuacion = @Puntuacion,
            id_usuario = @IdUsuario
        WHERE id_instancia = @IdInstancia;";

    MySqlConnection connection = null;
    try
    {
        connection = new MySqlConnection(connectionString);
        connection.Open();
        using var command = new MySqlCommand(query, connection);

        command.Parameters.AddWithValue("@Puntuacion", request.Puntuacion);
        command.Parameters.AddWithValue("@IdUsuario", request.IdUsuario);
        command.Parameters.AddWithValue("@IdInstancia", request.IdInstancia);

        int rowsAffected = command.ExecuteNonQuery();

        if (rowsAffected > 0)
            return Ok(new { message = "Puntaje actualizado correctamente." });
        else
            return BadRequest(new { message = "No se encontró la instancia o no se actualizó nada." });
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error en UpdateGameResult: {ex.Message}");
        return StatusCode(500, new { message = "Error interno del servidor.", details = ex.Message });
    }
    finally
    {
        connection?.Close();
    }
}



}
