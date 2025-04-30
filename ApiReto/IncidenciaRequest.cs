namespace ApiReto.Models
{
    public class IncidenciaRequest
    {
        public int Id_Asesor { get; set; }
        public int Id_Oxxo { get; set; }    
        public string Tipo_Incidencia { get; set; }
        public string Descripcion { get; set; }

        // NUEVO: para recibir imagen codificada en base64
        public string Foto_Base64 { get; set; }
    }
}