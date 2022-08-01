using IRM.ReplaceSettings.Services;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.IO;
using Xunit;

namespace IRM.ReplaceSettingsTests
{
    public class ReplaceSettingsTests
    {
        [Fact]
        public void ErroModelInvalida()
        {
            // Arrange
            var arquivoJsonMapeamento = Path.Combine(AppContext.BaseDirectory, "filesTeste\\fake_json.json");

            // Act
            var subsituicoesService = new SubstituicoesService(arquivoJsonMapeamento);

            // Assert
            Assert.Equal(subsituicoesService.SubstituicoesModel?.Arquivos, null);
        }

        [Fact]
        public void Substituir_Arquivo_SemCoringa_Asterisco()
        {
            // Arrange
            var arquivoJsonMapeamento = Path.Combine(AppContext.BaseDirectory, "filesTeste\\teste_sem_asterisco.json");
            var pagina = Path.Combine(AppContext.BaseDirectory, "filesTeste\\index_sem_asterisco.html");

            ChromeOptions options = new ChromeOptions();
            options.AddArguments("--headless");
            options.LeaveBrowserRunning = false;
            IWebDriver client = new ChromeDriver("C:\\dev\\SeleniumWebDriver\\chrome", options);

            // Act
            var subsituicoesService = new SubstituicoesService(arquivoJsonMapeamento);
            subsituicoesService.ProcessarArquivos();

            client.Navigate().GoToUrl(pagina);

            var headBase = client.FindElement(By.XPath("/html/head/base"));
            var hrefHeadBase = headBase.GetAttribute("href");
            var bodyAppRoot = client.FindElement(By.XPath("/html/body/app-root/p"));

            var script1 = client.FindElement(By.Id("script1")).GetAttribute("src");
            var script2 = client.FindElement(By.Id("script2")).GetAttribute("src");
            var script3 = client.FindElement(By.Id("script3")).GetAttribute("src");

            // Assert
            Assert.Equal("Pagina ReplaceSettingsTests", client.Title);
            Assert.Equal("URL: http://url-teste.replace.local:882/body", bodyAppRoot.Text);
            Assert.Equal("http://url-teste.replace.local:88/base", hrefHeadBase);
            Assert.Equal("http://url-teste.replace.local:881/js_bundle", script1);
            Assert.Equal("http://url-teste.replace.local:881/js_bundle", script2);
            Assert.Equal("http://url-teste.replace.local:881/js_bundle", script3);
        }

        [Fact]
        public void Substituir_Arquivos_ES_ComCoringa_Asterisco()
        {
            // Arrange
            var arquivoJsonMapeamento = Path.Combine(AppContext.BaseDirectory, "filesTeste\\teste_com_asterisco.json");
            var scriptFile = Path.Combine(AppContext.BaseDirectory, "filesTeste\\main-es2015.d0985647656532d18cb6.js");

            // Act
            var subsituicoesService = new SubstituicoesService(arquivoJsonMapeamento);
            subsituicoesService.ProcessarArquivos();

            var text = File.ReadAllText(scriptFile);

            bool containsSearchResult1 = text.Contains("https://url_api.teste.replace.local:88/base/url_api");
            bool containsSearchResult2 = text.Contains("https://back_api.teste.replace.local:89");
            bool containsSearchResult3 = text.Contains("new_mapa_back_api.teste.replace.local:91");
            bool containsSearchResult4 = text.Contains("new_auth_back_api.teste.replace.local:101");
            bool containsSearchResult5 = text.Contains("new_mapa_back_api.teste.replace.local:81");

            // Assert
            Assert.True(containsSearchResult1);
            Assert.True(containsSearchResult2);
            Assert.True(containsSearchResult3);
            Assert.True(containsSearchResult4);
            Assert.True(containsSearchResult5);
        }
    }
}