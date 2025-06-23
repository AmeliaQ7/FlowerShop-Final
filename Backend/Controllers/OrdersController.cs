using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Kwiaciarnia.Models;

namespace Kwiaciarnia.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly OrderContext _context;

        public OrdersController(OrderContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrders()
        {
            try
            {
                return await _context.Orders.ToListAsync();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Wystąpił błąd serwera: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetOrder(int id)
        {
            try
            {
                var order = await _context.Orders.FindAsync(id);
                if (order == null)
                {
                    return NotFound($"Nie znaleziono zamówienia o id = {id}");
                }
                return order;
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Wystąpił błąd serwera: {ex.Message}");
            }
        }

        [HttpGet("byname/{Client}")]
        public async Task<ActionResult<Order>> GetOrderByName(string Client)
        {
            try
            {
                var order = await _context.Orders
                    .FirstOrDefaultAsync(p => p.Client.ToLower() == Client.ToLower());

                if (order == null)
                {
                    return NotFound($"Nie znaleziono zamówienia dla klienta: {Client}");
                }

                return order;
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Wystąpił błąd serwera: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<ActionResult<Order>> PostOrder(Order order)
        {
            try
            {
                if (string.IsNullOrEmpty(order.OrderName))
                {
                    return BadRequest("Order name is required");
                }
                if (string.IsNullOrEmpty(order.Client))
                {
                    return BadRequest("Client is required");
                }

                order.Id = 0;
                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, order);
            }
            catch (DbUpdateException dbEx)
            {
                return StatusCode(500, $"Błąd zapisu do bazy danych: {dbEx.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Wystąpił błąd serwera: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutOrder(int id, Order order)
        {
            if (id != order.Id)
            {
                return BadRequest("Id w URL i w ciele zamówienia muszą być takie same");
            }

            var existingOrder = await _context.Orders.FindAsync(id);
            if (existingOrder == null)
            {
                return NotFound($"Nie znaleziono zamówienia o id = {id}");
            }

            try
            {
                _context.Entry(existingOrder).State = EntityState.Detached;
                _context.Orders.Update(order);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await OrderExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Wystąpił błąd serwera: {ex.Message}");
            }

            return Ok(order);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            try
            {
                var order = await _context.Orders.FindAsync(id);

                if (order == null)
                {
                    return NotFound($"Nie znaleziono zamówienia o id = {id}");
                }

                _context.Orders.Remove(order);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Wystąpił błąd serwera: {ex.Message}");
            }
        }

        private async Task<bool> OrderExists(int id)
        {
            return await _context.Orders.AnyAsync(e => e.Id == id);
        }
    }
}




