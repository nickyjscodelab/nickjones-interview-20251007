# Workflow Management System

A modern workflow management system built with Vue 3 (TypeScript) frontend and .NET 8 Web API backend.

## Features

- **Role-based Authentication**: Login as Engineer, QA, or Manager
- **Project Request Management**: Create, view, and manage project requests
- **Workflow Approval**: Multi-role sign-off system for project approvals
- **Modern UI**: Clean, responsive interface built with Vue 3 and TypeScript

## Getting Started

### Prerequisites

- .NET 8 SDK
- Node.js (18.0.0 or higher)
- npm

### Backend Setup

1. Navigate to the backend directory:
   ```bash
   cd backend/Workflow.Api
   ```

2. Run the API:
   ```bash
   dotnet run
   ```

   The API will be available at `http://localhost:5100`
   Swagger documentation at `http://localhost:5100/swagger`

### Frontend Setup

1. Navigate to the frontend directory:
   ```bash
   cd frontend/workflow-ui
   ```

2. Install dependencies:
   ```bash
   npm install
   ```

3. Start the development server:
   ```bash
   npm run dev
   ```

   The application will be available at `http://localhost:3000`

## Usage

### Login

1. Open the application in your browser
2. Enter your name
3. Select your role (Engineer, QA, or Manager)
4. Click "Continue to Workflow"

### Creating Project Requests

1. Click "New Request" button
2. Fill in the request details:
   - Title (required)
   - Description (required)
   - Priority (Low, Medium, High, Critical)
   - Due Date (optional)
3. Click "Create Request"

### Signing Off on Requests

1. Click on any project request card to view details
2. If you haven't signed off and the request is "Submitted", you'll see an "Add Sign-off" button
3. Select your decision (Pending, Approved, Rejected)
4. Add optional comments
5. Submit your sign-off

### Filtering and Search

- Use the status filter dropdown to filter requests by status
- Use the search box to find requests by title, description, or requester

## Project Structure

### Backend (`backend/Workflow.Api`)

- **Domain**: Core entities and interfaces (ProjectRequest, SignOff, etc.)
- **Data**: Entity Framework repositories and database context
- **Services**: Business logic layer
- **Endpoints**: Web API endpoints
- **Infrastructure**: DI configuration and data seeding

### Frontend (`frontend/workflow-ui`)

- **views**: Page components (LoginView, WorkflowView)
- **components**: Reusable UI components (CreateEditRequest, RequestDetail)
- **composables**: Vue composition functions (useAuth)
- **services**: API service layer
- **types**: TypeScript type definitions
- **router**: Vue Router configuration

## API Endpoints

- `GET /api/requests` - Get all project requests (with optional filters)
- `GET /api/requests/{id}` - Get specific project request
- `POST /api/requests` - Create new project request
- `PUT /api/requests/{id}/status` - Update request status
- `POST /api/requests/{id}/signoffs` - Add sign-off to request

## Technologies Used

### Backend
- .NET 8
- ASP.NET Core Web API
- Entity Framework Core (In-Memory)
- Swagger/OpenAPI

### Frontend
- Vue 3 (Composition API)
- TypeScript
- Vue Router 4
- Vite
- Axios for HTTP requests

## Notes

- The application uses an in-memory database that resets when the backend is restarted
- CORS is configured to allow requests from `localhost:3000` and `localhost:5173`
- The application includes seed data with sample project requests
