package com.manybox.chofer.api

import retrofit2.http.Body
import retrofit2.http.GET
import retrofit2.http.POST
import retrofit2.Call


data class PizzaLoginRequest(val email: String, val password: String)
data class PizzaLoginResponse(val token: String)

data class PizzaRegisterRequest(
    val nombre: String,
    val email: String,
    val password: String,
    val telefono: String
)

data class SucursalDto(
    val id: Int,
    val nombre: String,
    val direccion: String,
    val ciudad: String,
    val estado: String?,
    val telefono: String?,
    val googleMapsUrl: String
)


interface PizzaApiService {
    @POST("api/auth/login")
    fun login(@Body body: PizzaLoginRequest): Call<PizzaLoginResponse>

    @POST("api/auth/register")
    fun register(@Body body: PizzaRegisterRequest): Call<Void>

    @GET("api/sucursales")
    fun getSucursales(): Call<List<SucursalDto>>
}
