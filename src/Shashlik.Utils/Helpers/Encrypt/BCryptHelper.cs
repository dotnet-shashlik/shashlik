namespace Shashlik.Utils.Helpers
{
    public static class BCryptHelper
    {
        /// <summary>
        /// BCrypt HASH
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public static string Hash(string password)
        {
            return global::BCrypt.Net.BCrypt.HashPassword(password);
        }

        /// <summary>
        /// 验证BCrypt
        /// </summary>
        /// <param name="password"></param>
        /// <param name="hash">HASH数据</param>
        /// <returns></returns>
        public static bool Verify(string password, string hash)
        {
            return global::BCrypt.Net.BCrypt.Verify(password, hash);
        }
    }
}