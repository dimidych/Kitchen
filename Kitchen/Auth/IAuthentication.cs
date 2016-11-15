using MvcStudy.Models;

namespace MvcStudy.Auth
{
    public interface IAuthentication
    {
        tbl_people CurrentUser { get; }
        bool IsCurrentUserInAdminRole { get; }
        tbl_people Login(string login, string password);
        void Logout();
    }
}