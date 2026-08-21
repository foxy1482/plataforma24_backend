namespace Plataforma24API.DTOs.Tareas
{
    public class TareaDTO
    {
        public int Numero { get; set; }

        public string Titulo { get; set; }

        public string Descripcion { get; set; }

        public DateTime FechaEntrega { get; set; }

        public DateTime PublicadaDesde { get; set; }

        public int NumMateria { get; set; }
    }
}
