using System;
using System.Collections.Generic;
using System.CommandLine;
using System.IO;
using System.Threading.Tasks;
using Cli.Handlers;
using Microsoft.Extensions.DependencyInjection;
using Typesense.Setup;

namespace Cli
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var serviceProvider = new ServiceCollection()
                .AddTypesenseClient(ClientConfig)
                .AddSingleton<CollectionHandlers>()
                .BuildServiceProvider();

            var rootCmd = new RootCommand("Interact with a Typesense server");
            CollectionCommands(serviceProvider, rootCmd);

            await rootCmd.InvokeAsync(args);
        }

        private static void ClientConfig(Config config)
        {
            config.ApiKey = Environment.GetEnvironmentVariable("TYPESENSE_API_KEY") ?? "foo";
            config.Nodes = new List<Node>
                    {
                        new Node
                        (
                            Environment.GetEnvironmentVariable("TYPESENSE_HOST") ?? "localhost",
                            Environment.GetEnvironmentVariable("TYPESENSE_PORT") ?? "443",
                            Environment.GetEnvironmentVariable("TYPESENSE_PROTOCOL") ?? "https"
                        )
                    };
        }

        private static void CollectionCommands(IServiceProvider serviceProvider, RootCommand rootCommand)
        {
            var collectionHandlers = serviceProvider.GetService<CollectionHandlers>();

            var collectionCmd = new Command("collection");
            collectionCmd.Description = "Perform operations on collections";

            var createCmd = new Command("create");
            collectionCmd.Add(createCmd);
            var schemaArg = new Argument<FileInfo>("schemaFile", "Schema definition JSON file");
            createCmd.Add(schemaArg);
            createCmd.SetHandler(collectionHandlers.CreateCollection, schemaArg);

            var detailsCmd = new Command("details");
            collectionCmd.Add(detailsCmd);
            var nameArg = new Argument<string>("name", "The name of the collection to retreive");
            detailsCmd.Add(nameArg);
            detailsCmd.SetHandler(collectionHandlers.RetrieveCollection, nameArg);

            var listCmd = new Command("list");
            collectionCmd.Add(listCmd);
            listCmd.SetHandler(collectionHandlers.ListCollections);

            var deleteCmd = new Command("drop");
            collectionCmd.Add(deleteCmd);
            var dropArg = new Argument<string>("name", "The name of the collection to drop");
            deleteCmd.Add(dropArg);
            deleteCmd.SetHandler(collectionHandlers.DeleteCollection, dropArg);

            rootCommand.Add(collectionCmd);
        }
    }
}
