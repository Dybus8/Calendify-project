using Microsoft.AspNetCore.Mvc;
using StarterKit.Models;
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
                .Select(r => new
                {
                    r.Id,
                    r.Rating,
                    r.Comment,
                    r.CreatedDate,
                    UserName = r.User.UserName
                })
                .OrderByDescending(r => r.CreatedDate)
                .ToList();

            return Ok(reviews);
        }

        // POST: api/reviews
        [HttpPost]
        public async Task<IActionResult> AddReview([FromBody] Review review)
        {
            if (review.Rating < 1 || review.Rating > 10)
            {
                return BadRequest("Rating must be between 1 and 10.");
            }

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetReviewsByEvent), new { eventId = review.EventId }, review);
        }
    }
}
