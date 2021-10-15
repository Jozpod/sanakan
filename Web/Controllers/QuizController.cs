using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Sanakan.Extensions;
using Microsoft.EntityFrameworkCore;
using Sanakan.Config;
using Sanakan.Common;
using DAL.Repositories.Abstractions;
using Sanakan.DAL.Models;

namespace Sanakan.Web.Controllers
{
    [ApiController, Authorize(Policy = AuthorizePolicies.Site)]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class QuizController : ControllerBase
    {
        private readonly IRepository _repository;
        private readonly ICacheManager _cacheManager;

        public QuizController(
            IRepository repository,
            ICacheManager cacheManager)
        {
            _repository = repository;
            _cacheManager = cacheManager;
        }

        /// <summary>
        /// Gets the collection of questions.
        /// </summary>
        [HttpGet("questions")]
        public async Task<IActionResult> GetQuestionsAsync()
        {
            var result = await _repository.GetCachedAllQuestionsAsync();

            return Ok(result);
        }

        /// <summary>
        /// Pobiera pytanie po id
        /// </summary>
        /// <param name="id">id pytania</param>
        /// <response code="500">Internal Server Error</response>
        [HttpGet("question/{id}")]
        public async Task<IActionResult> GetQuestionAsync(ulong id)
        {
            var result = await _repository.GetCachedQuestionAsync(id);
            return Ok(result);
        }

        /// <summary>
        /// Add new question
        /// </summary>
        /// <param name="question">The question</param>
        [HttpPost("question")]
        public async Task<IActionResult> AddQuestionAsync([FromBody]Question question)
        {
            await _repository.AddQuestionAsync(question);

            _cacheManager.ExpireTag(new string[] { $"quiz" });

            return Ok("Question added!");
        }

        /// <summary>
        /// Deletes the question.
        /// </summary>
        /// <param name="id">The question identifier</param>
        /// <response code="404">Question not found</response>
        /// <response code="500">Internal Server Error</response>
        [HttpDelete("question/{id}")]
        public async Task<IActionResult> RemoveQuestionAsync(ulong id)
        {
            var question = await _repository.GetQuestionAsync(id);
            
            if (question != null)
            {
                await _repository.RemoveQuestionAsync(question);

                _cacheManager.ExpireTag(new string[] { $"quiz" });

                return Ok("Question removed!");
            }

            return NotFound("Question not found!");
        }
    }
}