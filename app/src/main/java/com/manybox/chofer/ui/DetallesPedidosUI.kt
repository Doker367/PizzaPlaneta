package com.manybox.chofer.ui

import androidx.compose.foundation.Image
import androidx.compose.foundation.background
import androidx.compose.foundation.clickable
import androidx.compose.foundation.layout.*
import androidx.compose.foundation.shape.CircleShape
import androidx.compose.foundation.shape.RoundedCornerShape
import androidx.compose.ui.draw.clip
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.filled.ArrowBack
import androidx.compose.material3.*
import androidx.compose.runtime.*
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.layout.ContentScale
import androidx.compose.ui.res.painterResource
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.sp

/** Pantalla de detalles antes de confirmar un pedido */
@Composable
fun DetallesPedidoScreen(
    item: MenuItem,
    onBack: (() -> Unit)? = null,
    onAddMore: () -> Unit,
    onConfirm: (qty: Int, notes: String?) -> Unit,
    headerImageRes: Int? = null
) {
    val Navy = Color(0xFF1D3557)
    val PanelBg = Color(0xFFF2F2F2)
    val ButtonRed = Color(0xFFE53935)
    val ButtonBlue = Color(0xFF1D3557)

    var qty by remember { mutableStateOf(1) }
    var notes by remember { mutableStateOf("") }

    Box(modifier = Modifier.fillMaxSize().background(Navy)) {
        if (headerImageRes != null) {
            Box(modifier = Modifier
                .fillMaxWidth()
                .height(140.dp)) {
                Image(
                    painter = painterResource(id = headerImageRes),
                    contentDescription = "Header",
                    modifier = Modifier.fillMaxSize(),
                    contentScale = ContentScale.Crop
                )
                if (onBack != null) {
                    Box(
                        modifier = Modifier
                            .padding(8.dp)
                            .size(36.dp)
                            .clip(CircleShape)
                            .background(Color.White)
                            .clickable { onBack() },
                        contentAlignment = Alignment.Center
                    ) {
                        Icon(Icons.Default.ArrowBack, contentDescription = "Volver", tint = Color.Black)
                    }
                }
            }
            Box(
                modifier = Modifier
                    .fillMaxWidth()
                    .height(80.dp)
                    .background(Navy, shape = RoundedCornerShape(topStart = 96.dp, topEnd = 96.dp))
            )
        }

        Card(
            modifier = Modifier
                .fillMaxWidth()
                .padding(16.dp)
                .align(Alignment.TopCenter)
                // move the card further down (center) so the header artwork is clearly visible
                .offset(y = if (headerImageRes != null) 200.dp else 12.dp),
            colors = CardDefaults.cardColors(containerColor = PanelBg),
            shape = RoundedCornerShape(12.dp)
        ) {
            Column(modifier = Modifier.padding(16.dp), verticalArrangement = Arrangement.spacedBy(12.dp)) {
                Row(verticalAlignment = Alignment.CenterVertically) {
                    Image(
                        painter = painterResource(id = item.imageRes),
                        contentDescription = item.name,
                        modifier = Modifier.size(88.dp),
                        contentScale = ContentScale.Crop
                    )
                    Spacer(Modifier.width(12.dp))
                    Column {
                        Text(item.name, fontWeight = FontWeight.Bold, fontSize = 18.sp, color = Color.Black)
                        Spacer(Modifier.height(4.dp))
                        Text("Precio: $" + "%.0f".format(item.price), color = Color.DarkGray)
                    }
                }

                Row(verticalAlignment = Alignment.CenterVertically, horizontalArrangement = Arrangement.spacedBy(8.dp)) {
                    Text("Cantidad:", fontWeight = FontWeight.SemiBold)
                    Button(onClick = { if (qty > 1) qty -= 1 }, modifier = Modifier.size(36.dp), colors = ButtonDefaults.buttonColors(containerColor = PanelBg)) { Text("-") }
                    Text(qty.toString(), modifier = Modifier.width(28.dp), textAlign = androidx.compose.ui.text.style.TextAlign.Center)
                    Button(onClick = { qty += 1 }, modifier = Modifier.size(36.dp), colors = ButtonDefaults.buttonColors(containerColor = PanelBg)) { Text("+") }
                    Spacer(Modifier.weight(1f))
                    Text("Subtotal: $"+"%.0f".format(item.price * qty), fontWeight = FontWeight.Bold)
                }

                OutlinedTextField(value = notes, onValueChange = { notes = it }, label = { Text("Agregar nota / extras") }, modifier = Modifier.fillMaxWidth(), colors = OutlinedTextFieldDefaults.colors(focusedBorderColor = Color(0xFFBDBDBD), unfocusedBorderColor = Color(0xFFBDBDBD), focusedContainerColor = Color.White, unfocusedContainerColor = Color.White))

                Spacer(Modifier.height(8.dp))

                Row(modifier = Modifier.fillMaxWidth(), horizontalArrangement = Arrangement.spacedBy(8.dp)) {
                    OutlinedButton(onClick = onAddMore, modifier = Modifier.weight(1f), colors = ButtonDefaults.outlinedButtonColors(containerColor = Color.White)) { Text("Agregar algo más") }
                    if (onBack != null) {
                        OutlinedButton(onClick = { onBack() }, modifier = Modifier.weight(1f), colors = ButtonDefaults.outlinedButtonColors(containerColor = Color.White)) { Text("Volver") }
                    } else {
                        Spacer(modifier = Modifier.weight(1f))
                    }
                    Button(onClick = { onConfirm(qty, notes) }, modifier = Modifier.weight(1f), colors = ButtonDefaults.buttonColors(containerColor = ButtonBlue)) { Text("Confirmar pedido — $" + "%.0f".format(item.price * qty), color = Color.White) }
                }
            }
        }
    }
}
