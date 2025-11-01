@file:OptIn(ExperimentalMaterial3Api::class)

package com.manybox.chofer.ui

import androidx.compose.foundation.*
import androidx.compose.foundation.layout.*
import androidx.compose.foundation.lazy.LazyColumn
import androidx.compose.foundation.lazy.items
import androidx.compose.foundation.shape.CircleShape
import androidx.compose.foundation.shape.RoundedCornerShape
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.filled.ArrowBack
import androidx.compose.material.icons.filled.FavoriteBorder
import androidx.compose.material3.*
import androidx.compose.runtime.*
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.draw.clip
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.graphics.SolidColor
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.sp
import androidx.compose.ui.res.painterResource
import androidx.compose.ui.layout.ContentScale
import androidx.compose.ui.text.style.TextAlign
import com.manybox.chofer.R
import android.content.Context
import androidx.compose.runtime.LaunchedEffect
import androidx.compose.runtime.rememberCoroutineScope
import androidx.compose.ui.platform.LocalContext
import com.manybox.chofer.api.MenuItemDto
import com.manybox.chofer.api.RetrofitProvider
import com.manybox.chofer.api.CreateOrderRequest
import com.manybox.chofer.api.OrderItemRequest
import retrofit2.Call
import retrofit2.Callback
import retrofit2.Response
import kotlinx.coroutines.launch

// Definición de colores
val OrangeBrand = Color(0xFFFF9800)
val RedBrand = Color(0xFFD32F2F)
val GrayBackground = Color(0xFFF5F5F5)
val BrownDark = Color(0xFF3E2723)
val DarkBackground = Color(0xFF212121) // Color oscuro para la imagen de fondo

// Modelo UI derivado del DTO (sin imágenes por ahora)
data class MenuUiItem(
    val id: Int,
    val name: String,
    val description: String,
    val price: Double
)

