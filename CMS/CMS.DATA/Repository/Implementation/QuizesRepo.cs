using CMS.DATA.Context;
using CMS.DATA.DTO;
using CMS.DATA.Entities;
using CMS.DATA.Enum;
using CMS.DATA.Repository.RepositoryInterface;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace CMS.DATA.Repository.Implementation
{
    public class QuizesRepo : IQuizesRepo
    {
        private readonly CMSDbContext _context;

        public QuizesRepo(CMSDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Quiz>> GetAllQuizAsync()
        {
            return await _context.Quizs.ToListAsync();
        }

        public async Task<Quiz> GetQuizByIdAsync(string quizId)
        {
            return await _context.Quizs.FirstOrDefaultAsync(e => e.Id == quizId);
        }
   

        public async Task<IEnumerable<Quiz>> GetQuizByLessonAsync(string lessonId)
        {
            var lesson = await _context.Lessons.FindAsync(lessonId);
            if (lesson == null)
                throw new Exception("Lesson does not exist");

            return await _context.Quizs.Where(x => x.LessonId == lessonId).ToListAsync();
        }

        public async Task<IEnumerable<Quiz>> GetQuizByUserAsync(string userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                throw new Exception("User does not exist");

            return await _context.Quizs.Where(x => x.AddedById == userId).ToListAsync();
        }

        public async Task<Quiz> AddQuiz(Quiz entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            await _context.Quizs.AddAsync(entity);
            var Status = await _context.SaveChangesAsync();

            if (Status > 0)
                return entity;

            return null;
        }
        
        public async Task<Quiz> DeleteQuizAsync(Quiz entity)
        {
            _context.Quizs.Remove(entity);
            var status = await _context.SaveChangesAsync();

            if (status > 0)
                return entity;

            return null;
        }

        public async Task<Quiz> UpdateQuiz(Quiz entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));
            _context.Quizs.Update(entity);
            var status = await _context.SaveChangesAsync();

            if (status > 0)
                return entity;
            return null;
        }


       
        public async Task<QuizResponseDto<QuizReviewRequest>> QuizReviewRequest(QuizReviewRequestDTO model)
        {
            try
            {
                var review = new QuizReviewRequest();
                review.CourseId = model.CourseId;
                review.UserId = model.UserId;
                review.Notes = model.Notes;
                review.IsSatisfied = model.IsSatisfied;
                review.DateCreated = DateTime.Now;
                review.DateUpdated = DateTime.Now;
                _context.QuizReviews.Add(review);
                _context.SaveChanges();

                var quizresponse = new QuizResponseDto<QuizReviewRequest>();
                quizresponse.StatusCode = 200;
                quizresponse.DisplayMessage = "Successful";
                quizresponse.Result = review;
                return quizresponse;
            }
            catch (Exception ex)
            {
                var quizresponse = new QuizResponseDto<QuizReviewRequest>();
                quizresponse.StatusCode = 400;
                quizresponse.DisplayMessage = ex.Message;
                quizresponse.Result = null;
                return quizresponse;
            }
            
        }

        public async Task<QuizResponseDto<QuizSummaryDto>> Summary(string courseId, string userId)
        {
            try
            {
                var lessons = _context.Lessons.Where(l => l.CourseId == courseId).ToList();
                var userQuizTaken = _context.UserQuizTaken.Where(u => u.UserId == userId).ToList();

                // Skipped questions
                //var skippedQuestions = userQuizTaken.Count(l => !userQuizTaken.Any(u => u.QuizId == l.Id));
                var skippedQuestions = userQuizTaken.Count(x => string.IsNullOrEmpty(x.Preferredanswer.Trim()));

                // Correct answers
                var correctAnswers = userQuizTaken.Count(u => u.Preferredanswer == u.CorrectAnswer);

                // Incorrect answers
                var incorrectAnswers = userQuizTaken.Count(u => u.Preferredanswer != u.CorrectAnswer);

                // Score in percentage
                var totalQuestions = skippedQuestions + correctAnswers + incorrectAnswers;
                var scorePercentage = totalQuestions > 0 ? (correctAnswers * 100) / totalQuestions : 0;

                // Retrieve the distinct weeks from the lessons
                //var weeks = lessons.Select(l => l.Weeks).Distinct().ToList();
                var weeks = lessons.Select(l => l.Weeks).Distinct().ToList();

                var summaryData = new QuizSummaryDto
                {
                    SkippedCount = skippedQuestions,
                    CorrectCount = correctAnswers,
                    IncorrectCount = incorrectAnswers,
                    Percentage = scorePercentage,
                    Weeks = weeks
                    
                };


                var response = new QuizResponseDto<QuizSummaryDto>();
                response.StatusCode = 200;
                response.DisplayMessage = "Success";
                response.Result = summaryData;
                return response;
            }
            catch (Exception ex)
            {
                var response = new QuizResponseDto<QuizSummaryDto>();
                response.StatusCode = 400;
                response.DisplayMessage = ex.Message;
                response.Result = null;
                return response;
            }
        }





    }



}

