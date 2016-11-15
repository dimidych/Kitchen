using System.Collections.Generic;
using System.Web;

namespace MvcStudy.Models
{
    public partial interface IKitchenRepository
    {
        List<tbl_duty> CalculateDutyPeople(bool forFutureWeek, out string error);
        bool AcceptDutyPeople(List<tbl_duty> dutyPeopleDct, out string error);
        tbl_duty[] GetDutyRecords(tbl_duty filter, out string error);
        bool UpdateDutyMan(tbl_duty dutyRecord, out string error);
        long GetTblDutyNextId();
        bool CheckForFutureWeekEaters(bool forFutureWeek, out string error);
        bool PunishDutyMan(long peopleId, out string error);
        bool ShowDutyAlert(long peopleId, out string error);
    }
}