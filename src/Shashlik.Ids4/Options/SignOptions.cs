﻿// ReSharper disable CheckNamespace

namespace Shashlik.Ids4
{
    /// <summary>
    /// 签名配置
    /// </summary>
    public class SignOptions
    {
        /// <summary>
        /// 签名证书类型,不区分大小写,Dev/Rsa/X509,默认Dev,Dev配置存在集群问题,最好使用Rsa或者X509
        /// </summary>
        public CredentialType CredentialType { get; set; } = CredentialType.Dev;

        /// <summary>
        /// 签名算法,RS256,rsa/x509证书配置有效
        /// </summary>
        public string? SigningAlgorithm { get; set; } = "RS256";

        /// <summary>
        /// rsa: rsa私钥 pem格式，需要带有BEGIN/END(JWT token签名用)
        /// </summary>
        public string? RsaPrivateKey { get; set; }

        /// <summary>
        /// x509: x509证书内容,文件base64后,一般pfx格式,和<see cref="X509CertificateFilePath"/>选择配置一个就可以,优先使用此配置
        /// </summary>
        public string? X509CertificateFileBase64 { get; set; }

        /// <summary>
        /// x509: 证书文件地址,和<see cref="X509CertificateFileBase64"/>选择配置一个就可以
        /// </summary>
        public string? X509CertificateFilePath { get; set; }

        /// <summary>
        /// x509: 证书密码
        /// </summary>
        public string? X509CertificatePassword { get; set; }
    }
}