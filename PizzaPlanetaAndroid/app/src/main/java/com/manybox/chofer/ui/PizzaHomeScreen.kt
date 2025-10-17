package com.manybox.chofer.ui

import androidx.compose.animation.AnimatedVisibility
import androidx.compose.foundation.background
import androidx.compose.foundation.clickable
import androidx.compose.foundation.layout.*
import androidx.compose.foundation.shape.CircleShape
import androidx.compose.foundation.shape.RoundedCornerShape
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.filled.ArrowBack
import androidx.compose.material.icons.filled.Menu
import androidx.compose.material.icons.filled.Person
import androidx.compose.material3.*
import androidx.compose.runtime.*
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.draw.clip
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.text.input.PasswordVisualTransformation
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.sp
import com.manybox.chofer.api.PizzaApiService
import com.manybox.chofer.api.PizzaLoginRequest
import com.manybox.chofer.api.PizzaLoginResponse
import com.manybox.chofer.api.SucursalDto
import okhttp3.OkHttpClient
import okhttp3.logging.HttpLoggingInterceptor
import retrofit2.Call
import retrofit2.Callback
import retrofit2.Response
import retrofit2.Retrofit
import retrofit2.converter.gson.GsonConverterFactory
import java.security.SecureRandom
import java.security.cert.X509Certificate
import javax.net.ssl.HostnameVerifier
import javax.net.ssl.SSLContext
import javax.net.ssl.TrustManager
import javax.net.ssl.X509TrustManager
import android.os.Build

