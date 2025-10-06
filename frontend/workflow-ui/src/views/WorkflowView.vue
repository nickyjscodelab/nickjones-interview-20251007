<template>
  <div class="workflow-container">
    <!-- Header -->
    <header class="header">
      <div class="header-content">
        <h1 class="header-title">Workflow Management</h1>
        <div class="header-actions">
          <span class="user-info">
            {{ currentUser?.name }} ({{ currentUser?.role }})
          </span>
          <button @click="logout" class="logout-button">
            Logout
          </button>
        </div>
      </div>
    </header>

    <!-- Main Content -->
    <main class="main-content">
      <!-- Action Bar -->
      <div class="action-bar">
        <div class="filters">
          <select v-model="statusFilter" class="filter-select">
            <option value="">All Status</option>
            <option v-for="status in statuses" :key="status" :value="status">
              {{ status }}
            </option>
          </select>
          <input
            v-model="searchQuery"
            type="text"
            placeholder="Search requests..."
            class="search-input"
          />
        </div>
        <button @click="openCreateModal" class="create-button">
          + New Request
        </button>
      </div>

      <!-- Project Requests Grid -->
      <div class="requests-grid">
        <div
          v-for="request in filteredRequests"
          :key="request.id"
          class="request-card"
          @click="selectRequest(request)"
        >
          <div class="request-header">
            <h3 class="request-title">{{ request.title }}</h3>
            <span class="request-status" :class="getStatusClass(request.status)">
              {{ request.status }}
            </span>
          </div>
          <p class="request-description">{{ request.description }}</p>
          <div class="request-meta">
            <span class="request-priority" :class="getPriorityClass(request.priority)">
              {{ request.priority }}
            </span>
            <span class="request-date">
              {{ formatDate(request.createdUtc) }}
            </span>
          </div>
          <div class="signoffs-summary">
            <span class="signoffs-count">
              {{ getSignOffSummary(request.signOffs) }}
            </span>
          </div>
        </div>
      </div>
    </main>

    <!-- Create/Edit Modal -->
    <div v-if="showCreateModal" class="modal-overlay" @click="closeCreateModal">
      <div class="modal-content" @click.stop>
        <CreateEditRequest
          :request="selectedRequest"
          @save="handleSave"
          @cancel="closeCreateModal"
        />
      </div>
    </div>

    <!-- Request Detail Modal -->
    <div v-if="showDetailModal" class="modal-overlay" @click="closeDetailModal">
      <div class="modal-content" @click.stop>
        <RequestDetail
          :request="selectedRequest!"
          :current-user="currentUser!"
          @sign-off="handleSignOff"
          @close="closeDetailModal"
          @refresh="loadRequests"
        />
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useAuth } from '@/composables/useAuth'
import { projectRequestApi } from '@/services/api'
import type { ProjectRequest, RequestStatus, CreateSignOff } from '@/types'
import { RequestStatus as RequestStatusEnum } from '@/types'
import CreateEditRequest from '@/components/CreateEditRequest.vue'
import RequestDetail from '@/components/RequestDetail.vue'

const router = useRouter()
const { currentUser, logout: authLogout } = useAuth()

const requests = ref<ProjectRequest[]>([])
const statusFilter = ref<RequestStatus | ''>('')
const searchQuery = ref('')
const showCreateModal = ref(false)
const showDetailModal = ref(false)
const selectedRequest = ref<ProjectRequest | null>(null)
const loading = ref(false)

const statuses = Object.values(RequestStatusEnum)

const filteredRequests = computed(() => {
  let filtered = requests.value

  if (statusFilter.value) {
    filtered = filtered.filter(r => r.status === statusFilter.value)
  }

  if (searchQuery.value) {
    const query = searchQuery.value.toLowerCase()
    filtered = filtered.filter(r =>
      r.title.toLowerCase().includes(query) ||
      r.description.toLowerCase().includes(query) ||
      r.requestedBy.toLowerCase().includes(query)
    )
  }

  return filtered
})

const loadRequests = async () => {
  try {
    loading.value = true
    requests.value = await projectRequestApi.getAll()
  } catch (error) {
    console.error('Failed to load requests:', error)
  } finally {
    loading.value = false
  }
}

const logout = () => {
  authLogout()
  router.push('/')
}

const openCreateModal = () => {
  selectedRequest.value = null
  showCreateModal.value = true
}

const closeCreateModal = () => {
  showCreateModal.value = false
  selectedRequest.value = null
}

const selectRequest = (request: ProjectRequest) => {
  selectedRequest.value = request
  showDetailModal.value = true
}

const closeDetailModal = () => {
  showDetailModal.value = false
  selectedRequest.value = null
}

const handleSave = async () => {
  closeCreateModal()
  await loadRequests()
}

