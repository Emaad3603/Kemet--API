
using Kemet.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kemet.Core.Services.Interfaces
{
    public interface ISearchInterface
    {
        Task<RagionalSearchDTO> SearchAll ( string textA);
    }
}
