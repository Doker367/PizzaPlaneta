using ManyBoxApi.Helpers;
using ManyBoxApi.Data;
using ManyBoxApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System;
using System.Threading.Tasks;
using System.Net.Http.Json;
using System.Collections.Generic;

public class EnvioApiService
{
    private readonly HttpClient _httpClient;

    public EnvioApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<Envio>> ObtenerEnviosAsync()
    {
        // Igual que otros servicios: usa solo la ruta relativa
        return await _httpClient.GetFromJsonAsync<List<Envio>>("api/envios") ?? new List<Envio>();
    }
}