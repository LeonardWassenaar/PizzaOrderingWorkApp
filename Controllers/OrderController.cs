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
    }
}