using CMS.API.Models;
using CMS.API.Services.ServicesInterface;
using CMS.DATA.DTO;
using CMS.DATA.Entities;
using CMS.DATA.Enum;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CMS.API.Controllers
{
    [Route("api/lesson")]
    [ApiController]
    public class LessonsController : ControllerBase
    {
        private readonly ILessonsService _lessonsService;


        public LessonsController(ILessonsService lessonsService)
        {
            _lessonsService = lessonsService;

        }
        [Authorize(Roles = "Facilitator, Admin")]
        [Authorize(Policy = "can_add")]
        [HttpPost("add")]
        public async Task<IActionResult> AddLesson(AddLessonDTO addLesson)
        {
            var result = await _lessonsService.AddLessonNew(addLesson);
            if (result.StatusCode == 200)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }

        [Authorize(Roles = "Facilitator, Admin")]
        [Authorize(Policy = "can_delete")]
        [HttpDelete("{lessonid}/delete")]
        public async Task<IActionResult> DeleteLeson(string lessonid)
        {
            var result = await _lessonsService.DeleteLessonbyid(lessonid);
            if (result.StatusCode == 200)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }
        [Authorize]
        [HttpGet("{moduleid}")]
        public async Task<IActionResult> GetLessonByModule(Modules moduleid)
        {
            var result = await _lessonsService.GetLessonByModuleAsync(moduleid);
            if (result.StatusCode == 200)
            {
                return Ok(result);
            }
            else if (result.StatusCode == 404)
            {
                return NotFound(result);
            }
            else
            {
                return BadRequest(result);
            }
        }

        [Authorize(Policy = "can_update")]
        [Authorize(Roles = "Facilitator, Admin")]
        [HttpPut("{lessonId}/update")]
        public async Task<IActionResult> UpdateLesson(UpdateLessonDTO lesson, string lessonId)
        {
            var result = await _lessonsService.UpdateLesson(lesson, lessonId);
            if (result.StatusCode == 200)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }
        [Authorize]
        [HttpGet("all")]
        public async Task<IActionResult> GetAllLessonsAsync()
        {
            var result = await _lessonsService.GetAllLessons();
            if (result.StatusCode == 200)
                return Ok(result);

            return BadRequest(result);
        }


        [Authorize(Roles = "Facilitator, Admin")]
        [HttpGet("topic")]
        public async Task<IActionResult> GetLessonTopic(string topic)
        {
            var result = await _lessonsService.GetLessonsByTopic(topic);
            if (result.StatusCode == 200)
            {
                return Ok(result);
            }
            else if (result.StatusCode == 404)
            {
                return NotFound(result);
            }
            else
            {
                return BadRequest(result);

            }
        }
        [Authorize(Roles = "Facilitator, Admin")]
        [HttpGet("lesson/{id}")]
        public async Task<IActionResult> GetLessonId(string id)
        {
            var result = await _lessonsService.GetLessonById(id);
            if (result.StatusCode == 200)
            {
                return Ok(result);
            }
            else if (result.StatusCode == 404)
            {
                return NotFound(result);
            }
            else
            {
                return BadRequest(result);
            }
        }

        [Authorize(Roles = "Facilitator, Admin,Student")]
        [HttpPatch("{lessonId}")]
        public async Task<ActionResult> UpdateLessonCompletionStatus(string lessonId, bool completed)
        {
            try
            {
                var respFromService = await _lessonsService.UpdateCompletionStatus(lessonId, completed);
                var res = new LessonUpdateStatus();
                res.status = respFromService;
                var response = new ResponseDto<LessonUpdateStatus>();
                response.StatusCode = StatusCodes.Status200OK;
                response.Result = res;
                response.DisplayMessage = "Successful";
                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = new ResponseDto<LessonUpdateStatus>();
                response.StatusCode = StatusCodes.Status400BadRequest;
                response.DisplayMessage = ex.Message;
                return BadRequest(response);

            }

        }

        [Authorize(Roles = "Facilitator, Admin,Student")]
        [HttpGet]
        [Route("{lessonId}/quizzes")]
        public async Task<ActionResult<List<QuizDto>>> GetQuizzesByLessonId(string lessonId)
        {
            try
            {
                // Retrieve quizzes from the database for the specified lessonId,
                // selecting only the desired attributes
                var quizzes = await _lessonsService.GetQuizzesByLessonId(lessonId);

                if (quizzes == null || quizzes.Count == 0)
                {
                    return NotFound(); // Return a 404 Not Found status code if no quizzes are found
                }

                return Ok(quizzes);
            }
            catch (Exception ex)
            {
        
                // Create a response object with error details
                var response = new ResponseDto<LessonUpdateStatus>();
                response.StatusCode = StatusCodes.Status500InternalServerError;
                response.DisplayMessage = "An error occurred while retrieving quizzes.";

                // You may choose to include the exception message in the response for debugging purposes
                // response.ErrorMessage = ex.Message;

                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }



    }
}