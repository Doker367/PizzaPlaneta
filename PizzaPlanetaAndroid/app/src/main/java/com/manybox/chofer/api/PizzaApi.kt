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

// Menu de una sucursal
data class MenuItemDto(
    val productoId: Int,
    val nombre: String,
    val descripcion: String,
    val precio: Double,
    val calorias: Int?,
    val disponible: Boolean
)

// Crear pedido
data class OrderItemRequest(
    val productoId: Int,
    val cantidad: Int
)

data class CreateOrderRequest(
    val sucursalId: Int,
    val items: List<OrderItemRequest>
)


interface PizzaApiService {
    @POST("api/auth/login")
    fun login(@Body body: PizzaLoginRequest): Call<PizzaLoginResponse>

    @POST("api/auth/register")
    fun register(@Body body: PizzaRegisterRequest): Call<Void>

    @GET("api/sucursales")
    fun getSucursales(): Call<List<SucursalDto>>

    @GET("api/sucursales/{id}/menu")
    fun getMenuBySucursalId(@retrofit2.http.Path("id") sucursalId: Int): Call<List<MenuItemDto>>

    @POST("api/orders")
    fun createOrder(@Body body: CreateOrderRequest): Call<Void>
}
