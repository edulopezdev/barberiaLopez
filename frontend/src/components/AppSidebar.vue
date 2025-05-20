<template>
  <div class="sidebar">
    <!-- Nombre o logo de la app -->
    <img
      src="../assets/images/logo.png"
      alt="Logo Barbería López"
      class="app-logo"
    />
    <ul class="menu-list">
      <li v-for="(item, i) in menuItems" :key="i">
        <router-link :to="item.route" class="menu-item" active-class="active">
          <i :class="['menu-icon', item.icon]"></i>
          <span class="menu-label">{{ item.label }}</span>
        </router-link>
      </li>
    </ul>
  </div>
</template>

<script>
export default {
  name: "AppSidebar",
  data() {
    return {
      menuItems: [
        { label: "Inicio", route: "/", icon: "pi pi-home" },
        { label: "Clientes", route: "/clientes", icon: "pi pi-users" },
        { label: "Turnos", route: "/turnos", icon: "pi pi-calendar" },
        { label: "Productos", route: "/productos", icon: "pi pi-box" },
        { label: "Servicios", route: "/servicios", icon: "pi pi-wrench" },
        { label: "Caja", route: "/caja", icon: "pi pi-credit-card" },
        { label: "Usuarios", route: "/usuarios", icon: "pi pi-user" },
        { label: "Perfil", route: "/perfil", icon: "pi pi-user-edit" },
      ],
    };
  },
};
</script>

<style>
:root {
  --sidebar-margin: 1rem;
  --topbar-height: 4rem;
  --color-active-bg: #28a745;
  --color-active-text: #ffffff;
}
</style>

<style scoped>
/* Sidebar principal */
.sidebar {
  position: fixed;
  top: calc(var(--topbar-height) + var(--sidebar-margin));
  left: var(--sidebar-margin);
  z-index: 100;
  width: 250px;
  height: calc(100vh - var(--topbar-height) - calc(var(--sidebar-margin) * 2));
  background-color: #18181b;
  border-radius: 1rem;
  box-shadow: 0 4px 16px rgba(0, 0, 0, 0.06);
  padding: 1rem 1rem 1rem 1rem;
  overflow-y: auto;
  transition: box-shadow 0.3s ease;
}

/* Título */
.sidebar-title {
  text-align: center;
  font-weight: bold;
  font-size: 1.2rem;
  color: #ffffff;
  margin-bottom: 1.25rem;
}

/* Menú */
.menu-list {
  list-style: none;
  padding: 0;
  margin: 0;
}

.menu-item {
  display: flex;
  align-items: center;
  padding: 0.75rem 1.5rem;
  color: #ffffff;
  text-decoration: none;
  border-radius: 0.5rem;
  transition: all 0.3s ease;
}

.menu-item:hover {
  background-color: #383737;
  color: #ffffff;
}

.menu-item.active {
  color: #28a745;
}

.menu-item.active .menu-icon {
  color: #28a745;
}

.menu-icon {
  margin-right: 0.75rem;
  font-size: 1.2rem;
  color: #ffffff;
  transition: color 0.3s ease;
}

.menu-label {
  font-size: 1rem;
  font-weight: 500;
}

.app-logo {
  height: 60px;
  object-fit: contain;
  position: relative;
  margin-bottom: 1.5rem;
}
</style>
