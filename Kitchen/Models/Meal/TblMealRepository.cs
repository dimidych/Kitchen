using System;
using System.Linq;

namespace MvcStudy.Models
{
    public partial class KitchenRepository : IKitchenRepository
    {
        public bool AddMeal(tbl_meal newMeal, out string error)
        {
            error = string.Empty;

            try
            {
                if (DbCtx == null)
                    throw new Exception("Отсутствует подключение к БД");

                if (newMeal == null)
                    throw new Exception("Еда не определена");

                if (string.IsNullOrEmpty(newMeal.meal_name.Trim()))
                    throw new Exception("Наименование еды не может быть пустым");

                if (newMeal.id < 1)
                    newMeal.id = GetTblMealNextId();

                var existedMeal =
                    DbCtx.tbl_meal.FirstOrDefault(
                        x => x.meal_name.Trim().ToLower().Equals(newMeal.meal_name.Trim().ToLower()));

                if (existedMeal != null)
                    throw new Exception("Еда с таким наименованием уже существует");

                DbCtx.tbl_meal.Add(newMeal);

                if (DbCtx.SaveChanges() <= 0)
                    throw new Exception("Не удалось добавить еду");

                return true;
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return false;
            }
        }

        public bool DeleteMeal(long mealId, out string error)
        {
            error = string.Empty;

            try
            {
                if (DbCtx == null)
                    throw new Exception("Отсутствует подключение к БД");

                if (mealId < 1)
                    throw new Exception("Не указан ИД еды");

                var selected = DbCtx.tbl_meal.FirstOrDefault(x => x.id == mealId);

                if (selected == null)
                    throw new Exception("Еда не найдена");

                DbCtx.tbl_meal.Remove(selected);

                if (DbCtx.SaveChanges() <= 0)
                    throw new Exception("Не удалось удалить еду");

                return true;
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return false;
            }
        }

        public bool UpdateMeal(tbl_meal meal, out string error)
        {
            error = string.Empty;

            try
            {
                if (DbCtx == null)
                    throw new Exception("Отсутствует подключение к БД");

                if (meal == null)
                    throw new Exception("Еда не определена");

                if (meal.id < 1)
                    throw new Exception("Не указан ИД еды");

                if (string.IsNullOrEmpty(meal.meal_name.Trim()))
                    throw new Exception("Наименование еды не может быть пустым");

                var selected = DbCtx.tbl_meal.FirstOrDefault(x => x.id == meal.id);

                if (selected == null)
                    throw new Exception("Еда не найдена");

                selected.meal_name = meal.meal_name;

                if (DbCtx.SaveChanges() <= 0)
                    throw new Exception("Не удалось изменить еду");

                return true;
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return false;
            }
        }

        public tbl_meal[] GetMeals(tbl_meal filterMeal, out string error)
        {
            tbl_meal[] result = null;
            error = string.Empty;

            try
            {
                if (DbCtx == null)
                    throw new Exception("Отсутствует подключение к БД");

                if (filterMeal != null && filterMeal.id > 0)
                    result = DbCtx.tbl_meal.Where(x => x.id == filterMeal.id).ToArray();
                else
                    result = DbCtx.tbl_meal.ToArray();
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return null;
            }

            return result;
        }

        public long GetTblMealNextId()
        {
            if (DbCtx == null)
                throw new Exception("Отсутствует подключение к БД");

            return DbCtx.tbl_meal.Any() ? DbCtx.tbl_meal.Max(x => x.id) + 1 : 1;
        }
    }
}