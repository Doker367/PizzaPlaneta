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
import androidx.compose.ui.draw.alpha
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.graphics.RectangleShape
import androidx.compose.ui.graphics.graphicsLayer
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.text.input.PasswordVisualTransformation
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.sp
import androidx.compose.foundation.Image
import androidx.compose.ui.res.painterResource
import androidx.compose.ui.layout.ContentScale
import androidx.compose.animation.core.animateFloatAsState
import androidx.compose.foundation.interaction.MutableInteractionSource
import androidx.compose.foundation.interaction.collectIsPressedAsState
import com.manybox.chofer.R
import com.manybox.chofer.api.PizzaApiService
import com.manybox.chofer.api.PizzaLoginRequest
import com.manybox.chofer.api.PizzaLoginResponse
import com.manybox.chofer.api.SucursalDto
import com.manybox.chofer.api.RetrofitProvider
import com.manybox.chofer.api.TokenStore
import com.manybox.chofer.api.AuthTokenHolder
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
import androidx.compose.ui.platform.LocalContext
import androidx.compose.ui.zIndex
import kotlinx.coroutines.CoroutineScope
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.launch

@Composable
fun PizzaHomeScreen() {
    val context = LocalContext.current
    var showLogin by remember { mutableStateOf(false) }
    var showRegister by remember { mutableStateOf(false) }
    var showForgot by remember { mutableStateOf(false) }
    var forgotStep by remember { mutableStateOf(0) }
    var showSideMenu by remember { mutableStateOf(false) }
    var sideType by remember { mutableStateOf("default") }
    var showOrderSelector by remember { mutableStateOf(false) }
    var isLoggingIn by remember { mutableStateOf(false) }
    var loginError by remember { mutableStateOf<String?>(null) }
    var showMenu by remember { mutableStateOf(false) }

    // Palette
    val Navy = Color(0xFF1D3557)
    val Orange = Color(0xFFF77F00)
    val OnNavy = Color(0xFFEAEAEA)

    Box(
        modifier = Modifier
            .fillMaxSize()
            .background(Navy)
    ) {

        Column(modifier = Modifier.fillMaxSize()) {
            // Header: small white circular hamburger button at top-left
            Box(
                modifier = Modifier
                    .fillMaxWidth()
                    .padding(horizontal = 12.dp, vertical = 16.dp)
            ) {
                Box(
                    modifier = Modifier
                        .size(44.dp)
                        .clip(RoundedCornerShape(10.dp))
                        .background(Color.White)
                        .align(Alignment.CenterStart)
                        .clickable { showSideMenu = true; sideType = "default" },
                    contentAlignment = Alignment.Center
                ) {
                    Icon(
                        Icons.Default.Menu,
                        contentDescription = "Menu",
                        tint = Navy
                    )
                }
            }

            // Bloque: Logo + "Bienvenido" más abajo (padding solo afecta a este bloque)
            Column(
                modifier = Modifier
                    .fillMaxWidth()
                    .padding(top = 72.dp), // mueve ambos más abajo (ajusta aquí)
                horizontalAlignment = Alignment.CenterHorizontally
            ) {
                Image(
                    painter = painterResource(id = R.drawable.il1_alt),
                    contentDescription = "Logo",
                    modifier = Modifier
                        .fillMaxWidth(0.9f)
                        .height(120.dp)
                )
                Spacer(Modifier.height(12.dp))
                Text(
                    "Bienvenido",
                    color = OnNavy,
                    fontSize = 20.sp,
                    fontWeight = FontWeight.Bold,
                )
            }

            // Se elimina la imagen decorativa inferior para que se vea como en el mock

            Spacer(Modifier.weight(1f))
        }

        // Imagen decorativa fija arriba a la derecha (sin offset)
        Image(
            painter = painterResource(id = R.drawable.il1),
            contentDescription = null,
            modifier = Modifier
                .align(Alignment.TopEnd)
                .offset(y = (-20).dp)   // empuja hacia arriba para eliminar el gap del PNG
                .padding(end = 0.dp)
                .width(180.dp)
                .height(140.dp)
                .alpha(0.75f),          // sin zIndex (el drawer sigue encima)
            contentScale = ContentScale.Fit
        )



        // Bottom orange action bar overlay (Cuenta / Ordenar)
        Box(
            modifier = Modifier
                .align(Alignment.BottomCenter)
                .fillMaxWidth()
                .height(110.dp)
                .background(Orange, RectangleShape)
        ) {
            Row(
                modifier = Modifier
                    .fillMaxSize()
                    .padding(horizontal = 24.dp, vertical = 8.dp),
                horizontalArrangement = Arrangement.SpaceBetween,
                verticalAlignment = Alignment.Top
            ) {
                run {
                    val interaction = remember { MutableInteractionSource() }
                    val pressed by interaction.collectIsPressedAsState()
                    val scale by animateFloatAsState(targetValue = if (pressed) 1.06f else 1f, label = "cuentaScale")
                    Column(
                        horizontalAlignment = Alignment.CenterHorizontally,
                        modifier = Modifier
                            .weight(1f)
                            .offset(y = (-35).dp)
                            .zIndex(2f)
                            .clickable(interactionSource = interaction, indication = null) { showLogin = true; showRegister = false; showForgot = false; forgotStep = 0 }
                    ) {
                        Box(
                            modifier = Modifier
                                .size(72.dp)
                                .clip(RoundedCornerShape(12.dp))
                                .background(if (pressed) Color.White.copy(alpha = 0.12f) else Color.Transparent)
                                .graphicsLayer(scaleX = scale, scaleY = scale),
                            contentAlignment = Alignment.Center
                        ) {
                            Image(
                                painter = painterResource(id = R.drawable.i2l),
                                contentDescription = "Cuenta",
                                modifier = Modifier.size(56.dp),
                                contentScale = ContentScale.Fit
                            )
                        }
                    Spacer(Modifier.height(2.dp))
                    Text(
                        "Cuenta",
                        color = Color.Black,
                        fontWeight = FontWeight.Medium,
                        fontSize = 16.sp
                    )
                    }
                }

                run {
                    val interaction = remember { MutableInteractionSource() }
                    val pressed by interaction.collectIsPressedAsState()
                    val scale by animateFloatAsState(targetValue = if (pressed) 1.06f else 1f, label = "ordenarScale")
                    Column(
                        horizontalAlignment = Alignment.CenterHorizontally,
                        modifier = Modifier
                            .weight(1f)
                            .offset(y = (-35).dp)
                            .zIndex(2f)
                            .clickable(interactionSource = interaction, indication = null) { showOrderSelector = true }
                    ) {
                        Box(
                            modifier = Modifier
                                .size(72.dp)
                                .clip(RoundedCornerShape(12.dp))
                                .background(if (pressed) Color.White.copy(alpha = 0.12f) else Color.Transparent)
                                .graphicsLayer(scaleX = scale, scaleY = scale),
                            contentAlignment = Alignment.Center
                        ) {
                            Image(
                                painter = painterResource(id = R.drawable.i3l),
                                contentDescription = "Ordenar",
                                modifier = Modifier.size(56.dp),
                                contentScale = ContentScale.Fit
                            )
                        }
                    Spacer(Modifier.height(2.dp))
                    Text(
                        "Ordenar",
                        color = Color.Black,
                        fontWeight = FontWeight.Medium,
                        fontSize = 16.sp
                    )
                    }
                }
            }
        }

        // Login / Register modal
        if (showLogin) {
            if (showRegister) {
                // Pantalla de registro a pantalla completa, con imagen superior
                RegistroScreen(
                    headerImageRes = R.drawable.pizzorra,
                    onBack = { showRegister = false },
                    onSubmit = { _, _, _, _ ->
                        // TODO: Integración con API de registro
                        showRegister = false
                    },
                    onLoginClick = { showRegister = false }
                )
            } else {
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
                                    val api = RetrofitProvider.api(context)
                                    api.login(PizzaLoginRequest(email, pass)).enqueue(object: Callback<PizzaLoginResponse> {
                                        override fun onResponse(call: Call<PizzaLoginResponse>, response: Response<PizzaLoginResponse>) {
                                            isLoggingIn = false
                                            if (response.isSuccessful) {
                                                val rawToken = response.body()?.token ?: ""
                                                // Persist token and keep it in memory
                                                CoroutineScope(Dispatchers.IO).launch {
                                                    TokenStore.saveToken(context, rawToken)
                                                }
                                                // Cargar sucursales como prueba de endpoint protegido (header via interceptor)
                                                api.getSucursales().enqueue(object: Callback<List<SucursalDto>> {
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
                        }
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
                    Row(
                        verticalAlignment = Alignment.CenterVertically,
                        modifier = Modifier.fillMaxWidth()
                    ) {
                        IconButton(onClick = { showOrderSelector = false }) {
                            Icon(
                                Icons.Default.ArrowBack,
                                contentDescription = "Regresar",
                                tint = Color.White
                            )
                        }
                        Text(
                            "Selecciona tu restaurante", 
                            color = Color.White, 
                            fontWeight = FontWeight.Bold
                        )
                    }
                    
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
                            Button(onClick = { 
                                showOrderSelector = false
                                showMenu = true 
                            }) { Text("ELEGIR") }
                        }
                    }
                }
            }
        }

        // Menu de platillos (nuevo componente)
        if (showMenu) {
            MenuPlatillos(
                onBackClick = { showMenu = false },
                onMenuClick = { showSideMenu = true; sideType = "order" }
            )
        }

        // Side menu overlay (siempre por encima)
        AnimatedVisibility(visible = showSideMenu) {
            Surface(
                color = Color(0x66000000),
                modifier = Modifier
                    .fillMaxSize()
                    .clickable { showSideMenu = false }
                    .zIndex(10f) // encima de todo
            ) {}
        }
        AnimatedVisibility(visible = showSideMenu) {
            Card(
                modifier = Modifier
                    .fillMaxHeight()
                    .width(300.dp)
                    .align(Alignment.CenterStart)
                    .zIndex(11f), // encima del scrim
                colors = CardDefaults.cardColors(containerColor = Color.White),
                shape = RoundedCornerShape(topEnd = 16.dp, bottomEnd = 16.dp)
            )  {
                if (sideType == "order") {
                    Column(modifier = Modifier.padding(16.dp), horizontalAlignment = Alignment.CenterHorizontally) {
                        Spacer(Modifier.height(8.dp))
                        Image(
                            painter = painterResource(id = R.drawable.im1),
                            contentDescription = "Decorativo menú",
                            modifier = Modifier.size(88.dp),
                            contentScale = ContentScale.Fit
                        )
                        Spacer(Modifier.height(12.dp))
                        Text("JOSE TADEO", color = Navy, fontWeight = FontWeight.Bold, fontSize = 20.sp)
                        Spacer(Modifier.height(12.dp))
                        SideOption("Método de pago", color = Color.Black)
                        SideOption("Carrito", color = Color.Black)
                        SideOption("Direcciones", color = Color.Black)
                        SideOption("Lugares favoritos", color = Color.Black)
                    }
                } else {
                    Column(modifier = Modifier.padding(16.dp), horizontalAlignment = Alignment.CenterHorizontally) {
                        Spacer(Modifier.height(8.dp))
                        Image(
                            painter = painterResource(id = R.drawable.im1),
                            contentDescription = "Decorativo menú",
                            modifier = Modifier.size(88.dp),
                            contentScale = ContentScale.Fit
                        )
                        Spacer(Modifier.height(12.dp))
                        Text("Hola, Usuario!", color = Navy, fontWeight = FontWeight.Bold, fontSize = 20.sp)
                        Spacer(Modifier.height(12.dp))
                        DrawerOptionWithIcon(text = "Mi cuenta", iconRes = R.drawable.cuenta, color = Color.Black)
                        DrawerOptionWithIcon(text = "Más cercano", iconRes = R.drawable.cercano, color = Color.Black)
                    }
                }
            }
        }
    }
}

@Composable
private fun SideOption(text: String, color: Color = Color(0xFFB0B3B8)) {
    Text(text, color = color, modifier = Modifier
        .fillMaxWidth()
        .padding(vertical = 10.dp))
}

@Composable
private fun DrawerOptionWithIcon(text: String, iconRes: Int, color: Color = Color.Black) {
    Row(
        modifier = Modifier
            .fillMaxWidth()
            .padding(vertical = 10.dp),
        verticalAlignment = Alignment.CenterVertically
    ) {
        Image(
            painter = painterResource(id = iconRes),
            contentDescription = text,
            modifier = Modifier
                .size(24.dp)
                .padding(end = 12.dp),
            contentScale = ContentScale.Fit
        )
        Text(text, color = color, fontSize = 16.sp)
    }
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

// RegisterForm fue movido a un archivo dedicado (`RegistroUI.kt`) como `RegistroScreen`.

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
