using Microsoft.AspNetCore.Mvc;
using OpenAI_API.Completions;
using OpenAI_API;
using Microsoft.AspNetCore.Authorization;

namespace AskJavra.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GPTController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public GPTController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        [HttpGet]
        [Route("ChatGPT")]
        [AllowAnonymous]
        public async Task<IActionResult> UseChatGPT(string query)
        {
            string outputResult = "";
            string getSecretKey = _configuration.GetValue<string>("OpenAISecretKey");
            var openai = new OpenAIAPI(getSecretKey);
            CompletionRequest completionRequest = new CompletionRequest();
            completionRequest.Prompt = query;
            completionRequest.Model = OpenAI_API.Models.Model.ChatGPTTurboInstruct;
            completionRequest.MaxTokens = 1024;

            var completions = await openai.Completions.CreateCompletionAsync(completionRequest);

            foreach (var completion in completions.Completions)
            {
                outputResult += completion.Text;
            }

            return Ok(outputResult);

        }
    }
}
