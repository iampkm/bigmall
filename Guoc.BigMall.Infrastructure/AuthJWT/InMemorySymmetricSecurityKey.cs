using Microsoft.IdentityModel.Tokens;

namespace Guoc.BigMall.Infrastructure.AuthJWT
{
    internal class InMemorySymmetricSecurityKey : SecurityKey
    {
        private byte[] v;

        public InMemorySymmetricSecurityKey(byte[] v)
        {
            this.v = v;
        }

        public override int KeySize { get { return this.v.Length; } }
    }
}