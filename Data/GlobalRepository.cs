using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaGestionPersonal.Data
{
    public static class GlobalRepository
    {
        public static InMemoryRepository Repository { get; } = new InMemoryRepository();
    }
}