@Composable
fun MenuPlatillos(
    onBackClick: () -> Unit,
    onMenuClick: () -> Unit,
    sucursalId: Int = 3
) {
    val context = LocalContext.current
    var isLoading by remember { mutableStateOf(true) }
    var error by remember { mutableStateOf<String?>(null) }
    var items by remember { mutableStateOf<List<MenuUiItem>>(emptyList()) }
    var selected by remember { mutableStateOf<MenuUiItem?>(null) }
    var qty by remember { mutableStateOf(1) }

    LaunchedEffect(sucursalId) {
        isLoading = true
        error = null
        RetrofitProvider.pizzaApi(context).getMenuBySucursalId(sucursalId)
            .enqueue(object : Callback<List<MenuItemDto>> {
                override fun onResponse(
                    call: Call<List<MenuItemDto>>, response: Response<List<MenuItemDto>>
                ) {
                    if (response.isSuccessful) {
                        val body = response.body().orEmpty()
                        items = body.map {
                            MenuUiItem(
                                id = it.productoId,
                                name = it.nombre,
                                description = it.descripcion,
                                price = it.precio.toDouble()
                            )
                        }
                        isLoading = false
                    } else {
                        error = "Error ${response.code()} al cargar menú"
                        isLoading = false
                    }
                }

                override fun onFailure(call: Call<List<MenuItemDto>>, t: Throwable) {
                    error = t.message
                    isLoading = false
                }
            })
    }

    val snackbarHostState = remember { SnackbarHostState() }
    val scope = rememberCoroutineScope()
    var isSubmitting by remember { mutableStateOf(false) }

    Scaffold(
        containerColor = GrayBackground,
        snackbarHost = { SnackbarHost(snackbarHostState) }
    ) { paddingValues ->
        LazyColumn(
            modifier = Modifier
                .fillMaxSize()
                .padding(paddingValues),
            horizontalAlignment = Alignment.CenterHorizontally
        ) {
            // =======================================================
            // 1. Encabezado con Imagen, Botones de Navegación y Banner
            // =======================================================
            item {
                Box(
                    modifier = Modifier
                        .fillMaxWidth()
                        .height(280.dp) 
                ) {
                    // Imagen de fondo
                    Image(
                        painter = painterResource(id = R.drawable.pm1),
                        contentDescription = null,
                        modifier = Modifier
                            .fillMaxWidth()
                            .height(200.dp),
                        contentScale = ContentScale.Crop
                    )

                    // Botones de Regreso y Favoritos
                    Row(
                        modifier = Modifier
                            .fillMaxWidth()
                            .padding(16.dp),
                        horizontalArrangement = Arrangement.SpaceBetween
                    ) {
                        // Botón de Regreso
                        FloatingActionButton(
                            onClick = onBackClick,
                            containerColor = Color.Black.copy(alpha = 0.5f), 
                            shape = CircleShape,
                            elevation = FloatingActionButtonDefaults.elevation(defaultElevation = 0.dp),
                            modifier = Modifier.size(40.dp)
                        ) {
                            Icon(Icons.Default.ArrowBack, "Regresar", tint = Color.White)
                        }
                        // Botón de Favorito
                        FloatingActionButton(
                            onClick = { /* TODO */ },
                            containerColor = Color.Black.copy(alpha = 0.5f), 
                            shape = CircleShape,
                            elevation = FloatingActionButtonDefaults.elevation(defaultElevation = 0.dp),
                            modifier = Modifier.size(40.dp)
                        ) {
                            Icon(Icons.Default.FavoriteBorder, "Favorito", tint = Color.White)
                        }
                    }

                    // Banner de Información del Restaurante
                    Card(
                        modifier = Modifier
                            .align(Alignment.BottomCenter) 
                            .padding(horizontal = 16.dp)
                            .fillMaxWidth(),
                        shape = RoundedCornerShape(8.dp),
                        colors = CardDefaults.cardColors(containerColor = Color.White),
                        elevation = CardDefaults.cardElevation(defaultElevation = 4.dp)
                    ) {
                        Row(
                            modifier = Modifier.padding(16.dp),
                            verticalAlignment = Alignment.CenterVertically
                        ) {
                            // Icono del Restaurante
                            Box(
                                modifier = Modifier
                                    .size(40.dp)
                                    .clip(RoundedCornerShape(8.dp))
                                    .background(RedBrand.copy(alpha = 0.1f)),
                                contentAlignment = Alignment.Center
                            ) {
                                Image(
                                    painter = painterResource(id = R.drawable.i4m),
                                    contentDescription = "Logo restaurante",
                                    modifier = Modifier.size(32.dp),
                                    contentScale = ContentScale.Fit
                                )
                            }

                            Spacer(Modifier.width(12.dp))

                            Column {
                                Text(
                                    "Pizza Planet Centro",
                                    fontWeight = FontWeight.Bold,
                                    fontSize = 18.sp,
                                    color = DarkBackground 
                                )
                                Text(
                                    "Av. 1ra poniente #2396",
                                    fontSize = 14.sp,
                                    color = Color.Gray
                                )
                            }
                        }
                    }
                }
            }

            // Estado de carga o error
            if (isLoading) {
                item { 
                    Spacer(Modifier.height(24.dp))
                    CircularProgressIndicator(color = RedBrand)
                    Spacer(Modifier.height(24.dp))
                }
            }
            if (error != null) {
                item {
                    Spacer(Modifier.height(12.dp))
                    Text(error!!, color = Color.Red)
                }
            }

            // Lista de productos del menú
            item {
                Spacer(Modifier.height(16.dp))
                CategoryButton(text = "Menú")
                Spacer(Modifier.height(16.dp))
            }

            items(items) { item ->
                MenuItemRow(item = item, onView = { selected = item; qty = 1 })
                Spacer(Modifier.height(12.dp))
            }

            // Espaciador para el área del BottomBar flotante
            item {
                Spacer(Modifier.height(80.dp))
            }
        }
        
        // =======================================================
        // 5. Bottom Bar Flotante
        // =======================================================
        Box(
            modifier = Modifier
                .fillMaxSize()
                .padding(paddingValues),
            contentAlignment = Alignment.BottomCenter 
        ) {
            Card(
                modifier = Modifier
                    .fillMaxWidth()
                    .height(70.dp),
                shape = RoundedCornerShape(topStart = 8.dp, topEnd = 8.dp), 
                colors = CardDefaults.cardColors(containerColor = RedBrand),
                elevation = CardDefaults.cardElevation(defaultElevation = 8.dp) 
            ) {
                Row(
                    modifier = Modifier
                        .fillMaxWidth()
                        .padding(horizontal = 16.dp),
                    verticalAlignment = Alignment.CenterVertically
                ) {
                    // Modificar el Box del icono de menú para hacerlo clickeable
                    Box(
                        modifier = Modifier
                            .size(30.dp)
                            .clickable(onClick = onMenuClick),
                        contentAlignment = Alignment.Center
                    ) {
                        Text("☰", fontSize = 20.sp, color = Color.White)
                    }
                    
                    Spacer(Modifier.weight(1f)) 

                    // Texto de bienvenida y perfil
                    Row(
                        verticalAlignment = Alignment.CenterVertically,
                        modifier = Modifier.padding(vertical = 12.dp)
                    ) {
                        Text(
                            "BIENVENIDO, TADEO",
                            color = Color.White,
                            fontWeight = FontWeight.Bold,
                            fontSize = 14.sp,
                            textAlign = TextAlign.Center
                        )

                        Spacer(Modifier.width(8.dp))

                        // Placeholder de la foto de perfil (círculo)
                        Box(
                            modifier = Modifier
                                .size(36.dp)
                                .clip(CircleShape)
                                .background(Color.Gray),
                            contentAlignment = Alignment.Center
                        ) {
                            Text("👤", fontSize = 20.sp, color = Color.White)
                        }
                    }
                }
            }
        }
    }

    // Detalle del producto con contador y sugeridos
    if (selected != null) {
        val current = selected!!
        ModalBottomSheet(
            onDismissRequest = { selected = null },
            containerColor = Color.White
        ) {
            Column(modifier = Modifier.padding(16.dp)) {
                Text(current.name, fontWeight = FontWeight.Bold, fontSize = 20.sp)
                if (current.description.isNotEmpty()) {
                    Spacer(Modifier.height(6.dp))
                    Text(current.description, color = Color.Gray)
                }
                Spacer(Modifier.height(10.dp))
                Text("$" + "%.0f".format(current.price), fontWeight = FontWeight.ExtraBold, fontSize = 18.sp, color = BrownDark)

                Spacer(Modifier.height(16.dp))
                Text("Cantidad", fontWeight = FontWeight.SemiBold)
                Row(verticalAlignment = Alignment.CenterVertically) {
                    OutlinedButton(onClick = { if (qty > 1) qty-- }) { Text("-") }
                    Text(qty.toString(), modifier = Modifier.padding(horizontal = 16.dp))
                    OutlinedButton(onClick = { qty++ }) { Text("+") }
                }

                Spacer(Modifier.height(16.dp))
                Button(
                    enabled = !isSubmitting,
                    onClick = {
                        isSubmitting = true
                        val body = CreateOrderRequest(
                            sucursalId = sucursalId,
                            items = listOf(OrderItemRequest(productoId = current.id, cantidad = qty))
                        )
                        RetrofitProvider.pizzaApi(context).createOrder(body)
                            .enqueue(object : Callback<Void> {
                                override fun onResponse(call: Call<Void>, response: Response<Void>) {
                                    isSubmitting = false
                                    if (response.isSuccessful) {
                                        selected = null
                                        qty = 1
                                        scope.launch { snackbarHostState.showSnackbar("Producto agregado al pedido") }
                                    } else {
                                        scope.launch { snackbarHostState.showSnackbar("Error ${response.code()} al agregar") }
                                    }
                                }

                                override fun onFailure(call: Call<Void>, t: Throwable) {
                                    isSubmitting = false
                                    scope.launch { snackbarHostState.showSnackbar("Error: ${t.message}") }
                                }
                            })
                    },
                    modifier = Modifier.fillMaxWidth(),
                    colors = ButtonDefaults.buttonColors(containerColor = RedBrand)
                ) {
                    if (isSubmitting) {
                        CircularProgressIndicator(color = Color.White, modifier = Modifier.size(18.dp), strokeWidth = 2.dp)
                        Spacer(Modifier.width(8.dp))
                    }
                    Text("Agregar ($" + "%.0f".format(current.price * qty) + ")", color = Color.White)
                }

                Spacer(Modifier.height(20.dp))
                Text("Sugeridos", fontWeight = FontWeight.Bold)
                Spacer(Modifier.height(8.dp))
                val suggestions = items.filter { it.id != current.id }.take(3)
                suggestions.forEach { s ->
                    SuggestionRow(item = s, onClick = { selected = s; qty = 1 })
                    Spacer(Modifier.height(8.dp))
                }
                Spacer(Modifier.height(16.dp))
            }
        }
    }
}

