package com.manybox.chofer.ui

import androidx.compose.foundation.background
import androidx.compose.foundation.layout.*
import androidx.compose.foundation.lazy.LazyColumn
import androidx.compose.foundation.lazy.items
import androidx.compose.material3.*
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.filled.Home
import androidx.compose.material.icons.filled.List
import androidx.compose.material.icons.filled.Book
import androidx.compose.material.icons.filled.Notifications
import androidx.compose.runtime.*
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.sp
import com.manybox.chofer.model.GuiaAsignada

@Composable
fun DashboardScreen(
    nombre: String,
    pendientes: Int,
    confirmados: Int,
    incidencias: Int,
    guias: List<GuiaAsignada>,
    onRecoger: (GuiaAsignada) -> Unit,
    onIniciarEntrega: (GuiaAsignada) -> Unit,
    onEntregar: (GuiaAsignada) -> Unit,
    onScanBarcode: () -> Unit
) {
    val TAG = "DashboardScreen"
    var selectedTab by remember { mutableStateOf(1) } // 0:Inicio, 1:Guías, 2:Bitácora, 3:Notificaciones
    Column(
        modifier = Modifier
            .fillMaxSize()
            .background(Color(0xFF181A20))
    ) {
        Box(modifier = Modifier.weight(1f)) {
            when (selectedTab) {
                0 -> {
                    Column(modifier = Modifier.padding(16.dp)) {
                        Text("Bienvenido, $nombre", fontSize = 24.sp, fontWeight = FontWeight.Bold, color = Color.White)
                        Text("Tu panel de trabajo diario", fontSize = 16.sp, color = Color(0xFFB0B3B8), modifier = Modifier.padding(bottom = 16.dp))
                        Row(
                            modifier = Modifier.fillMaxWidth(),
                            horizontalArrangement = Arrangement.SpaceBetween
                        ) {
                            SummaryCard("Pendientes", pendientes, Color(0xFF6C63FF))
                            SummaryCard("Confirmados", confirmados, Color(0xFF00C853))
                            SummaryCard("Incidencias", incidencias, Color(0xFFFF5252))
                        }
                        // Log para depuración de datos en inicio
                        LaunchedEffect(nombre, pendientes, confirmados, incidencias) {
                            android.util.Log.i("DashboardScreen", "Inicio: nombre=$nombre, pendientes=$pendientes, confirmados=$confirmados, incidencias=$incidencias")
                        }
                    }
                }
                1 -> {
                    Column(modifier = Modifier.padding(16.dp)) {
                        Text("Guías Asignadas", color = Color.White, fontWeight = FontWeight.Bold, fontSize = 18.sp)
                        if (guias.isEmpty()) {
                            Text("No tienes guías asignadas", color = Color(0xFFB0B3B8), modifier = Modifier.padding(top = 24.dp))
                        } else {
                            LazyColumn(modifier = Modifier.weight(1f, fill = false)) {
                                items(guias) { guia ->
                                    GuiaCard(guia, onRecoger, onIniciarEntrega, onEntregar)
                                }
                            }
                        }
                        Spacer(modifier = Modifier.height(16.dp))
                        Button(
                            onClick = onScanBarcode,
                            colors = ButtonDefaults.buttonColors(containerColor = Color(0xFF6C63FF))
                        ) {
                            Text("Escanear Código de Barras", color = Color.White)
                        }
                    }
                }
                2 -> {
                    Column(modifier = Modifier.padding(16.dp)) {
                        Text("Bitácora", color = Color.White, fontWeight = FontWeight.Bold, fontSize = 18.sp)
                        Text("Aquí aparecerán los eventos y registros importantes.", color = Color(0xFFB0B3B8), modifier = Modifier.padding(top = 24.dp))
                    }
                }
                3 -> {
                    Column(modifier = Modifier.padding(16.dp)) {
                        Text("Notificaciones", color = Color.White, fontWeight = FontWeight.Bold, fontSize = 18.sp)
                        Text("Aquí aparecerán tus notificaciones.", color = Color(0xFFB0B3B8), modifier = Modifier.padding(top = 24.dp))
                    }
                }
            }
        }
        BottomNavigationBar(selectedTab) { selectedTab = it }
    }

}