const handleSignOff = async (signOff: CreateSignOff) => {
  if (selectedRequest.value) {
    try {
      await projectRequestApi.addSignOff(selectedRequest.value.id, signOff)
      await loadRequests()
      // Refresh the selected request
      selectedRequest.value = await projectRequestApi.getById(selectedRequest.value.id)
    } catch (error) {
      console.error('Failed to add sign-off:', error)
    }
  }
}

const getStatusClass = (status: RequestStatus) => {
  const classes: Record<RequestStatus, string> = {
    Draft: 'status-draft',
    Submitted: 'status-submitted',
    Approved: 'status-approved',
    Rejected: 'status-rejected'
  }
  return classes[status]
}

const getPriorityClass = (priority: string) => {
  const classes: Record<string, string> = {
    Low: 'priority-low',
    Medium: 'priority-medium',
    High: 'priority-high',
    Critical: 'priority-critical'
  }
  return classes[priority] || ''
}

const formatDate = (dateString: string) => {
  return new Date(dateString).toLocaleDateString()
}

const getSignOffSummary = (signOffs: any[]) => {
  const approved = signOffs.filter(s => s.decision === 'Approved').length
  const total = signOffs.length
  return `${approved}/${total} approved`
}

onMounted(() => {
  loadRequests()
})
</script>

<style scoped>
.workflow-container {
  min-height: 100vh;
  background-color: #f9fafb;
}

.header {
  background: white;
  border-bottom: 1px solid #e5e7eb;
  position: sticky;
  top: 0;
  z-index: 10;
}

.header-content {
  max-width: 1200px;
  margin: 0 auto;
  padding: 1rem 2rem;
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.header-title {
  font-size: 1.5rem;
  font-weight: 600;
  color: #1f2937;
  margin: 0;
}

.header-actions {
  display: flex;
  align-items: center;
  gap: 1rem;
}

.user-info {
  color: #6b7280;
  font-weight: 500;
}

.logout-button {
  padding: 0.5rem 1rem;
  background: #ef4444;
  color: white;
  border: none;
  border-radius: 6px;
  cursor: pointer;
  font-weight: 500;
  transition: background-color 0.2s;
}

.logout-button:hover {
  background: #dc2626;
}

.main-content {
  max-width: 1200px;
  margin: 0 auto;
  padding: 2rem;
}

.action-bar {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 2rem;
  gap: 1rem;
}

.filters {
  display: flex;
  gap: 1rem;
}

.filter-select,
.search-input {
  padding: 0.5rem;
  border: 1px solid #d1d5db;
  border-radius: 6px;
  font-size: 0.875rem;
}

.search-input {
  min-width: 200px;
}

.create-button {
  padding: 0.5rem 1rem;
  background: #3b82f6;
  color: white;
  border: none;
  border-radius: 6px;
  cursor: pointer;
  font-weight: 500;
  transition: background-color 0.2s;
}

.create-button:hover {
  background: #2563eb;
}

.requests-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(350px, 1fr));
  gap: 1.5rem;
}

.request-card {
  background: white;
  border-radius: 8px;
  padding: 1.5rem;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.1);
  cursor: pointer;
  transition: all 0.2s;
}

.request-card:hover {
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
}

.request-header {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  margin-bottom: 0.5rem;
}

.request-title {
  font-size: 1.125rem;
  font-weight: 600;
  color: #1f2937;
  margin: 0;
  flex: 1;
}

.request-status {
  padding: 0.25rem 0.5rem;
  border-radius: 4px;
  font-size: 0.75rem;
  font-weight: 500;
  text-transform: uppercase;
}

.status-draft { background: #f3f4f6; color: #6b7280; }
.status-submitted { background: #dbeafe; color: #1d4ed8; }
.status-approved { background: #dcfce7; color: #166534; }
.status-rejected { background: #fee2e2; color: #dc2626; }

.request-description {
  color: #6b7280;
  margin-bottom: 1rem;
  line-height: 1.5;
}

.request-meta {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 0.5rem;
}

.request-priority {
  padding: 0.25rem 0.5rem;
  border-radius: 4px;
  font-size: 0.75rem;
  font-weight: 500;
}

.priority-low { background: #f3f4f6; color: #6b7280; }
.priority-medium { background: #fef3c7; color: #92400e; }
.priority-high { background: #fed7aa; color: #c2410c; }
.priority-critical { background: #fee2e2; color: #dc2626; }

.request-date {
  color: #9ca3af;
  font-size: 0.875rem;
}

.signoffs-summary {
  text-align: right;
}

.signoffs-count {
  color: #6b7280;
  font-size: 0.875rem;
}

.modal-overlay {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: rgba(0, 0, 0, 0.5);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 50;
  padding: 1rem;
}

.modal-content {
  background: white;
  border-radius: 12px;
  min-width: 500px;
  max-width: 90vw;
  max-height: 90vh;
  overflow: auto;
  width: auto;
}

@media (max-width: 640px) {
  .modal-content {
    min-width: auto;
    width: 95vw;
    margin: 1rem;
  }
}
</style>