// Composable para el botón de categoría (Pizzas, Snacks, Bebidas)
@Composable
fun CategoryButton(text: String) {
    Button(
        onClick = { /* TODO */ },
        colors = ButtonDefaults.buttonColors(containerColor = OrangeBrand),
        shape = RoundedCornerShape(8.dp),
        modifier = Modifier.widthIn(min = 150.dp, max = 200.dp) 
    ) {
        Text(text, color = Color.White, fontWeight = FontWeight.SemiBold)
    }
}

// Composable para cada elemento del menú (Pizza, Snack, Bebida)
@Composable
fun MenuItemRow(item: MenuUiItem, onView: () -> Unit) {
    Card(
        modifier = Modifier
            .fillMaxWidth()
            .padding(horizontal = 16.dp),
        shape = RoundedCornerShape(8.dp),
        colors = CardDefaults.cardColors(containerColor = Color.White),
        elevation = CardDefaults.cardElevation(defaultElevation = 2.dp)
    ) {
        Row(
            modifier = Modifier
                .fillMaxWidth()
                .padding(12.dp),
            verticalAlignment = Alignment.CenterVertically
        ) {
            // Placeholder de imagen hasta que tengamos URLs
            Box(
                modifier = Modifier
                    .size(90.dp)
                    .clip(RoundedCornerShape(8.dp))
                    .background(Color(0xFFF1F1F1)),
                contentAlignment = Alignment.Center
            ) { Text("🍕", fontSize = 28.sp) }

            Spacer(Modifier.width(16.dp))

            Column(
                modifier = Modifier.weight(1f)
            ) {
                Text(
                    item.name,
                    fontWeight = FontWeight.Bold,
                    fontSize = 16.sp
                )
                // Mostrar descripción si existe
                if (item.description.isNotEmpty()) {
                    Text(
                        item.description,
                        color = Color.Gray,
                        fontSize = 12.sp, 
                        modifier = Modifier.padding(vertical = 4.dp)
                    )
                }
                Text(
                    "$" + "%.0f".format(item.price),
                    fontWeight = FontWeight.ExtraBold,
                    color = BrownDark, 
                    fontSize = 16.sp
                )
            }

            Spacer(Modifier.width(8.dp))

            // Botón "agregar"
            OutlinedButton(
                onClick = onView,
                colors = ButtonDefaults.outlinedButtonColors(
                    contentColor = BrownDark, 
                    containerColor = Color.White 
                ),
                border = BorderStroke(1.dp, Color.LightGray), 
                shape = RoundedCornerShape(8.dp), 
                contentPadding = PaddingValues(horizontal = 16.dp, vertical = 8.dp),
                modifier = Modifier.height(35.dp) 
            ) {
                Text("ver", fontSize = 12.sp)
            }
        }
    }
}

