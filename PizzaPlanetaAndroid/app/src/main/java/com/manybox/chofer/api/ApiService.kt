package com.manybox.chofer.api

import com.manybox.chofer.model.GuiaAsignada
import com.manybox.chofer.model.BitacoraEntrega
import com.manybox.chofer.model.EmpleadoPerfil
import retrofit2.http.Body
import retrofit2.http.GET
import retrofit2.http.POST
import retrofit2.http.Path
import retrofit2.http.Header
import retrofit2.Call
import retrofit2.http.PUT
import retrofit2.http.Query
import com.google.gson.annotations.SerializedName

data class EntregasPorDia(val Fecha: String, val Cantidad: Int)

// Login
 data class LoginRequest(val Username: String, val Password: String)
 data class LoginResponse(val token: String, val user: User)
data class User(
    @SerializedName("Id") val id: Int,
    @SerializedName("Username") val username: String?,
    @SerializedName("Nombre") val nombre: String?,
    @SerializedName("Apellido") val apellido: String?,
    @SerializedName("Rol") val rol: String?,
    @SerializedName("Activo") val activo: Boolean,
    @SerializedName("EmpleadoId") val empleadoId: Int
)

interface ApiService {
    @POST("api/auth/login")
    fun login(@Body request: LoginRequest): Call<LoginResponse>

    @GET("api/envios/asignados/empleado/{empleadoId}")
    fun getGuiasAsignadas(@Path("empleadoId") empleadoId: Int, @Header("Authorization") token: String): Call<List<GuiaAsignada>>

    @POST("api/envios/{envioId}/cambiar-estado")
    fun cambiarEstadoGuia(@Path("envioId") envioId: Int, @Body body: EstadoRequest, @Header("Authorization") token: String): Call<Void>

    @GET("api/envios/bitacora/{empleadoId}")
    fun getBitacoraChofer(@Path("empleadoId") empleadoId: Int, @Header("Authorization") token: String): Call<List<BitacoraEntrega>>

    @GET("api/empleados/{empleadoId}/perfil")
    fun getEmpleadoPerfil(@Path("empleadoId") empleadoId: Int, @Header("Authorization") token: String): Call<EmpleadoPerfil>

    @GET("api/empleados/{empleadoId}/entregas-por-dia")
    fun getEntregasPorDia(@Path("empleadoId") empleadoId: Int, @Header("Authorization") token: String): Call<List<EntregasPorDia>>

    // Notificaciones (según NotificacionesController)
    @GET("api/notificaciones/usuario/{usuarioId}")
    fun getNotificacionesUsuario(@Path("usuarioId") usuarioId: Int, @Header("Authorization") token: String): Call<List<NotificacionUsuarioVM>>

    @PUT("api/usuarios/{usuarioId}/notificaciones/{notificacionId}/leida")
    fun marcarNotificacionLeida(@Path("usuarioId") usuarioId: Int, @Path("notificacionId") notificacionId: Int, @Header("Authorization") token: String): Call<Void>

    // MAUI usa este endpoint para marcar todas como leídas
    @PUT("api/usuarios/{usuarioId}/notificaciones/marcar-todas-leidas")
    fun marcarTodasLeidas(@Path("usuarioId") usuarioId: Int, @Header("Authorization") token: String): Call<Void>

    // Alternativa que crea la entrega si no existe: PUT api/notificaciones/entrega/{notificacionId}/leida?usuarioId=123
    @PUT("api/notificaciones/entrega/{notificacionId}/leida")
    fun marcarEntregaLeida(
        @Path("notificacionId") notificacionId: Int,
        @Query("usuarioId") usuarioId: Int,
        @Header("Authorization") token: String
    ): Call<Void>
}

data class EstadoRequest(val status: String)

// ViewModel de notificación (api/notificaciones/usuario/{usuarioId})
data class NotificacionUsuarioVM(
    @SerializedName("Id") val id: Int,
    @SerializedName("Tipo") val tipo: String?,
    @SerializedName("Titulo") val titulo: String?,
    @SerializedName("Mensaje") val mensaje: String?,
    @SerializedName("Prioridad") val prioridad: String?,
    @SerializedName("FechaCreacion") val fechaCreacion: String?,
    @SerializedName("Datos") val datos: String?,
    @SerializedName("Expiracion") val expiracion: String?,
    @SerializedName("Leido") val leido: Boolean
)
