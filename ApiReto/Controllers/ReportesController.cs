using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Collections.Generic;
using System.IO;
using ApiReto.Models;

namespace ApiReto.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class IncidenciaController : ControllerBase
    {
        string connectionString = "Server=mysql-93cf659-tamtok2-09a8.b.aivencloud.com;Port=23481;Database=BDOxxo;Uid=avnadmin;Password=AVNS_-9SXvTjsy8x6dg2kaJR";

        [HttpPost("crear")]
        public IActionResult CrearIncidencia([FromBody] IncidenciaRequest request)
        {
            if (request == null)
                return BadRequest("Datos inválidos.");

            string rutaRelativaImagen = null;

            // Procesar imagen si existe
            if (!string.IsNullOrEmpty(request.Foto_Base64))
            {
                try
                {
                    byte[] imagenBytes = Convert.FromBase64String(request.Foto_Base64);
                    string nombreArchivo = $"incidencia_{DateTime.UtcNow.Ticks}.jpg";
                    rutaRelativaImagen = Path.Combine("imagenes", nombreArchivo);
                    string rutaAbsoluta = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", rutaRelativaImagen);

                    // Crear carpeta si no existe
                    Directory.CreateDirectory(Path.GetDirectoryName(rutaAbsoluta));

                    System.IO.File.WriteAllBytes(rutaAbsoluta, imagenBytes);
                }
                catch (Exception ex)
                {
                    return BadRequest(new { error = "Error procesando la imagen.", detalle = ex.Message });
                }
            }

            using (var conexion = new MySqlConnection(connectionString))
            {
                conexion.Open();

                var query = @"
                    INSERT INTO Incidencias (id_asesor, id_oxxo, tipo_incidencia, descripcion, foto_base64)
                    VALUES (@IdAsesor, @IdOxxo, @TipoIncidencia, @Descripcion, @RutaFoto);
                ";

                using (var command = new MySqlCommand(query, conexion))
                {
                    command.Parameters.AddWithValue("@IdAsesor", request.Id_Asesor);
                    command.Parameters.AddWithValue("@IdOxxo", request.Id_Oxxo);
                    command.Parameters.AddWithValue("@TipoIncidencia", request.Tipo_Incidencia);
                    command.Parameters.AddWithValue("@Descripcion", request.Descripcion);
                    command.Parameters.AddWithValue("@RutaFoto", rutaRelativaImagen);

                    try
                    {
                        command.ExecuteNonQuery();
                        return Ok(new { message = "Incidencia creada correctamente." });
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error en CrearIncidencia: {ex.Message}");
                        return StatusCode(500, new { error = "Error interno al crear la incidencia.", details = ex.Message });
                    }
                }
            }
        }

        [HttpGet("tiendas/{idAsesor}")]
        public IActionResult GetTiendasPorAsesor(int idAsesor)
        {
            List<object> tiendas = new List<object>();

            using (var conexion = new MySqlConnection(connectionString))
            {
                conexion.Open();

                string query = @"
                    SELECT id_oxxo, nombre_tienda
                    FROM oxxos
                    WHERE id_asesor = @IdAsesor;
                ";

                using (var cmd = new MySqlCommand(query, conexion))
                {
                    cmd.Parameters.AddWithValue("@IdAsesor", idAsesor);
                    cmd.Prepare();

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            tiendas.Add(new
                            {
                                id_oxxo = reader.GetInt32("id_oxxo"),
                                nombre_tienda = reader["nombre_tienda"].ToString()
                            });
                        }
                    }
                }

                conexion.Close();
            }

            return Ok(tiendas);
        }

[HttpGet("listar/asesor/{idAsesor}")]
public IActionResult ListarIncidenciasPorAsesor(int idAsesor)
{
    List<object> incidencias = new List<object>();

    using (var conexion = new MySqlConnection(connectionString))
    {
        conexion.Open();

        string query = @"
            SELECT id_incidencia, id_asesor, id_oxxo, tipo_incidencia, descripcion, foto_base64
            FROM Incidencias
            WHERE id_asesor = @IdAsesor;
        ";

        using (var cmd = new MySqlCommand(query, conexion))
        {
            cmd.Parameters.AddWithValue("@IdAsesor", idAsesor);

            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    string rutaImagen = reader["foto_base64"] != DBNull.Value
                        ? $"{Request.Scheme}://{Request.Host}/" + reader["foto_base64"].ToString().Replace("\\", "/")
                        : null;

                    incidencias.Add(new
                    {
                        id_incidencia = reader.GetInt32("id_incidencia"),
                        id_asesor = reader.GetInt32("id_asesor"),
                        id_oxxo = reader.GetInt32("id_oxxo"),
                        tipo_incidencia = reader["tipo_incidencia"].ToString(),
                        descripcion = reader["descripcion"].ToString(),
                        foto_url = rutaImagen
                    });
                }
            }
        }
    }

    return Ok(incidencias);
}

}

    }

