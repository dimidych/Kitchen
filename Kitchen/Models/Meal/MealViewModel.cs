namespace MvcStudy.Models
{
    public class MealViewModel : tbl_meal
    {
        public bool Selected { get; set; }

        public MealViewModel()
        {}

        public MealViewModel(tbl_meal sourceMeal)
        {
            id = sourceMeal.id;
            meal_name = sourceMeal.meal_name;
        }
    }
}