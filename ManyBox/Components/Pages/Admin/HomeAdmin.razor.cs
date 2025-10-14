using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using ManyBox.Utils;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Json;
using System.Linq;
using ManyBox.Models.Client; // reemplaza ManyBoxApi.DTOs y Models

namespace ManyBox.Components.Pages.Admin
{
    public partial class HomeAdmin
    {
        [Inject] NavigationManager Navigation { get; set; } = default!;
        [Inject] IJSRuntime JS { get; set; } = default!;
        [Inject] private HttpClient Http { get; set; } = default!;

        private class DashboardStat
        {
            public string Title { get; set; } = string.Empty;
            public int Value { get; set; }
            public string Unit { get; set; } = string.Empty;
            public string Change { get; set; } = string.Empty;
            public string Icon { get; set; } = string.Empty;
            public string Class { get; set; } = string.Empty;
            public string TrendClass { get; set; } = string.Empty;
            public string TrendIcon { get; set; } = string.Empty;
        }

        private class ChartData
        {
            public string Label { get; set; } = string.Empty;
            public int DeliveredValue { get; set; }
            public int PendingValue { get; set; }
        }

        private class BranchMetric
        {
            public string Name { get; set; } = string.Empty;
            public int Value { get; set; }
        }

        private class ActivityItem
        {
            public string Title { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;
            public string Time { get; set; } = string.Empty;
            public string Icon { get; set; } = string.Empty;
        }

        private class NotificationItem
        {
            public string Title { get; set; } = string.Empty;
            public string Message { get; set; } = string.Empty;
            public string Time { get; set; } = string.Empty;
            public string Icon { get; set; } = string.Empty;
            public string Priority { get; set; } = string.Empty;
        }

        private bool showNotifications = false;
        private string selectedPeriod = "month";

        private List<NotificationItem> notifications = new();
        private List<DashboardStat> dashboardStats = new();
        private List<ChartData> monthlyData = new();
        private List<BranchMetric> branchMetrics = new();
        private List<ActivityItem> recentActivity = new();

        private List<PaqueteDto> paquetes = new();
        private List<PaqueteDto> paquetesFiltrados = new();

        private string filtroDestinatario = "";
        private DateTime? filtroFecha = null;
        private string filtroEstado = "";

        private string GetAreaPath(List<ChartData> data, int dataIndex)
        {
            var width = 100.0 / (data.Count - 1);
            var path = "M 0 300 ";

            for (int i = 0; i < data.Count; i++)
            {
                var x = i * width;
                var y = dataIndex == 0 ?
                    300 - (data[i].DeliveredValue * 3) :
                    300 - (data[i].PendingValue * 3);
                path += $"L {x} {y} ";
            }

            path += $"L {100} 300 Z";
            return path;
        }

        private string GetLinePath(List<ChartData> data, int dataIndex)
        {
            var width = 100.0 / (data.Count - 1);
            var path = "";

            for (int i = 0; i < data.Count; i++)
            {
                var x = i * width;
                var y = dataIndex == 0 ?
                    300 - (data[i].DeliveredValue * 3) :
                    300 - (data[i].PendingValue * 3);
                path += i == 0 ? $"M {x} {y} " : $"L {x} {y} ";
            }

            return path;
        }

        private void ToggleNotifications()
        {
            showNotifications = !showNotifications;
        }

        private void NavigateToSettings()
        {
            Navigation.NavigateTo("/admin/settings");
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await JS.InvokeVoidAsync("initializeAnimations");
            }
        }

