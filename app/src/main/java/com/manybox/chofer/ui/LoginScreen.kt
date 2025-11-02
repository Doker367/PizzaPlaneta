package com.manybox.chofer.ui

import androidx.compose.foundation.Image
import androidx.compose.foundation.background
import androidx.compose.foundation.clickable
import androidx.compose.foundation.layout.*
import androidx.compose.foundation.shape.RoundedCornerShape
import androidx.compose.foundation.shape.CircleShape
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.filled.ArrowBack
import androidx.compose.material.icons.filled.Visibility
import androidx.compose.material.icons.filled.VisibilityOff
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
import androidx.compose.ui.text.input.VisualTransformation
import androidx.compose.ui.text.style.TextDecoration
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.sp


@Composable
fun LoginScreen(
    onLogin: (String, String) -> Unit,
    isLoading: Boolean,
    errorMessage: String?,
    onBack: (() -> Unit)? = null,
    onRegisterClick: (() -> Unit)? = null,
    headerImageRes: Int? = null
) {
    val Navy = Color(0xFF1D3557)
    val PanelBg = Color(0xFFF2F2F2)
    val ButtonRed = Color(0xFFE53935)
    val ButtonBlue = Color(0xFF1D3557)

    var username by remember { mutableStateOf("") }
    var password by remember { mutableStateOf("") }
    var passwordVisible by remember { mutableStateOf(false) }

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

    Box(
        modifier = Modifier
            .fillMaxSize()
            .background(Navy)
    ) {
        Column(modifier = Modifier.fillMaxSize()) {
            if (headerImageRes != null) {
                Box(modifier = Modifier
                    .fillMaxWidth()
                    .height(180.dp)) {
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
                        .height(90.dp)
                        .background(Navy, shape = RoundedCornerShape(topStart = 96.dp, topEnd = 96.dp))
                )
            }

            Box(modifier = Modifier.fillMaxSize()) {
                Card(
                    modifier = Modifier
                        .fillMaxWidth()
                        .padding(horizontal = 16.dp)
                        .align(Alignment.TopCenter)
                        .offset(y = if (headerImageRes != null) (-60).dp else 0.dp),
                    colors = CardDefaults.cardColors(containerColor = PanelBg),
                    elevation = CardDefaults.cardElevation(defaultElevation = 12.dp),
                    shape = RoundedCornerShape(12.dp)
                ) {
                    Column(modifier = Modifier.padding(16.dp)) {
                        if (headerImageRes == null) {
                            Row(verticalAlignment = Alignment.CenterVertically) {
                                if (onBack != null) {
                                    IconButton(onClick = { onBack() }) {
                                        Icon(Icons.Default.ArrowBack, contentDescription = "Volver", tint = Color.Black)
                                    }
                                    Spacer(Modifier.width(4.dp))
                                }
                                Text(
                                    text = "INICIAR SESIÓN",
                                    color = Color.Black,
                                    fontSize = 20.sp,
                                    fontWeight = FontWeight.Bold
                                )
                            }
                            Spacer(Modifier.height(8.dp))
                        } else {
                            Text(
                                text = "LOGIN",
                                color = Color.Black,
                                fontSize = 20.sp,
                                fontWeight = FontWeight.Bold,
                                letterSpacing = 1.sp,
                                modifier = Modifier.align(Alignment.CenterHorizontally)
                            )
                            Spacer(Modifier.height(8.dp))
                        }
                        Text(text = "CORREO", color = Color.Black, fontSize = 12.sp)
                        OutlinedTextField(
                            value = username,
                            onValueChange = { username = it },
                            singleLine = true,
                            modifier = Modifier.fillMaxWidth(),
                            shape = RoundedCornerShape(8.dp),
                            colors = tfColors
                        )

                        Spacer(Modifier.height(10.dp))
                        Text(text = "CONTRASEÑA", color = Color.Black, fontSize = 12.sp)
                        OutlinedTextField(
                            value = password,
                            onValueChange = { password = it },
                            singleLine = true,
                            modifier = Modifier.fillMaxWidth(),
                            visualTransformation = if (passwordVisible) VisualTransformation.None else PasswordVisualTransformation(),
                            shape = RoundedCornerShape(8.dp),
                            trailingIcon = {
                                val icon = if (passwordVisible) Icons.Default.VisibilityOff else Icons.Default.Visibility
                                IconButton(onClick = { passwordVisible = !passwordVisible }) {
                                    Icon(imageVector = icon, contentDescription = if (passwordVisible) "Ocultar" else "Mostrar", tint = ButtonRed)
                                }
                            },
                            colors = tfColors
                        )

                        Spacer(Modifier.height(4.dp))
                        Row(modifier = Modifier.fillMaxWidth(), horizontalArrangement = Arrangement.End) {
                            Text(
                                text = "¿Olvidaste tu contraseña?",
                                color = Color(0xFF8E8E93),
                                fontSize = 12.sp,
                                textDecoration = TextDecoration.Underline,
                                modifier = Modifier.clickable { /* TODO: Forgot password action */ }
                            )
                        }

                        Spacer(Modifier.height(14.dp))
                        Button(
                            onClick = { onLogin(username, password) },
                            enabled = !isLoading,
                            modifier = Modifier
                                .fillMaxWidth()
                                .height(40.dp),
                            colors = ButtonDefaults.buttonColors(containerColor = ButtonBlue),
                            shape = RoundedCornerShape(8.dp)
                        ) {
                            Text("INICIAR SESIÓN", color = Color.White, fontWeight = FontWeight.Medium)
                        }

                        Spacer(Modifier.height(8.dp))
                        Button(
                            onClick = { onRegisterClick?.invoke() },
                            modifier = Modifier
                                .fillMaxWidth()
                                .height(40.dp),
                            colors = ButtonDefaults.buttonColors(containerColor = ButtonRed),
                            shape = RoundedCornerShape(8.dp)
                        ) {
                            Text("CREAR CUENTA", color = Color.White, fontWeight = FontWeight.Medium)
                        }

                        Spacer(Modifier.height(8.dp))
                        OutlinedButton(
                            onClick = { /* Google Sign-In */ },
                            modifier = Modifier
                                .fillMaxWidth()
                                .height(36.dp),
                            shape = RoundedCornerShape(8.dp),
                            colors = ButtonDefaults.outlinedButtonColors(containerColor = Color.White)
                        ) {
                            // Puedes agregar un ícono de Google aquí si tienes el recurso
                            Text("Continuar con Google", color = Color.Black)
                        }

                        if (isLoading) {
                            Spacer(modifier = Modifier.height(16.dp))
                            CircularProgressIndicator(color = ButtonRed)
                        }
                        if (!errorMessage.isNullOrEmpty()) {
                            Spacer(modifier = Modifier.height(16.dp))
                            Text(errorMessage, color = Color.Red, fontWeight = FontWeight.Bold)
                        }
                    }
                }
            }
        }
    }
}
