using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using static Sanakan.Web.ResponseExtensions;
using Sanakan.Common;
using Sanakan.DAL.Models;
using Microsoft.AspNetCore.Http;
using Sanakan.DAL.Repositories.Abstractions;
using Sanakan.Common.Cache;

namespace Sanakan.Web.Controllers
{
    [ApiController, Authorize(Policy = AuthorizePolicies.Site)]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class QuizController : ControllerBase
    {
        private readonly IQuestionRepository _questionRepository;
        private readonly ICacheManager _cacheManager;

        public QuizController(
            IQuestionRepository questionRepository,
            ICacheManager cacheManager)
        {
            _questionRepository = questionRepository;
            _cacheManager = cacheManager;
        }

        /// <summary>
        /// Gets the collection of questions.
        /// </summary>
        [HttpGet("questions")]
        [ProducesResponseType(typeof(List<Question>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetQuestionsAsync()
        {
            var result = await _questionRepository.GetCachedAllQuestionsAsync();

            return Ok(result);
        }

        /// <summary>
        /// Gets question by identifier.
        /// </summary>
        /// <param name="questionId">The question identifier.</param>
        [HttpGet("question/{questionId}")]
        [ProducesResponseType(typeof(ShindenPayload), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetQuestionAsync(ulong questionId)
        {
            var result = await _questionRepository.GetCachedQuestionAsync(questionId);
            return Ok(result);
        }

        /// <summary>
        /// Add new question
        /// </summary>
        /// <param name="question">The question content.</param>
        [HttpPost("question")]
        [ProducesResponseType(typeof(Question), StatusCodes.Status200OK)]
        public async Task<IActionResult> AddQuestionAsync([FromBody]Question question)
        {
            _questionRepository.Add(question);
            await _questionRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(CacheKeys.Quiz);

            return ShindenOk("Question added!");
        }

        /// <summary>
        /// Deletes the question.
        /// </summary>
        /// <param name="id">The question identifier</param>
        [HttpDelete("question/{id}")]
        [ProducesResponseType(typeof(Question), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Question), StatusCodes.Status200OK)]
        public async Task<IActionResult> RemoveQuestionAsync(ulong id)
        {
            var question = await _questionRepository.GetByIdAsync(id);
            
            if (question != null)
            {
                _questionRepository.Remove(question);
                await _questionRepository.SaveChangesAsync();

                _cacheManager.ExpireTag(CacheKeys.Quiz);

                return ShindenOk("Question removed!");
            }

            return ShindenNotFound("Question not found!");
        }
    }
}