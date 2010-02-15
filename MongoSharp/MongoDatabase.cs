﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoSharp.Protocol.Messages;
using MongoSharp.Protocol.SystemMessages.Requests;
using MongoSharp.Protocol.SystemMessages.Responses;

namespace MongoSharp
{
    public class MongoDatabase
    {
        private String _dbName;
        private MongoServer _server;
        /// <summary>
        /// A reference to the database found using the specified context.
        /// </summary>
        /// <param name="dbname"></param>
        /// <param name="context"></param>
        public MongoDatabase(String dbname, MongoServer server)
        {
            this._dbName = dbname;
            this._server = server;
            this.SizeOnDisk = 0.0;
        }

        public Double SizeOnDisk { get; set; }

        /// <summary>
        /// The database name for this database.
        /// </summary>
        public String DatabaseName
        {
            get
            {
                return this._dbName;
            }
        }


        /// <summary>
        /// Removes the specified collection from the database.
        /// </summary>
        /// <param name="collectionName"></param>
        /// <returns></returns>
        public bool DropCollection(String collectionName)
        {
            var retval = false;
            var cmd = this.GetCollection<GenericCommandResponse>("$cmd");
            var result = cmd.FindOne(new DropCollectionRequest(collectionName));
            
            if (result != null && result.OK == 1.0)
            {
                retval = true;
            }
            return retval;
        }

        /// <summary>
        /// Produces a mongodb collection that will produce and
        /// manipulate objects of the specified type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collectionName"></param>
        /// <returns></returns>
        public MongoCollection<T> GetCollection<T>(string collectionName) where T : class, new()
        {
            return new MongoCollection<T>(collectionName, this, this._server);
        }


        /// <summary>
        /// Produces a list of all collections currently in this database.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<CollectionInfo> GetAllCollections()
        {
            var results = this.GetCollection<CollectionInfo>("system.namespaces").Find();                

            return results;
        }

        public CollectionStatistics GetCollectionStatistics(string collectionName)
        {
            var response = this._server.GetDatabase(this.DatabaseName)
                .GetCollection<CollectionStatistics>("$cmd")
                .FindOne<CollectionStatistics>(new CollectionStatistics() { collstats = collectionName });

            return response;
        }


    }
}
