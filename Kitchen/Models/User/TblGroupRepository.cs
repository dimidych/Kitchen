using System;
using System.Linq;

namespace MvcStudy.Models
{
    public partial class KitchenRepository : IKitchenRepository
    {
        public bool AddGroup(tbl_group newGroup, out string error)
        {
            error = string.Empty;

            try
            {
                if (DbCtx == null)
                    throw new Exception("Отсутствует подключение к БД");

                if (newGroup == null || string.IsNullOrEmpty(newGroup.group_name.Trim()))
                    return false;

                if (newGroup.id < 1)
                    newGroup.id = GetTblGroupNextId();

                var selected =
                    DbCtx.tbl_group.FirstOrDefault(
                        x =>
                            string.Equals(x.group_name.Trim(), newGroup.group_name.Trim(),
                                StringComparison.InvariantCultureIgnoreCase));

                if (selected != null)
                    return false;

                DbCtx.tbl_group.Add(newGroup);
                return DbCtx.SaveChanges() > 0;
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return false;
            }
        }

        public bool DeleteGroup(long groupId, out string error)
        {
            error = string.Empty;

            try
            {
                if (DbCtx == null)
                    throw new Exception("Отсутствует подключение к БД");

                if (groupId < 1)
                    return false;

                var selected = DbCtx.tbl_group.FirstOrDefault(x => x.id == groupId);

                if (selected == null)
                    return false;

                if (selected.tbl_people != null)
                    return false;

                DbCtx.tbl_group.Remove(selected);
                return DbCtx.SaveChanges() > 0;
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return false;
            }
        }

        public bool UpdateGroup(tbl_group group, out string error)
        {
            error = string.Empty;

            try
            {
                if (DbCtx == null)
                    throw new Exception("Отсутствует подключение к БД");

                if (group == null || group.id < 1)
                    return false;

                var selected = DbCtx.tbl_group.FirstOrDefault(x => x.id == group.id);
                var existed =
                    DbCtx.tbl_group.FirstOrDefault(
                        x =>
                            string.Equals(x.group_name.Trim(), group.group_name.Trim(),
                                StringComparison.InvariantCultureIgnoreCase));

                if (selected == null || existed != null)
                    return false;

                selected.group_name = group.group_name;
                selected.is_admin = group.is_admin;
                return DbCtx.SaveChanges() > 0;
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return false;
            }
        }

        public tbl_group[] GetGroups(tbl_group filterGroup, out string error)
        {
            error = string.Empty;

            try
            {
                if (DbCtx == null)
                    throw new Exception("Отсутствует подключение к БД");

                tbl_group[] result = null;

                if (filterGroup != null && filterGroup.id > 0)
                    result = DbCtx.tbl_group.Where(x => x.id == filterGroup.id).ToArray();
                else
                    result = DbCtx.tbl_group.ToArray();

                return result;
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return null;
            }
        }

        public tbl_group GetGroupByUser(long userId, out string error)
        {
            error = string.Empty;

            try
            {
                if (DbCtx == null)
                    throw new Exception("Отсутствует подключение к БД");

                var user = DbCtx.tbl_people.FirstOrDefault(x => x.id == userId);
                return user == null ? null : user.tbl_group;
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return null;
            }
        }

        public long GetTblGroupNextId()
        {
            if (DbCtx == null)
                throw new Exception("Отсутствует подключение к БД");

            return DbCtx.tbl_group.Any() ? DbCtx.tbl_group.Max(x => x.id) + 1 : 1;
        }
    }
}