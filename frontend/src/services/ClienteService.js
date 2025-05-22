import axios from 'axios'

const apiClient = axios.create({
  baseURL: 'http://localhost:5042/api',
  headers: {
    'Content-Type': 'application/json',
    'Authorization': `Bearer ${localStorage.getItem('token')}` // Incluir el token en las cabeceras
  }
})

export default {
  getClientes(page = 1, pageSize = 10) {
    return apiClient.get(`/Usuarios/clientes?page=${page}&pageSize=${pageSize}`)
  }
}