        protected override async Task OnInitializedAsync()
        {
            try
            {
                var stats = await Http.GetFromJsonAsync<DashboardStatsDto>("api/dashboard/stats");
                if (stats != null)
                {
                    dashboardStats = new List<DashboardStat>
                    {
                        new DashboardStat { Title = "Envíos Activos", Value = stats.EnviosActivos, Icon = "fas fa-shipping-fast", Class = "deliveries" },
                        new DashboardStat { Title = "Tasa de Entrega", Value = stats.TasaEntrega, Unit = "%", Icon = "fas fa-check-circle", Class = "success-rate" },
                        new DashboardStat { Title = "Nuevos Paquetes", Value = stats.NuevosPaquetes, Icon = "fas fa-box", Class = "new-packages" },
                        new DashboardStat { Title = "Satisfacción", Value = stats.Satisfaccion, Unit = "%", Icon = "fas fa-smile", Class = "satisfaction" }
                    };
                }

                var entregasMensuales = await Http.GetFromJsonAsync<List<EntregasMensualesDto>>("api/dashboard/entregas-mensuales");
                if (entregasMensuales != null)
                {
                    monthlyData = entregasMensuales.Select(e => new ChartData
                    {
                        Label = NombreMes(e.Mes),
                        DeliveredValue = e.Entregados,
                        PendingValue = e.Pendientes
                    }).ToList();
                }

                var actividades = await Http.GetFromJsonAsync<List<ActividadRecienteDto>>("api/dashboard/actividad-reciente");
                if (actividades != null)
                {
                    recentActivity = actividades.Select(a => new ActivityItem
                    {
                        Title = a.Titulo,
                        Description = a.Descripcion,
                        Time = a.Fecha.ToString("HH:mm dd/MM"),
                        Icon = a.Icono
                    }).ToList();
                }

                var notifs = await Http.GetFromJsonAsync<List<NotificacionDto>>("api/dashboard/notificaciones");
                if (notifs != null)
                {
                    notifications = notifs.Select(n => new NotificationItem
                    {
                        Title = n.Titulo,
                        Message = n.Mensaje,
                        Time = n.Fecha.ToString("HH:mm dd/MM"),
                        Icon = "fas fa-bell",
                        Priority = n.Prioridad
                    }).ToList();
                }

                // Cargar lista de paquetes desde la API (endpoint corregido)
                var paquetesApi = await Http.GetFromJsonAsync<List<PaqueteDto>>("api/paquetes");
                if (paquetesApi != null)
                {
                    // Mapeo de PaqueteDto a PaqueteModel si es necesario, o cambiar el tipo de la lista.
                    // Por ahora, asumimos que el modelo es compatible o se puede adaptar.
                    // paquetes = paquetesApi; // Esto fallará si los modelos no son iguales.
                    // paquetesFiltrados = paquetes.ToList();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al cargar el dashboard o los paquetes: {ex.Message}");
                dashboardError = $"Error al cargar el dashboard o los paquetes: {ex.Message}";
                paquetes = new();
                paquetesFiltrados = new();
            }
        }

        private string NombreMes(int mes)
        {
            // Devuelve el nombre del mes en español
            return mes switch
            {
                1 => "Ene",
                2 => "Feb",
                3 => "Mar",
                4 => "Abr",
                5 => "May",
                6 => "Jun",
                7 => "Jul",
                8 => "Ago",
                9 => "Sep",
                10 => "Oct",
                11 => "Nov",
                12 => "Dic",
                _ => ""
            };
        }

        private string usuario = SessionState.Usuario;

        private async Task RecargarDashboardAsync()
        {
            // Estadísticas principales
            var stats = await Http.GetFromJsonAsync<DashboardStatsDto>("http://localhost:53649/api/dashboard/stats");
            dashboardStats = new List<DashboardStat>
            {
                new DashboardStat
                {
                    Title = "Envíos Activos",
                    Value = stats.EnviosActivos,
                    Unit = "",
                    Change = "+12.5%",
                    Icon = "fas fa-shipping-fast",
                    Class = "deliveries",
                    TrendClass = "positive",
                    TrendIcon = "fas fa-arrow-up"
                },
                new DashboardStat
                {
                    Title = "Tasa de Entrega",
                    Value = stats.TasaEntrega,
                    Unit = "%",
                    Change = "+2.3%",
                    Icon = "fas fa-check-circle",
                    Class = "success-rate",
                    TrendClass = "positive",
                    TrendIcon = "fas fa-arrow-up"
                },
                new DashboardStat
                {
                    Title = "Nuevos Paquetes",
                    Value = stats.NuevosPaquetes,
                    Unit = "",
                    Change = "+1.1%",
                    Icon = "fas fa-box",
                    Class = "new-packages",
                    TrendClass = "positive",
                    TrendIcon = "fas fa-arrow-up"
                },
                new DashboardStat
                {
                    Title = "Satisfacción",
                    Value = stats.Satisfaccion,
                    Unit = "%",
                    Change = "+1.8%",
                    Icon = "fas fa-smile",
                    Class = "satisfaction",
                    TrendClass = "positive",
                    TrendIcon = "fas fa-arrow-up"
                }
            };

            // Entregas mensuales
            var entregasMensuales = await Http.GetFromJsonAsync<List<EntregasMensualesDto>>("http://localhost:53649/api/dashboard/entregas-mensuales");
            monthlyData = entregasMensuales.Select(e => new ChartData
            {
                Label = NombreMes(e.Mes),
                DeliveredValue = e.Entregados,
                PendingValue = e.Pendientes
            }).ToList();

            // Rendimiento por sucursal
            var sucursales = await Http.GetFromJsonAsync<List<SucursalRendimientoDto>>("http://localhost:53649/api/dashboard/rendimiento-sucursales");
            branchMetrics = sucursales.Select(s => new BranchMetric
            {
                Name = s.Nombre,
                Value = s.Porcentaje
            }).ToList();

            // Actividad reciente
            var actividades = await Http.GetFromJsonAsync<List<ActividadRecienteDto>>("http://localhost:53649/api/dashboard/actividad-reciente");
            recentActivity = actividades.Select(a => new ActivityItem
            {
                Title = a.Titulo,
                Description = a.Descripcion,
                Time = a.Fecha.ToString("HH:mm dd/MM"),
                Icon = a.Icono
            }).ToList();

            // Notificaciones
            var notifs = await Http.GetFromJsonAsync<List<NotificacionDto>>("http://localhost:53649/api/dashboard/notificaciones");
            notifications = notifs.Select(n => new NotificationItem
            {
                Title = n.Titulo,
                Message = n.Mensaje,
                Time = n.Fecha.ToString("HH:mm dd/MM"),
                Icon = "fas fa-bell",
                Priority = n.Prioridad
            }).ToList();

            StateHasChanged(); // Actualiza la UI
        }

        private string dashboardError = string.Empty;

        private void FiltrarPaquetes()
        {
            paquetesFiltrados = paquetes.Where(p =>
                (string.IsNullOrWhiteSpace(filtroDestinatario) || p.DestinatarioNombre.Contains(filtroDestinatario, StringComparison.OrdinalIgnoreCase))
            ).ToList();
        }

        private string CalcularCambioPorcentual(int actual, int anterior)
        {
            if (anterior == 0) return "+100%";
            var cambio = ((double)(actual - anterior) / anterior) * 100;
            var signo = cambio >= 0 ? "+" : "";
            return $"{signo}{cambio:F1}%";
        }
    }
}
