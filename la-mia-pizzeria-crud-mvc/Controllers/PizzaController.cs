using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using la_mia_pizzeria_crud_mvc.Models;
using la_mia_pizzeria_crud_mvc.Database;
using Microsoft.Docs.Samples;
using System.Globalization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using la_mia_pizzeria_crud_mvc.CustomLoggers;
using la_mia_pizzeria_crud_mvc.Models.DataBaseModels;
using Azure;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace la_mia_pizzeria_crud_mvc.Controllers
{
    public class PizzaController : Controller
    {
        // Dependency Injection
        private ICustomLogger _myLogger;

        private PizzeriaContext _myDb;

        // Dependency Injection in the constructor
        public PizzaController(ICustomLogger myLogger, PizzeriaContext myDb)
        {
            _myLogger = myLogger;
            _myDb = myDb;
        }



        // GET: PizzaController
        public ActionResult Index()
        {
            try
            {
                _myLogger.WriteLog("User has reached the page Pizza > Index");

                List<Pizza> pizzas = _myDb.Pizzas.ToList<Pizza>();

                return View("Index", pizzas);

            }
            catch (Exception ex)
            {
                // Log the exception details here
                var errorModel = new ErrorViewModel
                {
                    //ErrorMessage = "An error occurred while retrieving the data from the database.",
                    ErrorMessage = ex.Message,
                    RequestId = HttpContext.TraceIdentifier // This is optional, just if you want to include the request ID
                };
                return View("Error", errorModel);

            }

        }

        // GET: PizzaController/Details/5


        public ActionResult Details(string name)
        {
            try
            {
                _myLogger.WriteLog($"User has reached the page Pizza {name} > Details");


                Pizza? foundedPizza = _myDb.Pizzas.Where(pizza => pizza.Name == name).Include(pizza => pizza.PizzaCategory).Include(pizza => pizza.Ingredients).FirstOrDefault();

                if (foundedPizza == null)
                {

                    var errorModel = new ErrorViewModel
                    {
                        ErrorMessage = $"The item '{name}' was not found!",
                        RequestId = HttpContext.TraceIdentifier
                    };
                    return View("Error", errorModel);
                }
                else
                {
                    return View("Details", foundedPizza);
                }

            }
            catch (Exception ex)
            {
                var errorModel = new ErrorViewModel
                {
                    //ErrorMessage = "An error occurred while retrieving the pizza details.",
                    ErrorMessage = ex.Message,
                    RequestId = HttpContext.TraceIdentifier // This is optional, just if you want to include the request ID
                };
                return View("Error", errorModel);
            }
        }



        // GET: PizzaController/Create
        [HttpGet]
        public ActionResult Create()
        {
            List<PizzaCategory> pizzaCategories = _myDb.PizzaCategories.ToList();

            List<SelectListItem> allIngredientsSelectList = new List<SelectListItem>();
            List<Ingredient> databaseAllIngredients = _myDb.Ingredients.ToList();

            foreach (Ingredient ingredient in databaseAllIngredients)
            {
                allIngredientsSelectList.Add(
                    new SelectListItem
                    {
                        Text = ingredient.Name,
                        Value = ingredient.Id.ToString()
                    });
            }

            PizzaFormModel formModel = new PizzaFormModel { Pizza = new Pizza(), PizzaCategories = pizzaCategories, Ingredients = allIngredientsSelectList };
            return View("Create", formModel);
        }

        // POST: PizzaController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(PizzaFormModel data)
        {
            try
            {
                if (string.IsNullOrEmpty(data.Pizza.ImageUrl))
                {
                    data.Pizza.ImageUrl = "/images/default_pizza.png";
                    //ModelState.Remove("ImageUrl");
                }
                if (!ModelState.IsValid)
                {
                    List<PizzaCategory> pizzaCategories = _myDb.PizzaCategories.ToList();
                    data.PizzaCategories = pizzaCategories;
                    // now I initialize a new emptu list
                    List<SelectListItem> allIngredientsSelectList = new List<SelectListItem>();
                    // and I fill it with all the ingredients because I want to show them in the form if the form is not filled well so the user can input them again

                    List<Ingredient> databaseAllIngredients = _myDb.Ingredients.ToList();

                    foreach (Ingredient ingredient in databaseAllIngredients)
                    {
                        allIngredientsSelectList.Add(
                            new SelectListItem
                            {
                                Text = ingredient.Name,
                                Value = ingredient.Id.ToString()
                            });
                    }

                    data.Ingredients = allIngredientsSelectList;

                    return View("Create", data);
                }
                // here if the form is filled well I can continue the process and eventually add the pizza to the database
                data.Pizza.Ingredients = new List<Ingredient>();

                //if I have any ingredient selected
                if (data.SelectedIngredientsId != null)
                {
                    foreach (string IngredientSelectedId in data.SelectedIngredientsId)
                    {
                        // down here I need to transform the string into an integer because the database stores the id as an integer but the form send it as a string
                        int intIngredientSelectedId = int.Parse(IngredientSelectedId);
                        // down here I get the ingredient from the database matching the id of the selected ingredient
                        Ingredient? ingredientInDb = _myDb.Ingredients.Where(Ingredient => Ingredient.Id == intIngredientSelectedId).FirstOrDefault();
                        // after a control for null value I add it to the list of ingredients of the pizza
                        if (ingredientInDb != null)
                        {
                            data.Pizza.Ingredients.Add(ingredientInDb);
                        }
                    }

                } // else I do not add any ingredient because the user has not selected any ingredient, the list of the ingredients to the pizza I want to add to the db remains empty       

                //finally I create the pizza and I add it to the database with all the parameters it needs to store

                Pizza newPizza = new()
                {
                    Name = data.Pizza.Name,
                    Description = data.Pizza.Description,
                    Price = data.Pizza.Price,
                    ImageUrl = data.Pizza.ImageUrl,
                    PizzaCategoryId = data.Pizza.PizzaCategoryId,
                    Ingredients = data.Pizza.Ingredients
                };

                _myDb.Pizzas.Add(newPizza);
                _myDb.SaveChanges();

                return RedirectToAction("Index");


            }
            catch (Exception ex)
            {
                string? innerException = ex.InnerException.ToString();
                var errorModel = new ErrorViewModel
                {
                    //ErrorMessage = $"An error occurred while inserting the new pizza in the database: {ex.InnerException}",

                    ErrorMessage = $"{ex.Message}: {innerException}",
                    RequestId = HttpContext.TraceIdentifier
                };
                return View("Error", errorModel);

            }

        }


        // GET: PizzaController/Update/5
        [HttpGet]
        public ActionResult Update(string name)
        {
            try
            {

                Pizza? pizzaToEdit = _myDb.Pizzas.Where(Pizza => Pizza.Name == name).FirstOrDefault();

                if (pizzaToEdit == null)
                {
                    var errorModel = new ErrorViewModel
                    {
                        ErrorMessage = $"The pizza you are searching has not been found",
                        RequestId = HttpContext.TraceIdentifier
                    };
                    return View("Error", errorModel);
                }
                else
                {
                    List<PizzaCategory> pizzaCategories = _myDb.PizzaCategories.ToList();

                    PizzaFormModel formModel = new PizzaFormModel { Pizza = pizzaToEdit, PizzaCategories = pizzaCategories };
                    return View("Update", formModel);
                }


            }
            catch (Exception ex)
            {
                var errorModel = new ErrorViewModel
                {
                    //ErrorMessage = $"An error occurred: {ex.Message}",
                    ErrorMessage = ex.Message,
                    RequestId = HttpContext.TraceIdentifier
                };
                return View("Error", errorModel);
            }

        }

        // POST: PizzaController/Update/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update(int id, PizzaFormModel data)
        {
            try
            {

                if (!ModelState.IsValid)
                {
                    List<PizzaCategory> pizzaCategories = _myDb.PizzaCategories.ToList();
                    data.PizzaCategories = pizzaCategories;
                    return View("Update", data);
                }

                Pizza? PizzaToUpdate = _myDb.Pizzas.Find(id);

                if (PizzaToUpdate != null)
                {
                    // The code below threw an error
                    /*
                    EntityEntry<Pizza> entryEntity = _myDb.Entry(PizzaToUpdate);
                    entryEntity.CurrentValues.SetValues(data.Pizza);
                    */
                    // this code worked
                    /**/
                    PizzaToUpdate.Name = data.Pizza.Name;
                    PizzaToUpdate.Description = data.Pizza.Description;
                    PizzaToUpdate.Price = data.Pizza.Price;
                    PizzaToUpdate.ImageUrl = data.Pizza.ImageUrl;
                    PizzaToUpdate.PizzaCategoryId = data.Pizza.PizzaCategoryId;

                    _myDb.SaveChanges();

                    return RedirectToAction("Index");
                }
                else
                {
                    var errorModel = new ErrorViewModel
                    {
                        ErrorMessage = $"The pizza you are trying to modify has not been found",
                        RequestId = HttpContext.TraceIdentifier
                    };
                    return View("Error", errorModel);
                }
            }
            catch (Exception ex)
            {
                var errorModel = new ErrorViewModel
                {
                    //ErrorMessage = $"An error occurred: {ex.Message}",
                    ErrorMessage = ex.Message,
                    RequestId = HttpContext.TraceIdentifier
                };
                return View("Error", errorModel);

            }
        }

        // GET: PizzaController/Delete/5
        public ActionResult Delete(int id)
        {
            try
            {
                Pizza? PizzaToDelete = _myDb.Pizzas.Where(Pizza => Pizza.Id == id).FirstOrDefault();

                if (PizzaToDelete != null)
                {
                    _myDb.Pizzas.Remove(PizzaToDelete);
                    _myDb.SaveChanges();

                    return RedirectToAction("Index");
                }
                else
                {
                    var errorModel = new ErrorViewModel
                    {
                        ErrorMessage = $"The pizza you are trying to delete is not present in the database",
                        RequestId = HttpContext.TraceIdentifier
                    };
                    return View("Error", errorModel);
                }
            }
            catch (Exception ex)
            {
                var errorModel = new ErrorViewModel
                {

                    ErrorMessage = ex.Message,
                    RequestId = HttpContext.TraceIdentifier
                };
                return View("Error", errorModel);

            }
        }

        // POST: PizzaController/Delete/5
        /*
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
        */


    }
}
