package com.manybox.chofer.ui

import androidx.compose.foundation.Image
import androidx.compose.foundation.background
import androidx.compose.foundation.clickable
import androidx.compose.foundation.layout.*
import androidx.compose.foundation.shape.CircleShape
import androidx.compose.foundation.shape.RoundedCornerShape
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.filled.ArrowBack
import androidx.compose.material.icons.filled.CheckCircle
import androidx.compose.material3.*
import androidx.compose.runtime.Composable
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.draw.clip
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.layout.ContentScale
import androidx.compose.ui.res.painterResource
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.sp
import androidx.compose.ui.tooling.preview.Preview

data class OrderInfo(
    val orderId: String,
    val branchName: String,
    val pickupLocation: String,
    val readyInMinutes: Int,
    val itemsDescription: String,
    val totalAmount: Double
)

@Composable
fun OrderConfirmationScreen(
    info: OrderInfo,
    onClose: () -> Unit,
    onBack: (() -> Unit)? = null,
    headerImageRes: Int? = null
) {
    val Navy = Color(0xFF1D3557)
    val PanelBg = Color(0xFFF2F2F2)
    val ButtonBlue = Color(0xFF1D3557)

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
                // move the confirmation card further down (center) so the header artwork is clearly visible
                .offset(y = if (headerImageRes != null) 200.dp else 12.dp),
            colors = CardDefaults.cardColors(containerColor = PanelBg),
            shape = RoundedCornerShape(12.dp)
        ) {
            Column(modifier = Modifier.padding(16.dp), verticalArrangement = Arrangement.spacedBy(12.dp)) {
                Row(verticalAlignment = Alignment.CenterVertically) {
                    Icon(Icons.Default.CheckCircle, contentDescription = null, tint = Color(0xFF2E7D32), modifier = Modifier.size(36.dp))
                    Spacer(Modifier.width(8.dp))
                    Column {
                        Text("Pedido confirmado", fontWeight = FontWeight.Bold, fontSize = 18.sp, color = Color.Black)
                        Text("Orden #${info.orderId}", color = Color.Gray)
                    }
                }

                Text("Sucursal: ${info.branchName}", fontWeight = FontWeight.SemiBold, color = Color.Black)
                Text("Lugar de retiro: ${info.pickupLocation}")
                Text("Tiempo estimado de preparación: ${info.readyInMinutes} minutos", fontWeight = FontWeight.SemiBold)

                Text("Items: ${info.itemsDescription}")
                Text("Total: S/ ${"%.2f".format(info.totalAmount)}", fontWeight = FontWeight.Bold)

                Spacer(Modifier.height(8.dp))
                Button(onClick = onClose, modifier = Modifier.fillMaxWidth(), colors = ButtonDefaults.buttonColors(containerColor = ButtonBlue)) { Text("Aceptar", color = Color.White) }
            }
        }
    }
}

@Preview(showBackground = true)
@Composable
private fun OrderConfirmationPreview() {
    OrderConfirmationScreen(
        info = OrderInfo(
            orderId = "12345",
            branchName = "Sucursal Central",
            pickupLocation = "Av. Principal 123",
            readyInMinutes = 25,
            itemsDescription = "1 x Margarita, 2 x Pepperoni",
            totalAmount = 59.5
        ),
        onClose = {},
        onBack = {},
        headerImageRes = com.manybox.chofer.R.drawable.pizzorra
    )
}
