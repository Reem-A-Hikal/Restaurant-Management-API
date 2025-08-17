//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Rest.API.Models;
//using Rest.API.Services.Interfaces;

//namespace Rest.API.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class DeliveryController : ControllerBase
//    {
//        private readonly IDeliveryService _deliveryService;

//        public DeliveryController(IDeliveryService deliveryService)
//        {
//            _deliveryService = deliveryService;
//        }

//        // GET: api/delivery
//        [HttpGet]
//        public async Task<ActionResult<IEnumerable<Delivery>>> GetAll()
//        {
//            var deliveries = await _deliveryService.GetAllDeliveriesAsync();
//            return Ok(deliveries);
//        }

//        // GET: api/delivery/5
//        [HttpGet("{id}")]
//        public async Task<ActionResult<Delivery>> GetById(int id)
//        {
//            var delivery = await _deliveryService.GetDeliveryByIdAsync(id);
//            if (delivery == null)
//            {
//                return NotFound();
//            }
//            return Ok(delivery);
//        }

//        // POST: api/delivery
//        [HttpPost]
//        public async Task<ActionResult<Delivery>> Create([FromBody] Delivery delivery)
//        {
//            if (!ModelState.IsValid)
//                return BadRequest(ModelState);

//            await _deliveryService.AddDeliveryAsync(delivery);

//            // Return created resource URI and object
//            return CreatedAtAction(nameof(GetById), new { id = delivery.DeliveryId }, delivery);
//        }

//        // PUT: api/delivery/5
//        [HttpPut("{id}")]
//        public async Task<IActionResult> Update(int id, [FromBody] Delivery delivery)
//        {
//            if (id != delivery.DeliveryId)
//                return BadRequest("Delivery ID mismatch");

//            if (!ModelState.IsValid)
//                return BadRequest(ModelState);

//            var existingDelivery = await _deliveryService.GetDeliveryByIdAsync(id);
//            if (existingDelivery == null)
//                return NotFound();

//            existingDelivery.Status = delivery.Status;
//            existingDelivery.StatusChangeTime = delivery.StatusChangeTime;
//            existingDelivery.DeliveryStartTime = delivery.DeliveryStartTime;
//            existingDelivery.DeliveryEndTime = delivery.DeliveryEndTime;
//            existingDelivery.Latitude = delivery.Latitude;
//            existingDelivery.Longitude = delivery.Longitude;
//            existingDelivery.Notes = delivery.Notes;
//            existingDelivery.DeliveryPersonId = delivery.DeliveryPersonId;
//            existingDelivery.OrderId = delivery.OrderId;

//            await _deliveryService.UpdateDeliveryAsync(existingDelivery);

//            return NoContent();
//        }

//        // DELETE: api/delivery/5
//        [HttpDelete("{id}")]
//        public async Task<IActionResult> Delete(int id)
//        {
//            var existingDelivery = await _deliveryService.GetDeliveryByIdAsync(id);
//            if (existingDelivery == null)
//                return NotFound();

//            await _deliveryService.DeleteDeliveryAsync(id);

//            return NoContent();
//        }
//    }
//}

