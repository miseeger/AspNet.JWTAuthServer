using System.Collections.Generic;
using System.Linq;
using AspNet.IdentityEx.NPoco.Users;
using Microsoft.AspNet.Identity;
using NPoco;

namespace AspNet.IdentityEx.NPoco.UserLogins
{

    /// <summary>
    /// Class that represents the UserLogin table in the database
    /// </summary>
    public class UserLoginTable
    {
        private readonly Database _database;

        public UserLoginTable(Database database)
        {
            _database = database;
        }


        public int Delete(IdentityUserLogin userLogin)
        {
            return
               _database.Execute(
                   "DELETE FROM \r\n" +
                   "   UserLogin \r\n" +
                    "WHERE \r\n" +
                    "   UserId = @0 \r\n" +
                    "   AND ProviderKey = @1"
                    , userLogin.UserId
                    , userLogin.ProviderKey
               );
        }


        public int DeleteAll(string userId)
        {
            return
               _database.Execute(
                   "DELETE FROM \r\n" +
                   "   UserLogin \r\n" +
                   "WHERE \r\n" +
                   "   UserId = @0"
                   , userId
               );
        }


        public int Insert(IdentityUserLogin userLogin)
        {
            return
                _database.Insert("UserLogin", "", userLogin) == null 
                    ? 0 
                    : 1;
        }


        public IdentityUser FindUserByLogin(IdentityUserLogin userLogin)
        {
            var result = 
                _database.FirstOrDefault<IdentityUser>(
                    "SELECT \r\n" +
                    "   u.* \r\n" +
                    "FROM \r\n" +
                    "   UserLogin ul\r\n" +
                    "   JOIN User u ON (u.Id = ul.UserId) \r\n" +
                    "WHERE \r\n" +
                    "   ul.LoginProvider = @0 \r\n" +
                    "   AND ul.ProviderKey = @1"
                    , userLogin.LoginProvider
                    , userLogin.ProviderKey
                );

	        return result;
        }


        public List<UserLoginInfo> FindByUserId(string userId)
        {
            return
                _database.Fetch<IdentityUserLogin>(
                    "SELECT \r\n" +
                    "   * \r\n" +
                    "FROM \r\n" +
                    "   UserLogin \r\n" +
                    "WHERE \r\n" +
                    "   UserId = @0",
                    userId
                )
                .Select(userLogin => new UserLoginInfo(userLogin.LoginProvider, userLogin.ProviderKey))
                .ToList();
        }

    }

}
