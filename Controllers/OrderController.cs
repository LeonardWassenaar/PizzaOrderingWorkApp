using PizzaOrderingWorkApp.EntityFramework;
using PizzaOrderingWorkApp.Models;
using System;
using System.Collections.Generic;
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
            return View();
        }

        public JsonResult getOrder()
        {
            Data data = new Data();
            List<Order> orderList = data.orders.ToList();
            return Json(orderList, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult saveOrder(Order order)
        {

            Data data = new Data();
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
            return Json(orderList, JsonRequestBehavior.AllowGet);
        }
    }
}