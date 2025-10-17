package com.manybox.chofer

import android.os.Bundle
import androidx.activity.ComponentActivity
import androidx.activity.compose.setContent
import androidx.compose.runtime.*
import com.manybox.chofer.ui.LoginScreen
import com.manybox.chofer.ui.DashboardScreen
import com.manybox.chofer.api.*
import com.manybox.chofer.model.GuiaAsignada
import com.manybox.chofer.model.BitacoraEntrega
import com.manybox.chofer.model.EmpleadoPerfil
import retrofit2.Retrofit
import retrofit2.converter.gson.GsonConverterFactory
import retrofit2.Call
import retrofit2.Callback
import retrofit2.Response
import com.microsoft.signalr.HubConnection
import com.microsoft.signalr.HubConnectionBuilder
import android.widget.Toast
import androidx.lifecycle.lifecycleScope
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.launch
import kotlinx.coroutines.withContext
import kotlinx.coroutines.delay
import okhttp3.OkHttpClient
import okhttp3.logging.HttpLoggingInterceptor
import java.util.concurrent.TimeUnit
import android.os.Build
import okhttp3.Interceptor
import java.net.ConnectException

class MainActivity : ComponentActivity() {
    private var hubConnection: HubConnection? = null
    private var bitHubConnection: HubConnection? = null
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContent {
            var isLoggedIn by remember { mutableStateOf(false) }
            var isLoading by remember { mutableStateOf(false) }
            var errorMessage by remember { mutableStateOf<String?>(null) }
            var token by remember { mutableStateOf("") }
            var user by remember { mutableStateOf<User?>(null) }
            var empleadoPerfil by remember { mutableStateOf<EmpleadoPerfil?>(null) }
            var guias by remember { mutableStateOf(listOf<GuiaAsignada>()) }
            var bitacoraEntregas by remember { mutableStateOf(listOf<BitacoraEntrega>()) }
            var entregasPorDia by remember { mutableStateOf(listOf<com.manybox.chofer.api.EntregasPorDia>()) }
            var pendientes by remember { mutableStateOf(0) }
            var confirmados by remember { mutableStateOf(0) }
            var incidencias by remember { mutableStateOf(0) }
            var notificaciones by remember { mutableStateOf(listOf<com.manybox.chofer.api.NotificacionUsuarioVM>()) }
            var notificacionesNoLeidas by remember { mutableStateOf(0) }
            var notifsConectado by remember { mutableStateOf(false) }

            val retrofit = Retrofit.Builder()
                .baseUrl(getApiBaseUrl())
                .client(buildOkHttp())
                .addConverterFactory(GsonConverterFactory.create())
                .build()
            val api = retrofit.create(ApiService::class.java)

            // Preparación diferida de hubs; se construyen con token después del login

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
                                        
                                        // Cargar datos del chofer
                                        // Cargar datos del chofer (si existe empleadoId válido)
                                        val perfilIdCheck = body.user.empleadoId
                                        if (perfilIdCheck > 0) {
                                            cargarPerfilEmpleado(api, perfilIdCheck, token) { perfil ->
                                                empleadoPerfil = perfil ?: EmpleadoPerfil(
                                                    id = perfilIdCheck,
                                                    nombre = body.user.nombre ?: "",
                                                    apellido = body.user.apellido ?: "",
                                                    nombreCompleto = "${body.user.nombre ?: ""} ${body.user.apellido ?: ""}",
                                                    correo = body.user.username ?: "",
                                                    telefono = "",
                                                    sucursalNombre = "No asignada",
                                                    totalEntregas = 0,
                                                    entregasCompletadas = 0,
                                                    entregasPendientes = 0,
                                                    entregasHoy = 0,
                                                    entregasSemana = 0,
                                                    entregasMes = 0
                                                )
                                            }
                                        } else {
                                            // No hay empleado asociado; crear un perfil seguro con valores por defecto
                                            empleadoPerfil = EmpleadoPerfil(
                                                id = perfilIdCheck,
                                                nombre = body.user.nombre ?: "",
                                                apellido = body.user.apellido ?: "",
                                                nombreCompleto = "${body.user.nombre ?: ""} ${body.user.apellido ?: ""}",
                                                correo = body.user.username ?: "",
                                                telefono = "",
                                                sucursalNombre = "No asignada",
                                                totalEntregas = 0,
                                                entregasCompletadas = 0,
                                                entregasPendientes = 0,
                                                entregasHoy = 0,
                                                entregasSemana = 0,
                                                entregasMes = 0
                                            )
                                        }
                                        // Cargar datos históricos para gráfica solo si hay empleado válido
                                        val perfilId = body.user.empleadoId
                                        if (perfilId > 0) {
                                            val callGraf = api.getEntregasPorDia(perfilId, token)
                                            callGraf.enqueue(object : Callback<List<com.manybox.chofer.api.EntregasPorDia>> {
                                                override fun onResponse(call: Call<List<com.manybox.chofer.api.EntregasPorDia>>, response: Response<List<com.manybox.chofer.api.EntregasPorDia>>) {
                                                    if (response.isSuccessful) {
                                                        entregasPorDia = response.body() ?: emptyList()
                                                    } else {
                                                        entregasPorDia = emptyList()
                                                    }
                                                }
                                                override fun onFailure(call: Call<List<com.manybox.chofer.api.EntregasPorDia>>, t: Throwable) {
                                                    entregasPorDia = emptyList()
                                                }
                                            })
                                        } else {
                                            entregasPorDia = emptyList()
                                        }
                                        // Cargar notificaciones del usuario
                                        cargarNotificaciones(api, body.user.id, token) { lista ->
                                            notificaciones = lista
                                            notificacionesNoLeidas = lista.count { !it.leido }
                                        }
                                        // Iniciar conexiones SignalR con token (enviar raw token sin "Bearer ")
                                        try {
                                            val rawToken = body.token
                                            // Notificaciones dependen del usuario.id (puede ser 0 si backend no retornó correctamente)
                                            iniciarNotificacionesHub(rawToken, body.user.id,
                                                onNew = {
                                                    cargarNotificaciones(api, body.user.id, token) { lista ->
                                                        notificaciones = lista
                                                        notificacionesNoLeidas = lista.count { !it.leido }
                                                    }
                                                },
                                                onConnectionStateChanged = { connected -> notifsConectado = connected }
                                            )

                                            // Bitácora hub y otras llamadas relacionadas con empleado solo si hay empleadoId válido
                                            if (body.user.empleadoId > 0) {
                                                iniciarBitacoraHub(rawToken, body.user.empleadoId,
                                                    onUpdate = {
                                                        cargarBitacora(api, body.user.empleadoId, token) { entregas ->
                                                            bitacoraEntregas = entregas
                                                        }
                                                    }
                                                )
                                            }
                                        } catch (ex: Exception) {
                                            android.util.Log.e("SignalR", "Error iniciando SignalR: ${ex.message}")
                                        }
                                        // Cargar guías y bitácora solo si hay empleado válido
                                        if (body.user.empleadoId > 0) {
                                            cargarGuias(api, body.user.empleadoId, token) { g, p, c, i ->
                                                guias = g
                                                pendientes = p
                                                confirmados = c
                                                incidencias = i
                                            }
                                            cargarBitacora(api, body.user.empleadoId, token) { entregas ->
                                                bitacoraEntregas = entregas
                                            }
                                        } else {
                                            guias = emptyList()
                                            bitacoraEntregas = emptyList()
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
                    empleadoPerfil = empleadoPerfil,
                    pendientes = pendientes,
                    confirmados = confirmados,
                    incidencias = incidencias,
                    guias = guias,
                    bitacoraEntregas = bitacoraEntregas,
                    entregasPorDia = entregasPorDia,
                    notificaciones = notificaciones,
                    notificacionesNoLeidas = notificacionesNoLeidas,
                    notifsConectado = notifsConectado,
                    onMarcarNotificacionLeida = { notifId ->
                        val u = user ?: return@DashboardScreen
                        // Optimista: marcar localmente para feedback inmediato
                        val actualizado = notificaciones.map { if (it.id == notifId) it.copy(leido = true) else it }
                        notificaciones = actualizado
                        notificacionesNoLeidas = actualizado.count { !it.leido }

                        // MAUI usa PUT api/notificaciones/entrega/{notificacionId}/leida?usuarioId=...
                        val call = api.marcarEntregaLeida(notifId, u.id, token)
                        call.enqueue(object: Callback<Void> {
                            override fun onResponse(call: Call<Void>, response: Response<Void>) {
                                if (!response.isSuccessful) {
                                    // Fallback: endpoint anterior por compatibilidad
                                    val fallback = api.marcarNotificacionLeida(u.id, notifId, token)
                                    fallback.enqueue(object: Callback<Void> {
                                        override fun onResponse(call: Call<Void>, response: Response<Void>) {
                                            cargarNotificaciones(api, u.id, token) { lista ->
                                                notificaciones = lista
                                                notificacionesNoLeidas = lista.count { !it.leido }
                                            }
                                        }
                                        override fun onFailure(call: Call<Void>, t: Throwable) {
                                            cargarNotificaciones(api, u.id, token) { lista ->
                                                notificaciones = lista
                                                notificacionesNoLeidas = lista.count { !it.leido }
                                            }
                                        }
                                    })
                                } else {
                                    cargarNotificaciones(api, u.id, token) { lista ->
                                        notificaciones = lista
                                        notificacionesNoLeidas = lista.count { !it.leido }
                                    }
                                }
                            }
                            override fun onFailure(call: Call<Void>, t: Throwable) {
                                // Fallback también en fallo de red: intentar el endpoint antiguo
                                val fallback = api.marcarNotificacionLeida(u.id, notifId, token)
                                fallback.enqueue(object: Callback<Void> {
                                    override fun onResponse(call: Call<Void>, response: Response<Void>) {
                                        cargarNotificaciones(api, u.id, token) { lista ->
                                            notificaciones = lista
                                            notificacionesNoLeidas = lista.count { !it.leido }
                                        }
                                    }
                                    override fun onFailure(call: Call<Void>, t: Throwable) {
                                        cargarNotificaciones(api, u.id, token) { lista ->
                                            notificaciones = lista
                                            notificacionesNoLeidas = lista.count { !it.leido }
                                        }
                                    }
                                })
                            }
                        })
                    },
                    onMarcarTodasLeidas = {
                        val u = user ?: return@DashboardScreen
                        val call = api.marcarTodasLeidas(u.id, token)
                        call.enqueue(object: Callback<Void> {
                            override fun onResponse(call: Call<Void>, response: Response<Void>) {
                                cargarNotificaciones(api, u.id, token) { lista ->
                                    notificaciones = lista
                                    notificacionesNoLeidas = lista.count { !it.leido }
                                }
                            }
                            override fun onFailure(call: Call<Void>, t: Throwable) {
                                cargarNotificaciones(api, u.id, token) { lista ->
                                    notificaciones = lista
                                    notificacionesNoLeidas = lista.count { !it.leido }
                                }
                            }
                        })
                    },
                    onRecoger = { guia -> cambiarEstado(api, guia, "En camino", token) { 
                        cargarGuias(api, user!!.empleadoId, token) { g, p, c, i -> guias = g; pendientes = p; confirmados = c; incidencias = i }
                        cargarBitacora(api, user!!.empleadoId, token) { entregas -> bitacoraEntregas = entregas }
                    } },
                    // Backend espera un valor específico para el estado "Último tramo"; usar la misma cadena que mapea el API
                    onIniciarEntrega = { guia -> cambiarEstado(api, guia, "Último tramo", token) { 
                        cargarGuias(api, user!!.empleadoId, token) { g, p, c, i -> guias = g; pendientes = p; confirmados = c; incidencias = i }
                        cargarBitacora(api, user!!.empleadoId, token) { entregas -> bitacoraEntregas = entregas }
                    } },
                    onEntregar = { guia -> cambiarEstado(api, guia, "Entregado", token) { 
                        cargarGuias(api, user!!.empleadoId, token) { g, p, c, i -> guias = g; pendientes = p; confirmados = c; incidencias = i }
                        cargarBitacora(api, user!!.empleadoId, token) { entregas -> bitacoraEntregas = entregas }
                    } },
                    onScanBarcode = { /* TODO: Navegar a pantalla de escaneo */ }
                )
            }
        }
    }

    override fun onDestroy() {
        super.onDestroy()
        try {
            hubConnection?.stop()
            bitHubConnection?.stop()
        } catch (ex: Exception) {
            android.util.Log.e("SignalR", "Error deteniendo SignalR: ${ex.message}")
        }
    }

    // Iniciar y mantener conexión al hub de notificaciones con reconexión automática
    private fun iniciarNotificacionesHub(
        rawToken: String,
        usuarioId: Int,
        onNew: () -> Unit,
        onConnectionStateChanged: (Boolean) -> Unit
    ) {
        // Agregamos usuarioId como querystring para que el Hub pueda agrupar en OnConnectedAsync
        val hubBase = getApiBaseUrl().trimEnd('/')
        hubConnection = HubConnectionBuilder
            .create("$hubBase/notificacioneshub?usuarioId=$usuarioId")
            .withAccessTokenProvider(io.reactivex.rxjava3.core.Single.just(rawToken))
            .build()

        val refrescar: (Any?) -> Unit = { _ ->
            lifecycleScope.launch(Dispatchers.Main) {
                Toast.makeText(this@MainActivity, "Nueva notificación", Toast.LENGTH_SHORT).show()
                onNew()
            }
        }
        hubConnection?.on("nuevaNotificacion", refrescar, Any::class.java)
        hubConnection?.on("RecibirNotificacion", refrescar, Any::class.java)

        lifecycleScope.launch {
            var delayMs = 1000L
            while (true) {
                try {
                    hubConnection?.start()?.blockingAwait()
                    onConnectionStateChanged(true)
                    hubConnection?.send("JoinUserGroup", usuarioId)
                    // Esperar hasta que se cierre
                    while (hubConnection?.connectionState == com.microsoft.signalr.HubConnectionState.CONNECTED) {
                        delay(5000)
                    }
                } catch (e: Exception) {
                    android.util.Log.e("SignalR", "Notifs hub desconectado: ${e.message}")
                }
                onConnectionStateChanged(false)
                delay(delayMs)
                delayMs = (delayMs * 2).coerceAtMost(30000)
            }
        }
    }

    // Iniciar y mantener conexión al hub de bitácora con reconexión automática
    private fun iniciarBitacoraHub(
        rawToken: String,
        empleadoId: Int,
        onUpdate: () -> Unit
    ) {
        val hubBase = getApiBaseUrl().trimEnd('/')
        bitHubConnection = HubConnectionBuilder
            .create("$hubBase/bitacorahub")
            .withAccessTokenProvider(io.reactivex.rxjava3.core.Single.just(rawToken))
            .build()

        bitHubConnection?.on("BitacoraActualizada", { _: Any? ->
            lifecycleScope.launch(Dispatchers.Main) { onUpdate() }
        }, Any::class.java)

        lifecycleScope.launch {
            var delayMs = 1000L
            while (true) {
                try {
                    bitHubConnection?.start()?.blockingAwait()
                    bitHubConnection?.send("JoinBitacoraGroup", empleadoId)
                    while (bitHubConnection?.connectionState == com.microsoft.signalr.HubConnectionState.CONNECTED) {
                        delay(5000)
                    }
                } catch (e: Exception) {
                    android.util.Log.e("SignalR", "Bitacora hub desconectado: ${e.message}")
                }
                delay(delayMs)
                delayMs = (delayMs * 2).coerceAtMost(30000)
            }
        }
    }

}

