using IRM.ReplaceSettings.Models;
using Newtonsoft.Json.Linq;
using System.IO;

namespace IRM.ReplaceSettings.Services
{
    public class SubstituicoesService
    {
        protected string _nomeArquivoComCaminho;
        public SubstituicoesModel SubstituicoesModel { get; set; }

        public SubstituicoesService(string nomeArquivoComCaminho)
        {
            _nomeArquivoComCaminho = nomeArquivoComCaminho;

            SubstituicoesModel = AbrirArquivo();
        }

        private SubstituicoesModel AbrirArquivo()
        {
            SubstituicoesModel substituicoesModel;
            string jsonTexto = File.ReadAllText(@_nomeArquivoComCaminho);

            if (!ValidarJson(jsonTexto))
            {
                return null;
            }

            substituicoesModel = Newtonsoft.Json.JsonConvert.DeserializeObject<SubstituicoesModel>(jsonTexto);

            return substituicoesModel;

        }

        private bool ValidarJson(string jsonTexto)
        {
            try
            {
                var obj = JToken.Parse(jsonTexto);
            }

            catch (System.Exception ex) 
            {
                throw ex;
            }


            return true;
        }
        public void ProcessarArquivos()
        {
            if(this.SubstituicoesModel== null)
            {
                throw new System.Exception("SubstituicoesModel inválida!");
            }

            foreach (var arquivo in this.SubstituicoesModel.Arquivos)
            {
                string text = File.ReadAllText(arquivo.NomeArquivo);

                foreach(var substituicao in arquivo.Substituicoes)
                {
                    text = text.ToLower().Replace(substituicao.De.ToLower(), substituicao.Para);
                    File.WriteAllText(arquivo.NomeArquivo, text);
                }

            }

        }
    }
}