@Composable
fun BottomNavigationBar(selectedTab: Int, onTabSelected: (Int) -> Unit) {
    NavigationBar(containerColor = Color(0xFF23272F)) {
        NavigationBarItem(
            selected = selectedTab == 0,
            onClick = { onTabSelected(0) },
            icon = { Icon(Icons.Default.Home, contentDescription = "Inicio", tint = Color.White) },
            label = { Text("Inicio", color = Color.White) }
        )
        NavigationBarItem(
            selected = selectedTab == 1,
            onClick = { onTabSelected(1) },
            icon = { Icon(Icons.Default.List, contentDescription = "Guías", tint = Color.White) },
            label = { Text("Guías", color = Color.White) }
        )
        NavigationBarItem(
            selected = selectedTab == 2,
            onClick = { onTabSelected(2) },
            icon = { Icon(Icons.Default.Book, contentDescription = "Bitácora", tint = Color.White) },
            label = { Text("Bitácora", color = Color.White) }
        )
        NavigationBarItem(
            selected = selectedTab == 3,
            onClick = { onTabSelected(3) },
            icon = { Icon(Icons.Default.Notifications, contentDescription = "Notificaciones", tint = Color.White) },
            label = { Text("Notificaciones", color = Color.White) }
        )
    }
}

@Composable
fun RowScope.SummaryCard(label: String, value: Int, color: Color) {
    Card(
        modifier = Modifier
            .weight(1f)
            .padding(4.dp),
        colors = CardDefaults.cardColors(containerColor = color)
    ) {
        Column(
            modifier = Modifier.padding(12.dp),
            horizontalAlignment = Alignment.CenterHorizontally
        ) {
            Text(label, color = Color.White, fontWeight = FontWeight.Bold)
            Text("$value", color = Color.White, fontSize = 20.sp, fontWeight = FontWeight.Bold)
        }
    }
}

@Composable
fun GuiaCard(
    guia: GuiaAsignada,
    onRecoger: (GuiaAsignada) -> Unit,
    onIniciarEntrega: (GuiaAsignada) -> Unit,
    onEntregar: (GuiaAsignada) -> Unit
) {
    val TAG = "GuiaCard"
    Card(
        modifier = Modifier
            .fillMaxWidth()
            .padding(vertical = 4.dp),
        colors = CardDefaults.cardColors(containerColor = Color(0xFF23272F))
    ) {
        Column(modifier = Modifier.padding(12.dp)) {
            Text(guia.guiaRastreo, color = Color(0xFF6C63FF), fontWeight = FontWeight.Bold)
            Text(guia.destinatario, color = Color.White)
            Spacer(modifier = Modifier.height(8.dp))
            Row {
                when (guia.estadoActual) {
                    0 -> Button(onClick = {
                        android.util.Log.i(TAG, "Recoger: ${guia.guiaRastreo}")
                        onRecoger(guia)
                    }, colors = ButtonDefaults.buttonColors(containerColor = Color(0xFF6C63FF))) { Text("Recoger", color = Color.White) }
                    1 -> Button(onClick = {
                        android.util.Log.i(TAG, "Iniciar Entrega: ${guia.guiaRastreo}")
                        onIniciarEntrega(guia)
                    }, colors = ButtonDefaults.buttonColors(containerColor = Color(0xFF6C63FF))) { Text("Iniciar Entrega", color = Color.White) }
                    2 -> Button(onClick = {
                        android.util.Log.i(TAG, "Entregar: ${guia.guiaRastreo}")
                        onEntregar(guia)
                    }, colors = ButtonDefaults.buttonColors(containerColor = Color(0xFF00C853))) { Text("Entregar", color = Color.White) }
                }
            }
        }
    }
}