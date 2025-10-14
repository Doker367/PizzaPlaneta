package com.manybox.chofer

import android.os.Bundle
import androidx.activity.ComponentActivity
import androidx.activity.compose.setContent
import androidx.compose.runtime.*
import com.manybox.chofer.ui.LoginScreen
import com.manybox.chofer.ui.DashboardScreen
import com.manybox.chofer.api.*
import com.manybox.chofer.model.GuiaAsignada
import retrofit2.Retrofit
import retrofit2.converter.gson.GsonConverterFactory
import retrofit2.Call
import retrofit2.Callback
import retrofit2.Response

class MainActivity : ComponentActivity() {
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContent {
            var isLoggedIn by remember { mutableStateOf(false) }
            var isLoading by remember { mutableStateOf(false) }
            var errorMessage by remember { mutableStateOf<String?>(null) }
            var token by remember { mutableStateOf("") }
            var user by remember { mutableStateOf<User?>(null) }
            var guias by remember { mutableStateOf(listOf<GuiaAsignada>()) }
            var pendientes by remember { mutableStateOf(0) }
            var confirmados by remember { mutableStateOf(0) }
            var incidencias by remember { mutableStateOf(0) }

            val retrofit = Retrofit.Builder()
                .baseUrl("http://100.64.197.11:5000/")
                .addConverterFactory(GsonConverterFactory.create())
                .build()
            val api = retrofit.create(ApiService::class.java)

            if (!isLoggedIn) {
                LoginScreen(
                    onLogin = { username, password ->
                        isLoading = true
                        errorMessage = null
                        val call = api.login(LoginRequest(username, password))
                        call.enqueue(object : Callback<LoginResponse> {
                            override fun onResponse(call: Call<LoginResponse>, response: Response<LoginResponse>) {
                                isLoading = false
                                if (response.isSuccessful) {
                                    val body = response.body()
                                    if (body != null) {
                                        token = "Bearer ${body.token}"
                                        user = body.user
                                        android.util.Log.i("LoginScreen", "Usuario tras login: $user, empleadoId: ${body.user.empleadoId}")
                                        isLoggedIn = true
                                        // Cargar guías al iniciar sesión
                                        cargarGuias(api, body.user.empleadoId, token) { g, p, c, i ->
                                            guias = g
                                            pendientes = p
                                            confirmados = c
                                            incidencias = i
                                        }
                                    }
                                } else {
                                    errorMessage = "Usuario o contraseña incorrectos"
                                }
                            }
                            override fun onFailure(call: Call<LoginResponse>, t: Throwable) {
                                isLoading = false
                                errorMessage = "Error de red: ${t.message}"
                            }
                        })
                    },
                    isLoading = isLoading,
                    errorMessage = errorMessage
                )
            } else {
                DashboardScreen(
                    nombre = user?.nombre ?: "",
                    pendientes = pendientes,
                    confirmados = confirmados,
                    incidencias = incidencias,
                    guias = guias,
                    onRecoger = { guia -> cambiarEstado(api, guia, "En camino", token) { cargarGuias(api, user!!.empleadoId, token) { g, p, c, i -> guias = g; pendientes = p; confirmados = c; incidencias = i } } },
                    onIniciarEntrega = { guia -> cambiarEstado(api, guia, "Último tramo", token) { cargarGuias(api, user!!.empleadoId, token) { g, p, c, i -> guias = g; pendientes = p; confirmados = c; incidencias = i } } },
                    onEntregar = { guia -> cambiarEstado(api, guia, "Entregado", token) { cargarGuias(api, user!!.empleadoId, token) { g, p, c, i -> guias = g; pendientes = p; confirmados = c; incidencias = i } } },
                    onScanBarcode = { /* TODO: Navegar a pantalla de escaneo */ }
                )
            }
        }
    }
}

fun cargarGuias(api: ApiService, empleadoId: Int, token: String, onResult: (List<GuiaAsignada>, Int, Int, Int) -> Unit) {
    val TAG = "ApiLog"
    android.util.Log.i(TAG, "Petición de guías asignadas para empleadoId: $empleadoId, token: $token")
    val call = api.getGuiasAsignadas(empleadoId, token)
    call.enqueue(object : Callback<List<GuiaAsignada>> {
        override fun onResponse(call: Call<List<GuiaAsignada>>, response: Response<List<GuiaAsignada>>) {
            android.util.Log.i(TAG, "Respuesta guías: ${response.code()} ${response.message()}")
            android.util.Log.i(TAG, "Body: ${response.body()} ErrorBody: ${response.errorBody()?.string()}")
            if (response.isSuccessful) {
                val guias = response.body() ?: emptyList()
                android.util.Log.i(TAG, "Guias recibidas: ${guias.size} -> $guias")
                val pendientes = guias.count { it.estadoActual != 3 }
                val confirmados = guias.count { it.estadoActual == 3 }
                val incidencias = guias.count { it.estados.any { e -> e.titulo in listOf("Devuelto", "Extraviado", "Incidencia") } }
                onResult(guias, pendientes, confirmados, incidencias)
            } else {
                android.util.Log.e(TAG, "Error en respuesta de guías: ${response.errorBody()?.string()}")
                onResult(emptyList(), 0, 0, 0)
            }
        }
        override fun onFailure(call: Call<List<GuiaAsignada>>, t: Throwable) {
            android.util.Log.e(TAG, "Fallo en petición de guías: ${t.message}")
            onResult(emptyList(), 0, 0, 0)
        }
    })
}

fun cambiarEstado(api: ApiService, guia: GuiaAsignada, status: String, token: String, onResult: () -> Unit) {
    val TAG = "ApiLog"
    android.util.Log.i(TAG, "Petición cambio de estado: ${guia.envioId} a $status")
    val call = api.cambiarEstadoGuia(guia.envioId, EstadoRequest(status), token)
    call.enqueue(object : Callback<Void> {
        override fun onResponse(call: Call<Void>, response: Response<Void>) {
            android.util.Log.i(TAG, "Respuesta cambio estado: ${response.code()} ${response.message()}")
            onResult()
        }
        override fun onFailure(call: Call<Void>, t: Throwable) {
            android.util.Log.e(TAG, "Fallo en cambio de estado: ${t.message}")
            onResult()
        }
    })
}