import { ref, computed } from 'vue'
import type { User, Role } from '@/types'

const currentUser = ref<User | null>(null)

export function useAuth() {
  const isLoggedIn = computed(() => currentUser.value !== null)
  
  const login = (name: string, role: Role) => {
    currentUser.value = { name, role }
    localStorage.setItem('currentUser', JSON.stringify(currentUser.value))
  }
  
  const logout = () => {
    currentUser.value = null
    localStorage.removeItem('currentUser')
  }
  
  const initAuth = () => {
    const stored = localStorage.getItem('currentUser')
    if (stored) {
      currentUser.value = JSON.parse(stored)
    }
  }
  
  return {
    currentUser: computed(() => currentUser.value),
    isLoggedIn,
    login,
    logout,
    initAuth
  }
}