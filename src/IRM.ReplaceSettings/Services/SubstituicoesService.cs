﻿using IRM.ReplaceSettings.Models;
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
                string text;
                string arquivoSubstituido;

                if (arquivo.NomeArquivo.Contains("*"))
                {
                    string diretorio = arquivo.NomeArquivo.Substring(0, arquivo.NomeArquivo.LastIndexOf("/"));
                    string coringa = arquivo.NomeArquivo.Substring(arquivo.NomeArquivo.LastIndexOf("/")+1);
                    var fileList = new DirectoryInfo(diretorio).GetFiles(coringa, SearchOption.TopDirectoryOnly);

                    arquivoSubstituido = fileList[0].FullName;
                }
                else
                {
                    arquivoSubstituido = arquivo.NomeArquivo;
                }

                text = File.ReadAllText(arquivoSubstituido);

                foreach (var substituicao in arquivo.Substituicoes)
                {
                    text = text.Replace(substituicao.De, substituicao.Para);
                    File.WriteAllText(arquivoSubstituido, text);
                }

            }

        }
    }
}
