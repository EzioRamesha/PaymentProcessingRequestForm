using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApp.DAL.DAL;

namespace WebApp.DAL.Helpers
{
    internal class TokenParts
    {
        public string Reason { get; set; }
        public string UserEmail { get; set; }
        public string ApprovalToken { get; set; }

    }
    internal class EmailToken
    {
        private UserDAL _userDAL = new UserDAL();

        public string Token { get; set; }
        public string Lengths { get; set; }


        public EmailToken GenerateToken(TokenParts tokenParts)
        {
            var reason = tokenParts.Reason;
            var userEmail = tokenParts.UserEmail;
            var approverToken = tokenParts.ApprovalToken;
            if (!(string.IsNullOrEmpty(reason) || string.IsNullOrEmpty(userEmail) || string.IsNullOrEmpty(approverToken)))
            {
                var user = _userDAL.List().Where(w => w.Email.Equals(userEmail, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                if (user != null)
                {
                    byte[] _token = Guid.Parse(approverToken).ToByteArray();

                    byte[] _time = BitConverter.GetBytes(DateTime.UtcNow.ToBinary());
                    byte[] _key = Guid.Parse(user.ExternalUser.SecurityStamp).ToByteArray();
                    byte[] _Id = Encoding.ASCII.GetBytes(userEmail);
                    byte[] _reason = Encoding.ASCII.GetBytes(reason);

                    byte[] data = new byte[_time.Length + _key.Length + _reason.Length + _Id.Length + approverToken.Length];

                    System.Buffer.BlockCopy(_time, 0, data, 0, _time.Length);
                    System.Buffer.BlockCopy(_key, 0, data, _time.Length, _key.Length);
                    System.Buffer.BlockCopy(_reason, 0, data, _time.Length + _key.Length, _reason.Length);
                    System.Buffer.BlockCopy(_Id, 0, data, _time.Length + _key.Length + _reason.Length, _Id.Length);
                    System.Buffer.BlockCopy(_token, 0, data, _time.Length + _key.Length + _reason.Length + _Id.Length, _token.Length);

                    EmailToken token = new EmailToken()
                    {
                        Token = Convert.ToBase64String(data.ToArray()),
                        Lengths = _time.Length + "_" + _key.Length + "_" + _reason.Length + "_" + _Id.Length + "_" + _token.Length
                    };
                    return token;
                }
            }
            return null;
        }

        public TokenParts DecryptEmailToken()
        {
            TokenParts tokenParts = null;
            try
            {
                int[] lengths = Array.ConvertAll(Lengths.Split('_'), int.Parse);
                byte[] data = Convert.FromBase64String(Token);

                byte[] _time = data.Take(lengths[0]).ToArray();
                byte[] _key = data.Skip(lengths[0]).Take(lengths[1]).ToArray();
                byte[] _reason = data.Skip(lengths[0] + lengths[1]).Take(lengths[2]).ToArray();
                byte[] _Id = data.Skip(lengths[0] + lengths[1] + lengths[2]).Take(lengths[3]).ToArray();
                byte[] _token = data.Skip(lengths[0] + lengths[1] + lengths[2] + lengths[3]).Take(lengths[4]).ToArray();


                var time = DateTime.FromBinary(BitConverter.ToInt64(_time, 0));
                tokenParts = new TokenParts
                {
                    ApprovalToken = new Guid(_token).ToString(),
                    Reason = System.Text.Encoding.Default.GetString(_reason),
                    UserEmail = System.Text.Encoding.Default.GetString(_Id)
                };
            }
            catch (Exception)
            {
                tokenParts = null;
                //throw,
            }
            return tokenParts;
        }
    }
}
