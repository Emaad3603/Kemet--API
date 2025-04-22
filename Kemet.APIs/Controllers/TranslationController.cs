using Google.Cloud.Speech.V1;
using Google.Cloud.TextToSpeech.V1;
using Google.Cloud.Translate.V3;
using Kemet.APIs.DTOs.TranslateDTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace Kemet.APIs.Controllers
{
    public class TranslationController : BaseApiController
    {
        [HttpPost("Translate")]
        public async Task<IActionResult> Translate([FromBody] TranslateRequest request)
        {
            // Path to your credentials JSON (same one you used in Program.cs)
            var credentialsPath = Path.Combine(Directory.GetCurrentDirectory(), "secrets", "kemet-457321-0a298073023d.json");

            // Read the project_id from the JSON file
            var json = await System.IO.File.ReadAllTextAsync(credentialsPath);
            var projectId = JObject.Parse(json)["project_id"]?.ToString();

            if (string.IsNullOrEmpty(projectId))
                return BadRequest("Unable to find project_id in credentials file.");

            var client = TranslationServiceClient.Create();
            var response = await client.TranslateTextAsync(new TranslateTextRequest
            {
                Contents = { request.Text },
                SourceLanguageCode = request.SourceLang,
                TargetLanguageCode = request.TargetLang,
                Parent = $"projects/{projectId}/locations/global"
            });

            return Ok(response.Translations.First().TranslatedText);
        }
        [HttpPost("recognize")]
        public async Task<IActionResult> Recognize([FromBody] SpeechRequest request)
        {
            var speech = SpeechClient.Create();
            var response = await speech.RecognizeAsync(new RecognitionConfig
            {
                Encoding = RecognitionConfig.Types.AudioEncoding.Linear16,
                SampleRateHertz = 16000,
                LanguageCode = request.LanguageCode
            }, RecognitionAudio.FromBytes(Convert.FromBase64String(request.AudioBase64)));

            return Ok(response.Results.FirstOrDefault()?.Alternatives.FirstOrDefault()?.Transcript);
        }

        [HttpPost("Synthesize")]
        public async Task<IActionResult> Synthesize([FromBody] TtsRequest request)
        {
            var client = TextToSpeechClient.Create();
            var response = await client.SynthesizeSpeechAsync(new SynthesizeSpeechRequest
            {
                Input = new SynthesisInput { Text = request.Text },
                Voice = new VoiceSelectionParams
                {
                    LanguageCode = request.LanguageCode,
                    SsmlGender = SsmlVoiceGender.Neutral
                },
                AudioConfig = new AudioConfig
                {
                    AudioEncoding = AudioEncoding.Mp3
                }
            });

            return File(response.AudioContent.ToByteArray(), "audio/mpeg", "output.mp3");
        }
    }
}