@Composable
fun PizzaHomeScreen() {
    var showLogin by remember { mutableStateOf(false) }
    var showRegister by remember { mutableStateOf(false) }
    var showForgot by remember { mutableStateOf(false) }
    var forgotStep by remember { mutableStateOf(0) }
    var showSideMenu by remember { mutableStateOf(false) }
    var sideType by remember { mutableStateOf("default") }
    var showOrderSelector by remember { mutableStateOf(false) }
    var isLoggingIn by remember { mutableStateOf(false) }
    var loginError by remember { mutableStateOf<String?>(null) }

    Box(modifier = Modifier
        .fillMaxSize()
        .background(Color(0xFF0F1419))) {

        Column(modifier = Modifier.fillMaxSize()) {
            // Header
            Row(
                modifier = Modifier
                    .fillMaxWidth()
                    .padding(12.dp),
                verticalAlignment = Alignment.CenterVertically,
                horizontalArrangement = Arrangement.SpaceBetween
            ) {
                IconButton(onClick = { showSideMenu = true; sideType = "default" }) {
                    Icon(Icons.Default.Menu, contentDescription = "Menu", tint = Color.White)
                }
                // Decoración
                Box(
                    modifier = Modifier
                        .size(48.dp)
                        .clip(CircleShape)
                        .background(Color(0xFF6C63FF)),
                    contentAlignment = Alignment.Center
                ) {
                    Icon(Icons.Default.Person, contentDescription = null, tint = Color.White)
                }
            }

            // Logo
            Box(modifier = Modifier.fillMaxWidth(), contentAlignment = Alignment.Center) {
                Text("Pizza Planet", color = Color.White, fontSize = 26.sp, fontWeight = FontWeight.Bold)
            }

            Spacer(Modifier.height(8.dp))
            Text("Bienvenido", color = Color.White, fontSize = 20.sp, modifier = Modifier.align(Alignment.CenterHorizontally))

            Spacer(Modifier.weight(1f))

            // Bottom bar
            Row(
                modifier = Modifier
                    .fillMaxWidth()
                    .padding(16.dp),
                horizontalArrangement = Arrangement.spacedBy(12.dp)
            ) {
                Button(
                    onClick = { showLogin = true; showRegister = false; showForgot = false; forgotStep = 0 },
                    modifier = Modifier.weight(1f)
                ) { Text("Cuenta") }
                Button(onClick = { showOrderSelector = true }, modifier = Modifier.weight(1f)) { Text("Ordenar") }
            }
        }

        // Login / Register modal
        if (showLogin) {
            Surface(
                color = Color(0xCC000000),
                modifier = Modifier
                    .fillMaxSize()
                    .clickable { /* backdrop, ignore */ },
            ) {}
            Card(
                modifier = Modifier
                    .fillMaxWidth()
                    .padding(16.dp)
                    .align(Alignment.Center),
                colors = CardDefaults.cardColors(containerColor = Color(0xFF1A1F29)),
                shape = RoundedCornerShape(16.dp)
            ) {
                Column(modifier = Modifier.padding(16.dp)) {
                    Row(verticalAlignment = Alignment.CenterVertically) {
                        IconButton(onClick = { showLogin = false; showRegister = false; showForgot = false; forgotStep = 0 }) {
                            Icon(Icons.Default.ArrowBack, contentDescription = null, tint = Color.White)
                        }
                        Spacer(Modifier.width(8.dp))
                        Text(if (!showRegister && !showForgot) "LOGIN" else if (showRegister) "CREAR CUENTA" else "RESTABLECER", color = Color.White, fontSize = 18.sp, fontWeight = FontWeight.Bold)
                    }
                    Spacer(Modifier.height(12.dp))
                    if (showForgot) {
                        when (forgotStep) {
                            0 -> ForgotStepEmail(onNext = { forgotStep = 1 })
                            1 -> ForgotStepCode(onNext = { forgotStep = 2 })
                            2 -> ForgotStepNewPassword(onConfirm = { showLogin = false; showForgot = false; forgotStep = 0 })
                        }
                    } else if (!showRegister) {
                        LoginForm(
                            onForgot = { showForgot = true; forgotStep = 0 },
                            onRegister = { showRegister = true },
                            onLogin = { email, pass ->
                                isLoggingIn = true
                                loginError = null
                                val retrofit = Retrofit.Builder()
                                    .baseUrl(getPizzaBaseUrl())
                                    .addConverterFactory(GsonConverterFactory.create())
                                    .client(buildDevHttpsClient())
                                    .build()
                                val api = retrofit.create(PizzaApiService::class.java)
                                api.login(PizzaLoginRequest(email, pass)).enqueue(object: Callback<PizzaLoginResponse> {
                                    override fun onResponse(call: Call<PizzaLoginResponse>, response: Response<PizzaLoginResponse>) {
                                        isLoggingIn = false
                                        if (response.isSuccessful) {
                                            val token = "Bearer ${response.body()?.token ?: ""}"
                                            // Cargar sucursales como prueba de endpoint protegido
                                            api.getSucursales(token).enqueue(object: Callback<List<SucursalDto>> {
                                                override fun onResponse(call: Call<List<SucursalDto>>, response: Response<List<SucursalDto>>) {
                                                    if (response.isSuccessful) {
                                                        showLogin = false
                                                        showSideMenu = true
                                                        sideType = "order"
                                                    } else {
                                                        loginError = "No se pudieron cargar sucursales (${response.code()})"
                                                    }
                                                }
                                                override fun onFailure(call: Call<List<SucursalDto>>, t: Throwable) {
                                                    loginError = "Error al cargar sucursales: ${t.message}"
                                                }
                                            })
                                        } else {
                                            loginError = "Usuario o contraseña incorrectos"
                                        }
                                    }
                                    override fun onFailure(call: Call<PizzaLoginResponse>, t: Throwable) {
                                        isLoggingIn = false
                                        loginError = "Error de red: ${t.message}"
                                    }
                                })
                            }
                        )
                        if (isLoggingIn) {
                            Spacer(Modifier.height(12.dp))
                            LinearProgressIndicator(modifier = Modifier.fillMaxWidth())
                        }
                        if (!loginError.isNullOrBlank()) {
                            Spacer(Modifier.height(8.dp))
                            Text(loginError!!, color = Color(0xFFFF6B6B))
                        }
                    } else {
                        RegisterForm(onLoginClick = { showRegister = false })
                    }
                }
            }
        }

        // Order selector
        if (showOrderSelector) {
            Surface(color = Color(0xCC000000), modifier = Modifier.fillMaxSize()) {}
            Card(
                modifier = Modifier
                    .fillMaxWidth()
                    .padding(16.dp)
                    .align(Alignment.Center),
                colors = CardDefaults.cardColors(containerColor = Color(0xFF1A1F29)),
                shape = RoundedCornerShape(16.dp)
            ) {
                Column(modifier = Modifier.padding(16.dp)) {
                    Text("Selecciona tu restaurante", color = Color.White, fontWeight = FontWeight.Bold)
                    Spacer(Modifier.height(12.dp))
                    Text("(Mapa)", color = Color(0xFF8E8E93))
                    Spacer(Modifier.height(12.dp))
                    OutlinedTextField(value = "", onValueChange = {}, label = { Text("Ciudad/CP", color = Color.White) })
                    Spacer(Modifier.height(12.dp))
                    // Lista mock de sucursales
                    repeat(3) { idx ->
                        Row(
                            modifier = Modifier
                                .fillMaxWidth()
                                .padding(vertical = 6.dp),
                            verticalAlignment = Alignment.CenterVertically
                        ) {
                            Box(modifier = Modifier.size(48.dp).clip(RoundedCornerShape(8.dp)).background(Color(0xFF2D3748)))
                            Spacer(Modifier.width(12.dp))
                            Column(modifier = Modifier.weight(1f)) {
                                Text("Pizza Planet ${listOf("Centro","Norte","Sur")[idx]}", color = Color.White)
                                Text("Av. Ejemplo 123", color = Color(0xFF8E8E93), fontSize = 12.sp)
                            }
                            Button(onClick = { showOrderSelector = false; showSideMenu = true; sideType = "order" }) { Text("ELEGIR") }
                        }
                    }
                }
            }
        }

        // Side menu overlay
        AnimatedVisibility(visible = showSideMenu) {
            Surface(color = Color(0x66000000), modifier = Modifier
                .fillMaxSize()
                .clickable { showSideMenu = false }) {}
        }
        AnimatedVisibility(visible = showSideMenu) {
            Card(
                modifier = Modifier
                    .fillMaxHeight()
                    .width(300.dp)
                    .align(Alignment.CenterStart),
                colors = CardDefaults.cardColors(containerColor = Color(0xFF1A1F29)),
                shape = RoundedCornerShape(topEnd = 16.dp, bottomEnd = 16.dp)
            ) {
                if (sideType == "order") {
                    Column(modifier = Modifier.padding(16.dp)) {
                        Text("JOSE TADEO", color = Color.White, fontWeight = FontWeight.Bold)
                        Spacer(Modifier.height(12.dp))
                        SideOption("Método de pago")
                        SideOption("Carrito")
                        SideOption("Direcciones")
                        SideOption("Lugares favoritos")
                    }
                } else {
                    Column(modifier = Modifier.padding(16.dp)) {
                        Text("Hola, Usuario!", color = Color.White, fontWeight = FontWeight.Bold)
                        Spacer(Modifier.height(12.dp))
                        SideOption("Mi cuenta")
                        SideOption("Más cercano")
                    }
                }
            }
        }
    }
}

