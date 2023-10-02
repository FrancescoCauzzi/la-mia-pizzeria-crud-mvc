using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using la_mia_pizzeria_crud_mvc.Models;
using la_mia_pizzeria_crud_mvc.Database;
using Microsoft.Docs.Samples;

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
                //ModelState.Remove("ImageUrl");
                if (!ModelState.IsValid)
                {
                    return View("Create", data);
                }
                if (string.IsNullOrEmpty(data.ImageUrl))
                {
                    data.ImageUrl = "/images/default_pizza.png";
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

        // GET: PizzaController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: PizzaController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
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

        // GET: PizzaController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: PizzaController/Delete/5
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


    }
}
