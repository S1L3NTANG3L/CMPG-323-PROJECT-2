using ConnectedOfficeAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace ConnectedOfficeAPI.Handler
{
    public class RefereshTokenGenerator : IRefereshTokenGenerator
    {
        private readonly ConnectedOfficeContexts _DBContext;
        public RefereshTokenGenerator(ConnectedOfficeContexts _DBContext)
        {
            this._DBContext = _DBContext;
        }
        public async Task<string> GenerateToken(string username)
        {
            var randomnumber = new byte[32];
            using (var ramdomnumbergenerator = RandomNumberGenerator.Create())
            {
                ramdomnumbergenerator.GetBytes(randomnumber);
                string refreshtoken = Convert.ToBase64String(randomnumber);
                var token = await this._DBContext.Refreshtoken.FirstOrDefaultAsync(item => item.UserId == username);
                if (token != null)
                {
                    token.RefreshToken = refreshtoken;
                }
                else
                {
                    this._DBContext.Refreshtoken.Add(new Refreshtoken()
                    {
                        UserId = username,
                        TokenId = new Random().Next().ToString(),
                        RefreshToken = refreshtoken,
                        IsActive = 1
                    });
                }
                await this._DBContext.SaveChangesAsync();

                return refreshtoken;
            }
        }
    }
}

