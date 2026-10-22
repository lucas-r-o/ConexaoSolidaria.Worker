namespace ConexaoSolidaria.Worker.Models
{
    public class ArrecadacaoCampanha
    {
        public Guid Id { get; set; }
        public Guid CampanhaId { get; set; }
        public decimal TotalArrecadado { get; private set; }
        public decimal MetaFinanceira { get; set; }
        public decimal UltimoTotalPublicado { get; private set; }
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
        public Status Status { get; set; } = Status.Ativo;
        public bool Ativo => Status == Status.Ativo;
        public bool MetaConquistada => TotalArrecadado >= MetaFinanceira;
        public bool HouveMudanca => TotalArrecadado != UltimoTotalPublicado;

        public void UpdateArrecadacao(decimal valor) => TotalArrecadado += valor;
        public void UpdateUltimoTotal() => UltimoTotalPublicado = TotalArrecadado;
        public void Finish() => Status = Status.Concluido;
    }
}
