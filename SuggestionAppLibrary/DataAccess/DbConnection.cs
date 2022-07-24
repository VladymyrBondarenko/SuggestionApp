﻿using Microsoft.Extensions.Configuration;

namespace SuggestionAppLibrary.DataAccess
{
    public class DbConnection : IDbConnection
    {
        private readonly IConfiguration _config;

        private readonly IMongoDatabase _db;

        private string _connectionId = "MongoDb";

        public string DbName { get; private set; }

        public string CategoryCollectionName { get; private set; } = "categories";

        public string StatusCollectionName { get; private set; } = "statuses";

        public string UserCollectionName { get; private set; } = "users";

        public string SuggestionCollectionName { get; private set; } = "suggestions";

        public MongoClient MongoClient { get; private set; }

        public IMongoCollection<CategoryModel> CategoryCollection { get; private set; }

        public IMongoCollection<StatusModel> StatusCollection { get; private set; }

        public IMongoCollection<SuggestionModel> SuggestionCollection { get; private set; }

        public IMongoCollection<UserModel> UserCollection { get; private set; }

        public DbConnection(IConfiguration configuration)
        {
            _config = configuration;
            MongoClient = new MongoClient(_config.GetConnectionString(_connectionId));
            DbName = _config["DatabaseName"];
            _db = MongoClient.GetDatabase(DbName);

            CategoryCollection = _db.GetCollection<CategoryModel>(CategoryCollectionName);
            StatusCollection = _db.GetCollection<StatusModel>(StatusCollectionName);
            SuggestionCollection = _db.GetCollection<SuggestionModel>(SuggestionCollectionName);
            UserCollection = _db.GetCollection<UserModel>(UserCollectionName);
        }
    }
}