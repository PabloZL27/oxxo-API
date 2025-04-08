namespace ApiReto;

public class Caso
{
    public int Id_Caso { get; set; }
    public string Titulo { get; set; }
    public string Descripcion { get; set; }
    public int Orden_Sprite { get; set; }

    public ICollection<Opcion> Opciones { get; set; }
}

