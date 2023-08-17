using CMS.DATA.DTO;
using CMS.DATA.Entities;
using System.Threading.Tasks;

namespace CMS.DATA.Repository.RepositoryInterface
{
    public interface IQuizesRepo
    {
        Task<Quiz> AddQuiz(Quiz entity);
        Task<Quiz> DeleteQuizAsync(Quiz entity);
        Task<Quiz> UpdateQuiz(Quiz entity);
        Task<Quiz> GetQuizByIdAsync(string Id);
        Task<IEnumerable<Quiz>> GetAllQuizAsync();
        Task<IEnumerable<Quiz>> GetQuizByLessonAsync(string LessonId);
        Task<IEnumerable<Quiz>> GetQuizByUserAsync(string userId);
        Task<QuizResponseDto<QuizSummaryDto>> Summary(string lessonId, string userId);

        Task<QuizResponseDto<QuizReviewRequest>> QuizReviewRequest(QuizReviewRequestDTO model);
    }
}