using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace Sample.Fluxo.Caixa.API.Tests.Config
{
    public static class HelpersTest
    {
        public async static Task<string> BaixarArquivo(HttpResponseMessage httpReponseMessage, string arquivo)
        {
            var fileInfo = new FileInfo(arquivo);
            using var ms = await httpReponseMessage.Content.ReadAsStreamAsync();
            using var fs = File.Create(fileInfo.FullName);
            ms.Seek(0, SeekOrigin.Begin);
            ms.CopyTo(fs);
            return arquivo;
        }
    }
}
