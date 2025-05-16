<template>
  <nav :class="['sidebar', { collapsed: isCollapsed }]">
    <div class="sidebar-header">
      <button class="toggle-btn" @click="toggleSidebar">☰</button>
    </div>

    <div class="logo-container">
      <img
        v-if="!isCollapsed"
        :src="require('@/assets/logo.png')"
        alt="Logo"
        class="logo"
      />
    </div>

    <ul class="nav-list">
      <li v-for="item in navItems" :key="item.path">
        <router-link
          :to="item.path"
          class="nav-link"
          exact-active-class="active-link"
        >
          <font-awesome-icon :icon="item.icon" class="icon" />
          <span v-if="!isCollapsed" class="label">{{ item.label }}</span>
        </router-link>
      </li>
    </ul>
  </nav>
</template>

<script>
export default {
  name: "SidebarMenu",
  props: {
    collapsed: {
      type: Boolean,
      default: false,
    },
  },
  emits: ["toggle-collapse"],
  data() {
    return {
      isCollapsed: this.collapsed,
      navItems: [
        { path: "/", label: "Inicio", icon: ["fas", "house"] },
        { path: "/turnos", label: "Turnos", icon: ["fas", "calendar"] },
        { path: "/clientes", label: "Clientes", icon: ["fas", "user"] },
      ],
    };
  },
  watch: {
    collapsed(newVal) {
      this.isCollapsed = newVal;
    },
  },
  methods: {
    toggleSidebar() {
      this.isCollapsed = !this.isCollapsed;
      this.$emit("toggle-collapse", this.isCollapsed);
    },
  },
};
</script>

<style scoped>
.sidebar {
  width: 270px; /* Ancho total de la sidebar */
  height: 100vh;
  position: fixed;
  top: 0;
  left: 0;
  background-color: rgba(0, 27, 11, 0.914); /* Fondo transparente */
  color: #ffffff; /* Texto claro */
  padding: 1.5rem 1rem;
  transition: width 0.3s ease;
  overflow-x: hidden;
  z-index: 1000;
  display: flex;
  flex-direction: column;
  align-items: center;
}

.sidebar.collapsed {
  width: 85px; /* Ancho cuando está colapsada */
}

.sidebar-header {
  width: 100%;
  display: flex;
  justify-content: flex-end;
  margin-bottom: 1rem;
}

.toggle-btn {
  background: none;
  border: none;
  color: #f9fafb;
  font-size: 1.5rem;
  cursor: pointer;
  padding: 0;
}

.logo-container {
  display: flex;
  flex-direction: column;
  align-items: center;
  text-align: center;
  margin-bottom: 2rem;
}

.logo {
  height: 100px;
  transition: height 0.3s ease;
}

.collapsed-logo {
  height: 60px;
}

.nav-list {
  list-style: none;
  padding: 0;
  margin: 0;
  width: 100%;
}

.nav-link {
  display: flex;
  align-items: center;
  gap: 10px;
  padding: 0.75rem 1rem;
  color: #f9fafb; /* Texto claro */
  text-decoration: none;
  border-radius: 4px;
  transition: background 0.2s ease, color 0.2s ease;
}

.nav-link:hover {
  background-color: #374151;
  color: #f9fafb;
}

.active-link {
  background-color: #006400;
  color: white;
  font-weight: bold;
}

.icon {
  width: 20px;
  min-width: 20px;
  text-align: center;
  font-size: 1.1rem;
}

.label {
  white-space: nowrap;
}
</style>
