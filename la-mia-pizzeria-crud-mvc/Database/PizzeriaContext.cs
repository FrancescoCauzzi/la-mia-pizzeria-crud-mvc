﻿using la_mia_pizzeria_crud_mvc.Models.DataBaseModels;
using Microsoft.EntityFrameworkCore;

namespace la_mia_pizzeria_crud_mvc.Database
{
    public class PizzeriaContext : DbContext
    {
        public DbSet<Pizza> Pizzas { get; set; }

        public DbSet<PizzaCategory> PizzaCategories { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Data Source=localhost;Initial Catalog=MVCEFPizzeria;" +
            "Integrated Security=True;TrustServerCertificate=True");
        }

    }
}
