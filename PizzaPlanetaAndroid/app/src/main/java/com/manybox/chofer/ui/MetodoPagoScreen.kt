package com.manybox.chofer.ui

import androidx.compose.foundation.Image
import androidx.compose.foundation.background
import androidx.compose.foundation.layout.*
import androidx.compose.foundation.lazy.LazyColumn
import androidx.compose.foundation.lazy.items
import androidx.compose.foundation.shape.RoundedCornerShape
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.filled.ArrowBack
import androidx.compose.material3.*
import androidx.compose.runtime.Composable
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.layout.ContentScale
import androidx.compose.ui.res.painterResource
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.text.style.TextAlign
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.sp
import com.manybox.chofer.R

@Composable
fun MetodoPagoScreen(
    onBack: () -> Unit,
    headerImageRes: Int // Opcional, para mantener consistencia
) {
    val Navy = Color(0xFF1D3557)
    val Orange = Color(0xFFF77F00)

    Scaffold(
        topBar = {
            TopAppBar(
                title = { Text("Métodos de Pago", color = Color.White) },
                navigationIcon = {
                    IconButton(onClick = onBack) {
                        Icon(Icons.Default.ArrowBack, contentDescription = "Regresar", tint = Color.White)
                    }
                },
                colors = TopAppBarDefaults.topAppBarColors(containerColor = Navy)
            )
        },
        containerColor = Color(0xFFF0F2F5) // Un color de fondo neutro
    ) { paddingValues ->
        LazyColumn(
            modifier = Modifier
                .fillMaxSize()
                .padding(paddingValues)
                .padding(16.dp),
            horizontalAlignment = Alignment.CenterHorizontally
        ) {
            item {
                // Estado vacío
                Column(
                    modifier = Modifier
                        .fillMaxWidth()
                        .padding(vertical = 32.dp),
                    horizontalAlignment = Alignment.CenterHorizontally,
                    verticalArrangement = Arrangement.Center
                ) {
                    Image(
                        painter = painterResource(id = R.drawable.generic_card),
                        contentDescription = "No hay tarjetas",
                        modifier = Modifier.height(80.dp), // Adjusted for aspect ratio
                        contentScale = ContentScale.Fit
                    )
                    Spacer(Modifier.height(16.dp))
                    Text(
                        "No tienes tarjetas guardadas",
                        fontSize = 18.sp,
                        fontWeight = FontWeight.Bold,
                        color = Color.DarkGray
                    )
                    Spacer(Modifier.height(8.dp))
                    Text(
                        "Agrega una tarjeta para un proceso de pago más rápido.",
                        fontSize = 14.sp,
                        color = Color.Gray,
                        textAlign = TextAlign.Center,
                        modifier = Modifier.padding(horizontal = 16.dp)
                    )
                }
            }

            item {
                // Botón para agregar nueva tarjeta
                Button(
                    onClick = { /* Lógica para mostrar formulario de agregar tarjeta */ },
                    modifier = Modifier
                        .fillMaxWidth()
                        .height(50.dp),
                    shape = RoundedCornerShape(12.dp),
                    colors = ButtonDefaults.buttonColors(containerColor = Orange)
                ) {
                    Text("AGREGAR NUEVA TARJETA", fontWeight = FontWeight.Bold, color = Color.White)
                }
            }

            item {
                Spacer(Modifier.height(32.dp))
                Text(
                    "Aceptamos:",
                    fontSize = 14.sp,
                    color = Color.Gray
                )
                Spacer(Modifier.height(16.dp))
                Row(
                    modifier = Modifier.fillMaxWidth(),
                    horizontalArrangement = Arrangement.Center,
                    verticalAlignment = Alignment.CenterVertically
                ) {
                    Image(
                        painter = painterResource(id = R.drawable.visa_logo),
                        contentDescription = "Visa",
                        modifier = Modifier.height(24.dp)
                    )
                    Spacer(Modifier.width(16.dp))
                    Image(
                        painter = painterResource(id = R.drawable.mastercard_logo),
                        contentDescription = "Mastercard",
                        modifier = Modifier.height(24.dp)
                    )
                    Spacer(Modifier.width(16.dp))
                    Image(
                        painter = painterResource(id = R.drawable.amex_logo),
                        contentDescription = "American Express",
                        modifier = Modifier.height(24.dp)
                    )
                }
            }
        }
    }
}
