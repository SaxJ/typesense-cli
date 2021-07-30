using System;
using System.IO;
using Typesense;
using System.Text.Json;
using System.Threading.Tasks;

namespace Cli.Handlers
{
    class DocumentHandlers
    {
        private readonly ITypesenseClient TypesenseClient;

        public DocumentHandlers(ITypesenseClient client)
        {
            TypesenseClient = client;
        }

        public async Task UpsertDocument<T>(string collection, FileInfo document)
        {
            if (document.Exists)
            {
                try
                {
                    using (var reader = new StreamReader(document.OpenRead()))
                    {
                        string json = reader.ReadToEnd();
                        T parsedDocument = JsonSerializer.Deserialize<T>(json);

                        var response = await TypesenseClient.UpsertDocument<T>(collection, parsedDocument);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            else
            {
                Console.WriteLine("File does not exist");
            }
        }

        public async Task GetDocument<T>(string collection, string id)
        {
            try
            {
                var response = await TypesenseClient.RetrieveDocument<T>(collection, id);
                string json = JsonSerializer.Serialize<T>(response, new JsonSerializerOptions() { WriteIndented = true });

                Console.WriteLine(json);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
