using Microsoft.Ajax.Utilities;
using PizzaOrderingWorkApp.EntityFramework;
using PizzaOrderingWorkApp.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Core.Common.CommandTrees;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace PizzaOrderingWorkApp.Controllers
{
 
    public class OrderController : Controller
    {
        // GET: Order
        public ActionResult Index()
        {

            ViewData["CurrentDate"] = DateTime.Now.ToString();
            return View();
        }

        public ActionResult Statistics(Order order)
        {
            using (Data data = new Data())
            {
                int totalNumOfOrders = (from o in data.orders
                                   select o.OrderId).Count();

                string[] names = (from o in data.orders
                             select o.DeveloperName).ToArray();

                ViewBag.TotalOrders = "TOTAL ORDERS PLACED: " + totalNumOfOrders;

                string pizzas = System.Configuration.ConfigurationManager.AppSettings["Pizzas"];
                string drinks = System.Configuration.ConfigurationManager.AppSettings["Drinks"];

                string [] pizzaArr = pizzas.Split(',');
                string [] drinksArr = drinks.Split(',');

                List<string> pizzaList = new List<string>();
                List<string> drinksList = new List<string>();

                foreach (string item in pizzaArr)
                {
                    int pizzaOrdered = (from o in data.orders
                                        where o.Pizza == item
                                            select o.OrderId).Count();
                    pizzaList.Add(item + " Ordered " + pizzaOrdered.ToString() + " time(s)");
                }

                ViewBag.GetPizzaOrders = pizzaList;

                foreach (string item in drinksArr)
                {
                    int drinksOrdered = (from o in data.orders
                                         where o.Drink == item
                                        select o.OrderId).Count();
                    drinksList.Add(item + " Ordered " + drinksOrdered.ToString() + " time(s)");
                }

                ViewBag.GetDrinksOrders = drinksList;
            }

            return View();
        }


        public JsonResult getOrder()
        {
            using(Data data = new Data())
            {
                List<Order> orderList = data.orders.ToList();
                return Json(orderList, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult saveOrder(Order order)
        {     
            
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Orders " + getTodaysDate() + ".txt");


            using (Data data = new Data())
            {
                data.orders.Add(order);
                try
                {

                    data.SaveChanges();

                }
                catch (System.Data.Entity.Validation.DbEntityValidationException ex)
                {
                    foreach (var entityValidationErrors in ex.EntityValidationErrors)
                    {
                        foreach (var validationError in entityValidationErrors.ValidationErrors)
                        {
                            Response.Write("Property: " + validationError.PropertyName + " Error: " + validationError.ErrorMessage);
                        }
                    }
                }
                catch (Exception exe)
                {
                    return Json(new { success = false });
                }

                List<Order> orderList = data.orders.ToList();

                var item = orderList.Last();
                

                if (item.DeveloperName != null || item.Pizza != null || item.Drink != null)
                {
                    using (StreamWriter sw = new StreamWriter(filePath, true))
                    {
       
                        if (new FileInfo(filePath).Length == 0)
                        {
                            sw.WriteLine(orderHeadings());

                        }

                        if (Convert.ToDateTime(item.Cur_Date).ToString("M-d-yyyy") == getTodaysDate())
                        {
                            string name = (from o in data.orders
                                           where o.OrderId == item.OrderId
                                           select o.DeveloperName).First();

                            string pizza = (from o in data.orders
                                            where o.OrderId == item.OrderId
                                            select o.Pizza).First();

                            string drink = (from o in data.orders
                                            where o.OrderId == item.OrderId
                                            select o.Drink).First();

                            int[] ids = (from o in data.orders
                                         where o.DeveloperName == item.DeveloperName
                                         select o.OrderId).ToArray();

                            int secondHighestId;

                            if (ids.Length == 1)
                            {
                                secondHighestId = ids[0];
                            }
                            else
                            {
                                secondHighestId = ids.OrderByDescending(r => r).Skip(1).FirstOrDefault();
                            }

                            string previousOrderDate = (from o in data.orders
                                                        where o.OrderId == secondHighestId
                                                        select o.Cur_Date).First();

                            int numOfOrders = (from o in data.orders
                                               where o.DeveloperName == item.DeveloperName
                                               select o.OrderId).Count();


                            sw.WriteLine(orderDetails(name, pizza, drink, previousOrderDate, numOfOrders));
                           
                        }
                    }
                }
                
                return Json(orderList, JsonRequestBehavior.AllowGet);
            }
        }

        public string orderDetails(string name, string pizza, string drink, string previousOrderDate, int numOfOrders)
        {
            return string.Format("{0,-30}{1,-20}{2,-20}{3,-25}{4,-3}", name, pizza, drink,previousOrderDate, numOfOrders);
        }

        public string orderHeadings()
        {
            return string.Format("{0,-30}{1,-20}{2,-20}{3,-25}{4,-3}", "Developer Name:", "Pizza:","Drink:","Previous Order Date:", "Total Orders:");
        }

        public string getTodaysDate()
        {
            return DateTime.Now.ToString("M-d-yyyy");
        }

 
    }

}