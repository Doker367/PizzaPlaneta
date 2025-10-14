using ManyBox.Models.Locals;
using ManyBox.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ManyBox.ViewModels
{
    public class PaquetesViewModel
    {
        private readonly LocalDatabaseService _dbService = new();

        public async Task InsertarPaquetesAsync(List<PaquetesLocal> paquetes)
        {
            await _dbService.InsertarPaqueteAsync(paquetes);
        }
    }
}
