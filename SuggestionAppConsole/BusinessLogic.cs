using SuggestionAppLibrary.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuggestionAppConsole
{
    /// <summary>
    /// </summary>
    public class BusinessLogic : IBusinessLogic
    {
        private IUserData _userData;

        public BusinessLogic(IUserData userData)
        {
            _userData = userData;
        }

        public async void ShowUsers()
        {
            var users = await _userData.GetUsersAsync();
            var sb = new StringBuilder();
            foreach (var user in users)
            {
                sb.AppendLine($"{user.Id} {user.FirstName} {user.LastName}");
            }
            Console.WriteLine(sb.ToString());
        }
    }
}