private fun buildOkHttp(): OkHttpClient {
    val logging = HttpLoggingInterceptor().apply { level = HttpLoggingInterceptor.Level.BASIC }
    return OkHttpClient.Builder()
        .connectTimeout(10, TimeUnit.SECONDS)
        .readTimeout(15, TimeUnit.SECONDS)
        .writeTimeout(15, TimeUnit.SECONDS)
        .addInterceptor(logging)
        .addInterceptor(FallbackHostInterceptor)
        .build()
}

private object FallbackHostInterceptor : Interceptor {
    override fun intercept(chain: Interceptor.Chain): okhttp3.Response {
        val request = chain.request()
        return try {
            chain.proceed(request)
        } catch (e: ConnectException) {
            // Si la IP 100.64.197.11 no es alcanzable y estamos en emulador, reintenta con 10.0.2.2
            val url = request.url
            if (url.host == "100.64.197.11" && isEmulator()) {
                val newUrl = url.newBuilder().host("10.0.2.2").build()
                val newReq = request.newBuilder().url(newUrl).build()
                chain.proceed(newReq)
            } else {
                throw e
            }
        }
    }
}

private fun isEmulator(): Boolean {
    return (Build.FINGERPRINT.startsWith("generic")
            || Build.FINGERPRINT.lowercase().contains("emulator")
            || Build.MODEL.contains("Emulator")
            || Build.MODEL.contains("Android SDK built for x86")
            || Build.MANUFACTURER.contains("Genymotion")
            || (Build.BRAND.startsWith("generic") && Build.DEVICE.startsWith("generic"))
            || "google_sdk" == Build.PRODUCT)
}

