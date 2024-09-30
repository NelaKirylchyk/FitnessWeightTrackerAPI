using Microsoft.AspNetCore.Mvc;
using FitnessWeightTrackerAPI.Models;
using FitnessWeightTrackerAPI.Data.DTO;
using FitnessWeightTrackerAPI.Services;

namespace FitnessWeightTrackerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FoodItemController : ControllerBase
    {
        private IFoodItemService _foodItemService;

        public FoodItemController(IFoodItemService foodItemService)
        {
            _foodItemService = foodItemService;
        }

        // GET: api/FoodItems
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FoodItem>>> GetFoodItems()
        {
           return await _foodItemService.GetAllFoodItems();
        }

        // GET: api/FoodItem/5
        [HttpGet("{id}")]
        public async Task<ActionResult<FoodItem>> GetFoodItem(int id)
        {
            var foodItem = await _foodItemService.GetFoodItem(id);

            if (foodItem == null)
            {
                return NotFound();
            }

            return foodItem;
        }

        // PUT: api/FoodItem/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFoodItem(int id, FoodItemDTO foodItem)
        {
            var record = await _foodItemService.UpdateFoodItem(id, foodItem);

            if (record == null)
            {
                return NotFound($"Food Item with id={id} was not found.");
            }

            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult<BodyWeightRecord>> PostFoodItem(FoodItemDTO foodItem)
        {
            var created = await _foodItemService.AddFoodItem(foodItem);

            if (created == null)
            {
                return NotFound($"Food item was not added.");
            }

            return CreatedAtAction("GetFoodItem", new { id = created.FoodItemId }, foodItem);

        }


        // DELETE: api/FoodItem/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFoodItem(int id)
        {
            var isDeleted = await _foodItemService.DeleteFoodItem(id);
            if (!isDeleted)
            {
                return NotFound();
            }
            return NoContent();
        }

        // DELETE: api/FoodItems
        [HttpDelete]
        public async Task<IActionResult> DeleteAllFoodItems()
        {
            await _foodItemService.DeleteAllFoodItems();
            return NoContent();
        }
    }
}
