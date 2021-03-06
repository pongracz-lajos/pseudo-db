﻿using PseudoDb.Interfaces.Metadata;
using PseudoDb.Interfaces.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PseudoDb.Interfaces.Indexing
{
    public class NonUniqueIndex : IConcreteIndex
    {
        private string databaseFileName;

        private IRepository repository;

        private Index index;

        public NonUniqueIndex(string databaseFileName, IRepository repository, Index index)
        {
            this.databaseFileName = databaseFileName;
            this.repository = repository;
            this.index = index;
        }

        public bool Exists(string key)
        {
            return repository.Exists(databaseFileName, index.Name, key);
        }

        public IEnumerable<string> Get(string key)
        {
            return repository.Get(databaseFileName, index.Name, key).Split(new string[] { "###" }, StringSplitOptions.RemoveEmptyEntries);
        }

        public void Put(string key, string value)
        {
            if (Exists(key))
            {
                var values = Get(key);
                Delete(key);

                var newValues = new List<string>(values);
                newValues.Add(value);

                repository.Put(databaseFileName, index.Name, key, string.Join("###", newValues));
            }
            else
            {
                repository.Put(databaseFileName, index.Name, key, value);
            }
        }

        public void Delete(string key)
        {
            repository.Delete(databaseFileName, index.Name, key);
        }


        public void Delete(string key, string subKey)
        {
            var values = Get(key);
            var newValues = values.ToList();
            newValues.Remove(subKey);

            if (newValues.Count > 0)
            {
                Delete(key);
                repository.Put(databaseFileName, index.Name, key, string.Join("###", newValues));
            }
            else
            {
                repository.Delete(databaseFileName, index.Name, key);
            }
        }

        public IEnumerable<KeyValuePair<string, string>> GetAll()
        {
            return repository.GetAll(databaseFileName, index.Name);
        }
    }
}
