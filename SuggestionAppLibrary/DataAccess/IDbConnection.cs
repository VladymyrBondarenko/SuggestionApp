using MongoDB.Driver;

namespace SuggestionAppLibrary.DataAccess
{
    public interface IDbConnection
    {
        IMongoCollection<CategoryModel> CategoryCollection { get; }
        string CategoryCollectionName { get; }
        string DbName { get; }
        MongoClient MongoClient { get; }
        IMongoCollection<StatusModel> StatusCollection { get; }
        string StatusCollectionName { get; }
        IMongoCollection<SuggestionModel> SuggestionCollection { get; }
        string SuggestionCollectionName { get; }
        IMongoCollection<UserModel> UserCollection { get; }
        string UserCollectionName { get; }
    }
}