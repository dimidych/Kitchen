using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace MvcStudy.Models.Menu
{
    public class MenuViewModel : tbl_menu
    {
        public IKitchenRepository DbRepository { get; set; }

        public List<MealViewModel> MealCollection { get; set; }

        public string DateView
        {
            get { return date.ToString("dd.MM.yyyy"); }
        }

        public string MealNameView { get; private set; }

        public bool Selected { get; set; }

        public int Qty { get; set; }

        public MenuViewModel()
        {
            Qty = 1;
        }

        public IEnumerable<SelectListItem> MealList
        {
            get
            {
                if (MealCollection == null)
                    InitMealCollection();

                if (!MealCollection.Any())
                    yield break;

                foreach (var meal in MealCollection)
                    yield return new SelectListItem
                    {
                        Text = meal.meal_name,
                        Value = meal.id.ToString(),
                        Selected = (meal.id == id_meal)
                    };
            }
        }

        public MenuViewModel(tbl_menu sourceMenu)
        {
            id = sourceMenu.id;
            id_meal = sourceMenu.id_meal;
            date = sourceMenu.date;
            MealNameView = sourceMenu.tbl_meal.meal_name;
            Qty = 1;
        }

        public void InitMealCollection()
        {
            if (DbRepository == null)
                throw new Exception("Соединение с БД потеряно");

            string error;
            MealCollection = DbRepository.GetMeals(null, out error).Select(x => new MealViewModel(x)).ToList();
        }

        public static tbl_menu CreateClone(MenuViewModel source)
        {
            return new tbl_menu
            {
                id = source.id,
                date = source.date,
                id_meal = source.id_meal,
            };
        }
    }
}