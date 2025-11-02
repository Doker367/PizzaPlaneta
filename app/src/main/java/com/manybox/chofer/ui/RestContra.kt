package com.manybox.chofer.ui

import androidx.compose.foundation.Image
import androidx.compose.foundation.background
import androidx.compose.foundation.clickable
import androidx.compose.foundation.layout.*
import androidx.compose.foundation.shape.CircleShape
import androidx.compose.foundation.shape.RoundedCornerShape
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.filled.ArrowBack
import androidx.compose.material3.*
import androidx.compose.runtime.*
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.draw.clip
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.layout.ContentScale
import androidx.compose.ui.res.painterResource
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.text.input.PasswordVisualTransformation
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.sp
import com.manybox.chofer.R

/**
 * Pantalla: Restablecer contraseña (solo front). Similar a RegistroScreen, con imagen "pizzorra" arriba.
 */
@Composable
fun RestablecerContrasenaScreen(
    headerImageRes: Int = R.drawable.pizzorra,
    onBack: () -> Unit = {},
    onSubmitEmail: (correo: String) -> Unit = {}
) {
    val Navy = Color(0xFF1D3557)
    val PanelBg = Color(0xFFF2F2F2)
    val ButtonRed = Color(0xFFE53935)

    var correo by remember { mutableStateOf("") }

    val tfColors = OutlinedTextFieldDefaults.colors(
        focusedBorderColor = Color(0xFFBDBDBD),
        unfocusedBorderColor = Color(0xFFBDBDBD),
        disabledBorderColor = Color(0xFFE0E0E0),
        focusedContainerColor = Color.White,
        unfocusedContainerColor = Color.White,
        disabledContainerColor = Color.White,
        focusedTextColor = Color.Black,
        unfocusedTextColor = Color.Black,
        cursorColor = Color.Black
    )

    Box(modifier = Modifier.fillMaxSize().background(Navy)) {
        Column(modifier = Modifier.fillMaxSize()) {
            // Imagen superior
            Box(modifier = Modifier.fillMaxWidth().height(180.dp)) {
                Image(
                    painter = painterResource(id = headerImageRes),
                    contentDescription = "Header",
                    modifier = Modifier.fillMaxSize(),
                    contentScale = ContentScale.Crop
                )
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
            Box(
                modifier = Modifier
                    .fillMaxWidth()
                    .height(90.dp)
                    .background(Navy, shape = RoundedCornerShape(topStart = 96.dp, topEnd = 96.dp))
            )

            // Tarjeta con formulario (superpuesta)
            Column(
                modifier = Modifier.fillMaxSize(),
                horizontalAlignment = Alignment.CenterHorizontally
            ) {
                Card(
                    modifier = Modifier
                        .fillMaxWidth()
                        .padding(horizontal = 16.dp)
                        .offset(y = (-60).dp),
                    colors = CardDefaults.cardColors(containerColor = PanelBg),
                    elevation = CardDefaults.cardElevation(defaultElevation = 12.dp),
                    shape = RoundedCornerShape(12.dp)
                ) {
                    Column(modifier = Modifier.padding(16.dp)) {
                        // Título y descripción
                        Text(
                            text = "🔑  RESTABLECER\nCONTRASEÑA",
                            color = Color.Black,
                            fontSize = 18.sp,
                            fontWeight = FontWeight.ExtraBold,
                            lineHeight = 20.sp
                        )
                        Spacer(Modifier.height(8.dp))
                        Text(
                            text = "Ingresa tu correo electrónico y te enviaremos un enlace para confirmar y crear una nueva contraseña.",
                            color = Color(0xFF475569),
                            fontSize = 13.sp
                        )

                        Spacer(Modifier.height(12.dp))
                        Text(text = "CORREO", color = Color.Black, fontSize = 12.sp)
                        OutlinedTextField(
                            value = correo,
                            onValueChange = { correo = it },
                            singleLine = true,
                            modifier = Modifier.fillMaxWidth(),
                            shape = RoundedCornerShape(8.dp),
                            colors = tfColors
                        )

                        Spacer(Modifier.height(12.dp))
                        Button(
                            onClick = { onSubmitEmail(correo) },
                            modifier = Modifier.fillMaxWidth().height(40.dp),
                            colors = ButtonDefaults.buttonColors(containerColor = ButtonRed),
                            shape = RoundedCornerShape(8.dp)
                        ) {
                            Text("ENVIAR", color = Color.White, fontWeight = FontWeight.Medium)
                        }
                    }
                }
            }
        }
    }
}

/**
 * Pantalla: Verificación de código (solo front). Reutiliza el header "pizzorra" y la tarjeta.
 */
@Composable
fun VerificarCodigoScreen(
    headerImageRes: Int = R.drawable.pizzorra,
    onBack: () -> Unit = {},
    onVerify: (code: String) -> Unit = {}
) {
    val Navy = Color(0xFF1D3557)
    val PanelBg = Color(0xFFF2F2F2)
    val ButtonRed = Color(0xFFE53935)

    var d1 by remember { mutableStateOf("") }
    var d2 by remember { mutableStateOf("") }
    var d3 by remember { mutableStateOf("") }
    var d4 by remember { mutableStateOf("") }

    val tfColors = OutlinedTextFieldDefaults.colors(
        focusedBorderColor = Color(0xFFBDBDBD),
        unfocusedBorderColor = Color(0xFFBDBDBD),
        disabledBorderColor = Color(0xFFE0E0E0),
        focusedContainerColor = Color.White,
        unfocusedContainerColor = Color.White,
        disabledContainerColor = Color.White,
        focusedTextColor = Color.Black,
        unfocusedTextColor = Color.Black,
        cursorColor = Color.Black
    )

    Box(modifier = Modifier.fillMaxSize().background(Navy)) {
        Column(modifier = Modifier.fillMaxSize()) {
            // Imagen superior
            Box(modifier = Modifier.fillMaxWidth().height(180.dp)) {
                Image(
                    painter = painterResource(id = headerImageRes),
                    contentDescription = "Header",
                    modifier = Modifier.fillMaxSize(),
                    contentScale = ContentScale.Crop
                )
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
            Box(
                modifier = Modifier
                    .fillMaxWidth()
                    .height(90.dp)
                    .background(Navy, shape = RoundedCornerShape(topStart = 96.dp, topEnd = 96.dp))
            )

            Column(modifier = Modifier.fillMaxSize(), horizontalAlignment = Alignment.CenterHorizontally) {
                Card(
                    modifier = Modifier
                        .fillMaxWidth()
                        .padding(horizontal = 16.dp)
                        .offset(y = (-60).dp),
                    colors = CardDefaults.cardColors(containerColor = PanelBg),
                    elevation = CardDefaults.cardElevation(defaultElevation = 12.dp),
                    shape = RoundedCornerShape(12.dp)
                ) {
                    Column(modifier = Modifier.padding(16.dp)) {
                        Text(
                            text = "🔑  Verificación de\ncódigo",
                            color = Color.Black,
                            fontSize = 18.sp,
                            fontWeight = FontWeight.ExtraBold,
                            lineHeight = 20.sp
                        )
                        Spacer(Modifier.height(8.dp))
                        Text(
                            text = "Hemos enviado un código de 4 dígitos a tu correo electrónico.\nPor favor, introdúcelo y continúa para continuar con el restablecimiento de tu contraseña.",
                            color = Color(0xFF475569),
                            fontSize = 13.sp
                        )
                        Spacer(Modifier.height(12.dp))
                        Row(horizontalArrangement = Arrangement.spacedBy(8.dp)) {
                            OutlinedTextField(
                                value = d1,
                                onValueChange = { if (it.length <= 1) d1 = it },
                                singleLine = true,
                                modifier = Modifier.width(56.dp),
                                colors = tfColors
                            )
                            OutlinedTextField(
                                value = d2,
                                onValueChange = { if (it.length <= 1) d2 = it },
                                singleLine = true,
                                modifier = Modifier.width(56.dp),
                                colors = tfColors
                            )
                            OutlinedTextField(
                                value = d3,
                                onValueChange = { if (it.length <= 1) d3 = it },
                                singleLine = true,
                                modifier = Modifier.width(56.dp),
                                colors = tfColors
                            )
                            OutlinedTextField(
                                value = d4,
                                onValueChange = { if (it.length <= 1) d4 = it },
                                singleLine = true,
                                modifier = Modifier.width(56.dp),
                                colors = tfColors
                            )
                        }

                        Spacer(Modifier.height(12.dp))
                        Button(
                            onClick = { onVerify(d1 + d2 + d3 + d4) },
                            modifier = Modifier.fillMaxWidth().height(40.dp),
                            colors = ButtonDefaults.buttonColors(containerColor = ButtonRed),
                            shape = RoundedCornerShape(8.dp)
                        ) {
                            Text("VERIFICAR", color = Color.White, fontWeight = FontWeight.Medium)
                        }
                    }
                }
            }
        }
    }
}

/**
 * Pantalla: Nueva contraseña (solo front). Mismo layout con dos campos y botón confirmar.
 */
@Composable
fun NuevaContrasenaScreen(
    headerImageRes: Int = R.drawable.pizzorra,
    onBack: () -> Unit = {},
    onConfirm: (newPass: String, repeatPass: String) -> Unit = { _, _ -> }
) {
    val Navy = Color(0xFF1D3557)
    val PanelBg = Color(0xFFF2F2F2)
    val ButtonRed = Color(0xFFE53935)

    var pass1 by remember { mutableStateOf("") }
    var pass2 by remember { mutableStateOf("") }

    val tfColors = OutlinedTextFieldDefaults.colors(
        focusedBorderColor = Color(0xFFBDBDBD),
        unfocusedBorderColor = Color(0xFFBDBDBD),
        disabledBorderColor = Color(0xFFE0E0E0),
        focusedContainerColor = Color.White,
        unfocusedContainerColor = Color.White,
        disabledContainerColor = Color.White,
        focusedTextColor = Color.Black,
        unfocusedTextColor = Color.Black,
        cursorColor = Color.Black
    )

    Box(modifier = Modifier.fillMaxSize().background(Navy)) {
        Column(modifier = Modifier.fillMaxSize()) {
            // Imagen superior
            Box(modifier = Modifier.fillMaxWidth().height(180.dp)) {
                Image(
                    painter = painterResource(id = headerImageRes),
                    contentDescription = "Header",
                    modifier = Modifier.fillMaxSize(),
                    contentScale = ContentScale.Crop
                )
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
            Box(
                modifier = Modifier
                    .fillMaxWidth()
                    .height(90.dp)
                    .background(Navy, shape = RoundedCornerShape(topStart = 96.dp, topEnd = 96.dp))
            )

            Column(modifier = Modifier.fillMaxSize(), horizontalAlignment = Alignment.CenterHorizontally) {
                Card(
                    modifier = Modifier
                        .fillMaxWidth()
                        .padding(horizontal = 16.dp)
                        .offset(y = (-60).dp),
                    colors = CardDefaults.cardColors(containerColor = PanelBg),
                    elevation = CardDefaults.cardElevation(defaultElevation = 12.dp),
                    shape = RoundedCornerShape(12.dp)
                ) {
                    Column(modifier = Modifier.padding(16.dp)) {
                        Text(
                            text = "🔑  Nueva\ncontraseña",
                            color = Color.Black,
                            fontSize = 18.sp,
                            fontWeight = FontWeight.ExtraBold,
                            lineHeight = 20.sp
                        )
                        Spacer(Modifier.height(8.dp))
                        Text(
                            text = "Ingresa tu nueva contraseña y confírmala para completar el proceso de restablecimiento.",
                            color = Color(0xFF475569),
                            fontSize = 13.sp
                        )

                        Spacer(Modifier.height(12.dp))
                        Text(text = "NUEVA CONTRASEÑA", color = Color.Black, fontSize = 12.sp)
                        OutlinedTextField(
                            value = pass1,
                            onValueChange = { pass1 = it },
                            singleLine = true,
                            modifier = Modifier.fillMaxWidth(),
                            shape = RoundedCornerShape(8.dp),
                            colors = tfColors,
                            visualTransformation = PasswordVisualTransformation()
                        )

                        Spacer(Modifier.height(10.dp))
                        Text(text = "REPETIR CONTRASEÑA", color = Color.Black, fontSize = 12.sp)
                        OutlinedTextField(
                            value = pass2,
                            onValueChange = { pass2 = it },
                            singleLine = true,
                            modifier = Modifier.fillMaxWidth(),
                            shape = RoundedCornerShape(8.dp),
                            colors = tfColors,
                            visualTransformation = PasswordVisualTransformation()
                        )

                        Spacer(Modifier.height(12.dp))
                        Button(
                            onClick = { onConfirm(pass1, pass2) },
                            modifier = Modifier.fillMaxWidth().height(40.dp),
                            colors = ButtonDefaults.buttonColors(containerColor = ButtonRed),
                            shape = RoundedCornerShape(8.dp)
                        ) {
                            Text("CONFIRMAR", color = Color.White, fontWeight = FontWeight.Medium)
                        }
                    }
                }
            }
        }
    }
}
