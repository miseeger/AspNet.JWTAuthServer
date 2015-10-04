using AspNet.IdentityEx.NPoco;
using NPoco;

namespace AspNet.JWTAuthServer.Infrastructure
{

    public class JWTServerDatabase : Database
    {

        public JWTServerDatabase() : base(IdentityConstants.ConnectionName)
        {
        }

        public static JWTServerDatabase Create()
        {
            return new JWTServerDatabase();
        }

    }

}