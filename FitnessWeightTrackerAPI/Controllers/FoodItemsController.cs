using FitnessWeightTrackerAPI.Data.DTO;
using FitnessWeightTrackerAPI.Filters;
using FitnessWeightTrackerAPI.Models;
using FitnessWeightTrackerAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitnessWeightTrackerAPI.Controllers
{
    [ValidateModel]
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class FoodItemsController : ControllerBase
    {
        private IFoodItemService _foodItemService;

        public FoodItemsController(IFoodItemService foodItemService)
        {
            _foodItemService = foodItemService;
        }

        // GET: api/FoodItems
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FoodItem>>> GetFoodItems()
        {
            return await _foodItemService.GetAllFoodItems();
        }

        // GET: api/FoodItems/5
        [HttpGet("{id}")]
        public async Task<ActionResult<FoodItem>> GetFoodItems(int id)
        {
            var foodItem = await _foodItemService.GetFoodItem(id);

            if (foodItem == null)
            {
                return NotFound();
            }

            return foodItem;
        }

        // PUT: api/FoodItems/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFoodItems(int id, FoodItemDTO foodItem)
        {
            await _foodItemService.UpdateFoodItem(id, foodItem);

            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult<BodyWeightRecord>> PostFoodItems(FoodItemDTO foodItem)
        {
            var created = await _foodItemService.AddFoodItem(foodItem);

            if (created == null)
            {
                return NotFound($"Food item was not added.");
            }

            return CreatedAtAction("GetFoodItems", new { id = created.Id }, foodItem);
        }

        // DELETE: api/FoodItems/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFoodItems(int id)
        {
            await _foodItemService.DeleteFoodItem(id);

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
