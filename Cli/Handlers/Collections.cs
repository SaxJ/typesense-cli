using System;
using System.IO;
using Typesense;
using System.Text.Json;
using System.Threading.Tasks;

namespace Cli.Handlers
{
    class Collections
    {
        private readonly ITypesenseClient TypesenseClient;

        public Collections(ITypesenseClient client)
        {
            TypesenseClient = client;
        }


        public async Task CreateCollection(FileInfo schemaFile)
        {
            if (schemaFile.Exists)
            {
                try
                {
                    using (var reader = new StreamReader(schemaFile.OpenRead()))
                    {
                        string json = reader.ReadToEnd();
                        var schema = JsonSerializer.Deserialize<Schema>(json);

                        await TypesenseClient.CreateCollection(schema);
                    }
                }
                catch (Exception error)
                {
                    Console.WriteLine(error.Message);
                }
            }
            else
            {
                Console.WriteLine("File does not exist");
            }
        }
    }
}
