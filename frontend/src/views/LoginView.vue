<template>
  <div class="login-container">
    <Card>
      <template #title> Iniciar Sesión </template>
      <template #content>
        <div class="p-fluid">
          <div class="p-field">
            <label for="email">Email</label>
            <InputText
              id="email"
              v-model="usuario.email"
              type="email"
              placeholder="tu@correo.com"
            />
          </div>
          <div class="p-field">
            <label for="password">Contraseña</label>
            <Password
              id="password"
              v-model="usuario.password"
              placeholder="*********"
              toggleMask
              :feedback="false"
            />
          </div>
          <Button
            label="Entrar"
            icon="pi pi-sign-in"
            @click="iniciarSesion"
            block
            severity="success"
          />
        </div>
      </template>
    </Card>
  </div>
</template>

<script>
import axios from "axios";

export default {
  data() {
    return {
      usuario: {
        email: "",
        password: "",
      },
    };
  },
  methods: {
    iniciarSesion() {
      if (!this.usuario.email || !this.usuario.password) {
        alert("Por favor ingresa email y contraseña");
        return;
      }

      const url = "http://localhost:5042/api/auth/login";

      axios
        .post(url, this.usuario)
        .then((res) => {
          // Guardar el token y los datos del usuario en localStorage
          localStorage.setItem("token", res.data.token);
          localStorage.setItem("user", JSON.stringify(res.data.usuario));

          // Redirigir a la ruta deseada (en este caso, /dashboard)
          this.$router.push("/dashboard"); // Redirigir después del login
        })
        .catch((err) => {
          console.error("Error al iniciar sesión:", err.response?.data || err);
          alert("Usuario o contraseña incorrectos");
        });
    },
  },
};
</script>

<style scoped>
.login-container {
  max-width: 450px;
  margin: 5rem auto;
  padding: 2rem;
  border-radius: 8px;
  background-color: rgba(255, 255, 255, 0.95);
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
  position: relative;
  z-index: 1;
}

.card {
  border: none;
}

.p-fluid .p-field {
  margin-bottom: 1.5rem;
}

.p-field label {
  font-weight: bold;
  color: #333;
}

.p-inputtext,
.p-password input {
  border-radius: 5px;
  padding: 0.8rem;
  background-color: #f5f5f5;
  border: 1px solid #ccc;
}

.p-inputtext:focus,
.p-password input:focus {
  border-color: #4caf50;
  box-shadow: 0 0 5px rgba(0, 128, 0, 0.4);
}

.p-button {
  border-radius: 5px;
  font-size: 1rem;
}

.p-button:focus {
  outline: none;
}

.p-button-success {
  background-color: #4caf50;
  border: 1px solid #4caf50;
}

.p-button-success:hover {
  background-color: #45a049;
}

.p-button-success:active {
  background-color: #388e3c;
}
</style>
