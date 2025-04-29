using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Collections.Generic;
using ApiReto.Models; // (Este namespace es donde estará tu IncidenciaRequest.cs)

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

    using (var conexion = new MySqlConnection(connectionString))
    {
        conexion.Open();

        var query = @"
            INSERT INTO Incidencias (id_asesor, id_oxxo, tipo_incidencia, descripcion)
            VALUES (@IdAsesor, @IdOxxo, @TipoIncidencia, @Descripcion);
        ";

        using (var command = new MySqlCommand(query, conexion))
        {
            command.Parameters.AddWithValue("@IdAsesor", request.Id_Asesor);
            command.Parameters.AddWithValue("@IdOxxo", request.Id_Oxxo);
            command.Parameters.AddWithValue("@TipoIncidencia", request.Tipo_Incidencia);
            command.Parameters.AddWithValue("@Descripcion", request.Descripcion);

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




    }
    
}
