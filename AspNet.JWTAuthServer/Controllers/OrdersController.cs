using System;
using System.Collections.Generic;
using System.Web.Http;

namespace AspNet.JWTAuthServer.Controllers
{

    [RoutePrefix("api/Orders")]
    public class OrdersController : ApiController
    {

        [Authorize(Roles = "User")]
		[Route("")]
        public IHttpActionResult Get()
        {
            return Ok(Order.CreateOrders());
        }

    }


    #region Helpers

    public class Order
    {
        public int OrderID { get; set; }
        public string CustomerName { get; set; }
        public string ShipperCity { get; set; }
        public Boolean IsShipped { get; set; }


        public static List<Order> CreateOrders()
        {
            List<Order> OrderList = new List<Order> 
            {
                new Order {OrderID = 10248, CustomerName = "Darth Vader", ShipperCity = "Deathstar III", IsShipped = true },
                new Order {OrderID = 10249, CustomerName = "Luke Skywalker", ShipperCity = "Tatooine", IsShipped = false},
                new Order {OrderID = 10250,CustomerName = "Princess Leia Organa", ShipperCity = "Alderaan", IsShipped = false },
                new Order {OrderID = 10251,CustomerName = "Han Solo", ShipperCity = "Tatooine", IsShipped = false},
                new Order {OrderID = 10252,CustomerName = "Boba Fett", ShipperCity = "Bespin", IsShipped = true}
            };

            return OrderList;
        }
    }

    #endregion
}
