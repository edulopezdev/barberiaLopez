<template>
  <div id="app">
    <!-- Mostrar Sidebar solo si está autenticado -->
    <AppSidebar v-if="isAuthenticated" class="sidebar-desktop" />

    <!-- Mostrar Topbar y pasar el Sidebar como slot si está autenticado -->
    <AppTopbar v-if="isAuthenticated">
      <template #mobile-sidebar>
        <!-- Sidebar visible en móviles al hacer clic en el botón -->
        <AppSidebar class="sidebar-mobile" />
      </template>
    </AppTopbar>

    <!-- Contenido principal -->
    <div :class="['content', { full: !isAuthenticated }]">
      <router-view />
    </div>
  </div>
</template>

<script>
import AppSidebar from './components/AppSidebar.vue'
import AppTopbar from './components/AppTopbar.vue'
import authService from './services/auth.service'

export default {
  components: {
    AppSidebar,
    AppTopbar
  },
  data() {
    return {
      isAuthenticated: false
    }
  },
  watch: {
    $route: {
      immediate: true,
      handler() {
        this.isAuthenticated = authService.isAuthenticated()
      }
    }
  }
}
</script>

<style scoped>
#app {
  display: flex;
  flex-direction: column; /* En móviles, columnas para que topbar quede arriba */
  min-height: 100vh;
}

.sidebar-desktop {
  display: none;
}

.sidebar-mobile {
  display: block;
}

/* Mostrar Sidebar solo en pantallas grandes */
@media (min-width: 1024px) {
  .sidebar-desktop {
    display: block;
  }
  .sidebar-mobile {
    display: none;
  }

  #app {
    flex-direction: row;
  }

  .content {
    margin-left: 250px;
    padding: 2rem;
    width: 100%;
    transition: margin-left 0.3s ease;
  }

  .content.full {
    margin-left: 0;
    padding: 0;
    width: 100%;
  }
}

@media (max-width: 1023px) {
  .content {
    margin-left: 0;
    padding: 1rem;
  }
}
</style>
