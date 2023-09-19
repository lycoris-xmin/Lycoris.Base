using Lycoris.Base.Extensions;
using System.Security.Cryptography;
using System.Text;

namespace Lycoris.Base.Google
{
    /// <summary>
    /// 
    /// </summary>
    public class GoogleAuthenticator
    {
        /// <summary>
        /// 秘钥(手机使用Base32码)
        /// </summary>
        private readonly string SerectKey;

        /// <summary>
        /// 间隔时间
        /// </summary>
        private readonly long Duration = 30;

        /// <summary>
        /// 迭代次数
        /// </summary>
        private long Counter => (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds / Duration;

        /// <summary>
        /// 到期秒数
        /// </summary>
        public long ExpireSeconds => (Duration - (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds % Duration);

        /// <summary>
        /// 手机端秘钥
        /// </summary>
        public string MobileSerectKey { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="SerectKey"></param>
        public GoogleAuthenticator(string SerectKey)
        {
            this.SerectKey = SerectKey;
            this.MobileSerectKey = GenerateMobileSerectKey.Generate(this.SerectKey);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="SerectKey"></param>
        /// <param name="Duration"></param>
        public GoogleAuthenticator(string SerectKey, long Duration)
        {
            this.SerectKey = SerectKey;
            this.MobileSerectKey = GenerateMobileSerectKey.Generate(this.SerectKey);
            this.Duration = Duration;
        }

        /// <summary>
        /// 生成认证码
        /// </summary>
        /// <returns></returns>
        public string Generate() => GenerateHashedCode(this.SerectKey, this.Counter);

        /// <summary>
        /// 按照次数生成哈希编码
        /// </summary>
        /// <param name="secret">秘钥</param>
        /// <param name="iterationNumber">迭代次数</param>
        /// <param name="digits">生成位数</param>
        /// <returns>返回验证码</returns>
        private static string GenerateHashedCode(string secret, long iterationNumber, int digits = 6)
        {
            byte[] counter = BitConverter.GetBytes(iterationNumber);

            if (BitConverter.IsLittleEndian)
                Array.Reverse(counter);

            byte[] key = Encoding.ASCII.GetBytes(secret);

            var hmac = new HMACSHA1(key);

            var hash = hmac.ComputeHash(counter);

            var offset = hash[^1] & 0xf;

            var binary = ((hash[offset] & 0x7f) << 24) | ((hash[offset + 1] & 0xff) << 16) | ((hash[offset + 2] & 0xff) << 8) | (hash[offset + 3] & 0xff);

            var password = binary % (int)Math.Pow(10, digits); // 6 digits

            return password.ToString(new string('0', digits));
        }

        internal class GenerateMobileSerectKey
        {
            internal static string Generate(string SerectKey)
            {
                if (SerectKey.IsNullOrEmpty())
                    throw new ArgumentNullException(nameof(SerectKey));

                var bytes = Encoding.UTF8.GetBytes(SerectKey);

                if (bytes == null || bytes.Length == 0)
                    throw new ArgumentException("serectkey is not valiad");

                var charCount = (int)Math.Ceiling(bytes.Length / 5d) * 8;
                var returnArray = new char[charCount];

                byte nextChar = 0, bitsRemaining = 5;
                int arrayIndex = 0;

                foreach (byte b in bytes)
                {
                    nextChar = (byte)(nextChar | (b >> (8 - bitsRemaining)));
                    returnArray[arrayIndex++] = ValueToChar(nextChar);

                    if (bitsRemaining < 4)
                    {
                        nextChar = (byte)((b >> (3 - bitsRemaining)) & 31);
                        returnArray[arrayIndex++] = ValueToChar(nextChar);
                        bitsRemaining += 5;
                    }

                    bitsRemaining -= 3;
                    nextChar = (byte)((b << bitsRemaining) & 31);
                }

                if (arrayIndex != charCount)
                {
                    returnArray[arrayIndex++] = ValueToChar(nextChar);

                    while (arrayIndex != charCount)
                        returnArray[arrayIndex++] = '=';
                }

                return new string(returnArray);
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="c"></param>
            /// <returns></returns>
            /// <exception cref="ArgumentException"></exception>
            private static int CharToValue(char c)
            {
                var value = (int)c;

                if (value < 91 && value > 64)
                    return value - 65;

                if (value < 56 && value > 49)
                    return value - 24;

                if (value < 123 && value > 96)
                    return value - 97;

                throw new ArgumentException("Character is not a Base32 character.", "c");
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="b"></param>
            /// <returns></returns>
            /// <exception cref="ArgumentException"></exception>
            private static char ValueToChar(byte b)
            {
                if (b < 26)
                    return (char)(b + 65);

                if (b < 32)
                    return (char)(b + 24);

                throw new ArgumentException("Byte is not a value Base32 value.", "b");
            }
        }
    }
}
