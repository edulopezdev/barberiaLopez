import { createRouter, createWebHistory } from 'vue-router'
import HomeView from '../views/HomeView.vue'
import ClientesView from '../views/ClientesView.vue'
import LoginView from '../views/LoginView.vue'
import DashboardView from '../views/DashboardView.vue'
import authService from '../services/auth.service'

const routes = [
  { path: '/', redirect: '/login' }, // 👈 Redirige la raíz al login
  {
    path: '/login',
    name: 'Login',
    component: LoginView
  },
  {
    path: '/clientes',
    name: 'Clientes',
    component: ClientesView,
    meta: { requiresAuth: true }
  },
  {
    path: '/dashboard',
    name: 'Dashboard',
    component: DashboardView,
    meta: { requiresAuth: true }
  },
  {
    path: '/home',
    name: 'Home',
    component: HomeView,
    meta: { requiresAuth: true }
  },
  {
    path: '/:catchAll(.*)', // Redirige cualquier ruta no existente al login
    redirect: '/login'
  }
]

// Protección de rutas: redirigir a Login si no está autenticado
const router = createRouter({
  history: createWebHistory(),
  routes
})

router.beforeEach((to, from, next) => {
  const requiresAuth = to.matched.some(record => record.meta.requiresAuth)

  if (requiresAuth && !authService.isAuthenticated()) {
    next('/login')  // Redirigir a login si no está autenticado
  } else {
    next()  // Continuar con la navegación
  }
})

export default router
