
namespace SuggestionAppLibrary.DataAccess
{
    public class MongoUserData : IUserData
    {
        private readonly IMongoCollection<UserModel> _users;

        public MongoUserData(IDbConnection dbConnection)
        {
            _users = dbConnection.UserCollection;
        }

        public async Task<List<UserModel>> GetUsersAsync()
        {
            var result = await _users.FindAsync(_ => true);
            return result.ToList();
        }

        public async Task<UserModel> GetUserByIdAsync(string id)
        {
            var result = await _users.FindAsync(u => u.Id == id);
            return result.FirstOrDefault();
        }

        public async Task<UserModel> GetUserByObjectIdentifierAsync(string objectIdentifier)
        {
            var result = await _users.FindAsync(u => u.ObjectIdentifier == objectIdentifier);
            return result.FirstOrDefault();
        }

        public Task CreateUser(UserModel user)
        {
            return _users.InsertOneAsync(user);
        }

        public Task UpdateUser(UserModel user)
        {
            return _users.ReplaceOneAsync(i => i.Id == user.Id, user, new ReplaceOptions { IsUpsert = true });
        }
    }
}