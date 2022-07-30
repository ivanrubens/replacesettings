using IRM.ReplaceSettings.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace IRM.ReplaceSettings.Services
{
    public class SubstituicoesService
    {
        protected string _nomeArquivoComCaminho;
        public SubstituicoesModel SubstituicoesModel { get; set; }

        public SubstituicoesService(string pathNomeArquivoMapeamento)
        {
            _nomeArquivoComCaminho = pathNomeArquivoMapeamento;
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

            substituicoesModel = JsonConvert.DeserializeObject<SubstituicoesModel>(jsonTexto);

            return substituicoesModel;
        }

        private bool ValidarJson(string jsonTexto)
        {
            JToken.Parse(jsonTexto);

            return true;
        }
        public void ProcessarArquivos()
        {
            try
            {
                if (this.SubstituicoesModel == null)
                {
                    throw new Exception("SubstituicoesModel inválida!");
                }

                foreach (var arquivo in this.SubstituicoesModel.Arquivos)
                {
                    if (arquivo.NomeArquivo.Contains("*"))
                    {

                        string diretorio = arquivo.NomeArquivo.Substring(0, ObterPosicaoDaUltimaBarra(arquivo.NomeArquivo));
                        string coringa = arquivo.NomeArquivo.Substring(ObterPosicaoDaUltimaBarra(arquivo.NomeArquivo) + 1);
                        var fileList = new DirectoryInfo(diretorio).GetFiles(coringa, SearchOption.TopDirectoryOnly);

                        if (fileList.Length == 0)
                        {
                            throw new Exception(string.Format("Arquivo com coringa '{0}' não encontrado!", coringa));
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
            catch (Exception ex)
            {
                throw;
            }
        }

        private int ObterPosicaoDaUltimaBarra(string nomeArquivo)
        {
            List<string> caracteres = new List<string>();
            caracteres.Add("/");
            caracteres.Add("//");
            caracteres.Add("\\");
            caracteres.Add("\\\\");

            foreach (var caractere in caracteres)
            {
                if (nomeArquivo.LastIndexOf(caractere) > 0)
                {
                    return nomeArquivo.LastIndexOf(caractere);
                }
            }

            return 0;
        }

        private void SubstituirVariaveisNoArquivo(Arquivo arquivo, string pathArquivoDestino)
        {
            string text;
            text = File.ReadAllText(pathArquivoDestino);

            foreach (var substituicao in arquivo.Substituicoes)
            {
                text = ReplacePlaceHolders(text, substituicao.De, substituicao.Para);
                File.WriteAllText(pathArquivoDestino, text);
                Console.WriteLine($"Arquivo: '{pathArquivoDestino}' | Substituição: '{substituicao.De}'");
            }
        }

        private string ReplacePlaceHolders(string texto, string substituicaoDe, string substituicaoPara)
        {
            Regex pattern = new Regex(substituicaoDe);
            string replacedText = pattern.Replace(texto, substituicaoPara);

            return replacedText;
        }
    }
}
