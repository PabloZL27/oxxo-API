using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System;

namespace ApiReto.Controllers 
{
    [ApiController]
    [Route("[controller]")]
    public class ScoreController : ControllerBase
    {
        private readonly string _connectionString = "Server=mysql-93cf659-tamtok2-09a8.b.aivencloud.com;Port=23481;Database=BDOxxo;Uid=avnadmin;Password=AVNS_-9SXvTjsy8x6dg2kaJR";

        [HttpPost("SaveGameResult")] // Ruta: POST /Score/SaveGameResult
        public IActionResult SaveGameResult([FromBody] SaveScoreRequest request)
        {

            string query = @"
                INSERT INTO instanciajuego (id_usuario, id_juego, puntuacion, fecha)
                VALUES (@IdUsuario, @IdJuego, @Puntuacion, @Fecha);";

            MySqlConnection connection = null;
            try
            {
                connection = new MySqlConnection(_connectionString);
                connection.Open();
                using var command = new MySqlCommand(query, connection);

                command.Parameters.AddWithValue("@IdUsuario", request.IdUsuario.Value);
                command.Parameters.AddWithValue("@IdJuego", request.IdJuego.Value);
                command.Parameters.AddWithValue("@Puntuacion", request.Puntuacion.Value);
                command.Parameters.AddWithValue("@Fecha", DateTime.UtcNow.Date); 

                int rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected > 0) return Ok(new { message = "Puntaje guardado." });
                else return BadRequest(new { message = "No se guardó el puntaje." });
            }
            catch (Exception ex) {
                Console.WriteLine($"Error en SaveGameResult: {ex.Message}"); 
                return StatusCode(500, new { message = "Error interno del servidor.", details = ex.Message });
            }
            finally {
                connection?.Close();
            }
        }

        [HttpGet("MaximoPuntaje/{idUsuario}/{idJuego}")]
        public IActionResult ObtenerMaximoPuntaje(int idUsuario, int idJuego)
        {
            string query = @"
               SELECT MAX(puntuacion) 
                FROM instanciajuego 
                WHERE id_usuario = @idUsuario AND id_juego = 2;";

            MySqlConnection connection = null;
            try
            {
                connection = new MySqlConnection(_connectionString);
                connection.Open();
                using var command = new MySqlCommand(query, connection);

                command.Parameters.AddWithValue("@IdUsuario", idUsuario);
                command.Parameters.AddWithValue("@IdJuego", idJuego);

                var result = command.ExecuteScalar();

                if (result != DBNull.Value && result != null)
                {
                    int puntuacionMaxima = Convert.ToInt32(result);
                    return Ok(new { idUsuario = idUsuario, idJuego = idJuego, puntuacionMaxima = puntuacionMaxima });
                }
                else
                {
                    return Ok(new { idUsuario = idUsuario, idJuego = idJuego, puntuacionMaxima = 0 });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en ObtenerMaximoPuntaje: {ex.Message}");
                return StatusCode(500, new { message = "Error interno del servidor.", details = ex.Message });
            }
            finally
            {
                connection?.Close();
            }
        }

    }
}