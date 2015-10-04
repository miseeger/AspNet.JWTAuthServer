using System.Collections.Generic;
using NPoco;

namespace AspNet.IdentityEx.NPoco.Users
{

    /// <summary>
    /// Class that represents the User table in the database
    /// </summary>
    public class UserTable<TUser>
        where TUser : IdentityUser
    {
        private readonly Database _database;

        public UserTable(Database database)
        {
            _database = database;
        }


        public IEnumerable<TUser> GetUsers()
        {
            return
                _database.Fetch<TUser>(
                    "SELECT \r\n" +
                    "   * \r\n" +
                    "FROM \r\n" +
                    "   User"
                );
        }


        public string GetUserName(string userId)
        {
            return
                _database.ExecuteScalar<string>(
                    "SELECT \r\n" +
                    "   UserName \r\n" +
                    "FROM \r\n" +
                    "   User \r\n" +
                    "WHERE \r\n" +
                    "   Id = @0",
                    userId
                );
        }


        public string GetUserId(string userName)
        {
            return
                _database.ExecuteScalar<string>(
                    "SELECT \r\n" +
                    "   Id \r\n" +
                    "FROM \r\n" +
                    "   User \r\n" +
                    "WHERE \r\n" +
                    "   UserName = @0",
                    userName);
        }


        public TUser GetUserById(string userId)
        {
            return
                _database.FirstOrDefault<TUser>(
                    "SELECT \r\n" +
                    "   * \r\n" +
                    "FROM \r\n" +
                    "   User \r\n" +
                    "WHERE \r\n" +
                    "    Id = @0",
                    userId
                );
        }


        public List<TUser> GetUserByName(string userName)
        {
            return
                _database.Fetch<TUser>(
                    "SELECT \r\n" +
                    "   * \r\n" +
                    "FROM \r\n" +
                    "   User \r\n" +
                    "WHERE \r\n" +
                    "   UserName = @0",
                    userName
                );
        }


        public List<TUser> GetUserByEmail(string email)
        {
            return
                _database.Fetch<TUser>(
                    "SELECT \r\n" +
                    "   * \r\n" +
                    "FROM \r\n" +
                    "   User \r\n" +
                    "WHERE \r\n" +
                    "   Email = @0",
                    email
                );
        }


        public string GetPasswordHash(string userId)
        {
            var passHash =
                _database.ExecuteScalar<string>(
                    "SELECT \r\n" +
                    "   PasswordHash \r\n" +
                    "FROM \r\n" +
                    "   User \r\n" +
                    "WHERE \r\n" +
                    "   Id = @0",
                    userId
                );

            return string.IsNullOrEmpty(passHash) ? null : passHash;
        }


        public int SetPasswordHash(string userId, string passwordHash)
        {
            return
                _database.Execute(
                    "UPDATE \r\n" +
                    "   User \r\n" +
                    "SET \r\n" +
                    "   PasswordHash = @0 \r\n" +
                    "WHERE \r\n" +
                    "   Id = @1",
                    passwordHash,
                    userId
             );
        }


        public string GetSecurityStamp(string userId)
        {
            return
                _database.ExecuteScalar<string>(
                    "SELECT \r\n" +
                    "   SecurityStamp \r\n" +
                    "FROM \r\n" +
                    "   User \r\n" +
                    "WHERE \r\n" +
                    "   Id = @0",
                    userId
                );
        }


        public int Insert(TUser user)
        {
            return _database.Insert("User", "", user) != null ? 1 : 0;
        }


        private int Delete(string userId)
        {
            var result = 
				_database.Execute(
                    "DELETE FROM \r\n" +
                    "   User \r\n" +
                    "WHERE \r\n" +
                    "   Id = @0",
                    userId
                );

	        if (result == 1)
	        {
		        _database.Execute(
			        "DELETE FROM \r\n" +
			        "   UserClaim \r\n" +
			        "WHERE \r\n" +
			        "   UserId = @0",
			        userId);
		        _database.Execute(
			        "DELETE FROM \r\n" +
			        "   UserRole \r\n" +
			        "WHERE \r\n" +
			        "   UserId = @0",
			        userId);
		        _database.Execute(
			        "DELETE FROM \r\n" +
			        "   UserLogin \r\n" +
			        "WHERE \r\n" +
			        "   UserId = @0",
			        userId);
	        }

	        return result;
        }


        public int Delete(TUser user)
        {
            return Delete(user.Id);
        }


        public int Update(TUser user)
        {
            return
                _database.Update("User", "Id", user);
        }

    }

}
