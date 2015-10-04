using System.Collections.Generic;
using AspNet.IdentityEx.NPoco.Helpers;
using AspNet.IdentityEx.NPoco.Roles;
using AspNet.IdentityEx.NPoco.Users;
using NPoco;

namespace AspNet.IdentityEx.NPoco.UserRoles
{

    /// <summary>
    /// Class that represents the UserRole table in the database
    /// </summary>
    public class UserRoleTable
    {
        private Database _database;


        public UserRoleTable(Database database)
        {
            _database = database;
        }


        public List<IdentityRole> GetRoles(string userId)
        {
            return
				_database.Fetch<IdentityRole>(
                    "SELECT \r\n" +
                    "   r.Id \r\n" +
					"   ,r.ClientId \r\n" +
					"   ,r.Name \r\n" +
                    "FROM \r\n" +
                    "   UserRole ur \r\n" +
					"   JOIN Role r ON(r.Id = ur.RoleId) \r\n" +
                    "WHERE \r\n" +
                    "   ur.UserId = @0",
                    userId
                );
        }


        public int Delete(string userId)
        {
            return
                _database.Execute(
                    "DELETE FROM \r\n" +
                    "   UserRole \r\n" +
                    "WHERE \r\n" +
                    "   UserId = @0",
                    userId
                );
        }


        public int Delete(string roleId, string userId)
        {
            return
                _database.Execute(
                    "DELETE FROM \r\n" +
                    "   UserRole \r\n" +
                    "WHERE \r\n" +
                    "   RoleId = @0 \r\n" +
                    "   AND UserId = @1",
                    roleId,
                    userId
                );

        }


		public int Insert(string userId, string roleId)
		{
			return
				_database.Insert("UserRole", "",
					new IdentityUserRole()
					{
						RoleId = roleId,
						UserId = userId
					}
				) != null
					? 1
					: 0;

		}

        public int Insert(IdentityUser user, string roleId)
        {
	        return
		        Insert(user.Id, roleId);
        }

    }

}