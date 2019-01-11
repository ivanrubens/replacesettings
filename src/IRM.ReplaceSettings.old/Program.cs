using IRM.ReplaceSettings.Services;
using System;
using System.IO;

namespace IRM.ReplaceSettings
{
    class Program
    {
        public string NomeArquivoComCaminho { get; set; }

        static void Main(string[] args)
        {
            if (!ValidarParametros(args))
            {
                throw new Exception("Parâmetro(s) inválido(s)!");
            }

            SubstituicoesService arquivoService = new SubstituicoesService(args[0]);
            arquivoService.ProcessarArquivos();

        }

        private static bool ValidarParametros(string[] args)
        {
            if (args.Length == 0)
            {
                return false;
            }

            if (!File.Exists(args[0]))
            {
                throw new Exception(string.Format("Arquivo '{0}' inexsitente ou sem permissão!", 0));
            }

            return true;
        }

        
    }


}
