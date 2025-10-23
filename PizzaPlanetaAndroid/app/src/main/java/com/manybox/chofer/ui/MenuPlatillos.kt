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

// Definición de colores
val OrangeBrand = Color(0xFFFF9800)
val RedBrand = Color(0xFFD32F2F)
val GrayBackground = Color(0xFFF5F5F5)
val BrownDark = Color(0xFF3E2723)
val DarkBackground = Color(0xFF212121) // Color oscuro para la imagen de fondo

// Data class genérica para todos los ítems (Pizza, Snack, Bebida)
data class MenuItem(
    val id: Int,
    val name: String,
    val description: String,
    val price: Double,
    val imageRes: Int // Aunque no se usará inmediatamente, mantiene la estructura
)

@Composable
fun MenuPlatillos(
    onBackClick: () -> Unit,
    onMenuClick: () -> Unit // Nuevo parámetro
) {
    // Datos originales de las Pizzas
    val pizzaItems = remember {
        listOf(
            MenuItem(1, "Clásica Pepperoni", "Salsa de tomate, queso mozzarella y pepperoni.", 149.0, R.drawable.pp1),
            MenuItem(2, "Cuatro Quesos", "Mezcla de mozzarella, parmesano, gorgonzola y cheddar.", 169.0, R.drawable.p4q),
            MenuItem(3, "Hawaiana Galáctica", "Jamón, piña y mozzarella.", 159.0, R.drawable.ph),
            MenuItem(4, "Peperoni con champiños", "Salsa de tomate, queso mozzarella, peperoni y champiñones.", 179.0, R.drawable.ppch),
            MenuItem(5, "Especial Espacial", "Salsa BBQ, pollo y cebolla morada.", 169.0, R.drawable.pe) 
        )
    }

    // NUEVOS DATOS DE SNACKS Y BEBIDAS
    val snackItems = remember {
        listOf(
            MenuItem(6, "Alitas de Meteoro", "Alitas de pollo con salsa búfalo.", 89.0, R.drawable.ab),
            MenuItem(7, "Asteroides de Queso", "Palitos de queso empanizados con salsa marinara.", 79.0, R.drawable.aq),
            MenuItem(8, "Papas Nebulosas", "Papas a la francesa con toque de especias.", 65.0, R.drawable.pn),
            MenuItem(9, "Pan Ajo Orbital", "Pan con mantequilla de ajo y queso.", 59.0, R.drawable.pao)
        )
    }

    val drinkItems = remember {
        listOf(
            MenuItem(10, "Refrescos clásicos", "(Cola, Naranja, Limón).", 25.0, R.drawable.rcnl),
            MenuItem(11, "Agua mineral planetaria.", "", 22.0, R.drawable.am),
            MenuItem(12, "Malteadas cósmicas", "(Choco, Vainilla, Fresa)", 49.0, R.drawable.mvcf)
        )
    }

    Scaffold(
        containerColor = GrayBackground // Color de fondo general del Scaffold
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

            // =======================================================
            // 2. Sección Pizzas
            // =======================================================
            item {
                Spacer(Modifier.height(16.dp))
                CategoryButton(text = "Pizzas")
                Spacer(Modifier.height(16.dp))
            }

            items(pizzaItems) { item ->
                MenuItemRow(item = item)
                Spacer(Modifier.height(12.dp))
            }
            
            // =======================================================
            // 3. Sección Snacks
            // =======================================================
            item {
                Spacer(Modifier.height(12.dp))
                CategoryButton(text = "Snacks")
                Spacer(Modifier.height(16.dp))
            }

            items(snackItems) { item ->
                MenuItemRow(item = item)
                Spacer(Modifier.height(12.dp))
            }

            // =======================================================
            // 4. Sección Bebidas
            // =======================================================
            item {
                Spacer(Modifier.height(12.dp))
                CategoryButton(text = "Bebidas")
                Spacer(Modifier.height(16.dp))
            }

            items(drinkItems) { item ->
                MenuItemRow(item = item)
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
fun MenuItemRow(item: MenuItem) {
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
            // Reemplazar el Box placeholder con una Image real
            Image(
                painter = painterResource(id = item.imageRes),
                contentDescription = item.name,
                modifier = Modifier
                    .size(90.dp)
                    .clip(RoundedCornerShape(8.dp)),
                contentScale = ContentScale.Crop
            )

            Spacer(Modifier.width(16.dp))

            Column(
                modifier = Modifier.weight(1f)
            ) {
                Text(
                    item.name,
                    fontWeight = FontWeight.Bold,
                    fontSize = 16.sp
                )
                // Usamos un Box para que la descripción se muestre solo si no está vacía
                if (item.description.isNotEmpty()) {
                    Text(
                        item.description,
                        color = Color.Gray,
                        fontSize = 12.sp, 
                        modifier = Modifier.padding(vertical = 4.dp)
                    )
                }
                Text(
                    "$" + "%.0f".format(item.price), // Formateo de precio sin decimales
                    fontWeight = FontWeight.ExtraBold,
                    color = BrownDark, 
                    fontSize = 16.sp
                )
            }

            Spacer(Modifier.width(8.dp))

            // Botón "agregar"
            OutlinedButton(
                onClick = { /* TODO */ },
                colors = ButtonDefaults.outlinedButtonColors(
                    contentColor = BrownDark, 
                    containerColor = Color.White 
                ),
                border = BorderStroke(1.dp, Color.LightGray), 
                shape = RoundedCornerShape(8.dp), 
                contentPadding = PaddingValues(horizontal = 16.dp, vertical = 8.dp),
                modifier = Modifier.height(35.dp) 
            ) {
                Text("agregar", fontSize = 12.sp)
            }
        }
    }
}