package com.manybox.chofer.ui

import androidx.compose.foundation.Image
import androidx.compose.foundation.background
import androidx.compose.foundation.layout.*
import androidx.compose.foundation.lazy.LazyColumn
import androidx.compose.foundation.lazy.items
import androidx.compose.foundation.shape.RoundedCornerShape
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.filled.ArrowBack
import androidx.compose.material.icons.filled.Add
import androidx.compose.material.icons.filled.Delete
import androidx.compose.material.icons.filled.Remove
import androidx.compose.material3.*
import androidx.compose.runtime.*
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.draw.clip
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.layout.ContentScale
import androidx.compose.ui.res.painterResource
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.sp
import com.manybox.chofer.R

data class LegacyCartItem(
    val id: Int,
    val name: String,
    val price: Double,
    val quantity: Int,
    val notes: String = ""
)

@OptIn(ExperimentalMaterial3Api::class)
@Composable
fun CarritoScreen(
    onBackClick: () -> Unit,
    onCheckoutClick: () -> Unit,
    items: List<LegacyCartItem>,
    onUpdateQuantity: (LegacyCartItem, Int) -> Unit,
    onDeleteItem: (LegacyCartItem) -> Unit,
    onClearCart: () -> Unit
) {
    var showDeleteDialog by remember { mutableStateOf<LegacyCartItem?>(null) }
    var showClearCartDialog by remember { mutableStateOf(false) }

    Scaffold(
        topBar = {
            Column {
                Box(
                    modifier = Modifier
                        .fillMaxWidth()
                        .height(60.dp)
                ) {
                    // Imagen de fondo PM1 (aumentar opacidad)
                    Image(
                        painter = painterResource(id = R.drawable.pm1),
                        contentDescription = null,
                        modifier = Modifier.fillMaxSize(),
                        contentScale = ContentScale.Crop,
                        alpha = 0.7f  // Cambiado de 0.2f a 0.7f para que se vea más
                    )
                    // Logo superpuesto
                    Image(
                        painter = painterResource(id = R.drawable.il1_alt),
                        contentDescription = "Pizza Planet Logo",
                        modifier = Modifier
                            .fillMaxWidth()
                            .padding(8.dp),
                        contentScale = ContentScale.Fit
                    )
                }
                TopAppBar(
                    title = { Text("Mi Carrito") },
                    navigationIcon = {
                        IconButton(onClick = onBackClick) {
                            Icon(Icons.Default.ArrowBack, contentDescription = "Regresar")
                        }
                    },
                    actions = {
                        if (items.isNotEmpty()) {
                            TextButton(
                                onClick = { showClearCartDialog = true },
                                colors = ButtonDefaults.textButtonColors(
                                    contentColor = Color.White
                                )
                            ) {
                                Text("Vaciar carrito")
                            }
                        }
                    },
                    colors = TopAppBarDefaults.topAppBarColors(
                        containerColor = Color(0xFFD32F2F),
                        titleContentColor = Color.White,
                        navigationIconContentColor = Color.White
                    )
                )
            }
        },
        bottomBar = {
            BottomAppBar(
                containerColor = Color.White,
                modifier = Modifier.height(130.dp)
            ) {
                Column(
                    modifier = Modifier
                        .fillMaxWidth()
                        .padding(16.dp)
                ) {
                    Row(
                        modifier = Modifier.fillMaxWidth(),
                        horizontalArrangement = Arrangement.SpaceBetween
                    ) {
                        Text("Total:", fontSize = 18.sp)
                        Text(
                            "S/ %.2f".format(items.sumOf { it.price * it.quantity }),
                            fontWeight = FontWeight.Bold,
                            fontSize = 18.sp
                        )
                    }
                    Spacer(Modifier.height(16.dp))
                    Button(
                        onClick = onCheckoutClick,
                        modifier = Modifier.fillMaxWidth(),
                        colors = ButtonDefaults.buttonColors(
                            containerColor = Color(0xFF1D3557)
                        )
                    ) {
                        Text("CONTINUAR")
                    }
                }
            }
        }
    ) { padding ->
        if (items.isEmpty()) {
            Box(
                modifier = Modifier
                    .fillMaxSize()
                    .padding(padding),
                contentAlignment = Alignment.Center
            ) {
                Text(
                    "Tu carrito está vacío",
                    color = Color.Gray,
                    fontSize = 16.sp
                )
            }
        } else {
            LazyColumn(
                modifier = Modifier
                    .fillMaxSize()
                    .padding(padding)
                    .padding(16.dp),
                verticalArrangement = Arrangement.spacedBy(16.dp)
            ) {
                items(items) { item ->
                    CartItemCard(
                        item = item,
                        onQuantityChange = { newQuantity ->
                            onUpdateQuantity(item, newQuantity)
                        },
                        onDelete = { showDeleteDialog = item }  // Modificado
                    )
                }
            }
        }
    }

    // Diálogo de confirmación para eliminar item
    if (showDeleteDialog != null) {
        AlertDialog(
            onDismissRequest = { showDeleteDialog = null },
            title = { Text("Confirmar eliminación") },
            text = { Text("¿Estás seguro de que deseas eliminar ${showDeleteDialog?.name} del carrito?") },
            confirmButton = {
                TextButton(
                    onClick = {
                        showDeleteDialog?.let { onDeleteItem(it) }
                        showDeleteDialog = null
                    }
                ) {
                    Text("Eliminar", color = Color(0xFFD32F2F))
                }
            },
            dismissButton = {
                TextButton(onClick = { showDeleteDialog = null }) {
                    Text("Cancelar")
                }
            }
        )
    }

    // Diálogo de confirmación para vaciar carrito
    if (showClearCartDialog) {
        AlertDialog(
            onDismissRequest = { showClearCartDialog = false },
            title = { Text("Vaciar carrito") },
            text = { Text("¿Estás seguro de que deseas vaciar todo el carrito?") },
            confirmButton = {
                TextButton(
                    onClick = {
                        onClearCart()
                        showClearCartDialog = false
                    }
                ) {
                    Text("Vaciar", color = Color(0xFFD32F2F))
                }
            },
            dismissButton = {
                TextButton(onClick = { showClearCartDialog = false }) {
                    Text("Cancelar")
                }
            }
        )
    }
}

