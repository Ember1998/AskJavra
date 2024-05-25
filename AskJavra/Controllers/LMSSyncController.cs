using AskJavra.Repositories;
using AskJavra.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using System.Text.Json;

namespace AskJavra.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LMSSyncController : ControllerBase
    {
        private readonly LMSSyncRepository _lmsRepo;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _config;


        public LMSSyncController(LMSSyncRepository lmsRepo, IHttpClientFactory httpClientFactory, IConfiguration config)
        {
            _lmsRepo = lmsRepo;
            _httpClientFactory = httpClientFactory;
            _config = config;

        }

        [HttpGet("SyncLMSdata")]
        public async Task<IActionResult> SyncLMSdata()
        {
            var httpClient = _httpClientFactory.CreateClient();

            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


            var response = await httpClient.GetAsync(_config.GetValue<string>("LMSBaseUrl"));
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                var jsonData = JsonSerializer.Deserialize<UserInfos>(data);
                await _lmsRepo.SyncLMSdata(jsonData);
                return Ok(jsonData);
            }
            return StatusCode((int)response.StatusCode, response.ReasonPhrase);
        }

    }
}
