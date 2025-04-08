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

}
