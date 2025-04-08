namespace ApiReto;

public class TipoOpcion
{
    public int Id_Tipo_Opcion { get; set; }
    public string Descripcion { get; set; }

    public ICollection<Opcion> Opciones { get; set; }
}
