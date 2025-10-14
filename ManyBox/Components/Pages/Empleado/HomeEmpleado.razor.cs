using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ManyBox.Utils;

namespace ManyBox.Components.Pages.Empleado
{
    public partial class HomeEmpleado
    {
        [Inject] NavigationManager Navigation { get; set; } = default!;
        [Inject] IJSRuntime JS { get; set; } = default!;

        private bool showNotifications = false;
        private string selectedPeriod = "week";

        private List<NotificationItem> notifications = new()
        {
            new NotificationItem
            {
                Title = "Entrega Urgente",
                Message = "Nuevo paquete prioritario #4532 asignado",
                Time = "Hace 5 minutos",
                Icon = "fas fa-exclamation-circle",
                Priority = "high"
            },
            new NotificationItem
            {
                Title = "Actualización de Sistema",
                Message = "Nueva versión disponible para instalar",
                Time = "Hace 15 minutos",
                Icon = "fas fa-sync",
                Priority = "medium"
            },
            new NotificationItem
            {
                Title = "Reporte Diario",
                Message = "El reporte de entregas está listo",
                Time = "Hace 30 minutos",
                Icon = "fas fa-file-alt",
                Priority = "low"
            }
        };

        private List<DashboardStat> dashboardStats = new()
        {
            new DashboardStat
            {
                Title = "Entregas de Hoy",
                Value = 12,
                Change = "+3 desde ayer",
                Icon = "fas fa-truck-loading",
                Class = "deliveries",
                TrendClass = "positive",
                TrendIcon = "fas fa-arrow-up"
            },
            new DashboardStat
            {
                Title = "Guías Registradas",
                Value = 8,
                Change = "+5 esta semana",
                Icon = "fas fa-clipboard-list",
                Class = "success-rate",
                TrendClass = "positive",
                TrendIcon = "fas fa-arrow-up"
            },
            new DashboardStat
            {
                Title = "Tareas Pendientes",
                Value = 3,
                Change = "Por completar",
                Icon = "fas fa-tasks",
                Class = "delivery-time",
                TrendClass = "neutral",
                TrendIcon = "fas fa-minus"
            },
            new DashboardStat
            {
                Title = "Tiempo Promedio",
                Value = 18,
                Unit = "min",
                Change = "-2.5 min",
                Icon = "fas fa-stopwatch",
                Class = "satisfaction",
                TrendClass = "positive",
                TrendIcon = "fas fa-arrow-down"
            }
        };

        private List<EntregaDiaria> entregasSemana = new()
        {
            new EntregaDiaria { Dia = "Lun", Entregas = 8 },
            new EntregaDiaria { Dia = "Mar", Entregas = 12 },
            new EntregaDiaria { Dia = "Mié", Entregas = 6 },
            new EntregaDiaria { Dia = "Jue", Entregas = 10 },
            new EntregaDiaria { Dia = "Vie", Entregas = 15 },
            new EntregaDiaria { Dia = "Sáb", Entregas = 4 },
            new EntregaDiaria { Dia = "Dom", Entregas = 2 }
        };

        private List<RendimientoMetric> rendimientoMetrics = new()
        {
            new RendimientoMetric { Name = "Lunes", Value = 85 },
            new RendimientoMetric { Name = "Martes", Value = 92 },
            new RendimientoMetric { Name = "Miércoles", Value = 75 },
            new RendimientoMetric { Name = "Jueves", Value = 88 }
        };

        private List<Actividad> actividadesRecientes = new()
        {
            new Actividad
            {
                Titulo = "Paquete entregado",
                Descripcion = "Entregaste el paquete #5432 a María López",
                Time = "Hace 2 horas",
                Icon = "fas fa-box-check"
            },
            new Actividad
            {
                Titulo = "Guía registrada",
                Descripcion = "Registraste una nueva guía para Carlos Gómez",
                Time = "Hace 4 horas",
                Icon = "fas fa-file-alt"
            },
            new Actividad
            {
                Titulo = "Costos consultados",
                Descripcion = "Consultaste el costo de envío a Guadalajara",
                Time = "Hace 6 horas",
                Icon = "fas fa-calculator"
            }
        };

        private string GetAreaPath(List<EntregaDiaria> data)
        {
            var width = 100.0 / (data.Count - 1);
            var path = "M 0 300 ";

            for (int i = 0; i < data.Count; i++)
            {
                var x = i * width;
                var y = 300 - (data[i].Entregas * 15);
                path += $"L {x} {y} ";
            }

            path += $"L {100} 300 Z";
            return path;
        }

        private string GetLinePath(List<EntregaDiaria> data)
        {
            var width = 100.0 / (data.Count - 1);
            var path = "";

            for (int i = 0; i < data.Count; i++)
            {
                var x = i * width;
                var y = 300 - (data[i].Entregas * 15);
                path += i == 0 ? $"M {x} {y} " : $"L {x} {y} ";
            }

            return path;
        }

        private void ToggleNotifications()
        {
            showNotifications = !showNotifications;
        }

        private void CerrarSesion()
        {
            SessionState.IsLoggedIn = false;
            SessionState.Usuario = null;
            Navigation.NavigateTo("/login", true);
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await JS.InvokeVoidAsync("initializeAnimations");
            }
        }
        private string usuario = SessionState.Usuario;
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

        private class EntregaDiaria
        {
            public string Dia { get; set; } = string.Empty;
            public int Entregas { get; set; }
        }

        private class RendimientoMetric
        {
            public string Name { get; set; } = string.Empty;
            public int Value { get; set; }
        }

        private class Actividad
        {
            public string Titulo { get; set; } = string.Empty;
            public string Descripcion { get; set; } = string.Empty;
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
    }
}