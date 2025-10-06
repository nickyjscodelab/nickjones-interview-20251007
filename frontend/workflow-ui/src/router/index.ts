import { createRouter, createWebHistory } from 'vue-router'
import { useAuth } from '@/composables/useAuth'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      name: 'login',
      component: () => import('../views/LoginView.vue'),
    },
    {
      path: '/workflow',
      name: 'workflow',
      component: () => import('../views/WorkflowView.vue'),
      meta: { requiresAuth: true }
    },
  ],
})

// Navigation guard for authentication
router.beforeEach((to, from, next) => {
  const { isLoggedIn, initAuth } = useAuth()
  
  // Initialize auth state from localStorage
  initAuth()
  
  if (to.meta.requiresAuth && !isLoggedIn.value) {
    next('/')
  } else if (to.name === 'login' && isLoggedIn.value) {
    next('/workflow')
  } else {
    next()
  }
})

export default router
