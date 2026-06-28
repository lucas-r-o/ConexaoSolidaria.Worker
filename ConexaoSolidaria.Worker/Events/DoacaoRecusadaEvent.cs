namespace ConexaoSolidaria.Events
{
    public class DoacaoRecusadaEvent
    {
        public Guid CampanhaId { get; init; }
        public Guid DoadorId { get; init; }
        public string Motivo { get; init; } = string.Empty;
        public DateTimeOffset OcorridoEm { get; init; }
    }
}
