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
            // get current date for date input
            ViewData["CurrentDate"] = DateTime.Now.ToString();
            return View();
        }

        public ActionResult Statistics(Order order)
        {
            using (Data data = new Data())
            {
                // LINQ total number of orders
                int totalNumOfOrders = (from o in data.orders
                                   select o.OrderId).Count();

                // LINQ gets developer name

                string[] names = (from o in data.orders
                             select o.DeveloperName).ToArray();

                //viewbag for total number of orders 
                ViewBag.TotalOrders = "TOTAL ORDERS PLACED: " + totalNumOfOrders;

                //gets product info from web config
                string pizzas = System.Configuration.ConfigurationManager.AppSettings["Pizzas"];
                string drinks = System.Configuration.ConfigurationManager.AppSettings["Drinks"];

                // splits product info(s) into array (split by comma)
                string [] pizzaArr = pizzas.Split(',');
                string [] drinksArr = drinks.Split(',');

                List<string> pizzaList = new List<string>();
                List<string> drinksList = new List<string>();

                //add ordered pizza count to string list

                foreach (string item in pizzaArr)
                {
                    int pizzaOrdered = (from o in data.orders
                                        where o.Pizza == item
                                            select o.OrderId).Count();
                    pizzaList.Add(item + " Ordered " + pizzaOrdered.ToString() + " time(s)");
                }

                // Viewbag to display ordered pizza count
                ViewBag.GetPizzaOrders = pizzaList;

                //add ordered drinks count to string list
                foreach (string item in drinksArr)
                {
                    int drinksOrdered = (from o in data.orders
                                         where o.Drink == item
                                        select o.OrderId).Count();
                    drinksList.Add(item + " Ordered " + drinksOrdered.ToString() + " time(s)");
                }

                // Viewbag to display ordered drinks count
                ViewBag.GetDrinksOrders = drinksList;
            }

            return View();
        }


        public JsonResult getOrder()
        {
            using(Data data = new Data())
                //get orders from data class
            {
                List<Order> orderList = data.orders.ToList();
                return Json(orderList, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult saveOrder(Order order)
        {     
            //current file path in project directory
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Orders " + getTodaysDate() + ".txt");

            // add orders info to db using the Data class, SaveChanges()
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
                    //return false when there is a failure with the data saves
                    return Json(new { success = false });
                }

                List<Order> orderList = data.orders.ToList();

                //gets last placed order to write into file (newly added to front-end)
                var item = orderList.Last();
                
                //checks if the imput values are not null
                if (item.DeveloperName != null || item.Pizza != null || item.Drink != null)
                {
                    using (StreamWriter sw = new StreamWriter(filePath, true))
                    {
                        // check if the file has no content, if not, then write headings to file
                        if (new FileInfo(filePath).Length == 0)
                        {
                            sw.WriteLine(orderHeadings());

                        }

                        
                        if (Convert.ToDateTime(item.Cur_Date).ToString("M-d-yyyy") == getTodaysDate())
                        {
                            //gets the developer name by using the newly added orderId in list
                            string name = (from o in data.orders
                                           where o.OrderId == item.OrderId
                                           select o.DeveloperName).First();

                            //gets the pizza name by using the newly added orderId in list
                            string pizza = (from o in data.orders
                                            where o.OrderId == item.OrderId
                                            select o.Pizza).First();

                            //gets the drink name by using the newly added orderId in list
                            string drink = (from o in data.orders
                                            where o.OrderId == item.OrderId
                                            select o.Drink).First();

                            // gets the ids of orders by developer name
                            int[] ids = (from o in data.orders
                                         where o.DeveloperName == item.DeveloperName
                                         select o.OrderId).ToArray();

                            int secondHighestId;

                            // determines the second highest id (auto-incremented in db), 
                            //which can be used to determine previous order data for the developer, 
                            //see previousOrderDate LINQ query below
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

                            //gets number of orders for the particular developer
                            int numOfOrders = (from o in data.orders
                                               where o.DeveloperName == item.DeveloperName
                                               select o.OrderId).Count();


                            //writes order details to file
                            sw.WriteLine(orderDetails(name, pizza, drink, previousOrderDate, numOfOrders));
                           
                        }
                    }
                }
                
                return Json(orderList, JsonRequestBehavior.AllowGet);
            }
        }

        public string orderDetails(string name, string pizza, string drink, string previousOrderDate, int numOfOrders)
        {
            //formated line to display order details
            return string.Format("{0,-30}{1,-20}{2,-20}{3,-25}{4,-3}", name, pizza, drink,previousOrderDate, numOfOrders);
        }

        public string orderHeadings()
        {
            //formated line to display order headings
            return string.Format("{0,-30}{1,-20}{2,-20}{3,-25}{4,-3}", "Developer Name:", "Pizza:","Drink:","Previous Order Date:", "Total Orders:");
        }

        public string getTodaysDate()
        {
            //gets current date without time
            return DateTime.Now.ToString("M-d-yyyy");
        }

 
    }

}