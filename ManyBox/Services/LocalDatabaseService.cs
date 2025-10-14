using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using ManyBox.Models.Locals; 

namespace ManyBox.Services
{
    public class LocalDatabaseService
    {
        private readonly SQLiteAsyncConnection _connection;
       
        public LocalDatabaseService()
        {
            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "app_local.db");
            _connection = new SQLiteAsyncConnection(dbPath);
            _connection.CreateTableAsync<RemitenteLocal>();
            _connection.CreateTableAsync<DestinatarioLocal>();
            _connection.CreateTableAsync<EnvioLocal>();
            _connection.CreateTableAsync<PaquetesLocal>();
            _connection.CreateTableAsync<EmpleadoLocal>();
            _connection.CreateTableAsync<ContactosLocal>();
        }
        
        public async Task InsertarContactoAsync(List<ContactosLocal> contacto)
        {
            await _connection.DeleteAllAsync<ContactosLocal>();
            await _connection.InsertAsync(contacto);
        }
        public async Task InsertarEmpleados (List<EmpleadoLocal> empleados)
        {
            await _connection.DeleteAllAsync<EmpleadoLocal>();
            await _connection.InsertAllAsync(empleados);
        }
        public async Task InsertarRemitenteAsync(List<RemitenteLocal> remitentes)
        {
            await _connection.DeleteAllAsync<RemitenteLocal>();
            await _connection.InsertAllAsync(remitentes);
        }
        public async Task InsertarDestinatarioAsync(List<DestinatarioLocal> destinatarios)
        {
            await _connection.DeleteAllAsync<DestinatarioLocal>();
            await _connection.InsertAllAsync(destinatarios);
        }
        public async Task InsertarEnvioAsync(List<EnvioLocal> envios)
        {
            await _connection.DeleteAllAsync<EnvioLocal>();
            await _connection.InsertAllAsync(envios);
        }
        public async Task InsertarPaqueteAsync(List<PaquetesLocal> paquetes)
        {
            await _connection.DeleteAllAsync<PaquetesLocal>();
            await _connection.InsertAllAsync(paquetes);
        }
        public async Task<List<ContactosLocal>> GetAllContactosAsync()
        {
            return await _connection.Table<ContactosLocal>().ToListAsync();
        }
        public async Task<List<EmpleadoLocal>> GetAllEmpleadosAsync()
        {
            return await _connection.Table<EmpleadoLocal>().ToListAsync();
        }
        public async Task<List<RemitenteLocal>> GetAllRemitentesAsync()
        {
            return await _connection.Table<RemitenteLocal>().ToListAsync();
        }
        public async Task<List<DestinatarioLocal>> GetAllDestinatariosAsync()
        {
            return await _connection.Table<DestinatarioLocal>().ToListAsync();
        }
        public async Task<List<EnvioLocal>> GetAllEnviosAsync()
        {
            return await _connection.Table<EnvioLocal>().ToListAsync();
        }
        public async Task<List<PaquetesLocal>> GetAllPaquetesAsync()
        {
            return await _connection.Table<PaquetesLocal>().ToListAsync();
        }
    }
}
