namespace Shashlik.Utils.Encrypt
{
    public static class BCryptExtensions
    {
        /// <summary>
        /// BCrypt加密
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public static string BCrypt(this string password)
        {
            return global::BCrypt.Net.BCrypt.HashPassword(password);
        }

        /// <summary>
        /// 验证BCrypt
        /// </summary>
        /// <param name="password"></param>
        /// <param name="hash">加密数据</param>
        /// <returns></returns>
        public static bool BCryptVerify(this string password, string hash)
        {
            return global::BCrypt.Net.BCrypt.Verify(password, hash);
        }
    }
}