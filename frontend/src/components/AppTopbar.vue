<template>
  <div class="app-topbar">
    <Toolbar class="toolbar">
      <template #start>
        <div class="topbar-left"></div>
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
            class="logout-btn"
            rounded
            severity="danger"
            text
            @click="logout"
            v-tooltip="'Cerrar sesión'"
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
import { confirmDialog } from "../utils/confirmDialog"; // Importamos la función reusable
import Swal from "sweetalert2";

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
    async logout() {
      const result = await confirmDialog({
        title: "Cerrar sesión",
        message: "¿Estás seguro de que querés salir?",
      });

      if (result.isConfirmed) {
        authService.logout();
        this.usuarioLogueado = null;
        this.$router.push("/login");

        Swal.fire({
          title: "Sesión cerrada",
          text: "Has salido correctamente.",
          icon: "success",
          timer: 2000,
          showConfirmButton: false,
          background: "#18181b",
          color: "#fff",
        });
      }
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
  background-color: #18181b;
  box-shadow: 0 4px 16px rgba(0, 0, 0, 0.06);
  padding: 0.5rem 1rem;
}

.toolbar {
  background-color: transparent;
  padding: 0;
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.topbar-left {
  display: flex;
  align-items: center;
  gap: 1rem;
}

.usuario-info {
  display: flex;
  align-items: center;
  gap: 0.75rem;
}

.nombre-usuario {
  font-weight: 500;
  color: #fff;
}

.avatar {
  width: 32px;
  height: 32px;
  border-radius: 50%;
  object-fit: cover;
}

.logout-btn {
  width: 36px;
  height: 36px;
  padding: 0;
  display: flex;
  align-items: center;
  justify-content: center;
  border-radius: 50%;
  color: #ffffff;
  background-color: transparent;
  border: none;
  transition: background-color 0.3s ease;
}

.logout-btn:hover {
  background-color: #5d5d5d;
}
</style>
