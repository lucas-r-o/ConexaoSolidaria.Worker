namespace ConexaoSolidaria.Worker.Models
{
    public class ArrecadacaoCampanha
    {
        public Guid Id { get; set; }
        public Guid CampanhaId { get; set; }
        public decimal TotalArrecadado { get; private set; }
        public DateTime CreatedAt { get; set; } = DateTime.Today;
        public DateTime UpdatedAt { get; set; } = DateTime.Today;
        public bool Ativo { get; set; } = true;

        public void UpdateArrecadacao(decimal valor) => TotalArrecadado += valor;
    }
}
