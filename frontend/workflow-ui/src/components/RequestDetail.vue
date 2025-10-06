<template>
  <div class="request-detail">
    <header class="detail-header">
      <div class="header-content">
        <h2 class="request-title">{{ request.title }}</h2>
        <div class="header-actions">
          <span class="request-status" :class="getStatusClass(request.status)">
            {{ request.status }}
          </span>
          <button @click="$emit('close')" class="close-button">
            ✕
          </button>
        </div>
      </div>
    </header>

    <div class="detail-content">
      <!-- Request Info -->
      <section class="section">
        <h3 class="section-title">Request Information</h3>
        <div class="info-grid">
          <div class="info-item">
            <span class="info-label">Description</span>
            <p class="info-value">{{ request.description }}</p>
          </div>
          <div class="info-item">
            <span class="info-label">Priority</span>
            <span class="priority-badge" :class="getPriorityClass(request.priority)">
              {{ request.priority }}
            </span>
          </div>
          <div class="info-item">
            <span class="info-label">Requested By</span>
            <span class="info-value">{{ request.requestedBy }}</span>
          </div>
          <div class="info-item">
            <span class="info-label">Created</span>
            <span class="info-value">{{ formatDate(request.createdUtc) }}</span>
          </div>
          <div v-if="request.dueUtc" class="info-item">
            <span class="info-label">Due Date</span>
            <span class="info-value">{{ formatDate(request.dueUtc) }}</span>
          </div>
        </div>
      </section>

      <!-- Sign-offs -->
      <section class="section">
        <div class="section-header">
          <h3 class="section-title">Sign-offs</h3>
          <button
            v-if="canSignOff"
            @click="showSignOffModal = true"
            class="signoff-button"
          >
            Add Sign-off
          </button>
        </div>

        <div v-if="request.signOffs.length === 0" class="empty-state">
          <p>No sign-offs yet</p>
        </div>

        <div v-else class="signoffs-list">
          <div
            v-for="signoff in request.signOffs"
            :key="signoff.id"
            class="signoff-card"
          >
            <div class="signoff-header">
              <div class="signoff-info">
                <span class="signoff-role">{{ signoff.role }}</span>
                <span class="signoff-reviewer">{{ signoff.reviewerName }}</span>
              </div>
              <div class="signoff-decision" :class="getDecisionClass(signoff.decision)">
                {{ signoff.decision }}
              </div>
            </div>
            <div v-if="signoff.comment" class="signoff-comment">
              {{ signoff.comment }}
            </div>
            <div class="signoff-timestamp">
              {{ formatDate(signoff.timestampUtc) }}
            </div>
          </div>
        </div>
      </section>
    </div>

    <!-- Sign-off Modal -->
    <div v-if="showSignOffModal" class="modal-overlay" @click="closeSignOffModal">
      <div class="modal-content" @click.stop>
        <h3 class="modal-title">Add Sign-off</h3>
        <form @submit.prevent="handleSignOff" class="signoff-form">
          <div class="form-group">
            <label class="form-label">Decision *</label>
            <select v-model="signOffForm.decision" class="form-select" required>
              <option value="">Select Decision</option>
              <option v-for="decision in decisions" :key="decision" :value="decision">
                {{ decision }}
              </option>
            </select>
          </div>
          
          <div class="form-group">
            <label class="form-label">Comment</label>
            <textarea
              v-model="signOffForm.comment"
              class="form-textarea"
              placeholder="Add your comments (optional)"
              rows="3"
            ></textarea>
          </div>
          
          <div class="form-actions">
            <button type="button" @click="closeSignOffModal" class="cancel-button">
              Cancel
            </button>
            <button type="submit" class="submit-button" :disabled="!signOffForm.decision">
              Submit Sign-off
            </button>
          </div>
        </form>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'
import type { ProjectRequest, User, CreateSignOff } from '@/types'
import { Decision } from '@/types'

interface Props {
  request: ProjectRequest
  currentUser: User
}

const props = defineProps<Props>()

const emit = defineEmits<{
  signOff: [signOff: CreateSignOff]
  close: []
  refresh: []
}>()

const showSignOffModal = ref(false)
const signOffForm = ref({
  decision: '' as Decision | '',
  comment: ''
})

const decisions = Object.values(Decision)

const canSignOff = computed(() => {
  // Check if user hasn't already signed off for their role
  const existingSignOff = props.request.signOffs.find(
    s => s.role === props.currentUser.role && s.reviewerName === props.currentUser.name
  )
  return !existingSignOff && props.request.status === 'Submitted'
})

const closeSignOffModal = () => {
  showSignOffModal.value = false
  signOffForm.value = { decision: '', comment: '' }
}

const handleSignOff = () => {
  if (!signOffForm.value.decision) return
  
  const signOff: CreateSignOff = {
    role: props.currentUser.role,
    reviewerName: props.currentUser.name,
    decision: signOffForm.value.decision as Decision,
    comment: signOffForm.value.comment || undefined
  }
  
  emit('signOff', signOff)
  closeSignOffModal()
}

