package com.manybox.chofer.api

import com.manybox.chofer.model.GuiaAsignada
import retrofit2.http.Body
import retrofit2.http.GET
import retrofit2.http.POST
import retrofit2.http.Path
import retrofit2.http.Header
import retrofit2.Call

// Login
 data class LoginRequest(val Username: String, val Password: String)
 data class LoginResponse(val token: String, val user: User)
data class User(
    val id: Int,
    val username: String,
    val nombre: String,
    val apellido: String,
    val rol: String,
    val activo: Boolean,
    val empleadoId: Int
)

interface ApiService {
    @POST("api/auth/login")
    fun login(@Body request: LoginRequest): Call<LoginResponse>

    @GET("api/envios/asignados/empleado/{empleadoId}")
    fun getGuiasAsignadas(@Path("empleadoId") empleadoId: Int, @Header("Authorization") token: String): Call<List<GuiaAsignada>>

    @POST("api/envios/{envioId}/cambiar-estado")
    fun cambiarEstadoGuia(@Path("envioId") envioId: Int, @Body body: EstadoRequest, @Header("Authorization") token: String): Call<Void>
}

data class EstadoRequest(val status: String)
