namespace Quiz.API.Controllers;

using Microsoft.AspNetCore.Mvc;
using Quiz.API.Contracts;
using Quiz.API.DataBase.Services;

[ApiController]
[Route("api/[controller]")]
public class QuizController(QuizService quizService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get() => Ok(await quizService.GetQuizzesAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var quiz = await quizService.GetQuizForUserAsync(id);
        return quiz != null ? Ok(quiz) : NotFound();
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateQuizRequest request)
    {
        var id = await quizService.CreateQuizAsync(request);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }
    [HttpPost("{id}/submit")]
    public async Task<IActionResult> Submit(string id, [FromBody] SubmitQuizRequest request) 
    {
        var result = await quizService.SumbitQuizAsync(id, request);
        if (!result) return NotFound();
        return Accepted(new { message = "Submission acccepted for proccessing"});
    }
}
