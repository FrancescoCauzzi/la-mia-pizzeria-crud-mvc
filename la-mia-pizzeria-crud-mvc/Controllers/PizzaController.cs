using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using la_mia_pizzeria_crud_mvc.Models;
using la_mia_pizzeria_crud_mvc.Database;
using Microsoft.Docs.Samples;
using System.Globalization;
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

                
                Pizza? foundedPizza = _myDb.Pizzas.Where(pizza => pizza.Name == name).FirstOrDefault();

                if (foundedPizza == null)
                {

                    //return NotFound($"The item {name} was not found!");
                    var errorModel = new ErrorViewModel
                    {
                        ErrorMessage = $"The item '{name}' was not found!",
                        RequestId = HttpContext.TraceIdentifier // This is optional, just if you want to include the request ID
                    };
                    return View("Error", errorModel);
                }
                else
                {
                    return View(foundedPizza);
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
            return View("Create");
        }

        // POST: PizzaController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Pizza data)
        {
            try
            {

                if (string.IsNullOrEmpty(data.ImageUrl))
                {
                    data.ImageUrl = "/images/default_pizza.png";
                    ModelState.Remove("ImageUrl");
                }
                if (!ModelState.IsValid)
                {
                    return View("Create", data);
                }
                
                Pizza newPizza = new Pizza();
                newPizza.Name = data.Name;
                newPizza.Description = data.Description;

                newPizza.Price = data.Price;
                newPizza.ImageUrl = data.ImageUrl;

                _myDb.Pizzas.Add(newPizza);
                _myDb.SaveChanges();

                return RedirectToAction("Index");
                

            }
            catch (Exception ex)
            {
                var errorModel = new ErrorViewModel
                {
                    //ErrorMessage = $"An error occurred while inserting the new pizza in the database: {ex.InnerException}",
                    ErrorMessage = ex.Message,
                    RequestId = HttpContext.TraceIdentifier
                };
                return View("Error", errorModel);

            }

        }


        // GET: PizzaController/Update/5
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
                    return View("Update", pizzaToEdit);
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
        public ActionResult Update(int id, Pizza modifiedPizza)
        {
            try
            {

                if (!ModelState.IsValid)
                {
                    return View("Update", modifiedPizza);
                }
                
                Pizza? PizzaToUpdate = _myDb.Pizzas.Find(id);

                if (PizzaToUpdate != null)
                {
                    EntityEntry<Pizza> entryEntity = _myDb.Entry(PizzaToUpdate);
                    entryEntity.CurrentValues.SetValues(modifiedPizza);

                    _myDb.SaveChanges();

                    return RedirectToAction("Index");
                }
                else
                {
                    var errorModel = new ErrorViewModel
                    {
                        ErrorMessage = $"The pizza you are searching has not been found",
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
                    //ErrorMessage = $"An error occurred: {ex.Message}",
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
