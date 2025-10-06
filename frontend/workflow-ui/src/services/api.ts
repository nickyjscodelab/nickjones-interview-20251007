import axios from 'axios'
import type { ProjectRequest, CreateProjectRequest, CreateSignOff, RequestStatus } from '@/types'

const API_BASE_URL = 'http://localhost:5100'

const api = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
  timeout: 10000,
})

// Add request interceptor for debugging
api.interceptors.request.use(request => {
  console.log('Making request to:', request.url)
  return request
})

// Add response interceptor for error handling
api.interceptors.response.use(
  response => response,
  error => {
    console.error('API Error:', error.response?.data || error.message)
    return Promise.reject(error)
  }
)

export const projectRequestApi = {
  // Get all project requests with optional filters
  getAll: async (status?: string, q?: string): Promise<ProjectRequest[]> => {
    const params = new URLSearchParams()
    if (status) params.append('status', status)
    if (q) params.append('q', q)
    
    const response = await api.get(`/api/requests?${params.toString()}`)
    return response.data
  },

  // Get a specific project request by ID
  getById: async (id: number): Promise<ProjectRequest> => {
    const response = await api.get(`/api/requests/${id}`)
    return response.data
  },

  // Create a new project request
  create: async (request: CreateProjectRequest): Promise<ProjectRequest> => {
    const response = await api.post('/api/requests', request)
    return response.data
  },

  // Update project request status
  updateStatus: async (id: number, status: RequestStatus): Promise<void> => {
    await api.put(`/api/requests/${id}/status`, { status })
  },

  // Add a sign-off to a project request
  addSignOff: async (id: number, signOff: CreateSignOff): Promise<void> => {
    await api.post(`/api/requests/${id}/signoffs`, signOff)
  },
}

export default api