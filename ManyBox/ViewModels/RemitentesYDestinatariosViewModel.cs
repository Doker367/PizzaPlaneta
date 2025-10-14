using ManyBox.Models.Locals;
using ManyBox.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManyBox.ViewModels
{
    public class RemitentesYDestinatariosViewModel
    {
        private readonly LocalDatabaseService _dbService = new();

        public async Task InsertarRemitentesAsync(List<RemitenteLocal> remitentes)
        {
            await _dbService.InsertarRemitenteAsync(remitentes);
        }

        public async Task InsertarDestinatariosAsync(List<DestinatarioLocal> destinatarios)
        {
            await _dbService.InsertarDestinatarioAsync(destinatarios);
        }
    }
}
