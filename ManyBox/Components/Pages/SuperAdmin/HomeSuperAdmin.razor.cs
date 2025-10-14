using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using ManyBox.Utils;
using Microsoft.AspNetCore.SignalR.Client;

namespace ManyBox.Components.Pages.SuperAdmin
{
    public partial class HomeSuperAdmin : IAsyncDisposable
    {
        [Inject] private NavigationManager Navigation { get; set; } = default!;
        [Inject] private IJSRuntime JS { get; set; } = default!;
        [Inject] private HttpClient Http { get; set; } = default!;

        private HubConnection? _hub;
        private Timer? _fallbackTimer;

        // Datos de paquetes
        private int paquetesPendientes = 0;
        private int paquetesEnCamino = 0;
        private int paquetesEntregados = 0;
        private int empleadosActivos = 0;

        // Estadísticas por día
        private List<EntregaDiaria> entregasPorDia = new();

        // Últimos paquetes
        private List<PaqueteResumen> ultimosPaquetes = new();

        // Modal y selección de sucursal
        private bool mostrarEstadisticasSucursal = false;
        private string sucursalSeleccionada = "";

        private List<string> sucursales = new();

        private Dictionary<string, int> EstadisticasPaquetes = new();
        private Dictionary<string, int> EstadisticasEmpleados = new();
        private Dictionary<string, int> EstadisticasSatisfaccion = new();

        // Métricas de la sucursal seleccionada
        private int sucursalPaquetes = 0;
        private int sucursalEmpleados = 0;
        private int sucursalSatisfaccion = 0;

        private string usuario = SessionState.Usuario;

        private int selectedPeriod = 7; // 7 o 30

        protected override async Task OnInitializedAsync()
        {
            await CargarDashboard();
            await ConectarHubAsync();
            // Fallback cada 60s si el hub no emite nada
            _fallbackTimer = new Timer(async _ => await CargarDashboard(), null, TimeSpan.FromSeconds(60), TimeSpan.FromSeconds(60));
        }

        private async Task ConectarHubAsync()
        {
            try
            {
                var baseUrl = Http.BaseAddress?.ToString()?.TrimEnd('/') ?? Navigation.BaseUri.TrimEnd('/');
                var hubUrl = $"{baseUrl}/dashboardhub";

                _hub = new HubConnectionBuilder()
                    .WithUrl(hubUrl)
                    .WithAutomaticReconnect()
                    .Build();

                _hub.On("dashboardUpdate", async () =>
                {
                    await CargarDashboard();
                });

                await _hub.StartAsync();
            }
            catch
            {
                // Ignorar: fallback por timer hará la recarga
            }
        }

        private async Task<bool> TryCargarConsolidado()
        {
            try
            {
                var ts = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                var dto = await Http.GetFromJsonAsync<EstadisticasDto>($"/api/superadmin/estadisticas?_ts={ts}");
                if (dto != null)
                {
                    paquetesPendientes = dto.paquetesPendientes;
                    paquetesEnCamino = dto.paquetesEnCamino;
                    paquetesEntregados = dto.paquetesEntregados;
                    empleadosActivos = dto.empleadosActivos;
                    return true;
                }
            }
            catch { /* ignore */ }
            return false;
        }

        private async Task<int> SafeGetCount(string url)
        {
            try
            {
                var ts = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                url = url.Contains('?') ? $"{url}&_ts={ts}" : $"{url}?_ts={ts}";
                var msg = await Http.GetAsync(url);
                msg.EnsureSuccessStatusCode();
                var contentType = msg.Content.Headers.ContentType?.MediaType ?? "";
                if (contentType.Contains("application/json", StringComparison.OrdinalIgnoreCase))
                {
                    try
                    {
                        var dto = await msg.Content.ReadFromJsonAsync<CountDto>();
                        if (dto != null) return dto.total;
                    }
                    catch
                    {
                        try
                        {
                            var value = await msg.Content.ReadFromJsonAsync<int>();
                            return value;
                        }
                        catch { }
                    }
                }
                else
                {
                    var raw = await msg.Content.ReadAsStringAsync();
                    if (int.TryParse(raw, out var n)) return n;
                }
            }
            catch { }
            return 0;
        }

