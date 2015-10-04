using System;
using System.Text;

namespace AspNet.IdentityEx.NPoco.Helpers
{

    // This class was taken from 
    // http://stackoverflow.com/questions/9543715/generating-human-readable-usable-short-but-unique-ids
    // and extended by giving the Id an (optional) prefix.

    public static class RandomIdHelper
    {
        private static char[] _base62chars =
            "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz"
            .ToCharArray();

        private static Random _random = new Random();


        private static string GetBase62(int length)
        {
            var sb = new StringBuilder(length);

            for (int i = 0; i < length; i++)
                sb.Append(_base62chars[_random.Next(62)]);

            return sb.ToString();
        }


        public static string GetBase62(string prefix , int length)
        {
            return String.Format("{0}{1}", prefix, GetBase62(length));
        }


        private static string GetBase36(int length)
        {
            var sb = new StringBuilder(length);

            for (int i = 0; i < length; i++)
                sb.Append(_base62chars[_random.Next(36)]);

            return sb.ToString();
        }


        public static string GetBase36(string prefix, int length)
        {
            return String.Format("{0}{1}", prefix, GetBase62(length));
        }

    }

}