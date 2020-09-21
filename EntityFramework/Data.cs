using PizzaOrderingWorkApp.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace PizzaOrderingWorkApp.EntityFramework
{
    public class Data:DbContext
    {
        public Data() : base("DbCon") { }

        public DbSet<Order> orders { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Order>().ToTable("Order");
            modelBuilder.Entity<Order>().HasKey(x => x.OrderId);
        }
    }
}