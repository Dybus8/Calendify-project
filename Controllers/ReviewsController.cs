using Microsoft.AspNetCore.Mvc;
using StarterKit.Models;
using StarterKit.Models.DTOs;
using StarterKit.Services;

namespace StarterKit.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        private readonly IReviewService _reviewService;

        public ReviewsController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        [HttpPost("{eventId}")]
        public async Task<ActionResult<Review>> CreateReview(int eventId, [FromBody] Models.DTOs.ReviewCreateDTO review)
        {
            try
            {
                var createdReview = await _reviewService.CreateReviewAsync(eventId, review);
                return Ok(createdReview);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("{eventId}/reviews")]
        public async Task<ActionResult<IEnumerable<Review>>> GetEventReviews(int eventId)
        {
            try
            {
                var reviews = await _reviewService.GetReviewsByEventIdAsync(eventId);
                return Ok(reviews);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
