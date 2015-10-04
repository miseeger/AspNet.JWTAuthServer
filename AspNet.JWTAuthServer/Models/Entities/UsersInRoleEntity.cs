using System.Collections.Generic;

namespace AspNet.JWTAuthServer.Models.Entities
{

    public class UsersInRoleEntity
    {

        public string Id { get; set; }
        public List<string> EnrolledUsers { get; set; }
        public List<string> RemovedUsers { get; set; }

    }

}