        private async Task CargarDashboard()
        {
            var okConsolidado = await TryCargarConsolidado();

            if (!okConsolidado || (paquetesPendientes == 0 && paquetesEnCamino == 0 && paquetesEntregados == 0 && empleadosActivos == 0))
            {
                var pendientesTask = SafeGetCount("/api/superadmin/paquetes/pendientes");
                var enCaminoTask = SafeGetCount("/api/superadmin/paquetes/en-camino");
                var entregadosTask = SafeGetCount("/api/superadmin/paquetes/entregados");
                var empleadosTask = SafeGetCount("/api/superadmin/empleados/activos");
                await Task.WhenAll(pendientesTask, enCaminoTask, entregadosTask, empleadosTask);
                paquetesPendientes = pendientesTask.Result;
                paquetesEnCamino = enCaminoTask.Result;
                paquetesEntregados = entregadosTask.Result;
                empleadosActivos = empleadosTask.Result;
            }

            try
            {
                var ts = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                var entregas = await Http.GetFromJsonAsync<List<EntregaDiaria>>($"/api/superadmin/entregas-por-dia?days={selectedPeriod}&_ts={ts}");
                if (entregas != null)
                    entregasPorDia = entregas;
            }
            catch { entregasPorDia = new(); }

            try
            {
                var ts = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                var ultimos = await Http.GetFromJsonAsync<List<PaqueteResumen>>($"/api/superadmin/ultimos-paquetes?_ts={ts}");
                if (ultimos != null)
                    ultimosPaquetes = ultimos;
            }
            catch { ultimosPaquetes = new(); }

            try
            {
                var ts = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                var sucursalesApi = await Http.GetFromJsonAsync<List<SucursalDto>>($"/api/Sucursales?_ts={ts}");
                if (sucursalesApi != null)
                    sucursales = sucursalesApi.Select(s => s.Nombre).ToList();

                EstadisticasPaquetes.Clear();
                EstadisticasEmpleados.Clear();
                EstadisticasSatisfaccion.Clear();
                foreach (var suc in sucursales)
                {
                    try
                    {
                        var est = await Http.GetFromJsonAsync<EstadisticasSucursalDto>($"/api/superadmin/estadisticas-sucursal?sucursal={Uri.EscapeDataString(suc)}&_ts={ts}");
                        if (est != null)
                        {
                            EstadisticasPaquetes[suc] = est.paquetes;
                            EstadisticasEmpleados[suc] = est.empleados;
                            EstadisticasSatisfaccion[suc] = est.satisfaccion;
                        }
                    }
                    catch { }
                }

                if (!string.IsNullOrEmpty(sucursalSeleccionada))
                    await CargarEstadisticasSucursalSeleccionada();
            }
            catch
            {
                sucursales = new();
                EstadisticasPaquetes.Clear();
                EstadisticasEmpleados.Clear();
                EstadisticasSatisfaccion.Clear();
            }

            StateHasChanged();
            try { await JS.InvokeVoidAsync("initializeAnimations"); } catch { }
        }

        private async Task OnSucursalChanged(ChangeEventArgs e)
        {
            sucursalSeleccionada = e.Value?.ToString() ?? string.Empty;
            await CargarEstadisticasSucursalSeleccionada();
        }

        private async Task OnPeriodChanged(ChangeEventArgs e)
        {
            if (e?.Value == null) return;
            if (int.TryParse(e.Value.ToString(), out var days))
            {
                selectedPeriod = days;
                await CargarDashboard();
                StateHasChanged();
            }
        }

        private async Task CargarEstadisticasSucursalSeleccionada()
        {
            if (string.IsNullOrEmpty(sucursalSeleccionada))
            {
                sucursalPaquetes = sucursalEmpleados = sucursalSatisfaccion = 0;
                StateHasChanged();
                return;
            }

            try
            {
                var ts = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                var est = await Http.GetFromJsonAsync<EstadisticasSucursalDto>($"/api/superadmin/estadisticas-sucursal?sucursal={Uri.EscapeDataString(sucursalSeleccionada)}&_ts={ts}");
                if (est != null)
                {
                    sucursalPaquetes = est.paquetes;
                    sucursalEmpleados = est.empleados;
                    sucursalSatisfaccion = est.satisfaccion;
                }
            }
            catch
            {
                sucursalPaquetes = sucursalEmpleados = sucursalSatisfaccion = 0;
            }

            StateHasChanged();
            try { await JS.InvokeVoidAsync("initializeAnimations"); } catch { }
        }

        public async ValueTask DisposeAsync()
        {
            if (_fallbackTimer != null)
            {
                await Task.Run(() => _fallbackTimer.Dispose());
            }
            if (_hub != null)
            {
                try { await _hub.DisposeAsync(); } catch { }
            }
        }