@Composable
private fun SuggestionRow(item: MenuUiItem, onClick: () -> Unit) {
    Card(
        modifier = Modifier.fillMaxWidth(),
        shape = RoundedCornerShape(8.dp),
        colors = CardDefaults.cardColors(containerColor = Color(0xFFFDFDFD)),
        elevation = CardDefaults.cardElevation(defaultElevation = 1.dp)
    ) {
        Row(modifier = Modifier
            .fillMaxWidth()
            .padding(12.dp), verticalAlignment = Alignment.CenterVertically) {
            Box(
                modifier = Modifier.size(48.dp).clip(RoundedCornerShape(8.dp)).background(Color(0xFFF1F1F1)),
                contentAlignment = Alignment.Center
            ) { Text("🍕") }
            Spacer(Modifier.width(12.dp))
            Column(modifier = Modifier.weight(1f)) {
                Text(item.name, fontWeight = FontWeight.SemiBold)
                if (item.description.isNotEmpty()) Text(item.description, color = Color.Gray, fontSize = 12.sp, maxLines = 1)
            }
            Text("$" + "%.0f".format(item.price), fontWeight = FontWeight.Bold)
            Spacer(Modifier.width(8.dp))
            OutlinedButton(onClick = onClick) { Text("ver") }
        }
    }
}