@OptIn(ExperimentalMaterial3Api::class)
@Composable
private fun CartItemCard(
    item: LegacyCartItem,
    onQuantityChange: (Int) -> Unit,
    onDelete: () -> Unit
) {
    Card(
        modifier = Modifier.fillMaxWidth(),
        colors = CardDefaults.cardColors(containerColor = Color.White)
    ) {
        Row(
            modifier = Modifier
                .fillMaxWidth()
                .padding(16.dp),
            verticalAlignment = Alignment.CenterVertically
        ) {
            // Imagen de la pizza
            Box(
                modifier = Modifier
                    .size(80.dp)
                    .clip(RoundedCornerShape(8.dp))
                    .background(Color(0xFFEEEEEE))
            ) {
                Image(
                    painter = painterResource(id = R.drawable.i4m), // Cambiado de pizza_placeholder a i4m
                    contentDescription = null,
                    modifier = Modifier.fillMaxSize(),
                    contentScale = ContentScale.Crop
                )
            }
            
            Spacer(Modifier.width(16.dp))
            
            Column(
                modifier = Modifier.weight(1f)
            ) {
                Row(
                    modifier = Modifier.fillMaxWidth(),
                    horizontalArrangement = Arrangement.SpaceBetween,
                    verticalAlignment = Alignment.CenterVertically
                ) {
                    Text(
                        item.name,
                        fontWeight = FontWeight.Bold,
                        fontSize = 16.sp
                    )
                    IconButton(onClick = onDelete) {
                        Icon(
                            Icons.Default.Delete,
                            contentDescription = "Eliminar",
                            tint = Color(0xFFD32F2F)
                        )
                    }
                }
                
                if (item.notes.isNotBlank()) {
                    Spacer(Modifier.height(4.dp))
                    Text(
                        item.notes,
                        color = Color.Gray,
                        fontSize = 14.sp
                    )
                }
                
                Spacer(Modifier.height(8.dp))
                
                Row(
                    verticalAlignment = Alignment.CenterVertically,
                    horizontalArrangement = Arrangement.SpaceBetween,
                    modifier = Modifier.fillMaxWidth()
                ) {
                    // Controles de cantidad
                    Row(
                        verticalAlignment = Alignment.CenterVertically,
                        horizontalArrangement = Arrangement.spacedBy(8.dp)
                    ) {
                        IconButton(
                            onClick = { if (item.quantity > 1) onQuantityChange(item.quantity - 1) },
                            enabled = item.quantity > 1
                        ) {
                            Icon(Icons.Default.Remove, contentDescription = "Reducir cantidad")
                        }
                        
                        Text(
                            item.quantity.toString(),
                            modifier = Modifier.widthIn(min = 24.dp),
                            fontSize = 16.sp
                        )
                        
                        IconButton(onClick = { onQuantityChange(item.quantity + 1) }) {
                            Icon(Icons.Default.Add, contentDescription = "Aumentar cantidad")
                        }
                    }
                    
                    // Precio
                    Text(
                        "S/ %.2f".format(item.price * item.quantity),
                        fontWeight = FontWeight.Bold
                    )
                }
            }
        }
    }
}
