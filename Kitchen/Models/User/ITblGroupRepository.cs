namespace MvcStudy.Models
{
    public partial interface IKitchenRepository
    {
        bool AddGroup(tbl_group newGroup, out string error);
        bool DeleteGroup(long groupId, out string error);
        bool UpdateGroup(tbl_group group, out string error);
        tbl_group[] GetGroups(tbl_group filterGroup, out string error);
        tbl_group GetGroupByUser(long userId, out string error);
        long GetTblGroupNextId();
    }
}