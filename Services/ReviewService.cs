using Microsoft.EntityFrameworkCore;
using StarterKit.Models;
using StarterKit.Models.DTOs;

namespace StarterKit.Services
{
    public class ReviewService : IReviewService
    {
        private readonly DatabaseContext _context;
        private readonly ILogger<ReviewService> _logger;
        private readonly ILoginService _loginService;

        public ReviewService(
            DatabaseContext context, 
            ILogger<ReviewService> logger,
            ILoginService loginService)
        {
            _context = context;
            _logger = logger;
            _loginService = loginService;
        }

        public async Task<Review> CreateReviewAsync(int eventId, Models.DTOs.ReviewCreateDTO review)
        {
            try
            {
                var @event = await _context.Events.FindAsync(eventId)
                    ?? throw new KeyNotFoundException($"Event {eventId} not found");

                var userInfo = await _loginService.GetCurrentUserSessionInfoAsync();
                if (userInfo == null)
                    throw new UnauthorizedAccessException("User must be logged in to create a review");

                var newReview = new Review
                {
                    EventId = eventId,
                    Rating = review.Rating,
                    Comment = review.Comment ?? string.Empty,
                    CreatedDate = DateTime.UtcNow,
                    UserId = userInfo.UserId
                };

                await _context.Reviews.AddAsync(newReview);
                await _context.SaveChangesAsync();

                return newReview;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating review for event {EventId}", eventId);
                throw;
            }
        }

        public async Task<IEnumerable<Review>> GetReviewsByEventIdAsync(int eventId)
        {
            return await _context.Reviews
                .Where(r => r.EventId == eventId)
                .Include(r => r.User)
                .OrderByDescending(r => r.CreatedDate)
                .ToListAsync();
        }
    }
}
