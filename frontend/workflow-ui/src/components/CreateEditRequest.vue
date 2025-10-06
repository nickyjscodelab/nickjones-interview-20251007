<template>
  <div class="create-edit-form">
    <h2 class="form-title">
      {{ isEditing ? 'Edit Request' : 'Create New Request' }}
    </h2>
    
    <form @submit.prevent="handleSubmit" class="form">
      <div class="form-group">
        <label for="title" class="form-label">Title *</label>
        <input
          id="title"
          v-model="form.title"
          type="text"
          class="form-input"
          placeholder="Enter request title"
          required
        />
      </div>
      
      <div class="form-group">
        <label for="description" class="form-label">Description *</label>
        <textarea
          id="description"
          v-model="form.description"
          class="form-textarea"
          placeholder="Enter request description"
          rows="4"
          required
        ></textarea>
      </div>
      
      <div class="form-row">
        <div class="form-group">
          <label for="priority" class="form-label">Priority *</label>
          <select
            id="priority"
            v-model="form.priority"
            class="form-select"
            required
          >
            <option value="">Select Priority</option>
            <option v-for="priority in priorities" :key="priority" :value="priority">
              {{ priority }}
            </option>
          </select>
        </div>
        
        <div class="form-group">
          <label for="dueDate" class="form-label">Due Date</label>
          <input
            id="dueDate"
            v-model="form.dueDate"
            type="date"
            class="form-input"
          />
        </div>
      </div>
      
      <div class="form-actions">
        <button
          type="button"
          @click="$emit('cancel')"
          class="cancel-button"
        >
          Cancel
        </button>
        <button
          type="submit"
          class="submit-button"
          :disabled="!isFormValid"
        >
          {{ isEditing ? 'Update' : 'Create' }} Request
        </button>
      </div>
    </form>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useAuth } from '@/composables/useAuth'
import { projectRequestApi } from '@/services/api'
import type { ProjectRequest, CreateProjectRequest } from '@/types'
import { Priority } from '@/types'

interface Props {
  request?: ProjectRequest | null
}

const props = defineProps<Props>()

const emit = defineEmits<{
  save: []
  cancel: []
}>()

const { currentUser } = useAuth()

const form = ref({
  title: '',
  description: '',
  priority: '' as Priority | '',
  dueDate: ''
})

const priorities = Object.values(Priority)

const isEditing = computed(() => props.request !== null)

const isFormValid = computed(() => {
  return form.value.title.trim() !== '' &&
         form.value.description.trim() !== '' &&
         form.value.priority !== ''
})

const handleSubmit = async () => {
  if (!isFormValid.value || !currentUser.value) return
  
  try {
    const requestData: CreateProjectRequest = {
      title: form.value.title.trim(),
      description: form.value.description.trim(),
      priority: form.value.priority as Priority,
      requestedBy: currentUser.value.name,
      dueUtc: form.value.dueDate ? new Date(form.value.dueDate).toISOString() : undefined
    }
    
    await projectRequestApi.create(requestData)
    emit('save')
  } catch (error) {
    console.error('Failed to save request:', error)
    alert('Failed to save request. Please try again.')
  }
}

onMounted(() => {
  if (props.request) {
    form.value = {
      title: props.request.title,
      description: props.request.description,
      priority: props.request.priority,
      dueDate: props.request.dueUtc
        ? new Date(props.request.dueUtc).toISOString().split('T')[0]
        : ''
    }
  }
})
</script>

<style scoped>
.create-edit-form {
  padding: 2rem;
  width: 100%;
  min-width: 500px;
  max-width: 600px;
}

@media (max-width: 640px) {
  .create-edit-form {
    min-width: auto;
    padding: 1.5rem;
  }
}

.form-title {
  font-size: 1.5rem;
  font-weight: 600;
  color: #1f2937;
  margin-bottom: 1.5rem;
}

.form {
  display: flex;
  flex-direction: column;
  gap: 1.5rem;
}

.form-row {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 1rem;
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

.form-input,
.form-select,
.form-textarea {
  padding: 0.75rem;
  border: 1px solid #d1d5db;
  border-radius: 8px;
  font-size: 1rem;
  transition: all 0.2s;
}

.form-input:focus,
.form-select:focus,
.form-textarea:focus {
  outline: none;
  border-color: #3b82f6;
  box-shadow: 0 0 0 3px rgba(59, 130, 246, 0.1);
}

.form-textarea {
  resize: vertical;
  min-height: 100px;
}

.form-actions {
  display: flex;
  justify-content: flex-end;
  gap: 1rem;
  margin-top: 1rem;
}

.cancel-button {
  padding: 0.75rem 1.5rem;
  background: #f3f4f6;
  color: #374151;
  border: none;
  border-radius: 8px;
  cursor: pointer;
  font-weight: 500;
  transition: background-color 0.2s;
}

.cancel-button:hover {
  background: #e5e7eb;
}

.submit-button {
  padding: 0.75rem 1.5rem;
  background: #3b82f6;
  color: white;
  border: none;
  border-radius: 8px;
  cursor: pointer;
  font-weight: 500;
  transition: all 0.2s;
}

.submit-button:hover:not(:disabled) {
  background: #2563eb;
}

.submit-button:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}
</style>