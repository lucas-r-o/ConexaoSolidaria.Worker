namespace ConexaoSolidaria.Events
{
    public record class ArrecadacaoAtualizadaEvent(Guid CampanhaId, decimal TotalArrecadado, DateTime PublicadoEm);
}
