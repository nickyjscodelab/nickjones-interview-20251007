Workflow WPF Client
====================

Overview
--------
This is a WPF (.NET 8) desktop client that mirrors the core functionality of the existing Vue `workflow-ui` frontend:

* List project requests
* Filter by status & free‑text search
* Create a new request
* View request details & sign‑offs
* Add a sign‑off (if the current user/role has not already signed off)

Technology / Patterns Demonstrated
----------------------------------
* WPF + XAML layout
* MVVM pattern (ViewModels implement `INotifyPropertyChanged` via a lightweight base class)
* ObservableCollection for live-updating lists
* DataBinding to enums, text inputs, selection, dialogs
* ICommand (simple RelayCommand + AsyncRelayCommand pattern)
* Value converters (enum -> Brush styling)
* Dialog windows for create request & sign‑off
* Basic HttpClient service abstraction

Important: macOS / Linux Limitation
-----------------------------------
WPF is Windows-only. While the project file sets `<EnableWindowsTargeting>true>`, you still need Windows to compile & run. On macOS you can: 
* Restore (`dotnet restore`) 
* Open & view code
But you cannot successfully build or launch the app.

Running (on Windows)
--------------------
1. Ensure the backend API is running on `http://localhost:5100` (adjust `ApiClient.BaseAddress` if different).
2. From solution root:
   dotnet build
3. Launch the WPF project:
   dotnet run --project frontend/workflow-wpf/Workflow.Wpf.csproj

Future Enhancements (not implemented to keep sample concise)
------------------------------------------------------------
* Authentication / role selection UI
* Editing existing requests
* Status transitions (approve / reject workflow at request level)
* Error & retry policies (Polly)
* Theming / Dark mode