private fun getApiBaseUrl(): String {
    // Si es emulador, usar 10.0.2.2 (host machine). Si no, mantener IP actual.
    return if (isEmulator()) "http://10.0.2.2:5000/" else "http://100.64.197.11:5000/"
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

fun cargarBitacora(api: ApiService, empleadoId: Int, token: String, onResult: (List<BitacoraEntrega>) -> Unit) {
    val TAG = "ApiLog"
    android.util.Log.i(TAG, "Petición de bitácora para empleadoId: $empleadoId")
    val call = api.getBitacoraChofer(empleadoId, token)
    call.enqueue(object : Callback<List<BitacoraEntrega>> {
        override fun onResponse(call: Call<List<BitacoraEntrega>>, response: Response<List<BitacoraEntrega>>) {
            android.util.Log.i(TAG, "Respuesta bitácora: ${response.code()} ${response.message()}")
            if (response.isSuccessful) {
                val entregas = response.body() ?: emptyList()
                android.util.Log.i(TAG, "Entregas recibidas: ${entregas.size}")
                onResult(entregas)
            } else {
                android.util.Log.e(TAG, "Error en respuesta de bitácora: ${response.errorBody()?.string()}")
                onResult(emptyList())
            }
        }
        override fun onFailure(call: Call<List<BitacoraEntrega>>, t: Throwable) {
            android.util.Log.e(TAG, "Fallo en petición de bitácora: ${t.message}")
            onResult(emptyList())
        }
    })
}

fun cargarPerfilEmpleado(api: ApiService, empleadoId: Int, token: String, onResult: (EmpleadoPerfil?) -> Unit) {
    val TAG = "ApiLog"
    android.util.Log.i(TAG, "Petición de perfil para empleadoId: $empleadoId")
    val call = api.getEmpleadoPerfil(empleadoId, token)
    call.enqueue(object : Callback<EmpleadoPerfil> {
        override fun onResponse(call: Call<EmpleadoPerfil>, response: Response<EmpleadoPerfil>) {
            android.util.Log.i(TAG, "Respuesta perfil: ${response.code()} ${response.message()}")
            if (response.isSuccessful) {
                val perfil = response.body()
                android.util.Log.i(TAG, "Perfil recibido: $perfil")
                onResult(perfil)
            } else {
                android.util.Log.e(TAG, "Error en respuesta de perfil: ${response.errorBody()?.string()}")
                onResult(null)
            }
        }
        override fun onFailure(call: Call<EmpleadoPerfil>, t: Throwable) {
            android.util.Log.e(TAG, "Fallo en petición de perfil: ${t.message}")
            onResult(null)
        }
    })
}

fun cargarNotificaciones(api: ApiService, usuarioId: Int, token: String, onResult: (List<com.manybox.chofer.api.NotificacionUsuarioVM>) -> Unit) {
    val call = api.getNotificacionesUsuario(usuarioId, token)
    call.enqueue(object: Callback<List<com.manybox.chofer.api.NotificacionUsuarioVM>> {
        override fun onResponse(
            call: Call<List<com.manybox.chofer.api.NotificacionUsuarioVM>>,
            response: Response<List<com.manybox.chofer.api.NotificacionUsuarioVM>>
        ) {
            onResult(response.body() ?: emptyList())
        }

        override fun onFailure(
            call: Call<List<com.manybox.chofer.api.NotificacionUsuarioVM>>,
            t: Throwable
        ) {
            onResult(emptyList())
        }
    })
}

