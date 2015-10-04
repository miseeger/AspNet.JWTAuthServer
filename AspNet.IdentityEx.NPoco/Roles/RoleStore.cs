using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using NPoco;

namespace AspNet.IdentityEx.NPoco.Roles
{

    /// <summary>
    /// Class that implements the key ASP.NET Identity role store iterfaces
    /// </summary>
    public class RoleStore<TRole> : IQueryableRoleStore<TRole>
        where TRole : IdentityRole
    {
        private RoleTable _roleTable;
		public Database NPocoDb { get; private set; }

        public RoleStore()
        {
            new RoleStore<TRole>(new Database(IdentityConstants.ConnectionName));
        }

        public RoleStore(Database database)
        {
			NPocoDb = database;
            _roleTable = new RoleTable(database);
        }


        public IQueryable<TRole> Roles
        {
            get { return _roleTable.GetRoles() as IQueryable<TRole>; }

        }


        public new virtual async Task<IdentityRoleList> GetRolesAsync()
        {
            var result = new IdentityRoleList
            {
                List = _roleTable.GetRoles() as List<IdentityRole>
            };

            return result;
        }


        public Task CreateAsync(TRole role)
        {
            if (role == null)
            {
                throw new ArgumentNullException(IdentityConstants.Role);
            }

            _roleTable.Insert(role);

            return Task.FromResult<object>(null);
        }


        public Task DeleteAsync(TRole role)
        {
            if (role == null)
            {
                throw new ArgumentNullException(IdentityConstants.User);
            }

            _roleTable.Delete(role.Id);

            return Task.FromResult<Object>(null);
        }


        public Task<TRole> FindByIdAsync(string roleId)
        {
            var result = _roleTable.GetRoleById(roleId) as TRole;

            return Task.FromResult<TRole>(result);
        }


        public Task<TRole> FindByNameAsync(string roleName)
        {
            var result = _roleTable.GetRoleByName(roleName) as TRole;

            return Task.FromResult<TRole>(result);
        }


		public Task<TRole> FindByNameAsync(string clientId, string roleName)
		{
			var result = _roleTable.GetRoleByName(clientId, roleName) as TRole;

			return Task.FromResult<TRole>(result);
		}


        public Task UpdateAsync(TRole role)
        {
            if (role == null)
            {
                throw new ArgumentNullException(IdentityConstants.User);
            }

            _roleTable.Update(role);

            return Task.FromResult<Object>(null);
        }


        public void Dispose()
        {
			if (NPocoDb != null)
            {
				NPocoDb.Dispose();
				NPocoDb = null;
            }

	        _roleTable = null;
        }

    }

}
