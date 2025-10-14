using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManyBox.Models.Locals;
using ManyBox.Services;

namespace ManyBox.ViewModels
{
    public class EnviosViewModel
    {
        private readonly LocalDatabaseService _dbService = new();

        public async Task InsertarEnviosAsync(List<EnvioLocal> envios)
        {
            await _dbService.InsertarEnvioAsync(envios);
        }
    }
}
