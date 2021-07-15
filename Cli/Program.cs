using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
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
                .AddSingleton<Collections>()
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
                        {
                            Host = Environment.GetEnvironmentVariable("TYPESENSE_HOST") ?? "localhost",
                            Port = Environment.GetEnvironmentVariable("TYPESENSE_PORT") ?? "8103",
                            Protocol = Environment.GetEnvironmentVariable("TYPESENSE_PROTOCOL") ?? "http",
                        }
                    };
        }

        private static void CollectionCommands(IServiceProvider serviceProvider, RootCommand rootCommand)
        {
            var collectionCmd = new Command("collection");
            collectionCmd.Description = "Perform operations on collections";

            var createCmd = new Command("create");
            collectionCmd.Add(createCmd);
            createCmd.Add(new Argument<FileInfo>("schemaFile", "Schema definition JSON file"));
            createCmd.Handler = CommandHandler.Create<FileInfo>(serviceProvider.GetService<Collections>().CreateCollection);

            rootCommand.Add(collectionCmd);
        }
    }
}
