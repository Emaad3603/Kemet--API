using System.Text.Json;
using System.Text.Json.Serialization;

namespace Kemet.APIs.Helpers
{
    public static class JsonOptionsHelper
    {
        public static JsonSerializerOptions GetOptions()
        {
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                ReferenceHandler = ReferenceHandler.Preserve,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals
            };

            options.Converters.Add(new DateOnlyConverter());

            return options;
        }
    }
}
