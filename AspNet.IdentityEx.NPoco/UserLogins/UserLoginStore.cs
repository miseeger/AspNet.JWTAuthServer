using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AspNet.IdentityEx.NPoco.Users;
using Microsoft.AspNet.Identity;
using NPoco;

namespace AspNet.IdentityEx.NPoco.UserLogins
{

    /// <summary>
    /// Class that implements a user login store
    /// </summary>
    public class UserLoginStore<TUser> where TUser : IdentityUser 
    {
        private UserLoginTable _userLoginTable;
		public Database NPocoDb { get; private set; }

        public UserLoginStore()
        {
			new UserLoginStore<TUser>(new Database(IdentityConstants.ConnectionName));
        }

		public UserLoginStore(Database database)
        {
			NPocoDb = database;
			_userLoginTable = new UserLoginTable(database);
        }


        public Task<IdentityUser> FindUserByLoginAsync(IdentityUserLogin userLogin)
        {
            var result = _userLoginTable.FindUserByLogin(userLogin);

            return Task.FromResult(result);
        }


        public List<UserLoginInfo> FindByUserId(string userId)
	    {
			return _userLoginTable.FindByUserId(userId);
	    }


        public Task<List<UserLoginInfo>> FindByUserIdAsync(string userId)
		{
			var result = _userLoginTable.FindByUserId(userId);

			return Task.FromResult(result);
		}



        public Task CreateAsync(IdentityUserLogin login)
        {
			if (login == null)
            {
                throw new ArgumentNullException(IdentityConstants.Login);
            }

			_userLoginTable.Insert(login);

            return Task.FromResult<object>(null);
        }


		public Task DeleteAsync(IdentityUserLogin login)
        {
			if (login == null)
            {
                throw new ArgumentNullException(IdentityConstants.Login);
            }

            _userLoginTable.Delete(login);

            return Task.FromResult<Object>(null);
        }


		public Task DeleteAsync(string userId)
		{
			_userLoginTable.DeleteAll(userId);

			return Task.FromResult<Object>(null);
		}


	    public void Dispose()
        {
			if (NPocoDb != null)
            {
				NPocoDb.Dispose();
				NPocoDb = null;
            }

		    _userLoginTable = null;
        }

    }

}
