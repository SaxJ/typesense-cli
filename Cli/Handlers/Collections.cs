using System;
using System.IO;
using Typesense;

namespace Cli.Handlers
{
    class Collections
    {
        private readonly ITypesenseClient TypesenseClient;

        public Collections(ITypesenseClient client)
        {
            TypesenseClient = client;
        }


        public void CreateCollection(FileInfo schemaFile)
        {
            if (schemaFile.Exists)
            {

            }
            else
            {
                Console.WriteLine("File does not exist");
            }
        }
    }
}
