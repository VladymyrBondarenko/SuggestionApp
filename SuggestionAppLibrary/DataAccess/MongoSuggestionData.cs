using Microsoft.Extensions.Caching.Memory;

namespace SuggestionAppLibrary.DataAccess
{
    public class MongoSuggestionData : ISuggestionData
    {
        private readonly IMongoCollection<SuggestionModel> _suggestions;
        private readonly IUserData _userData;
        private readonly IDbConnection _dbConnection;
        private readonly IMemoryCache _cache;
        private const string cacheName = "SuggestionData";

        public MongoSuggestionData(IDbConnection dbConnection, IUserData userData, IMemoryCache cache)
        {
            _dbConnection = dbConnection;
            _cache = cache;
            _suggestions = dbConnection.SuggestionCollection;
            _userData = userData;
        }

        public async Task<List<SuggestionModel>> GetAllSuggestions()
        {
            var output = _cache.Get<List<SuggestionModel>>(cacheName);
            if (output is null)
            {
                var result = (await _suggestions.FindAsync(i => !i.Archived));
                output = result.ToList();

                _cache.Set(cacheName, output, TimeSpan.FromMinutes(1));
            }
            return output;
        }

        public async Task<List<SuggestionModel>> GetAllApprovedSuggestion()
        {
            var result = await GetAllSuggestions();
            return result.Where(i => i.ApprovedForRelease).ToList();
        }

        public async Task<SuggestionModel> GetSuggestion(string id)
        {
            var result = await _suggestions.FindAsync(i => i.Id == id);
            return result.FirstOrDefault();
        }

        public async Task<List<SuggestionModel>> GetAllSuggestionsWaitingForApproval()
        {
            var result = await GetAllSuggestions();
            return result.Where(i => !i.Archived && !i.Rejected && !i.ApprovedForRelease).ToList();
        }

        public async Task<List<SuggestionModel>> GetUserSuggestions(string userId)
        {
            var output = _cache.Get<List<SuggestionModel>>(userId);
            if(output is null)
            {
                var result = await _suggestions.FindAsync(i => i.Author.Id == userId);
                output = result.ToList();
                _cache.Set(userId, output, TimeSpan.FromMinutes(1));
            }

            return output.ToList();
        }

        public async Task UpdateSuggestion(SuggestionModel suggestion)
        {
            await _suggestions.ReplaceOneAsync(i => i.Id == suggestion.Id, suggestion);
            _cache.Remove(cacheName);
        }

        public async Task UpvoteSuggestion(string suggestionId, string userId)
        {
            var client = _dbConnection.MongoClient;

            using var session = await client.StartSessionAsync();
            session.StartTransaction();

            try
            {
                var db = client.GetDatabase(_dbConnection.DbName);
                var suggestionsInTransaction = db.GetCollection<SuggestionModel>(_dbConnection.SuggestionCollectionName);
                var suggestion = (await suggestionsInTransaction.FindAsync(i => i.Id == suggestionId)).First();

                var isUpvoted = suggestion.UserVotes.Add(userId);
                if (!isUpvoted)
                {
                    suggestion.UserVotes.Remove(userId);
                }

                await suggestionsInTransaction.ReplaceOneAsync(session, i => i.Id == suggestionId, suggestion);

                var usersInTransactions = db.GetCollection<UserModel>(_dbConnection.UserCollectionName);
                var user = await _userData.GetUserByIdAsync(userId);

                if (isUpvoted)
                {
                    user.VotedOnSuggestions.Add(new BasicSuggestionModel(suggestion));
                }
                else
                {
                    var suggestionForRemove = (await _suggestions.FindAsync(i => i.Id == suggestionId)).First();
                    user.VotedOnSuggestions.Remove(new BasicSuggestionModel(suggestionForRemove));
                }

                await usersInTransactions.ReplaceOneAsync(session, i => i.Id == userId, user);

                await session.CommitTransactionAsync();

                _cache.Remove(cacheName);
            }
            catch
            {
                await session.AbortTransactionAsync();
                throw;
            }
        }

        public async Task CreateSuggestion(SuggestionModel suggestion)
        {
            var client = _dbConnection.MongoClient;
            using var session = await client.StartSessionAsync();
            session.StartTransaction();

            try
            {
                var db = client.GetDatabase(_dbConnection.DbName);
                var suggestionsInTransaction = db.GetCollection<SuggestionModel>(_dbConnection.SuggestionCollectionName);
                await suggestionsInTransaction.InsertOneAsync(session, suggestion);

                var usersInTransactions = db.GetCollection<UserModel>(_dbConnection.UserCollectionName);
                var user = await _userData.GetUserByIdAsync(suggestion.Author.Id);
                user.AuthoredSuggestion.Add(new BasicSuggestionModel(suggestion));
                await usersInTransactions.ReplaceOneAsync(session,i => i.Id == user.Id, user);

                await session.CommitTransactionAsync();
            }
            catch
            {
                await session.AbortTransactionAsync();
                throw;
            }
        }
    }
}