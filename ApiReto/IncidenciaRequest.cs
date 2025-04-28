namespace ApiReto.Models
{
    public class IncidenciaRequest
    {
        public int Id_Asesor { get; set; }
        public int Id_Oxxo { get; set; }    // Nuevo campo para la tienda
        public string Tipo_Incidencia { get; set; }
        public string Descripcion { get; set; }
    }
}
