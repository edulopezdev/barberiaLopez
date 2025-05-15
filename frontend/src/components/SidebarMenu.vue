<template>
  <nav :class="['sidebar', { collapsed: isCollapsed }]">
    <div class="sidebar-header">
      <button class="toggle-btn" @click="isCollapsed = !isCollapsed">☰</button>
      <h2 v-if="!isCollapsed" class="brand">Barbería López</h2>
    </div>

    <ul class="nav-list">
      <li v-for="item in navItems" :key="item.path">
        <router-link
          :to="item.path"
          class="nav-link"
          active-class="active-link"
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
  name: 'SidebarMenu',
  data() {
    return {
      isCollapsed: false,
      navItems: [
        { path: '/', label: 'Inicio', icon: ['fas', 'house'] },
        { path: '/turnos', label: 'Turnos', icon: ['fas', 'calendar'] },
        { path: '/login', label: 'Login', icon: ['fas', 'right-to-bracket'] },
      ],
    };
  },
};
</script>

<style scoped>
.sidebar {
  width: 240px;
  height: 100vh;
  position: fixed;
  top: 0;
  left: 0;
  background-color: #1f2937; /* gris oscuro */
  color: #fff;
  padding: 1.5rem 1rem;
  box-shadow: 2px 0 5px rgba(0, 0, 0, 0.2);
  transition: width 0.3s ease;
  overflow-x: hidden;
  z-index: 1000;
}

.collapsed {
  width: 70px;
}

.sidebar-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  margin-bottom: 2rem;
}

.toggle-btn {
  background: none;
  border: none;
  color: white;
  font-size: 1.5rem;
  cursor: pointer;
  padding: 0;
}

.brand {
  font-size: 1.3rem;
  font-weight: bold;
  margin-left: 10px;
}

.nav-list {
  list-style: none;
  padding: 0;
  margin: 0;
}

.nav-link {
  display: flex;
  align-items: center;
  gap: 10px;
  padding: 0.75rem 1rem;
  color: #cbd5e1; /* slate-300 */
  text-decoration: none;
  border-radius: 4px;
  transition: background 0.2s ease, color 0.2s ease;
}

.nav-link:hover {
  background-color: #374151; /* slate-700 */
  color: #fff;
}

.active-link {
  background-color: #4b5563; /* slate-600 */
  color: #fff;
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
