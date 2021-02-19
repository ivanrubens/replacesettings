using IRM.ReplaceSettings.Models;
using Newtonsoft.Json.Linq;
using System;
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
            if (this.SubstituicoesModel == null)
            {
                throw new System.Exception("SubstituicoesModel inválida!");
            }

            foreach (var arquivo in this.SubstituicoesModel.Arquivos)
            {
                if (arquivo.NomeArquivo.Contains("*"))
                {
                    string diretorio = arquivo.NomeArquivo.Substring(0, arquivo.NomeArquivo.LastIndexOf("/"));
                    string coringa = arquivo.NomeArquivo.Substring(arquivo.NomeArquivo.LastIndexOf("/") + 1);
                    var fileList = new DirectoryInfo(diretorio).GetFiles(coringa, SearchOption.TopDirectoryOnly);

                    if (fileList.Length == 0)
                    {
                        throw new System.Exception(string.Format("Arquivo com coringa '{0}' não encontrado!", coringa));
                    }

                    foreach (var file in fileList)
                    {
                        SubstituirVariaveisNoArquivo(arquivo, file.FullName);
                    }
                }
                else
                {
                    SubstituirVariaveisNoArquivo(arquivo, arquivo.NomeArquivo);
                }

            }

        }

        private void SubstituirVariaveisNoArquivo(Arquivo arquivo, string arquivoSubstituido)
        {
            string text;
            text = File.ReadAllText(arquivoSubstituido);

            foreach (var substituicao in arquivo.Substituicoes)
            {
                text = text.Replace(substituicao.De, substituicao.Para);
                File.WriteAllText(arquivoSubstituido, text);
                Console.WriteLine($"Arquivo: '{arquivoSubstituido}' | Substituição/De: '{substituicao.De}' | Substituição/Para: '{substituicao.Para}'");
            }
        }
    }
}