@Composable
private fun SideOption(text: String) {
    Text(text, color = Color(0xFFB0B3B8), modifier = Modifier
        .fillMaxWidth()
        .padding(vertical = 10.dp))
}

@Composable
private fun LoginForm(onForgot: () -> Unit, onRegister: () -> Unit, onLogin: (String, String) -> Unit) {
    var email by remember { mutableStateOf("") }
    var pass by remember { mutableStateOf("") }
    Text("LOGIN", color = Color.White, fontWeight = FontWeight.SemiBold)
    Spacer(Modifier.height(8.dp))
    OutlinedTextField(value = email, onValueChange = { email = it }, label = { Text("USUARIO", color = Color.White) }, singleLine = true)
    Spacer(Modifier.height(8.dp))
    OutlinedTextField(value = pass, onValueChange = { pass = it }, label = { Text("CONTRASEÑA", color = Color.White) }, singleLine = true, visualTransformation = PasswordVisualTransformation())
    TextButton(onClick = onForgot) { Text("¿Olvidó su contraseña?", color = Color(0xFF6C63FF)) }
    Button(onClick = { onLogin(email, pass) }, modifier = Modifier.fillMaxWidth()) { Text("INICIAR SESIÓN") }
    Spacer(Modifier.height(8.dp))
    Button(onClick = onRegister, modifier = Modifier.fillMaxWidth(), colors = ButtonDefaults.buttonColors(containerColor = Color(0xFF00C853))) { Text("CREAR CUENTA") }
    Spacer(Modifier.height(8.dp))
    OutlinedButton(onClick = { /* Google Sign-In */ }, modifier = Modifier.fillMaxWidth()) { Text("Continuar con Google") }
}

@Composable
private fun RegisterForm(onLoginClick: () -> Unit) {
    var nombre by remember { mutableStateOf("") }
    var correo by remember { mutableStateOf("") }
    var pass1 by remember { mutableStateOf("") }
    var pass2 by remember { mutableStateOf("") }
    Text("CREAR CUENTA", color = Color.White, fontWeight = FontWeight.SemiBold)
    Spacer(Modifier.height(8.dp))
    OutlinedTextField(value = nombre, onValueChange = { nombre = it }, label = { Text("Nombre", color = Color.White) }, singleLine = true)
    OutlinedTextField(value = correo, onValueChange = { correo = it }, label = { Text("CORREO", color = Color.White) }, singleLine = true)
    OutlinedTextField(value = pass1, onValueChange = { pass1 = it }, label = { Text("CONTRASEÑA", color = Color.White) }, singleLine = true, visualTransformation = PasswordVisualTransformation())
    OutlinedTextField(value = pass2, onValueChange = { pass2 = it }, label = { Text("CONFIRMAR CONTRASEÑA", color = Color.White) }, singleLine = true, visualTransformation = PasswordVisualTransformation())
    Spacer(Modifier.height(8.dp))
    Button(onClick = { /* Registrar */ }, modifier = Modifier.fillMaxWidth()) { Text("CREAR CUENTA") }
    Spacer(Modifier.height(8.dp))
    TextButton(onClick = onLoginClick) { Text("¿Ya tienes cuenta? Iniciar sesión", color = Color(0xFF6C63FF)) }
}

