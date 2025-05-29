import axios from "axios";

const apiClient = axios.create({
  baseURL: "http://localhost:5042/api",
  headers: {
    "Content-Type": "application/json",
  },
});

apiClient.interceptors.request.use((config) => {
  const token = sessionStorage.getItem("token");
  if (token) {
    config.headers["Authorization"] = `Bearer ${token}`;
  }
  return config;
});

export default {
  // === FUNCIONES GENERALES ===
  getUsuarios(page = 1, pageSize = 10, filtros = {}) {
    const params = new URLSearchParams({
      page,
      pageSize,
      ...filtros,
    });
    return apiClient.get(`/usuarios?${params.toString()}`);
  },

  getUsuario(id) {
    return apiClient.get(`/usuarios/${id}`);
  },

  crearUsuario(usuarioData) {
    const data = {
      nombre: usuarioData.nombre,
      email: usuarioData.email,
      telefono: usuarioData.telefono,
      rolId: usuarioData.rolId,
      accedeAlSistema: usuarioData.accedeAlSistema ?? false,
    };

    if (usuarioData.accedeAlSistema && usuarioData.password) {
      data.password = usuarioData.password;
    }

    return apiClient.post("/usuarios", data);
  },
  actualizarUsuario(id, usuarioData) {
    const data = {
      nombre: usuarioData.nombre,
      email: usuarioData.email,
      telefono: usuarioData.telefono,
      rolId: usuarioData.rolId,
      accedeAlSistema: usuarioData.accedeAlSistema ?? false,
    };

    return apiClient.put(`/usuarios/${id}`, data);
  },

  eliminarUsuario(id) {
    return apiClient.delete(`/usuarios/${id}`);
  },

  cambiarEstado(id, activo) {
    return apiClient.patch(`/usuarios/${id}/estado`, { activo });
  },

  // === FUNCIONES PARA CLIENTES (rolId = 3) ===
  getClientes(page = 1, pageSize = 10, filtros = {}) {
    const params = new URLSearchParams({
      page,
      pageSize,
      ...filtros,
    });

    return apiClient.get(`/usuarios/clientes?${params.toString()}`);
  },

  getCliente(id) {
    return apiClient.get(`/usuarios/${id}`);
  },

  crearCliente(clienteData) {
    const data = {
      nombre: clienteData.nombre,
      email: clienteData.email,
      telefono: clienteData.telefono,
      rolId: 3,
      accedeAlSistema: false,
    };

    return apiClient.post("/usuarios", data);
  },

  actualizarCliente(id, clienteData) {
    const dataParaEnviar = {
      nombre: clienteData.nombre,
      email: clienteData.email,
      telefono: clienteData.telefono,
      rolId: 3,
      accedeAlSistema: false,
      activo: clienteData.activo,
    };
    console.log("Datos para actualizar al backend:", dataParaEnviar);
    return apiClient.put(`/usuarios/${id}`, dataParaEnviar);
  },
};
