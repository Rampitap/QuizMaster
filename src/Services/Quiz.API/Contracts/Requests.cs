namespace Quiz.API.Contracts;

//quiz creation request
public record CreateQuizRequest(string Title, string Description, List<CreateQuestionRequest> Questions);
public record CreateQuestionRequest(string Text, List<string> Options, int CorrectOptionIndex);
//quiz dto for listing quizzes
public record QuizDto(string Id, string Title, string Description, int QuestionsCount);
//quiz dto for user attempt
public record QuizForUserResponse(string Id, string Title, List<QuestionForUserDto> Questions);
public record QuestionForUserDto(Guid Id, string Text, List<string> Options);
//request for quiz submission
public record SubmitQuizRequest(string UserId, string UserEmail, List<AnswerDto> Answers);
public record AnswerDto(Guid QuestionId, int SelectedOptionIndex);