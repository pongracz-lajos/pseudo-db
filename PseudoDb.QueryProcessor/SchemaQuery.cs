﻿using PseudoDb.Interfaces.Metadata;
using PseudoDb.Interfaces.Query;
using PseudoDb.Interfaces.Engine;
using System.Collections.Generic;

namespace PseudoDb.QueryProcessor
{
    public class SchemaQuery : ISchemaQuery
    {
        private IMetadata metadata;

        public SchemaQuery(IMetadata metadata)
        {
            this.metadata = metadata;
        }

        public ICollection<Database> GetDatabases()
        {
            return metadata.GetDatabases();
        }

        public void AddDatabase(string databaseName)
        {
            metadata.AddDatabase(databaseName);
        }

        public Database GetDatabase(string databaseName)
        {
            return metadata.GetDatabase(databaseName);
        }

        public void UpdateDatabase(string databaseName)
        {
            metadata.UpdateDatabase(databaseName);
        }

        public void RemoveDatabase(string databaseName)
        {
            metadata.RemoveDatabase(databaseName);
        }
    }
}
