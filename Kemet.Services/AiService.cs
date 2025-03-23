using Kemet.Core.Entities.AI_Entites;
using Kemet.Core.Services.Interfaces;
using Kemet.Repository.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Kemet.Services
{
    public class AiService : IAiService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly AppDbContext _context;

        public AiService(HttpClient httpClient, IConfiguration configuration, AppDbContext context )
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _context = context;
        }

        public async Task<AiResponseDto> CallAiApiAsync(AiRequestDto requestDto, string userId)
        {
            try
            {
                var user = _context.Users.FirstOrDefault(u => u.Id == userId);
                if (user == null) return null;

                var aiApiUrl = _configuration["AiApi:Url"];
                var aiApiKey = _configuration["AiApi:Key"];
                string __aiApiUrl = $"{aiApiUrl}:generateContent?key={aiApiKey}";

                if (string.IsNullOrEmpty(aiApiUrl) || string.IsNullOrEmpty(aiApiKey))
                    throw new Exception("AI API URL or Key is missing in configuration.");
                var places = await _context.Places.ToArrayAsync();
                string[] placesList = { };
                foreach ( var place in places )
                {
                    placesList.Append(string.Join(",", place));
                }
                var activities = await _context.Activities.ToArrayAsync();
                string []ActivityList = { };
                foreach (var activity in activities)
                {
                    //ActivityList = String.Concat(ActivityList, ",", activity.ToString()); 
                    ActivityList.Append(string.Join(",", activity));
                }
                var payload = JsonSerializer.Serialize(requestDto);
                var requestBody = new
                {
                    contents = new[]
           {
                new
                {
                    parts = new[]
                    {
                        new { text = $"Generate a travel itinerary based on these preferences: Historical, Cultural, budget 1.5k-3.5k, Water Sports, Adventure Activity. here's the places {placesList} and the activities {ActivityList} , Return EXACTLY this JSON structure (no additional text):\r\n\r\n    'success': true,\r\n    'travel_plan': \r\n        'itinerary': \r\n            'days': [\r\n                \r\n                    'day1': \r\n                        'place': \r\n                            'placeId': 1,\r\n                            'name': 'Example Place',\r\n                            'description': 'Place description',\r\n                            'address': 'Place Address',\r\n                            'entryCost': '100',\r\n                            'imageURLs': 'http://example.com/image.jpg',\r\n                            'categoryName': 'Historical',\r\n                        ,\r\n                        'activity': \r\n                            'activityId': 1,\r\n                            'name': 'Example Activity',\r\n                            'description': 'Activity description',\r\n                            'entryCost': '50',\r\n                            'imageURL': 'http://example.com/activity.jpg',\r\n                            'categoryName': 'Adventure',\r\n                            'address': 'Activity Address',\r\n                        \r\n                    \r\n                \r\n            ],\r\n            'total_budget': 'budget',\r\n            'total_days': duration\r\n        \r\n    \r\n\r\n\r\nIMPORTANT:\r\n1. Return ONLY the JSON object above with your data\r\n2. No additional text, comments, or markdown\r\n3. Use proper JSON formatting with no trailing commas\r\n4. Use only data from the provided places and activities\r\n5. Ensure all JSON is properly nested and formatted\r\n6. Use exact field names as shown\r\n7. Use numbers for IDs and numeric values (no quotes)\r\n8. Use quotes for strings\r\n9. Do not add any fields not shown in the example'''\r\n" }
                    }
                }
            }
                };

                string jsonRequest = JsonSerializer.Serialize(requestBody);
                var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
                // var content = new StringContent(payload, Encoding.UTF8, "application/json");

                // _httpClient.DefaultRequestHeaders.Clear();
                // _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {aiApiKey}");

                var response = await _httpClient.PostAsync(__aiApiUrl, content);
                if (!response.IsSuccessStatusCode)
                    return null;

                var jsonResponse = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<AiResponseDto>(jsonResponse, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            catch (Exception ex)
            {
                
                Console.WriteLine($"Error calling AI API: {ex.Message}");
                return null;
            }
        }
    }
}
