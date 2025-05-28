import { createRouter, createWebHistory } from "vue-router";
import HomeView from "../views/HomeView.vue";

import ClientesView from "../views/ClientesView.vue";
import CrearCliente from "@/views/CrearCliente.vue";
import EditarCliente from "@/views/EditarCliente.vue";

import LoginView from "../views/LoginView.vue";
import DashboardView from "../views/DashboardView.vue";
import authService from "../services/auth.service";

const routes = [
  { path: "/", redirect: "/login" },
  {
    path: "/login",
    name: "Login",
    component: LoginView,
  },
  {
    path: "/clientes",
    name: "Clientes",
    component: ClientesView,
    meta: { requiresAuth: true },
  },
  {
    path: "/clientes/nuevo",
    name: "CrearCliente",
    component: CrearCliente,
    meta: { requiresAuth: true },
  },
  {
    path: "/clientes/editar/:id",
    name: "EditarCliente",
    component: EditarCliente,
    meta: { requiresAuth: true },
  },
  {
    path: "/dashboard",
    name: "Dashboard",
    component: DashboardView,
    meta: { requiresAuth: true },
  },
  {
    path: "/home",
    name: "Home",
    component: HomeView,
    meta: { requiresAuth: true },
  },
  {
    path: "/:catchAll(.*)", // Redirige cualquier ruta no existente al login
    redirect: "/login",
  },
];

// Protección de rutas: redirigir a Login si no está autenticado
const router = createRouter({
  history: createWebHistory(),
  routes,
});

router.beforeEach((to, from, next) => {
  const requiresAuth = to.matched.some((record) => record.meta.requiresAuth);
  const isLoggedIn = authService.isAuthenticated();

  if (requiresAuth && !isLoggedIn) {
    next("/login");
  } else if (to.path === "/login" && isLoggedIn) {
    next("/dashboard"); // Si estamos en el login y estamos autenticados, redirigimos al dashboard
  } else {
    next();
  }
});

export default router;
