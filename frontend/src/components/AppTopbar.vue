<template>
  <div class="app-topbar">
    <Toolbar class="toolbar">
      <template #start>
        <div class="topbar-left">
        </div>
      </template>

      <template #end>
        <div v-if="usuarioLogueado" class="usuario-info">
          <img
            v-if="usuarioLogueado.avatarUrl"
            :src="usuarioLogueado.avatarUrl"
            alt="Avatar"
            class="avatar"
          />
          <span class="nombre-usuario">{{ usuarioLogueado.email }}</span>
          <Button
            icon="pi pi-sign-out"
            severity="danger"
            rounded
            text
            @click="logout"
            class="logout-btn"
            v-tooltip="'Cerrar sesión'"
          />
        </div>
        <div v-else>
          <Button
            label="Iniciar sesión"
            icon="pi pi-user"
            @click="$router.push('/login')"
            class="login-btn"
          />
        </div>
      </template>
    </Toolbar>
  </div>
</template>

<script>
import Toolbar from "primevue/toolbar";
import Button from "primevue/button";
import authService from "../services/auth.service";
import "primeicons/primeicons.css";

export default {
  name: "AppTopbar",
  components: {
    Toolbar,
    Button,
  },
  data() {
    return {
      usuarioLogueado: null,
    };
  },
  mounted() {
    this.usuarioLogueado = authService.getUser();
  },
  methods: {
    logout() {
      authService.logout();
      this.usuarioLogueado = null;
      this.$router.push("/login");
    },
  },
};
</script>

<style scoped>
.app-topbar {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  z-index: 1000;
}

.toolbar {
  background-color: #18181b;
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 1rem 1rem;
}

/* Contenedor izquierdo con logo */
.topbar-left {
  display: flex;
  align-items: center;
  gap: 1rem;
}

/* Nombre de la app */
.app-name {
  margin: 0;
  font-weight: 600;
  color: #ffffff;
}

/* Info usuario */
.usuario-info {
  display: flex;
  align-items: center;
  gap: 0.75rem;
}

.nombre-usuario {
  font-weight: 500;
}

.avatar {
  width: 32px;
  height: 32px;
  border-radius: 50%;
  object-fit: cover;
}

.logout-btn {
  margin-left: 0.5rem;
}

.login-btn {
  font-weight: 500;
}
</style>
