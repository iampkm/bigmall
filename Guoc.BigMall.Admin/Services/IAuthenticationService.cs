namespace Guoc.BigMall.Admin.Services
{
    public interface IAuthenticationService
    {
        void SignIn(string userName, string accountInfo, bool createPersistentCookie);
        void SignOut();
    }
}
