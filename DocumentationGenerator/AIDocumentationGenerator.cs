
using BizDataLayerGen.DocumentationGenerator;
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BizDataLayerGen.AI
{
    public sealed class AIDocumentationGenerator : ICodeDocumentionGenerator
    {
        private readonly HttpClient _httpClient;
       
        private readonly string _PromptToGenerateDocumentation = @" 
You are a senior .NET architect.

Your task is to add professional XML documentation comments to C# source code.

Rules:

- Do NOT modify the implementation.
- Preserve formatting.
- Add XML documentation for:
    - Classes
    - Constructors
    - Interfaces
    - Enums
    - Properties
    - Methods
    -DTOs

Use:

- <summary>
- <param>
- <returns>
- <typeparam>
- <remarks>
- <exception>
- <see cref>

Return ONLY valid C# code.
";

        public AIDocumentationGenerator()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("http://localhost:1234/")
            };
        }

        public async Task<string> GenerateDocumentationAsync(string sourceCode)
        {
            

            var request = new
            {
                model = "local-model", // ignored by LM Studio
                temperature = 0.2,
                messages = new object[]
                {
                new
                {
                    role = "system",
                    content = _PromptToGenerateDocumentation
                },
                new
                {
                    role = "user",
                    content = sourceCode
                }
                }
            };

            string json = JsonSerializer.Serialize(request);

            using var content = new StringContent(
                json,
                Encoding.UTF8,
                "application/json");

            var response =
                await _httpClient.PostAsync(
                    "v1/chat/completions",
                    content);

            response.EnsureSuccessStatusCode();

            string responseJson =
                await response.Content.ReadAsStringAsync();

            using JsonDocument document =
                JsonDocument.Parse(responseJson);

            return document
                .RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString()!;
        }


    }
}