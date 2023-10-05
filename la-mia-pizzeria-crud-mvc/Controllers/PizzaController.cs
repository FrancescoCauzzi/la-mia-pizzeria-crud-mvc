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

namespace la_mia_pizzeria_crud_mvc.Controllers
{
    public class PizzaController : Controller
    {
        // Dependency Injection
        private ICustomLogger _myLogger;

        private PizzeriaContext _myDb;

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

                
                Pizza? foundedPizza = _myDb.Pizzas.Where(pizza => pizza.Name == name).Include(pizza => pizza.PizzaCategory).FirstOrDefault();

                if (foundedPizza == null)
                {
                    //return NotFound($"The item {name} was not found!");
                    var errorModel = new ErrorViewModel
                    {
                        ErrorMessage = $"The item '{name}' was not found!",
                        RequestId = HttpContext.TraceIdentifier 
                    };
                    return View("Error", errorModel);
                }
                else
                {
                    return View("Details",foundedPizza);
                }
                
            }
            catch (Exception ex)
            {
                var errorModel = new ErrorViewModel
                {
                    //ErrorMessage = "An error occurred while retrieving the pizza details.",
                    ErrorMessage= ex.Message,
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

            PizzaFormModel formModel = new PizzaFormModel { Pizza = new Pizza(), PizzaCategories = pizzaCategories };
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
                    return View("Create", data);
                }
                
                Pizza newPizza = new Pizza();
                newPizza.Name = data.Pizza.Name;
                newPizza.Description = data.Pizza.Description;

                newPizza.Price = data.Pizza.Price;
                newPizza.ImageUrl = data.Pizza.ImageUrl;
                newPizza.PizzaCategoryId = data.Pizza.PizzaCategoryId;

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
                    ErrorMessage= ex.Message,
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
                    
                    ErrorMessage= ex.Message,
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