const getStatusClass = (status: string) => {
  const classes: Record<string, string> = {
    Draft: 'status-draft',
    Submitted: 'status-submitted',
    Approved: 'status-approved',
    Rejected: 'status-rejected'
  }
  return classes[status] || ''
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

const getDecisionClass = (decision: string) => {
  const classes: Record<string, string> = {
    Pending: 'decision-pending',
    Approved: 'decision-approved',
    Rejected: 'decision-rejected'
  }
  return classes[decision] || ''
}

const formatDate = (dateString: string) => {
  return new Date(dateString).toLocaleString()
}
</script>

<style scoped>
.request-detail {
  width: 100%;
  max-width: 800px;
  max-height: 90vh;
  display: flex;
  flex-direction: column;
}

.detail-header {
  background: white;
  border-bottom: 1px solid #e5e7eb;
  padding: 1.5rem 2rem;
}

.header-content {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
}

.request-title {
  font-size: 1.5rem;
  font-weight: 600;
  color: #1f2937;
  margin: 0;
  flex: 1;
}

.header-actions {
  display: flex;
  align-items: center;
  gap: 1rem;
}

.request-status {
  padding: 0.5rem 1rem;
  border-radius: 6px;
  font-size: 0.875rem;
  font-weight: 500;
  text-transform: uppercase;
}

.status-draft { background: #f3f4f6; color: #6b7280; }
.status-submitted { background: #dbeafe; color: #1d4ed8; }
.status-approved { background: #dcfce7; color: #166534; }
.status-rejected { background: #fee2e2; color: #dc2626; }

.close-button {
  padding: 0.5rem;
  background: none;
  border: none;
  cursor: pointer;
  font-size: 1.25rem;
  color: #6b7280;
  border-radius: 4px;
  transition: background-color 0.2s;
}

.close-button:hover {
  background: #f3f4f6;
}

.detail-content {
  flex: 1;
  overflow-y: auto;
  padding: 2rem;
  background: #f9fafb;
}

.section {
  background: white;
  border-radius: 8px;
  padding: 1.5rem;
  margin-bottom: 1.5rem;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.1);
}

.section-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 1rem;
}

.section-title {
  font-size: 1.125rem;
  font-weight: 600;
  color: #1f2937;
  margin: 0 0 1rem 0;
}

.info-grid {
  display: grid;
  gap: 1rem;
}

.info-item {
  display: flex;
  flex-direction: column;
  gap: 0.25rem;
}

.info-label {
  font-size: 0.875rem;
  font-weight: 500;
  color: #6b7280;
}

.info-value {
  color: #1f2937;
  margin: 0;
}

.priority-badge {
  padding: 0.25rem 0.75rem;
  border-radius: 4px;
  font-size: 0.75rem;
  font-weight: 500;
  width: fit-content;
}

.priority-low { background: #f3f4f6; color: #6b7280; }
.priority-medium { background: #fef3c7; color: #92400e; }
.priority-high { background: #fed7aa; color: #c2410c; }
.priority-critical { background: #fee2e2; color: #dc2626; }

.signoff-button {
  padding: 0.5rem 1rem;
  background: #10b981;
  color: white;
  border: none;
  border-radius: 6px;
  cursor: pointer;
  font-weight: 500;
  transition: background-color 0.2s;
}

.signoff-button:hover {
  background: #059669;
}

.empty-state {
  text-align: center;
  color: #6b7280;
  font-style: italic;
}

.signoffs-list {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.signoff-card {
  background: #f9fafb;
  border: 1px solid #e5e7eb;
  border-radius: 6px;
  padding: 1rem;
}

.signoff-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 0.5rem;
}

.signoff-info {
  display: flex;
  flex-direction: column;
  gap: 0.25rem;
}

.signoff-role {
  font-weight: 600;
  color: #374151;
}

.signoff-reviewer {
  font-size: 0.875rem;
  color: #6b7280;
}

.signoff-decision {
  padding: 0.25rem 0.75rem;
  border-radius: 4px;
  font-size: 0.75rem;
  font-weight: 500;
  text-transform: uppercase;
}

.decision-pending { background: #fef3c7; color: #92400e; }
.decision-approved { background: #dcfce7; color: #166534; }
.decision-rejected { background: #fee2e2; color: #dc2626; }

.signoff-comment {
  color: #374151;
  margin-bottom: 0.5rem;
  font-style: italic;
}

.signoff-timestamp {
  font-size: 0.75rem;
  color: #9ca3af;
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
  z-index: 100;
}

.modal-content {
  background: white;
  border-radius: 8px;
  padding: 2rem;
  width: 90%;
  max-width: 500px;
}

.modal-title {
  font-size: 1.25rem;
  font-weight: 600;
  color: #1f2937;
  margin: 0 0 1.5rem 0;
}

.signoff-form {
  display: flex;
  flex-direction: column;
  gap: 1.5rem;
}

.form-group {
  display: flex;
  flex-direction: column;
}

.form-label {
  font-weight: 500;
  color: #374151;
  margin-bottom: 0.5rem;
}

.form-select,
.form-textarea {
  padding: 0.75rem;
  border: 1px solid #d1d5db;
  border-radius: 6px;
  font-size: 1rem;
}

.form-select:focus,
.form-textarea:focus {
  outline: none;
  border-color: #3b82f6;
  box-shadow: 0 0 0 3px rgba(59, 130, 246, 0.1);
}

.form-actions {
  display: flex;
  justify-content: flex-end;
  gap: 1rem;
}

.cancel-button {
  padding: 0.75rem 1.5rem;
  background: #f3f4f6;
  color: #374151;
  border: none;
  border-radius: 6px;
  cursor: pointer;
  font-weight: 500;
}

.cancel-button:hover {
  background: #e5e7eb;
}

.submit-button {
  padding: 0.75rem 1.5rem;
  background: #10b981;
  color: white;
  border: none;
  border-radius: 6px;
  cursor: pointer;
  font-weight: 500;
}

.submit-button:hover:not(:disabled) {
  background: #059669;
}

.submit-button:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}
</style>