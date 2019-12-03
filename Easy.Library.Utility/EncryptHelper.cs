using System;
using System.IO;
using System.Security.AccessControl;
using System.Security.Cryptography;
using System.Text;

namespace Easy.Library.Utility
{
    /// <summary>
    /// description：RSA 加密解密操作类（非对称加密）
    /// </summary>
    public class EncryptRsa
    {
        /// <summary>
        /// 默认编码
        /// </summary>
        private static readonly Encoding DefaultEncoding = Encoding.UTF8;

        /// <summary>
        /// 密钥管理容器
        /// </summary>
        private static readonly CspParameters CspPara;

        static EncryptRsa()
        {
            CspParameters cspParams = new CspParameters
            {
                KeyContainerName = "key_container_pms",
                KeyNumber = 1, //设置密钥类型为Exchange
                Flags = CspProviderFlags.UseMachineKeyStore, //设置密钥容器保存到计算机密钥库（默认为用户密钥库）
                CryptoKeySecurity = new CryptoKeySecurity()
            };
            //设置密钥容器保存到计算机密钥库（默认为用户密钥库）
            cspParams.CryptoKeySecurity.SetAccessRule(new CryptoKeyAccessRule("everyone", CryptoKeyRights.FullControl, AccessControlType.Allow));
            CspPara = cspParams;
        }

        /// <summary>
        /// 生成密钥
        /// </summary>
        public static void CreateKeyToXml()
        {
            //声明一个RSA算法的实例，由RSACryptoServiceProvider类型的构造函数指定了密钥长度为1024位
            //实例化RSACryptoServiceProvider后，RSACryptoServiceProvider会自动生成密钥信息。
            //privateKey = "<RSAKeyValue><Modulus>weho0CBxsjhWjVcpJRqiNycnKDYmaKI4McIj4iOslbzkA6xlZEqmjB9InRAYmAAdThJd1XcnFkb9s2Rz8nYD/yfPMxzPvLBULGO7qMuob41E6MiLEnINaGoyvEeXRP5ZMKWNx1bxnEoph7F69oVjvBTatAa4X3qgTh1YimO9PJc=</Modulus><Exponent>AQAB</Exponent><P>5hUgyahhijUDNYCLffCTIMgUebL7sG0SiJ36uq57NVs7Mgy97sBwo7LFzYMru/DDl7G9Heczt1oWJnY2yktYGQ==</P><Q>18AdaFhxj/lbTLcDkj02DJXHzIuksEwyk4lUXuzbjo92rMarg42gWNe3MX8icylrim71tT/FhP/WVK7OBGqQLw==</Q><DP>NZPD79GK7SENHz9QvEHyMNcGlZRNMbckcrW+9gu9Wx5keXIoJFmhoSz3DLU30Oru0Pstm7IEA/UxZUFv3smOuQ==</DP><DQ>Nc1DyByeHTAqs2PEMTiwfMzxKTH9nLUzu5T4hD9+tPtTtdxJMyIjRWRt25r/pUZD+h6XiV5gzDzcXvvEhldoow==</DQ><InverseQ>d8IaEj4sBEhxR2PnR7rfdDQuOm6dxdYuB2p7flDt/xL0vQiOpztn+RTpz7BQ75VPc3arnonxXFbkliR+/nnACw==</InverseQ><D>RO+eC5ftM8Hpq1f4TbOZRehKeHY/02UQe0gcv7GVEqrTmSa56RzM9vXMN+JYC/nGcVIAP/qbT8UvPo6Q1fr00ivR7CtqIgr6J6Zr80X+hZWEq6LLEKZudhqx3z/A1+G4FKQL74NwTP7scr4ePSJoBY7pZ7HwmBu9o8opbO0dbPE=</D></RSAKeyValue>";
            //publicKey = "<RSAKeyValue><Modulus>weho0CBxsjhWjVcpJRqiNycnKDYmaKI4McIj4iOslbzkA6xlZEqmjB9InRAYmAAdThJd1XcnFkb9s2Rz8nYD/yfPMxzPvLBULGO7qMuob41E6MiLEnINaGoyvEeXRP5ZMKWNx1bxnEoph7F69oVjvBTatAa4X3qgTh1YimO9PJc=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                using (StreamWriter writer = new StreamWriter("PrivateKey.xml")) //这个文件要保密...
                {
                    //表示同时包含 RSA 公钥和私钥，用于解密
                    writer.WriteLine(rsa.ToXmlString(true));
                }
                using (StreamWriter writer = new StreamWriter("PublicKey.xml"))
                {
                    //参数为false表示生成公钥，用于加密
                    writer.WriteLine(rsa.ToXmlString(false));
                }
            }
        }

