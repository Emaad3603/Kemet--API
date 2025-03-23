using Kemet.Core.Entities.AI_Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kemet.Core.Services.Interfaces
{
    public interface IAiService
    {
        Task<AiResponseDto> CallAiApiAsync(AiRequestDto requestDto, string userId);
    }
}
