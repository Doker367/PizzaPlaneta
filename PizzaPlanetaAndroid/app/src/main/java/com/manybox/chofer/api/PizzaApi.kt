package com.manybox.chofer.api

import retrofit2.http.Body
import retrofit2.http.GET
import retrofit2.http.POST
import retrofit2.http.PUT
import retrofit2.http.DELETE
import retrofit2.Call
import retrofit2.http.Path


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

data class CreateSucursalRequest(
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
    val disponible: Boolean,
    val categoria: String? = null
)

// Productos
data class ProductDto(
    val id: Int,
    val nombre: String,
    val descripcion: String,
    val precio: Double,
    val categoria: String?,
    val calorias: Int?,
    val activo: Boolean?
)

data class CreateProductRequest(
    val nombre: String,
    val descripcion: String,
    val precio: Double,
    val categoria: String?,
    val calorias: Int?,
    val activo: Boolean = true
)

data class UpdateProductRequest(
    val nombre: String?,
    val descripcion: String?,
    val precio: Double?,
    val categoria: String?,
    val calorias: Int?,
)

data class AddMenuItemRequest(
    val productoId: Int,
    val precioEspecial: Double
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
    fun getMenuBySucursalId(@Path("id") sucursalId: Int): Call<List<MenuItemDto>>

    @POST("api/orders")
    fun createOrder(@Body body: CreateOrderRequest): Call<Void>

    // Públicos
    @GET("api/productos")
    fun getProductos(): Call<List<ProductDto>>

    @GET("api/productos/{id}")
    fun getProducto(@Path("id") id: Int): Call<ProductDto>

    // Admin (requiere token)
    @POST("api/sucursales")
    fun createSucursal(@Body body: CreateSucursalRequest): Call<SucursalDto>

    @POST("api/sucursales/{id}/menu")
    fun addProductoToSucursalMenu(@Path("id") sucursalId: Int, @Body body: AddMenuItemRequest): Call<MenuItemDto>

    @POST("api/productos")
    fun createProducto(@Body body: CreateProductRequest): Call<ProductDto>

    @PUT("api/productos/{id}")
    fun updateProducto(@Path("id") id: Int, @Body body: UpdateProductRequest): Call<ProductDto>

    @DELETE("api/productos/{id}")
    fun deleteProducto(@Path("id") id: Int): Call<Void>
}