@Composable
private fun ForgotStepEmail(onNext: () -> Unit) {
    Text("RESTABLECER\nCONTRASEÑA", color = Color.White, fontWeight = FontWeight.Bold)
    Spacer(Modifier.height(8.dp))
    Text("Ingresa tu correo electrónico y te enviaremos un enlace para confirmar y crear una nueva contraseña.", color = Color(0xFFB0B3B8), fontSize = 13.sp)
    Spacer(Modifier.height(8.dp))
    OutlinedTextField(value = "", onValueChange = {}, label = { Text("CORREO", color = Color.White) })
    Spacer(Modifier.height(8.dp))
    Button(onClick = onNext, modifier = Modifier.fillMaxWidth()) { Text("ENVIAR") }
}

@Composable
private fun ForgotStepCode(onNext: () -> Unit) {
    Text("Verificación de\ncódigo", color = Color.White, fontWeight = FontWeight.Bold)
    Spacer(Modifier.height(8.dp))
    Text("Hemos enviado un código de 4 dígitos a tu correo electrónico.\nPor favor, introdúcelo y continúa para continuar con el restablecimiento de tu contraseña.", color = Color(0xFFB0B3B8), fontSize = 13.sp)
    Spacer(Modifier.height(8.dp))
    Row(horizontalArrangement = Arrangement.spacedBy(8.dp)) {
        repeat(4) { OutlinedTextField(value = "", onValueChange = {}, modifier = Modifier.width(64.dp)) }
    }
    Spacer(Modifier.height(8.dp))
    Button(onClick = onNext, modifier = Modifier.fillMaxWidth()) { Text("VERIFICAR") }
}

@Composable
private fun ForgotStepNewPassword(onConfirm: () -> Unit) {
    Text("Nueva\ncontraseña", color = Color.White, fontWeight = FontWeight.Bold)
    Spacer(Modifier.height(8.dp))
    Text("Ingresa tu nueva contraseña y confírmala para completar el proceso de restablecimiento.", color = Color(0xFFB0B3B8), fontSize = 13.sp)
    Spacer(Modifier.height(8.dp))
    OutlinedTextField(value = "", onValueChange = {}, label = { Text("NUEVA CONTRASEÑA", color = Color.White) }, visualTransformation = PasswordVisualTransformation())
    OutlinedTextField(value = "", onValueChange = {}, label = { Text("REPETIR CONTRASEÑA", color = Color.White) }, visualTransformation = PasswordVisualTransformation())
    Spacer(Modifier.height(8.dp))
    Button(onClick = onConfirm, modifier = Modifier.fillMaxWidth()) { Text("CONFIRMAR") }
}

// Helpers de red para entorno de desarrollo
private fun buildDevHttpsClient(): OkHttpClient {
    return try {
        val trustAll = arrayOf<TrustManager>(object : X509TrustManager {
            override fun checkClientTrusted(chain: Array<out X509Certificate>?, authType: String?) {}
            override fun checkServerTrusted(chain: Array<out X509Certificate>?, authType: String?) {}
            override fun getAcceptedIssuers(): Array<X509Certificate> = arrayOf()
        })
        val ssl = SSLContext.getInstance("SSL").apply { init(null, trustAll, SecureRandom()) }
        val logging = HttpLoggingInterceptor().apply { level = HttpLoggingInterceptor.Level.BASIC }
        OkHttpClient.Builder()
            .sslSocketFactory(ssl.socketFactory, trustAll[0] as X509TrustManager)
            .hostnameVerifier(HostnameVerifier { _, _ -> true })
            .addInterceptor(logging)
            .build()
    } catch (e: Exception) {
        OkHttpClient.Builder().build()
    }
}

private fun isEmulator(): Boolean {
    val fingerprint = Build.FINGERPRINT
    val model = Build.MODEL
    val manufacturer = Build.MANUFACTURER
    val brand = Build.BRAND
    val device = Build.DEVICE
    val product = Build.PRODUCT

    return (
        fingerprint.startsWith("generic") ||
        fingerprint.lowercase().contains("emulator") ||
        model.contains("Emulator", ignoreCase = true) ||
        model.contains("Android SDK built for x86", ignoreCase = true) ||
        manufacturer.contains("Genymotion", ignoreCase = true) ||
        (brand.startsWith("generic") && device.startsWith("generic")) ||
        product == "google_sdk"
    )
}

private fun getPizzaBaseUrl(): String {
    // Usa HTTPS de desarrollo por defecto; si tu backend expone solo HTTP, cambia a 5000
    return if (isEmulator()) "https://10.0.2.2:5001/" else "https://localhost:5001/"
}
