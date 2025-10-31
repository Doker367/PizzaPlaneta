package com.manybox.chofer.ui
import coil.compose.AsyncImage

import androidx.compose.animation.AnimatedVisibility
import androidx.compose.foundation.background
import androidx.compose.foundation.clickable
import androidx.compose.foundation.layout.*
import androidx.compose.foundation.shape.CircleShape
import androidx.compose.foundation.shape.RoundedCornerShape
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.filled.ArrowBack
import androidx.compose.material.icons.filled.Menu
import androidx.compose.material.icons.filled.CheckCircle
import androidx.compose.animation.core.rememberInfiniteTransition
import androidx.compose.animation.core.infiniteRepeatable
import androidx.compose.animation.core.tween
import androidx.compose.animation.core.LinearEasing
import androidx.compose.animation.core.FastOutSlowInEasing
import androidx.compose.animation.core.RepeatMode
import androidx.compose.animation.core.animateFloat
import androidx.compose.material.icons.filled.Person
import androidx.compose.material.icons.filled.Search
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
    var showAccount by remember { mutableStateOf(false) }
    var showOrderSelector by remember { mutableStateOf(false) }
    var isLoggingIn by remember { mutableStateOf(false) }
    var loginError by remember { mutableStateOf<String?>(null) }
    var showMenu by remember { mutableStateOf(false) }
    var loginSuccess by remember { mutableStateOf(false) }
    var sucursales by remember { mutableStateOf<List<SucursalDto>>(emptyList()) }

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
                    .padding(top = 48.dp), // Reducido de 72.dp a 48.dp para subir todo el contenido
                horizontalAlignment = Alignment.CenterHorizontally
            ) {
                Image(
                    painter = painterResource(id = R.drawable.il1_alt),
                    contentDescription = "Logo",
                    modifier = Modifier
                        .fillMaxWidth(0.9f)
                        .height(120.dp)
                )
                Spacer(Modifier.height(8.dp))  // Reducido de 12.dp a 8.dp
                Text(
                    "Bienvenido",
                    color = OnNavy,
                    fontSize = 20.sp,
                    fontWeight = FontWeight.Bold,
                )
                Spacer(Modifier.height(2.dp))  // Reducido de 4.dp a 2.dp
                HotPizzaAnimation(
                    modifier = Modifier.padding(bottom = 16.dp),
                    size = 600f,
                    lineColor = Color.White.copy(alpha = 0.85f),
                    strokeWidth = 8f
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

        // Login / Register / Forgot
        if (showLogin) {
            if (showRegister) {
                var registroError by remember { mutableStateOf<String?>(null) }
                var registroExito by remember { mutableStateOf(false) }
                var isRegistering by remember { mutableStateOf(false) }
                Box {
                    RegistroScreen(
                        headerImageRes = R.drawable.pizzorra,
                        onBack = { showRegister = false },
                        isLoading = isRegistering,
                        errorMessage = registroError,
                        onSubmit = { nombre, correo, telefono, pass1, pass2 ->
                            registroError = null
                            registroExito = false
                            if (nombre.isBlank() || correo.isBlank() || telefono.isBlank() || pass1.isBlank() || pass2.isBlank()) {
                                registroError = "Completa todos los campos."
                                return@RegistroScreen
                            }
                            if (pass1 != pass2) {
                                registroError = "Las contraseñas no coinciden."
                                return@RegistroScreen
                            }
                            isRegistering = true
                            val api = RetrofitProvider.pizzaApi(context)
                            val req = com.manybox.chofer.api.PizzaRegisterRequest(
                                nombre = nombre,
                                email = correo,
                                password = pass1,
                                telefono = telefono
                            )
                            api.register(req).enqueue(object: retrofit2.Callback<Void> {
                                override fun onResponse(call: retrofit2.Call<Void>, response: retrofit2.Response<Void>) {
                                    isRegistering = false
                                    if (response.isSuccessful) {
                                        registroExito = true
                                        // Espera 1.5s y regresa a login
                                        kotlinx.coroutines.CoroutineScope(kotlinx.coroutines.Dispatchers.Main).launch {
                                            kotlinx.coroutines.delay(1500)
                                            showRegister = false
                                            showLogin = true
                                        }
                                    } else {
                                        val errorBody = response.errorBody()?.string()
                                        registroError = errorBody ?: "Error en el registro: ${response.code()}"
                                    }
                                }
                                override fun onFailure(call: retrofit2.Call<Void>, t: Throwable) {
                                    isRegistering = false
                                    registroError = "Error de red: ${t.message}"
                                }
                            })
                        },
                        onLoginClick = { showRegister = false }
                    )
                    // Mensaje de éxito animado
                    androidx.compose.animation.AnimatedVisibility(
                        visible = registroExito,
                        modifier = Modifier.fillMaxSize(),
                        enter = androidx.compose.animation.fadeIn() + androidx.compose.animation.expandVertically(),
                        exit = androidx.compose.animation.fadeOut() + androidx.compose.animation.shrinkVertically()
                    ) {
                        Box(
                            modifier = Modifier
                                .fillMaxSize()
                                .background(Color(0xAA1DE9B6)),
                            contentAlignment = Alignment.Center
                        ) {
                            Card(
                                shape = RoundedCornerShape(24.dp),
                                colors = CardDefaults.cardColors(containerColor = Color.White),
                                elevation = CardDefaults.cardElevation(12.dp),
                                modifier = Modifier
                                    .padding(32.dp)
                                    .fillMaxWidth(0.85f)
                            ) {
                                Column(
                                    modifier = Modifier.padding(32.dp),
                                    horizontalAlignment = Alignment.CenterHorizontally
                                ) {
                                    Icon(
                                        imageVector = Icons.Filled.CheckCircle,
                                        contentDescription = "Éxito",
                                        tint = Color(0xFF43A047),
                                        modifier = Modifier.size(64.dp)
                                    )
                                    Spacer(Modifier.height(16.dp))
                                    Text(
                                        "¡Cuenta creada exitosamente!",
                                        color = Color(0xFF43A047),
                                        fontWeight = FontWeight.Bold,
                                        fontSize = 22.sp
                                    )
                                    Spacer(Modifier.height(8.dp))
                                    Text(
                                        "Ahora puedes iniciar sesión",
                                        color = Color.Gray,
                                        fontSize = 16.sp
                                    )
                                }
                            }
                        }
                    }
                }
            } else if (showForgot) {
                if (forgotStep == 0) {
                    RestablecerContrasenaScreen(
                        headerImageRes = R.drawable.pizzorra,
                        onBack = { showForgot = false; forgotStep = 0 },
                        onSubmitEmail = { _ -> forgotStep = 1 }
                    )
                } else if (forgotStep == 1) {
                    VerificarCodigoScreen(
                        headerImageRes = R.drawable.pizzorra,
                        onBack = { forgotStep = 0 },
                        onVerify = { _ -> forgotStep = 2 }
                    )
                } else {
                    NuevaContrasenaScreen(
                        headerImageRes = R.drawable.pizzorra,
                        onBack = { forgotStep = 1 },
                        onConfirm = { _, _ -> showForgot = false; forgotStep = 0 }
                    )
                }
            } else {
                LoginScreen(
                    onLogin = { email, pass ->
                        isLoggingIn = true
                        loginError = null
                        val api = RetrofitProvider.pizzaApi(context)
                        api.login(PizzaLoginRequest(email, pass)).enqueue(object: Callback<PizzaLoginResponse> {
                            override fun onResponse(call: Call<PizzaLoginResponse>, response: Response<PizzaLoginResponse>) {
                                isLoggingIn = false
                                if (response.isSuccessful) {
                                    val rawToken = response.body()?.token?.removePrefix("Bearer ")?.trim() ?: ""
                                    CoroutineScope(Dispatchers.IO).launch {
                                        TokenStore.saveToken(context, rawToken)
                                    }
                                    // Nueva instancia para asegurar que el interceptor use el token actualizado
                                    val apiWithToken = RetrofitProvider.pizzaApi(context)
                                    apiWithToken.getSucursales().enqueue(object: Callback<List<SucursalDto>> {
                                        override fun onResponse(call: Call<List<SucursalDto>>, response: Response<List<SucursalDto>>) {
                                            if (response.isSuccessful) {
                                                loginSuccess = true
                                                // Actualiza la lista de sucursales con la respuesta
                                                sucursales = response.body() ?: emptyList()
                                                kotlinx.coroutines.CoroutineScope(kotlinx.coroutines.Dispatchers.Main).launch {
                                                    kotlinx.coroutines.delay(1200)
                                                    showLogin = false
                                                    showOrderSelector = true
                                                    loginSuccess = false
                                                }
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
                    },
                    isLoading = isLoggingIn,
                    errorMessage = loginError,
                    onBack = { showLogin = false; showRegister = false; showForgot = false; forgotStep = 0 },
                    onRegisterClick = { showRegister = true },
                    headerImageRes = R.drawable.pizzorra
                )
            }
        }

        // Mensaje de login exitoso futurista
        androidx.compose.animation.AnimatedVisibility(
            visible = loginSuccess,
            modifier = Modifier.fillMaxSize(),
            enter = androidx.compose.animation.fadeIn() + androidx.compose.animation.expandIn(expandFrom = Alignment.Center),
            exit = androidx.compose.animation.fadeOut() + androidx.compose.animation.shrinkOut(shrinkTowards = Alignment.Center)
        ) {
            // Mostrar GIF del horno de leña (sin animación extra)
            Box(
                modifier = Modifier
                    .fillMaxSize()
                    .background(
                        brush = androidx.compose.ui.graphics.Brush.radialGradient(
                            colors = listOf(Color(0xFFFFF3E0), Color(0xFFFFA726), Color(0xFF6D4C41)),
                            center = androidx.compose.ui.geometry.Offset(0.5f, 0.5f),
                            radius = 800f
                        )
                    ),
                contentAlignment = Alignment.Center
            ) {
                Card(
                    shape = RoundedCornerShape(32.dp),
                    colors = CardDefaults.cardColors(containerColor = Color.White.copy(alpha = 0.97f)),
                    elevation = CardDefaults.cardElevation(18.dp),
                    modifier = Modifier
                        .padding(32.dp)
                        .fillMaxWidth(0.85f)
                        .graphicsLayer {
                            shadowElevation = 24f
                            shape = RoundedCornerShape(32.dp)
                            clip = true
                        }
                ) {
                    Column(
                        modifier = Modifier.padding(36.dp),
                        horizontalAlignment = Alignment.CenterHorizontally
                    ) {
                        AsyncImage(
                            model = R.drawable.horno_de_lena,
                            contentDescription = "Horno de leña animado",
                            modifier = Modifier.size(110.dp),
                            contentScale = ContentScale.Fit
                        )
                        Spacer(Modifier.height(18.dp))
                        Text(
                            "¡Bienvenido a Pizza Planeta!",
                            color = Color(0xFFD84315),
                            fontWeight = FontWeight.ExtraBold,
                            fontSize = 26.sp,
                            letterSpacing = 2.sp
                        )
                        Spacer(Modifier.height(8.dp))
                        Text(
                            "Has iniciado sesión con éxito",
                            color = Color(0xFF6D4C41),
                            fontSize = 16.sp
                        )
                        Spacer(Modifier.height(8.dp))
                        androidx.compose.animation.AnimatedVisibility(
                            visible = loginSuccess,
                            enter = androidx.compose.animation.fadeIn() + androidx.compose.animation.slideInVertically(),
                            exit = androidx.compose.animation.fadeOut() + androidx.compose.animation.slideOutVertically()
                        ) {
                            androidx.compose.material3.LinearProgressIndicator(
                                color = Color(0xFFFFA726),
                                trackColor = Color(0xFFFFF3E0),
                                modifier = Modifier
                                    .fillMaxWidth()
                                    .padding(top = 16.dp)
                            )
                        }
                    }
                }
            }
        }

        // Order selector
        if (showOrderSelector) {
            Scaffold(
                containerColor = Color.White,
                bottomBar = {
                    BottomAppBar(
                        containerColor = RedBrand,
                        contentColor = Color.White,
                        modifier = Modifier.height(70.dp)
                    ) {
                        Row(
                            modifier = Modifier
                                .fillMaxWidth()
                                .padding(horizontal = 16.dp),
                            horizontalArrangement = Arrangement.SpaceBetween,
                            verticalAlignment = Alignment.CenterVertically
                        ) {
                            // Icono de menú
                            IconButton(
                                onClick = { 
                                    showSideMenu = true
                                    sideType = "order"
                                }
                            ) {
                                Icon(
                                    Icons.Default.Menu,
                                    contentDescription = "Menú",
                                    tint = Color.White
                                )
                            }

                            // Texto centrado
                            Text(
                                "BIENVENIDO, TADEO",
                                color = Color.White,
                                fontWeight = FontWeight.Bold,
                                fontSize = 14.sp
                            )
                        }
                    }
                }
            ) { paddingValues ->
                Column(
                    modifier = Modifier
                        .fillMaxSize()
                        .padding(paddingValues)
                        .padding(16.dp)
                ) {
                    // Header con flecha de regreso y título
                    Row(
                        verticalAlignment = Alignment.CenterVertically,
                        modifier = Modifier.fillMaxWidth()
                    ) {
                        IconButton(onClick = { showOrderSelector = false }) {
                            Icon(
                                Icons.Default.ArrowBack,
                                contentDescription = "Regresar",
                                tint = Color(0xFF1D3557)
                            )
                        }
                        Text(
                            "Selecciona tu restaurante",
                            fontWeight = FontWeight.Bold,
                            fontSize = 18.sp,
                            color = Color(0xFF1D3557)
                        )
                    }

                    Spacer(Modifier.height(16.dp))

                    // Mapa con imagen real
                    Box(
                        modifier = Modifier
                            .fillMaxWidth()
                            .height(150.dp)
                            .clip(RoundedCornerShape(8.dp))
                    ) {
                        Image(
                            painter = painterResource(id = R.drawable.map1),
                            contentDescription = "Mapa",
                            modifier = Modifier.fillMaxSize(),
                            contentScale = ContentScale.Crop
                        )
                    }

                    Spacer(Modifier.height(16.dp))

                    // Buscador de CP o Ciudad
                    OutlinedTextField(
                        value = "",
                        onValueChange = {},
                        modifier = Modifier.fillMaxWidth(),
                        placeholder = { Text("Ciudad y estado o CP") },
                        leadingIcon = {
                            Icon(
                                Icons.Default.Search,
                                contentDescription = "Buscar",
                                tint = Color.Gray
                            )
                        },
                        colors = OutlinedTextFieldDefaults.colors(
                            focusedBorderColor = Color(0xFF1D3557),
                            unfocusedBorderColor = Color.LightGray
                        ),
                        singleLine = true
                    )

                    Spacer(Modifier.height(16.dp))

                    // Lista de sucursales dinámica
                    sucursales.forEach { sucursal ->
                        Card(
                            modifier = Modifier
                                .fillMaxWidth()
                                .padding(vertical = 4.dp),
                            colors = CardDefaults.cardColors(containerColor = Color.White),
                            elevation = CardDefaults.cardElevation(2.dp)
                        ) {
                            Row(
                                modifier = Modifier
                                    .fillMaxWidth()
                                    .padding(12.dp),
                                verticalAlignment = Alignment.CenterVertically
                            ) {
                                // Imagen del restaurante
                                Image(
                                    painter = painterResource(id = R.drawable.i4m),
                                    contentDescription = null,
                                    modifier = Modifier
                                        .size(60.dp)
                                        .clip(RoundedCornerShape(8.dp))
                                )
                                Spacer(Modifier.width(12.dp))
                                Column(modifier = Modifier.weight(1f)) {
                                    Text(
                                        sucursal.nombre,
                                        fontWeight = FontWeight.Bold
                                    )
                                    Text(
                                        sucursal.direccion,
                                        color = Color.Gray,
                                        fontSize = 14.sp
                                    )
                                }
                                Button(
                                    onClick = { 
                                        showOrderSelector = false
                                        showMenu = true 
                                    },
                                    colors = ButtonDefaults.buttonColors(
                                        containerColor = Color(0xFF1D3557)
                                    )
                                ) {
                                    Text("ELEGIR")
                                }
                            }
                        }
                        Spacer(Modifier.height(8.dp))
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
                        DrawerOptionWithIcon(text = "Mi cuenta", iconRes = R.drawable.cuenta, color = Color.Black, onClick = { showAccount = true; showSideMenu = false })
                        DrawerOptionWithIcon(text = "Más cercano", iconRes = R.drawable.cercano, color = Color.Black)
                    }
                }
            }
        }
        // Mostrar pantalla de cuenta cuando se solicita
        if (showAccount) {
            CuentaUsuarioScreen(
                fullname = "Usuario Ejemplo",
                phone = "+51 900 000 000",
                email = "usuario@example.com",
                favorites = listOf("Margarita", "Pepperoni"),
                favoriteBranch = "Sucursal Central",
                paymentMethods = listOf("Tarjeta •••• 4242", "Efectivo"),
                orders = listOf(),
                headerImageRes = R.drawable.pizzorra,
                onBack = { showAccount = false },
                onEditProfile = { /* abrir edición */ },
                onLogout = { showAccount = false },
                onReorder = { orderId -> /* manejar reordenar */ }
            )
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
private fun DrawerOptionWithIcon(text: String, iconRes: Int, color: Color = Color.Black, onClick: (() -> Unit)? = null) {
    Row(
        modifier = Modifier
            .fillMaxWidth()
            .padding(vertical = 10.dp)
            .then(if (onClick != null) Modifier.clickable { onClick() } else Modifier),
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
    TextButton(onClick = onForgot) { Text("¿Olvidaste tu contraseña?", color = Color(0xFF6C63FF)) }
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
