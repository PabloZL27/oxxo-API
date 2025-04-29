using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System;

namespace ApiReto.Controllers 
{
    [ApiController]
    [Route("[controller]")]
    public class LogrosController : ControllerBase
    {
        private readonly string _connectionString = "Server=mysql-93cf659-tamtok2-09a8.b.aivencloud.com;Port=23481;Database=BDOxxo;Uid=avnadmin;Password=AVNS_-9SXvTjsy8x6dg2kaJR";

        [HttpPost("obtener")]
        public async Task<IActionResult> RegistrarLogro([FromBody] RegistrarLogroRequest request)
        {
            if (request == null || request.id_usuario <= 0 || request.id_logro <= 0)
                return BadRequest(new { mensaje = "Datos inválidos." });

            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    // Verificar si ya existe el logro
                    var checkCommand = new MySqlCommand(
                        "SELECT COUNT(*) FROM logros_usuario WHERE id_usuario = @idUsuario AND id_logro = @idLogro",
                        connection);
                    checkCommand.Parameters.AddWithValue("@idUsuario", request.id_usuario);
                    checkCommand.Parameters.AddWithValue("@idLogro", request.id_logro);

                    var existe = Convert.ToInt32(await checkCommand.ExecuteScalarAsync()) > 0;

                    if (existe)
                        return Ok(new { mensaje = "El usuario ya tiene este logro." });

                    // Insertar el logro
                    var insertCommand = new MySqlCommand(
                        "INSERT INTO logros_usuario (id_usuario, id_logro) VALUES (@idUsuario, @idLogro)",
                        connection);
                    insertCommand.Parameters.AddWithValue("@idUsuario", request.id_usuario);
                    insertCommand.Parameters.AddWithValue("@idLogro", request.id_logro);

                    await insertCommand.ExecuteNonQueryAsync();
                }

                return Ok(new { mensaje = "Logro registrado exitosamente." });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error en el servidor", detalle = ex.Message });
            }
        
        }

        [HttpGet("{id_usuario}")]
        public async Task<IActionResult> ObtenerLogrosDeUsuario(int id_usuario)
        {
            if (id_usuario <= 0)
                return BadRequest(new { mensaje = "ID de usuario inválido." });

            var logros = new List<LogroObtenidoDTO>();

            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    var cmd = new MySqlCommand(@"
                        SELECT l.id_logro, l.nombre, l.descripcion, l.minijuego, l.experiencia, lu.fecha_obtenido
                        FROM logros_usuario lu
                        JOIN logros l ON lu.id_logro = l.id_logro
                        WHERE lu.id_usuario = @idUsuario", connection);

                    cmd.Parameters.AddWithValue("@idUsuario", id_usuario);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            logros.Add(new LogroObtenidoDTO
                            {
                                id_logro = Convert.ToInt32(reader["id_logro"]),
                                nombre = reader["nombre"].ToString(),
                                descripcion = reader["descripcion"].ToString(),
                                minijuego = reader["minijuego"].ToString(),
                                experiencia = Convert.ToInt32(reader["experiencia"]),
                                fecha_obtenido = Convert.ToDateTime(reader["fecha_obtenido"])
                            });
                        }
                    }
                }

                return Ok(logros);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error en el servidor", detalle = ex.Message });
            }
        }
    }
}