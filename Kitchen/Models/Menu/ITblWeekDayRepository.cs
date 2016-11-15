namespace MvcStudy.Models
{
    public partial interface IKitchenRepository
    {
        tbl_weekday[] GetWeekDays(tbl_weekday filter, out string error);
    }
}