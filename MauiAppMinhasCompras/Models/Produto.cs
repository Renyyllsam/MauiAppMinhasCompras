using SQLite;

namespace MauiAppMinhasCompras.Models
{
    public class Produto
    {
        private string _descricao = string.Empty; // Corrigido: inicialização para evitar CS8618

        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string Descricao // Corrigido: propriedade única, removido duplicidade
        {
            get => _descricao;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new Exception("Por favor, preencha a descrição");
                }
                _descricao = value;
            }
        }

        public double Quantidade { get; set; }
        public double Preco { get; set; }
        public double Total { get => Quantidade * Preco; }
    }
}