        private string GetAreaPath(List<EntregaDiaria> data)
        {
            if (data == null || data.Count == 0) return string.Empty;
            var max = Math.Max(1, data.Max(d => d.Entregas));
            double height = 300.0;
            var width = 100.0 / Math.Max(1, (data.Count - 1));
            string PathCmd(double x, double y) => $"{x:0.###} {y:0.###}";

            var points = new List<(double x, double y)>();
            for (int i = 0; i < data.Count; i++)
            {
                var x = i * width;
                var y = height - (data[i].Entregas / (double)max) * (height - 10); // margen superior 10
                points.Add((x, y));
            }

            // suave con curvas cúbicas
            var path = $"M 0 {height} ";
            path += $"L {PathCmd(points[0].x, points[0].y)} ";
            for (int i = 1; i < points.Count; i++)
            {
                var p0 = points[i - 1];
                var p1 = points[i];
                var cx1 = p0.x + width / 2;
                var cy1 = p0.y;
                var cx2 = p1.x - width / 2;
                var cy2 = p1.y;
                path += $"C {PathCmd(cx1, cy1)} {PathCmd(cx2, cy2)} {PathCmd(p1.x, p1.y)} ";
            }
            path += $"L 100 {height} Z";
            return path;
        }

        private string GetLinePath(List<EntregaDiaria> data)
        {
            if (data == null || data.Count == 0) return string.Empty;
            var max = Math.Max(1, data.Max(d => d.Entregas));
            double height = 300.0;
            var width = 100.0 / Math.Max(1, (data.Count - 1));
            string PathCmd(double x, double y) => $"{x:0.###} {y:0.###}";

            var points = new List<(double x, double y)>();
            for (int i = 0; i < data.Count; i++)
            {
                var x = i * width;
                var y = height - (data[i].Entregas / (double)max) * (height - 10);
                points.Add((x, y));
            }

            var path = $"M {PathCmd(points[0].x, points[0].y)} ";
            for (int i = 1; i < points.Count; i++)
            {
                var p0 = points[i - 1];
                var p1 = points[i];
                var cx1 = p0.x + width / 2;
                var cy1 = p0.y;
                var cx2 = p1.x - width / 2;
                var cy2 = p1.y;
                path += $"C {PathCmd(cx1, cy1)} {PathCmd(cx2, cy2)} {PathCmd(p1.x, p1.y)} ";
            }
            return path;
        }

        private string GetActivityIconStyle(int index)
        {
            if (index == 0) return "background: linear-gradient(135deg, #00f2fe 0%, #4facfe 100%);";
            if (index == 1) return "background: linear-gradient(135deg, #f6d365 0%, #fda085 100%);";
            return "background: linear-gradient(135deg, #84fab0 0%, #8fd3f4 100%);";
        }

        private string GetStatusClass(string estado)
        {
            return estado.ToLower().Replace(" ", "-");
        }

        private void MostrarEstadisticasSucursal() => mostrarEstadisticasSucursal = true;

        private void CerrarEstadisticasSucursal()
        {
            mostrarEstadisticasSucursal = false;
            sucursalSeleccionada = "";
        }

        private void CerrarSesion()
        {
            SessionState.IsLoggedIn = false;
            SessionState.Usuario = null;
            Navigation.NavigateTo("/login", true);
        }

        private string GetTimeAgo(DateTime fecha)
        {
            var timeSpan = DateTime.Now - fecha;
            if (timeSpan.TotalMinutes < 60)
                return $"hace {(int)timeSpan.TotalMinutes}m";
            if (timeSpan.TotalHours < 24)
                return $"hace {(int)timeSpan.TotalHours}h";
            return $"hace {(int)timeSpan.TotalDays}d";
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                try { await JS.InvokeVoidAsync("initializeAnimations"); } catch { }
            }
        }

        public class PaqueteResumen
        {
            public int Id { get; set; }
            public string RemitenteNombre { get; set; } = string.Empty;
            public string DestinatarioNombre { get; set; } = string.Empty;
            public string Estado { get; set; } = string.Empty;
            public DateTime FechaRegistro { get; set; }
            public string Remitente => RemitenteNombre;
            public string Destinatario => DestinatarioNombre;
        }

        public class EntregaDiaria
        {
            public string Dia { get; set; } = string.Empty;
            public int Entregas { get; set; }
        }

        public class EstadisticasDto
        {
            public int paquetesPendientes { get; set; }
            public int paquetesEnCamino { get; set; }
            public int paquetesEntregados { get; set; }
            public int empleadosActivos { get; set; }
        }

        public class EstadisticasSucursalDto
        {
            public int paquetes { get; set; }
            public int empleados { get; set; }
            public int satisfaccion { get; set; }
        }

        public class SucursalDto
        {
            public int Id { get; set; }
            public string Nombre { get; set; } = string.Empty;
        }

        private class CountDto
        {
            public int total { get; set; }
        }

        private double ScaleY(int value, int max)
        {
            double height = 300.0;
            // margen superior 10px para no pegarse a la parte alta
            return height - (value / Math.Max(1.0, max)) * (height - 10);
        }
    }
}