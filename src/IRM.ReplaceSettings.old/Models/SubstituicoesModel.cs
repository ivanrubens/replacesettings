namespace IRM.ReplaceSettings.Models
{

    public class SubstituicoesModel
    {
        public Arquivo[] Arquivos { get; set; }
    }

    public class Arquivo
    {
        public string NomeArquivo { get; set; }
        public Substituicao[] Substituicoes { get; set; }
    }

    public class Substituicao
    {
        public string De { get; set; }
        public string Para { get; set; }
    }

}

