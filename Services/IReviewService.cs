using StarterKit.Models;
using StarterKit.Models.DTOs;

namespace StarterKit.Services
{
    public interface IReviewService
    {
        Task<Review> CreateReviewAsync(int eventId, Models.DTOs.ReviewCreateDTO review);
        Task<IEnumerable<Review>> GetReviewsByEventIdAsync(int eventId);
    }
}
