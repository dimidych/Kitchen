using System;
using System.Linq;

namespace MvcStudy.Models
{
    public partial class KitchenRepository : IKitchenRepository
    {
        public tbl_weekday[] GetWeekDays(tbl_weekday filter, out string error)
        {
            error = string.Empty;
            tbl_weekday[] result;

            try
            {
                if (DbCtx == null)
                    throw new Exception("Отсутствует подключение к БД");

                if (filter != null && filter.id > 0)
                    result = DbCtx.tbl_weekday.Where(x => x.id == filter.id).ToArray();
                else
                    result = DbCtx.tbl_weekday.ToArray();
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return null;
            }

            return result;
        }
    }
}