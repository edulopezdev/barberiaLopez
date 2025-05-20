const KEY_TOKEN = 'token'
const KEY_USER = 'user'

export default {
  login(email, password) {
    // Llamada a la API .NET
    const url = 'http://localhost:5042/api/auth/login'
    axios.post(url, { email, password })
      .then(res => {
        localStorage.setItem(KEY_TOKEN, res.data.token)
        localStorage.setItem(KEY_USER, JSON.stringify(res.data.usuario))
        window.location.href = '/dashboard'
      })
      .catch(err => {
        alert('Usuario o contraseña incorrectos')
      })
  },

  logout() {
    localStorage.removeItem(KEY_TOKEN)
    localStorage.removeItem(KEY_USER)
    window.location.href = '/login'
  },

  getToken() {
    return localStorage.getItem(KEY_TOKEN)
  },

  getUser() {
    const userStr = localStorage.getItem(KEY_USER)
    return userStr ? JSON.parse(userStr) : null
  },

  isAuthenticated() {
    return !!this.getToken()
  },

  isRole(role) {
    const user = this.getUser()
    return user && user.rol === role
  }
}