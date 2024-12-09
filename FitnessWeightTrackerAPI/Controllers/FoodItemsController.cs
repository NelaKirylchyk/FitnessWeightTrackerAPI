using FitnessWeightTrackerAPI.Data.DTO;
using FitnessWeightTrackerAPI.Filters;
using FitnessWeightTrackerAPI.Models;
using FitnessWeightTrackerAPI.Models.FoodItems.Commands.CreateFoodItemCommand;
using FitnessWeightTrackerAPI.Models.FoodItems.Commands.DeleteFoodItemCommand;
using FitnessWeightTrackerAPI.Models.FoodItems.Commands.UpdateFoodItemCommand;
using FitnessWeightTrackerAPI.Models.FoodItems.Queries.GetAllFoodItemsQuery;
using FitnessWeightTrackerAPI.Models.FoodItems.Queries.GetFoodItemQuery;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitnessWeightTrackerAPI.Controllers
{
    [ValidateModel]
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class FoodItemsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public FoodItemsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET: api/FoodItems
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FoodItem>>> GetFoodItems()
        {
            var query = new GetAllFoodItemsQuery();
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        // GET: api/FoodItems/5
        [HttpGet("{id}")]
        public async Task<ActionResult<FoodItem>> GetFoodItems(int id)
        {
            var query = new GetFoodItemByIdQuery()
            {
                Id = id
            };
            var result = await _mediator.Send(query);
            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        // PUT: api/FoodItems/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFoodItems(int id, FoodItemDTO foodItem)
        {
            var command = new UpdateFoodItemCommand
            {
                Id = id,
                Name = foodItem.Name,
                Calories = foodItem.Calories,
                Carbohydrates = foodItem.Carbohydrates,
                Protein = foodItem.Protein,
                Fat = foodItem.Fat,
                ServingSize = foodItem.ServingSize
            };

            await _mediator.Send(command);
            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult<BodyWeightRecord>> PostFoodItems(FoodItemDTO foodItem)
        {
            var command = new AddFoodItemCommand
            {
                Name = foodItem.Name,
                Calories = foodItem.Calories,
                Carbohydrates = foodItem.Carbohydrates,
                Protein = foodItem.Protein,
                Fat = foodItem.Fat,
                ServingSize = foodItem.ServingSize
            };

            var result = await _mediator.Send(command);
            if (result == null)
            {
                return NotFound($"Food item was not added.");
            }

            return CreatedAtAction(nameof(GetFoodItems), new { id = result.Id }, result);
        }

        // DELETE: api/FoodItems/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFoodItems(int id)
        {
            var command = new DeleteFoodItemCommand { Id = id };
            await _mediator.Send(command);
            return NoContent();
        }
    }
}
