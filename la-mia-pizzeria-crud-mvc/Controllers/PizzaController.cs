using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using la_mia_pizzeria_crud_mvc.Models;
using la_mia_pizzeria_crud_mvc.Database;
using Microsoft.Docs.Samples;
using System.Globalization;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace la_mia_pizzeria_crud_mvc.Controllers
{
    public class PizzaController : Controller
    {
        // GET: PizzaController
        public ActionResult Index()
        {
            try {
                using (PizzeriaContext db = new PizzeriaContext())
                {
                    List<Pizza> pizzas = db.Pizzas.ToList<Pizza>();                    
                     
                    return View("Index", pizzas);
                    

                }

            }
            catch (Exception ex){
                // Log the exception details here
                var errorModel = new ErrorViewModel
                {
                    ErrorMessage = "An error occurred while retrieving the data from the database.",
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
                using (PizzeriaContext db = new PizzeriaContext())
                {
                    Pizza? foundedPizza = db.Pizzas.Where(pizza => pizza.Name == name).FirstOrDefault();

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
            }
            catch (Exception ex)
            {
                var errorModel = new ErrorViewModel
                {
                    ErrorMessage = "An error occurred while retrieving the pizza details.",
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
                using (PizzeriaContext db = new PizzeriaContext())
                {
                    Pizza newPizza = new Pizza();
                    newPizza.Name = data.Name;
                    newPizza.Description = data.Description;
                    
                    newPizza.Price = data.Price;                    
                    newPizza.ImageUrl = data.ImageUrl;

                    db.Pizzas.Add(newPizza);
                    db.SaveChanges();

                    return RedirectToAction("Index");
                }

            }
            catch (Exception ex)
            {
                var errorModel = new ErrorViewModel
                {
                    ErrorMessage = $"An error occurred while inserting the new pizza in the database: {ex.InnerException}",
                    RequestId = HttpContext.TraceIdentifier 
                };
                return View("Error", errorModel);

            }
            
        }       


        // GET: PizzaController/Update/5
        public ActionResult Update(string name)
        {
            try {
                using (PizzeriaContext db = new PizzeriaContext())
                {
                    Pizza? pizzaToEdit = db.Pizzas.Where(Pizza => Pizza.Name == name).FirstOrDefault();

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
            }
            catch(Exception ex) {
                var errorModel = new ErrorViewModel
                {
                    ErrorMessage = $"An error occurred: {ex.Message}",
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
                using(PizzeriaContext db = new PizzeriaContext())
                {
                    Pizza? PizzaToUpdate = db.Pizzas.Find(id);

                    if (PizzaToUpdate != null)
                    {
                        EntityEntry<Pizza> entryEntity = db.Entry(PizzaToUpdate);
                        entryEntity.CurrentValues.SetValues(modifiedPizza);

                        db.SaveChanges();

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
                
            }
            catch(Exception ex)
            {
                var errorModel = new ErrorViewModel
                {
                    ErrorMessage = $"An error occurred: {ex.Message}",
                    RequestId = HttpContext.TraceIdentifier
                };
                return View("Error", errorModel);

            }
        }

        // GET: PizzaController/Delete/5
        public ActionResult Delete(int id)
        {
            try {

                using (PizzeriaContext db = new PizzeriaContext())
                {

                    Pizza? PizzaToDelete = db.Pizzas.Where(Pizza => Pizza.Id == id).FirstOrDefault();

                    if (PizzaToDelete != null)
                    {
                        db.Pizzas.Remove(PizzaToDelete);
                        db.SaveChanges();

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

            }
            catch(Exception ex){
                var errorModel = new ErrorViewModel
                {
                    ErrorMessage = $"An error occurred: {ex.Message}",
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
