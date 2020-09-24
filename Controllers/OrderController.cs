using Microsoft.Ajax.Utilities;
using PizzaOrderingWorkApp.EntityFramework;
using PizzaOrderingWorkApp.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
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

            string todaysdate = DateTime.Now.ToString("M-d-yyyy");
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Orders " + todaysdate + ".txt");
 

            using (Data data = new Data())
            using (StreamWriter sw = new StreamWriter(filePath,true))
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
                List<Order> orderList = data.orders.ToList();

                   
                if (new FileInfo(filePath).Length == 0)
                {
                    sw.WriteLine(orderHeadings());
                }

                var item = orderList.Last();


                if (Convert.ToDateTime(item.Cur_Date).ToString("M-d-yyyy") == todaysdate)
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
                return Json(orderList, JsonRequestBehavior.AllowGet);
            }
        }

        public string orderDetails(string name, string pizza, string drink, string previousOrderDate, int numOfOrders)
        {
            return string.Format("{0,-50}{1,-30}{2,-30}{3,-40}{4,-3}", name, pizza, drink,previousOrderDate, numOfOrders);
        }

        public string orderHeadings()
        {
            return string.Format("{0,-50}{1,-30}{2,-30}{3,-40}{4,-3}", "Developer Name", "Pizza","Drink","Previous Order Date", "Total Orders");
        }
    }

}