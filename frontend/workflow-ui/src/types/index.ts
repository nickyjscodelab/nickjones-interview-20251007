export enum Role {
  Engineer = 'Engineer',
  QA = 'QA',
  Manager = 'Manager'
}

export enum Decision {
  Pending = 'Pending',
  Approved = 'Approved',
  Rejected = 'Rejected'
}

export enum Priority {
  Low = 'Low',
  Medium = 'Medium',
  High = 'High',
  Critical = 'Critical'
}

export enum RequestStatus {
  Draft = 'Draft',
  Submitted = 'Submitted',
  Approved = 'Approved',
  Rejected = 'Rejected'
}

export interface SignOff {
  id: number
  projectRequestId: number
  role: Role
  reviewerName: string
  decision: Decision
  comment?: string
  timestampUtc: string
}

export interface ProjectRequest {
  id: number
  title: string
  description: string
  priority: Priority
  status: RequestStatus
  requestedBy: string
  createdUtc: string
  dueUtc?: string
  signOffs: SignOff[]
}

export interface CreateProjectRequest {
  title: string
  description: string
  priority: Priority
  requestedBy: string
  dueUtc?: string
}

export interface CreateSignOff {
  role: Role
  reviewerName: string
  decision: Decision
  comment?: string
}

export interface User {
  name: string
  role: Role
}