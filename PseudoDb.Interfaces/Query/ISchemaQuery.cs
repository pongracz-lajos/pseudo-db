﻿using PseudoDb.Interfaces.Engine;
using PseudoDb.Interfaces.Metadata;
using System.Collections.Generic;

namespace PseudoDb.Interfaces.Query
{
    public interface ISchemaQuery
    {
        ICollection<Database> GetDatabases();

        void AddDatabase(string databaseName);

        Database GetDatabase(string databaseName);

        void UpdateDatabase(string databaseName);

        void RemoveDatabase(string databaseName);
    }
}
