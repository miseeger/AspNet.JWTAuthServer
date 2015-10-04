using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AspNet.IdentityEx.NPoco.Claims;
using AspNet.IdentityEx.NPoco.Roles;
using AspNet.IdentityEx.NPoco.Users;
using NPoco;

namespace AspNet.IdentityEx.NPoco.UserRoles
{

    /// <summary>
    /// Class that implements the user role store iterfaces
    /// </summary>
    public class UserRoleStore<TRole, TUser> where TUser : IdentityUser where TRole : IdentityRole 
    {
        private UserRoleTable _userRoleTable;
		public Database NPocoDb { get; private set; }

        public UserRoleStore()
        {
			new UserRoleStore<TRole, TUser>(new Database(IdentityConstants.ConnectionName));
        }

		public UserRoleStore(Database database)
        {
			NPocoDb = database;
            _userRoleTable = new UserRoleTable(database);
        }


		public Task<List<TRole>> GetRolesAsync(string userId)
		{
			List<TRole> result = _userRoleTable.GetRoles(userId) as List<TRole>;

			return Task.FromResult(result);
		}



        public Task CreateAsync(TRole role, TUser user)
        {
			if (role == null)
            {
                throw new ArgumentNullException(IdentityConstants.Claim);
            }

			if (user == null)
			{
				throw new ArgumentNullException(IdentityConstants.User);
			}

			_userRoleTable.Insert(role.Id, user.Id);

            return Task.FromResult<object>(null);
        }


		public Task DeleteAsync(TRole role, TUser user)
        {
			if (role == null)
            {
                throw new ArgumentNullException(IdentityConstants.User);
            }

			if (user == null)
			{
				throw new ArgumentNullException(IdentityConstants.User);
			}

            _userRoleTable.Delete(role.Id, user.Id);

            return Task.FromResult<Object>(null);
        }


		public Task DeleteAsync(TUser user)
		{
			if (user == null)
			{
				throw new ArgumentNullException(IdentityConstants.User);
			}

			_userRoleTable.Delete(user.Id);

			return Task.FromResult<Object>(null);
		}


	    public void Dispose()
        {
			if (NPocoDb != null)
            {
				NPocoDb.Dispose();
				NPocoDb = null;
            }

		    _userRoleTable = null;
        }

    }

}
