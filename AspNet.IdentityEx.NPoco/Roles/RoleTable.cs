using System.Collections.Generic;
using NPoco;

namespace AspNet.IdentityEx.NPoco.Roles
{

    /// <summary>
    /// Class that represents the Role table in the database
    /// </summary>
    public class RoleTable 
    {
        private Database _database;

        public RoleTable(Database database)
        {
            _database = database;
        }


        public int Delete(string roleId)
        {
            return 
                _database.Execute(
                    "DELETE FROM \r\n" +
                    "   Role \r\n" +
                    "WHERE \r\n" +
                    "   Id = @0", 
                    roleId);
        }


        /// <summary>
        /// Inserts a new role.
        /// </summary>
        /// <param name="role"></param>
        /// <returns>
        ///     0 - Not inserted (error)
        ///     1 - Successfully inserted
        ///     2 - Gloabl role created
        ///     3 - No action needed
        /// </returns>
        public int Insert(IdentityRole role)
        {
            var existingRole = GetRoleByName(role.Name);

            if (existingRole != null)
            {
                // Role already exists but not for this client: Make existing role global 
                // if it not already is!
                if (existingRole.ClientId != null && existingRole.ClientId != role.ClientId)
                {
                    existingRole.ClientId = null;
                    return _database.Update("Role","Id",existingRole) != 0 ? 2 : 0;
                }

                return 3;
            }
            else
            {
                return _database.Insert("Role", "", role) != null ? 1 : 0;
            }
        }


        public IEnumerable<IdentityRole> GetRoles()
        {
			return
                _database.Fetch<IdentityRole>(
                    "SELECT \r\n" +
                    "   * \r\n" +
                    "FROM \r\n" +
                    "   Role"
                );
        }


        public string GetRoleName(string roleId)
        {
           return 
                _database.ExecuteScalar<string>(
                    "SELECT \r\n" +
                    "   Name \r\n" +
                    "FROM \r\n" +
                    "   Role \r\n" +
                    "WHERE \r\n" +
                    "   Id = @0",
                    roleId);
        }


        /// <summary>
        /// Gets the ID of a global role.
        /// </summary>
        /// <param name="roleName"></param>
        /// <returns></returns>
		public string GetRoleId(string roleName)
        {
            return 
                _database.ExecuteScalar<string>(
                    "SELECT \r\n" +
                    "   Id \r\n" +
                    "FROM \r\n" +
                    "   Role \r\n" +
                    "WHERE \r\n" +
					"   ClientId IS NULL" +
					"   AND Name = @0",
                    roleName);
        }


		/// <summary>
		/// Gets the ID of a client specific (or global) role.
		/// </summary>
		/// <param name="clientId"></param>
		/// <param name="roleName"></param>
		/// <returns></returns>
		public string GetRoleId(string clientId, string roleName)
		{
			return
				_database.ExecuteScalar<string>(
					"SELECT \r\n" +
					"   Id \r\n" +
					"FROM \r\n" +
					"   Role \r\n" +
					"WHERE \r\n" +
					"   ClientId = @0" +
					"   AND Name = @1",
					clientId, 
					roleName
				);
		}


        public IdentityRole GetRoleById(string roleId)
        {
            return
                _database.FirstOrDefault<IdentityRole>(
                    "SELECT \r\n" +
                    "   * \r\n" +
                    "FROM \r\n" +
                    "   Role \r\n" +
                    "WHERE \r\n" +
                    "   Id = @0",
                    roleId
                );
        }


        /// <summary>
        /// Gets a global role by name.
        /// </summary>
        /// <param name="roleName"></param>
        /// <returns></returns>
		public IdentityRole GetRoleByName(string roleName)
        {
            return
                _database.FirstOrDefault<IdentityRole>(
                    "SELECT \r\n" +
                    "   * \r\n" +
                    "FROM \r\n" +
                    "   Role \r\n" +
                    "WHERE \r\n" +
					"   Name = @0",
                    roleName
                );
        }


		/// <summary>
		/// Gets a client specific (or global) role by name.
		/// </summary>
		/// <param name="clientId"></param>
		/// <param name="roleName"></param>
		/// <returns></returns>
		public IdentityRole GetRoleByName(string clientId, string roleName)
		{
			return
				_database.FirstOrDefault<IdentityRole>(
					"SELECT \r\n" +
					"   * \r\n" +
					"FROM \r\n" +
					"   Role \r\n" +
					"WHERE \r\n" +
					"   ClientId = @0" +
					"   AND Name = @1",
					clientId,
					roleName
				);
		}


        public int Update(IdentityRole role)
        {
            return
                _database.Update("Role", "Id", role);

        }

    }

}
