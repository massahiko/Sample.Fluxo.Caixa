using CsvHelper;
using CsvHelper.Configuration;
using ServiceStack;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Sample.Fluxo.Caixa.API.Tests.Config
{
    public static class CsvHelperTest
    {
        public static IEnumerable<T> ConverterRelatorioParaObjeto<T>(string arquivo, bool primeiraLinhaCabecalho = true)
        {
            try
            {
                var csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    HasHeaderRecord = primeiraLinhaCabecalho,
                    HeaderValidated = null,
                    Delimiter = ",",
                    TrimOptions = TrimOptions.Trim,
                    MissingFieldFound = null
                };

                var streamReader = File.OpenText(arquivo);
                using (var csvReader = new CsvReader(streamReader, csvConfig))
                {
                    var data = csvReader.GetRecords<T>().ToList();
                    return data;
                }
            }
            catch (System.Exception)
            {
                return Enumerable.Empty<T>();
            }
        }
    }
}
