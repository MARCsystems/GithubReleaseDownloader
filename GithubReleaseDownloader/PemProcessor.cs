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
            // Read PEM File
            string pem = File.ReadAllText(pemPath)
                .Replace("-----BEGIN PRIVATE KEY-----", "")
                .Replace("-----END PRIVATE KEY-----", "")
                .Replace("\n", "")
                .Replace("\r", "");
            byte[] pemBytes = Convert.FromBase64String(pem);

            // Parse PKCS#8
            AsnReader rsnReader = new AsnReader(pemBytes, AsnEncodingRules.DER);
            AsnReader sequence = rsnReader.ReadSequence();

            BigInteger version = sequence.ReadInteger();
            var algorithmID = sequence.ReadSequence();
            string objectIdentifier = algorithmID.ReadObjectIdentifier();
            // Read a nullified line
            algorithmID.ReadNull();

            byte[] privateKeyOctet = sequence.ReadOctetString();
            
            // Parse RSA Private Key
            AsnReader pkReader = new AsnReader(privateKeyOctet, AsnEncodingRules.DER);
            AsnReader pkSeq = pkReader.ReadSequence();

            BigInteger rsaPKVersion = pkSeq.ReadInteger();
            RSAParameters rsaParameters = new RSAParameters
            {
                Modulus = pkSeq.ReadIntegerBytes().ToArray(),
                Exponent = pkSeq.ReadIntegerBytes().ToArray(),
                D = pkSeq.ReadIntegerBytes().ToArray(),
                P = pkSeq.ReadIntegerBytes().ToArray(),
                Q = pkSeq.ReadIntegerBytes().ToArray(),
                DP = pkSeq.ReadIntegerBytes().ToArray(),
                DQ = pkSeq.ReadIntegerBytes().ToArray(),
                InverseQ = pkSeq.ReadIntegerBytes().ToArray()
            };

            RSA rsaKey = RSA.Create();
            rsaKey.ImportParameters(rsaParameters);
            return rsaKey;
        }

        internal static string CreateJwt(RSA rsaKey, string appId)
        {
            RsaSecurityKey securityKey = new RsaSecurityKey(rsaKey);
            SigningCredentials creds = new SigningCredentials(securityKey, SecurityAlgorithms.RsaSha256);

            DateTimeOffset now = DateTimeOffset.UtcNow;
            JwtSecurityToken token = new JwtSecurityToken(
                issuer: appId,
                notBefore: now.UtcDateTime,
                expires: now.AddMinutes(10).UtcDateTime,
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        internal static async Task<string> GetInstallationToken(string jwt, string installationId)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.UserAgent.ParseAdd("MyDownloader");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github+json"));

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
