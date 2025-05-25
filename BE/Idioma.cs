namespace BE
{
    public class Idioma
    {
        public int IdiomaId { get; set; }
        public string Cultura { get; set; }
        public string Descripcion { get; set; }

        public override string ToString()
        {
            return Descripcion;
        }
    }
}