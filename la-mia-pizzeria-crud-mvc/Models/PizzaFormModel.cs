using la_mia_pizzeria_crud_mvc.Models.DataBaseModels;
namespace la_mia_pizzeria_crud_mvc.Models
{
    public class PizzaFormModel
    {
        public Pizza Pizza { get; set; }

        public List<PizzaCategory>? PizzaCategories { get; set; }
    }
}
