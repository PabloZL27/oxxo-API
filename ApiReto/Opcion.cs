namespace ApiReto;
public class Opcion
{
    public int Id_Opcion { get; set; }
    public int Id_Caso { get; set; }
    public string Texto_Opcion { get; set; }
    public int Sprite_Resultado { get; set; }
    public int Id_Tipo_Opcion { get; set; }

    public Caso Caso { get; set; }
    public TipoOpcion TipoOpcion { get; set; }
}
