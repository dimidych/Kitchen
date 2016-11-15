namespace MvcStudy.Models
{
    public partial interface IKitchenRepository
    {
        tbl_settings GetSettings(out string error);
        bool UpdateSettings(tbl_settings setting, out string error);
    }
}