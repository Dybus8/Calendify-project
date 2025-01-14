using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StarterKit.Models;
using StarterKit.Models.DTOs;  // Add this line
using System.Linq;
using System.Threading.Tasks;

namespace StarterKit.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReviewsController : ControllerBase
    {
        private readonly DatabaseContext _context;

        public ReviewsController(DatabaseContext context)
        {
            _context = context;
        }

        // GET: api/reviews/event/2
        [HttpGet("event/{eventId}")]
        public IActionResult GetReviewsByEvent(int eventId)
        {
            var reviews = _context.Reviews
                .Where(r => r.EventId == eventId)
                .Select(r => new ReviewDTO
                {
                    Id = r.Id,
                    Rating = r.Rating,
                    Comment = r.Comment,
                    CreatedDate = r.CreatedDate,
                    UserName = r.User.UserName
                })
                .OrderByDescending(r => r.CreatedDate)
                .ToList();

            return Ok(reviews);
        }

        // POST: api/reviews
        [HttpPost]
        public async Task<IActionResult> AddReview([FromBody] ReviewCreateRequestDTO reviewDto)
        {
            if (reviewDto.Rating < 1 || reviewDto.Rating > 10)
            {
                return BadRequest("Rating must be between 1 and 10.");
            }

            var user = await _context.UserAccounts.FindAsync(reviewDto.UserId);
            var event_ = await _context.Events.FindAsync(reviewDto.EventId);

            if (user == null)
            {
                return NotFound("User not found");
            }

            if (event_ == null)
            {
                return NotFound("Event not found");
            }

            var review = new Review
            {
                EventId = reviewDto.EventId,
                UserId = reviewDto.UserId,
                Rating = reviewDto.Rating,
                Comment = reviewDto.Comment,
                CreatedDate = DateTime.UtcNow
            };

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            var responseDto = new ReviewDTO
            {
                Id = review.Id,
                Rating = review.Rating,
                Comment = review.Comment,
                CreatedDate = review.CreatedDate,
                UserName = user.UserName
            };

            return CreatedAtAction(nameof(GetReviewsByEvent), new { eventId = review.EventId }, responseDto);
        }
    }
}
