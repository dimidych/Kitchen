namespace MvcStudy.Models
{
    public partial interface IKitchenRepository
    {
        bool AddUser(tbl_people newUser, out string error);
        bool DeleteUser(long userId, out string error);
        bool UpdateUser(tbl_people user, bool forProfile, out string error);
        tbl_people[] GetUsers(tbl_people filterUser, out string error);
        long GetTblPeopleNextId();
    }
}