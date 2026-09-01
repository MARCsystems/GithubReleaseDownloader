using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace GithubReleaseDownloader
{
    internal class PemProcessor
    {
        internal static RSA PrepareRsaToken(string pemPath)
        {
            string pem = File.ReadAllText(pemPath);
            short pkcs_staging = 0;
            if (pem.Contains("-----BEGIN RSA PRIVATE KEY-----"))
            {
                pkcs_staging = 1;
            }
            else if (pem.Contains("-----BEGIN PRIVATE KEY-----"))
            {
                pkcs_staging = 8;
            }
            pem = pem.Replace("-----BEGIN RSA PRIVATE KEY-----", "")
                .Replace("-----END RSA PRIVATE KEY-----", "")
                .Replace("-----BEGIN PRIVATE KEY-----", "")
                .Replace("-----END PRIVATE KEY-----", "")
                .Replace("\n", "")
                .Replace("\r", "");

            byte[] pemBytes = Convert.FromBase64String(pem);

            if (pkcs_staging == 1)
            {
                // PKCS#1 parsing
                AsnReader pkReader = new AsnReader(pemBytes, AsnEncodingRules.DER);
                AsnReader pkSeq = pkReader.ReadSequence();

                BigInteger version = pkSeq.ReadInteger();

                RSAParameters rsaParameters = new RSAParameters
                {
                    Modulus = IntegerFunc.Normalize(pkSeq.ReadIntegerBytes().ToArray(), 256),
                    Exponent = IntegerFunc.Normalize(pkSeq.ReadIntegerBytes().ToArray()),
                    D = IntegerFunc.Normalize(pkSeq.ReadIntegerBytes().ToArray(), 256),
                    P = IntegerFunc.Normalize(pkSeq.ReadIntegerBytes().ToArray(), 128),
                    Q = IntegerFunc.Normalize(pkSeq.ReadIntegerBytes().ToArray(), 128),
                    DP = IntegerFunc.Normalize(pkSeq.ReadIntegerBytes().ToArray(), 128),
                    DQ = IntegerFunc.Normalize(pkSeq.ReadIntegerBytes().ToArray(), 128),
                    InverseQ = IntegerFunc.Normalize(pkSeq.ReadIntegerBytes().ToArray(), 128)
                };

                Console.WriteLine($"PK Modulus = {rsaParameters.Modulus.Length}");
                Console.WriteLine($"PK Exponent = {rsaParameters.Exponent.Length}");
                Console.WriteLine($"PK D = {rsaParameters.D.Length}");
                Console.WriteLine($"PK P = {rsaParameters.P.Length}");
                Console.WriteLine($"PK Q = {rsaParameters.Q.Length}");
                Console.WriteLine($"PK DP = {rsaParameters.DP.Length}");
                Console.WriteLine($"PK DQ = {rsaParameters.Modulus.Length}");
                Console.WriteLine($"PK InverseQ = {rsaParameters.InverseQ.Length}");
                
                RSA rsaKey = RSA.Create();
                rsaKey.ImportParameters(rsaParameters);
                return rsaKey;
            }
            else
            {
                // PKCS#8 parsing
                AsnReader rsnReader = new AsnReader(pemBytes, AsnEncodingRules.DER);
                AsnReader sequence = rsnReader.ReadSequence();

                BigInteger version = sequence.ReadInteger();
                AsnReader algorithmID = sequence.ReadSequence();
                string objectIdentifier = algorithmID.ReadObjectIdentifier();
                algorithmID.ReadNull();

                byte[] privateKeyOctet = sequence.ReadOctetString();
                AsnReader pkReader = new AsnReader(privateKeyOctet, AsnEncodingRules.DER);
                AsnReader pkSeq = pkReader.ReadSequence();

                BigInteger rsaPKVersion = pkSeq.ReadInteger();
                RSAParameters rsaParameters = new RSAParameters
                {
                    Modulus = IntegerFunc.Normalize(pkSeq.ReadIntegerBytes().ToArray(), 256),
                    Exponent = IntegerFunc.Normalize(pkSeq.ReadIntegerBytes().ToArray()),
                    D = IntegerFunc.Normalize(pkSeq.ReadIntegerBytes().ToArray(), 256),
                    P = IntegerFunc.Normalize(pkSeq.ReadIntegerBytes().ToArray(), 128),
                    Q = IntegerFunc.Normalize(pkSeq.ReadIntegerBytes().ToArray(), 128),
                    DP = IntegerFunc.Normalize(pkSeq.ReadIntegerBytes().ToArray(), 128),
                    DQ = IntegerFunc.Normalize(pkSeq.ReadIntegerBytes().ToArray(), 128),
                    InverseQ = IntegerFunc.Normalize(pkSeq.ReadIntegerBytes().ToArray(), 128)
                };

                RSA rsaKey = RSA.Create();
                rsaKey.ImportParameters(rsaParameters);
                return rsaKey;
            }
        }

        internal static string CreateJwt(RSA rsaKey, string appId)
        {
            RsaSecurityKey securityKey = new RsaSecurityKey(rsaKey);
            SigningCredentials creds = new SigningCredentials(securityKey, SecurityAlgorithms.RsaSha256);

            DateTimeOffset now = DateTimeOffset.UtcNow;
            JwtPayload payload = new JwtPayload
            {
                { "iat", now.ToUnixTimeSeconds() },
                { "exp", now.AddMinutes(10).ToUnixTimeSeconds() },   // 10 minutes later
                { "iss", appId }
            };

            var header = new JwtHeader(new SigningCredentials(new RsaSecurityKey(rsaKey), SecurityAlgorithms.RsaSha256));
            var token = new JwtSecurityToken(header, payload);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        internal static async Task<string> GetInstallationToken(string applicationName, string jwt, string installationId)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github+json"));
                client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0");

                var url = $"https://api.github.com/app/installations/{installationId}/access_tokens";
                var response = await client.PostAsync(url, null);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                using (var doc = JsonDocument.Parse(json))
                {
                    return doc.RootElement.GetProperty("token").GetString();
                }
            }
        }
    }
}
