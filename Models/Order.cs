using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PizzaOrderingWorkApp.Models
{
    public class Order
    {
       
        public int OrderId { get; set; }

        [DisplayName("Name")]
        //[Required(ErrorMessage = "This field is required.")]
         public string DeveloperName { get; 
            set; }

        [DisplayName("Pizza")]
        //[Required(ErrorMessage = "This field is required.")]
        public string Pizza { get; set; }

        [DisplayName("Drink")]
        //[Required(ErrorMessage = "This field is required.")]
        public string Drink  { get; set; }

        [DisplayName("Date")]
        //[Required(ErrorMessage = "This field is required.")]
        public string Cur_Date { get; set; }
    }
}