        /// <summary>
        /// 获取公钥
        /// </summary>
        public static string GetPublicKey()
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            RSAParameters para = rsa.ExportParameters(true);
            string modulus = Convert.ToBase64String(para.Modulus);
            string exponent = Convert.ToBase64String(para.Exponent);
            return modulus + "," + exponent;
        }

        /// <summary>
        /// 使用公钥加密
        /// </summary>
        public static string EncryptData(string plainText, string xmlPublicKey = null)
        {
            //实例化RSA对象的时候，将CspParameters对象作为构造函数的参数传递给RSA对象，
            //如果名称为key_container_test的密钥容器不存在，RSA对象会创建这个密钥容器；
            //如果名称为key_container_test的密钥容器已经存在，RSA对象会使用这个密钥容器中的密钥进行实例化
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(CspPara))
            {
                if (!string.IsNullOrEmpty(xmlPublicKey))
                {
                    //将公钥导入到RSA对象中，准备加密；
                    rsa.FromXmlString(xmlPublicKey);
                }
                //byte[] plaindata = DefaultEncoding.GetBytes(plainText); //将要加密的字符串转换为字节数组
                //byte[] cipherData = rsa.Encrypt(plaindata, false); //将加密后的字节数据转换为新的加密字节数组
                //return Convert.ToBase64String(cipherData); //将加密后的字节数组转换为字符串
                byte[] plaindata = DefaultEncoding.GetBytes(plainText); //将要加密的字符串转换为字节数组
                int maxBlockSize = rsa.KeySize / 8 - 11;    //加密块最大长度限制

                if (plaindata.Length <= maxBlockSize)
                    return Convert.ToBase64String(rsa.Encrypt(plaindata, false));

                using (MemoryStream plaiStream = new MemoryStream(plaindata))
                using (MemoryStream crypStream = new MemoryStream())
                {
                    var buffer = new byte[maxBlockSize];
                    int blockSize = plaiStream.Read(buffer, 0, maxBlockSize);

                    while (blockSize > 0)
                    {
                        var toEncrypt = new byte[blockSize];
                        Array.Copy(buffer, 0, toEncrypt, 0, blockSize);

                        byte[] cryptograph = rsa.Encrypt(toEncrypt, false);
                        crypStream.Write(cryptograph, 0, cryptograph.Length);
                        blockSize = plaiStream.Read(buffer, 0, maxBlockSize);
                    }
                    return Convert.ToBase64String(crypStream.ToArray(), Base64FormattingOptions.None);
                }
            }
        }
        /// <summary>
        ///  使用私钥解密
        /// </summary>
        public static string DecryptData(string cipherText, string xmlPrivateKey = null)
        {
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(CspPara))
            {
                if (!string.IsNullOrEmpty(xmlPrivateKey))
                {
                    //将私钥导入RSA中，准备解密；
                    rsa.FromXmlString(xmlPrivateKey);
                }
                //byte[] encryptdata = Convert.FromBase64String(cipherText);
                //byte[] plaindata = rsa.Decrypt(encryptdata, false);
                //return DefaultEncoding.GetString(plaindata);
                var ciphertextData = Convert.FromBase64String(cipherText);
                int maxBlockSize = rsa.KeySize / 8;    //解密块最大长度限制

                if (ciphertextData.Length <= maxBlockSize)
                    return DefaultEncoding.GetString(rsa.Decrypt(ciphertextData, false));

                using (MemoryStream crypStream = new MemoryStream(ciphertextData))
                using (MemoryStream plaiStream = new MemoryStream())
                {
                    var buffer = new byte[maxBlockSize];
                    int blockSize = crypStream.Read(buffer, 0, maxBlockSize);

                    while (blockSize > 0)
                    {
                        var toEncrypt = new byte[blockSize];
                        Array.Copy(buffer, 0, toEncrypt, 0, blockSize);

                        var plaintext = rsa.Decrypt(toEncrypt, false);
                        plaiStream.Write(plaintext, 0, plaintext.Length);

                        blockSize = crypStream.Read(buffer, 0, maxBlockSize);
                    }

                    return DefaultEncoding.GetString(plaiStream.ToArray());
                }
            }
        }

        /// <summary>
        /// 密钥管理容器删除
        /// </summary>
        public static void CspParareMove()
        {
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(CspPara))
            {
                rsa.PersistKeyInCsp = false;
            }
        }

        /// <summary>
        /// 签名操作
        /// </summary>
        public static byte[] Sign(byte[] cipherText, string xmlPrivateKey)
        {
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                //导入私钥，准备签名
                rsa.FromXmlString(xmlPrivateKey);
                //将数据使用MD5进行消息摘要，然后对摘要进行签名并返回签名数据
                return rsa.SignData(cipherText, "MD5");
            }
        }

        /// <summary>
        /// 验签操作
        /// </summary>
        public static bool Verify(byte[] cipherText, byte[] signature, string xmlPublicKey)
        {
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                //导入公钥，准备验证签名
                rsa.FromXmlString(xmlPublicKey);
                //返回数据验证结果
                return rsa.VerifyData(cipherText, "MD5", signature);
            }
        }
    }

    /// <summary>
    /// description：AES 加密解密操作类(对称加密)
    /// </summary>
    public class EncryptAes
    {
        /// <summary>
        /// 默认编码
        /// </summary>
        private static readonly Encoding DefaultEncoding = Encoding.UTF8;
        /// <summary>
        /// Aes加解密钥必须32位
        /// </summary>
        public static string AesKey = "encrypt.aes.key";
        /// <summary>
        /// 获取Aes32位密钥
        /// </summary>
        /// <param name="key">Aes密钥字符串</param>
        /// <returns>Aes32位密钥</returns>
        static byte[] GetAesKey(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                key = "0".PadRight(32, '0');
            }
            else if (key.Length < 32)
            {
                // 不足32补全
                key = key.PadRight(32, '0');
            }
            else if (key.Length > 32)
            {
                key = key.Substring(0, 32);
            }
            return DefaultEncoding.GetBytes(key);
        }

        /// <summary>
        /// Aes加密
        /// </summary>
        /// <param name="plainText">源字符串</param>
        /// <param name="key">aes密钥，长度必须32位</param>
        /// <returns>加密后的字符串</returns>
        public static string EncryptData(string plainText, string key)
        {
            using (AesCryptoServiceProvider aesProvider = new AesCryptoServiceProvider())
            {
                aesProvider.Key = GetAesKey(key);
                aesProvider.Mode = CipherMode.ECB;
                aesProvider.Padding = PaddingMode.PKCS7;
                using (ICryptoTransform cryptoTransform = aesProvider.CreateEncryptor())
                {
                    byte[] inputBuffers = DefaultEncoding.GetBytes(plainText);
                    byte[] results = cryptoTransform.TransformFinalBlock(inputBuffers, 0, inputBuffers.Length);
                    aesProvider.Clear();
                    aesProvider.Dispose();
                    return Convert.ToBase64String(results, 0, results.Length);
                }
            }
        }

        /// <summary>
        /// Aes解密
        /// </summary>
        /// <param name="cipherText">加密的字符串</param>
        /// <param name="key">aes密钥，长度必须32位</param>
        /// <returns>解密后的字符串</returns>
        public static string DecryptAes(string cipherText, string key)
        {
            using (AesCryptoServiceProvider aesProvider = new AesCryptoServiceProvider())
            {
                aesProvider.Key = GetAesKey(key);
                aesProvider.Mode = CipherMode.ECB;
                aesProvider.Padding = PaddingMode.PKCS7;
                using (ICryptoTransform cryptoTransform = aesProvider.CreateDecryptor())
                {
                    byte[] inputBuffers = Convert.FromBase64String(cipherText);
                    byte[] results = cryptoTransform.TransformFinalBlock(inputBuffers, 0, inputBuffers.Length);
                    aesProvider.Clear();
                    aesProvider.Dispose();
                    return DefaultEncoding.GetString(results);
                }
            }
        }
    }

    public class EncryMd5
    {
        /// <summary>
        /// MD5加密
        /// </summary>
        /// <param name="plainText">需要加密的明文</param>
        /// <returns>返回32位加密结果</returns>
        public static string Encry(string plainText)
        {
            byte[] b = new MD5CryptoServiceProvider().ComputeHash(Encoding.UTF8.GetBytes(plainText));
            string ret = "";
            for (int i = 0; i < b.Length; i++)
            {
                ret += b[i].ToString("x").PadLeft(2, '0');
            }
            return ret;
        }
    }
}