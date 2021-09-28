using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Text;

namespace event_sourcing_demo.presistence.azure
{
    public class DbContainerProvider : IDbContainerProvider
    {
        private readonly Database _db;

        public DbContainerProvider(Database db)
        {
            _db = db;
        }

        public Microsoft.Azure.Cosmos.Container GetContainer(string containerName) => _db.GetContainer(containerName);
    }
}
