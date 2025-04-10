using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System.Data;

namespace ApiReto.Controllers;

[ApiController]
[Route("[controller]")]
public class LoginController : ControllerBase
{
     string connectionString = "Server=mysql-93cf659-tamtok2-09a8.b.aivencloud.com;Port=23481;Database=BDOxxo;Uid=avnadmin;Password=AVNS_-9SXvTjsy8x6dg2kaJR";

     // GET: /espacio/login?nickname=juanito&password=12345
  [HttpGet("login")]
    public IActionResult Login([FromQuery] string nickname, [FromQuery] string password)
    {
        using var connection = new MySqlConnection(connectionString);
        connection.Open();

        string query = "SELECT id_usuario FROM usuarios WHERE nickname = @nickname AND contrasena = @password";
        using var command = new MySqlCommand(query, connection);
        command.Parameters.AddWithValue("@nickname", nickname);
        command.Parameters.AddWithValue("@password", password);

        using var reader = command.ExecuteReader();

        if (reader.Read())
        {
            int idUsuario = reader.GetInt32("id_usuario");

            return Ok(new
            {
                acceso = true,
                id_usuario = idUsuario
            });
        }
        else
        {
            return Unauthorized(new { acceso = false });
        }
    }

}
