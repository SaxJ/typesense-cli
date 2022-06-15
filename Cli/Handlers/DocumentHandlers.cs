using System;
using System.IO;
using Typesense;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Cli.Handlers
{
    class DocumentHandlers
    {
        private readonly ITypesenseClient TypesenseClient;

        public DocumentHandlers(ITypesenseClient client)
        {
            TypesenseClient = client;
        }

        public async Task UpsertDocument<T>(string collection, FileInfo document) where T : class
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

        public async Task GetDocument<T>(string collection, string id) where T : class
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

        public async Task ImportDocuments<T>(string collection, FileInfo jsonlFile, int batchSize = 40) where T : class
        {
            if (jsonlFile.Exists)
            {
                try
                {
                    var lines = File.ReadLines(jsonlFile.FullName);

                    List<Task> tasks = new List<Task>();
                    List<T> docs = new List<T>();
                    foreach (var line in lines)
                    {
                        T doc = JsonSerializer.Deserialize<T>(line);
                        docs.Add(doc);

                        if (docs.Count >= batchSize)
                        {
                            tasks.Add(TypesenseClient.ImportDocuments<T>(collection, docs, batchSize));
                            docs = new List<T>();
                        }
                    }

                    await Task.WhenAll(tasks);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

            }
        }
    }
}
