namespace MvcStudy.Models
{
    public partial interface IKitchenRepository
    {
        bool AddMeal(tbl_meal newMeal, out string error);
        bool DeleteMeal(long mealId, out string error);
        bool UpdateMeal(tbl_meal meal, out string error);
        tbl_meal[] GetMeals(tbl_meal filterMeal, out string error);
        long GetTblMealNextId();
    }
}