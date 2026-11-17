# Repair Track — progress

Audit date: 2026-11-17.

Git root: `RepairTrack` (the inner project folder). Cursor workspace is the parent folder.

## Decisions (when the prompt was unclear)

- Total cost may be 0 (unknown yet). Payments must be greater than 0. Negative money is rejected.
- Device type is still required when adding a device (already in DeviceService). Serial number is also required.
- After saving a new customer, the app stays on Edit Customer so staff can add devices.
- After saving a device, the app goes back to the screen named in `AfterDeviceSave`.
- After creating a job, the app opens Job Order Detail.
- Row click opens the detail screen. Edit / Add buttons open edit screens.
- Device password on the edit form is plain text so staff can type it. On detail screens it stays hidden until Password is clicked.
- No paper-prototype image was in the project folder. Job Order Detail follows a two-column shop layout: progress + issue + history on the left, money + payments on the right. Technician note, payment method, and "by name" are omitted.
- Search waits 250ms after typing so results stay under 2 seconds without searching on every key.
- `Form1.cs`, `Form1.Designer.cs`, `Form1.resx`, and `PlaceholderScreen.cs` are leftover. They are listed here and were not deleted.

## Duplicate / unused files (left in place)

| File | Note |
| --- | --- |
| `Form1.cs`, `Form1.Designer.cs`, `Form1.resx` | Old connection test window. App starts with `MainForm`. Form1 talks to repositories directly (rule break, unused). |
| `Presentation/PlaceholderScreen.cs` | "coming soon" helper. Unused once real screens exist. |
| `RepairTrack.csproj.user` | Visual Studio user file (gitignored). |

## Rule breaks found in audit (to fix)

| Issue | Status after this pass |
| --- | --- |
| `CustomerEditScreen` shows "The device screen is not built yet." | Fix in Phase 4 |
| Shared search box does not search | Fix in Phase 3 |
| List screens do not open detail on row click | Fix in Phase 2 |
| New Job / Add Customer / Edit buttons do nothing | Fix in Phase 2 |
| `PaymentService` and `SearchService` missing | Fix in Phase 1 |
| `App.config.example` had a real password | Fix in Phase 7 (`YOUR_PASSWORD_HERE`) |
| Required asterisks not using required color | Fix in Phase 7 |
| Sidebar has no icons or offline badge | Fix in Phase 7 |
| Screens call services only (except leftover Form1) | OK |

## PHASES 1 to 7

| Item | Status | Files |
| --- | --- | --- |
| **Phase 1 — IdFormatter** | Done | `Services/IdFormatter.cs` |
| **Phase 1 — CustomerService** | Partial → Done | `Services/CustomerService.cs` (search added) |
| **Phase 1 — DeviceService** | Partial → Done | `Services/DeviceService.cs` (GetAll + search added) |
| **Phase 1 — JobOrderService** | Partial → Done | `Services/JobOrderService.cs` (status, balance, create, cost; payment moved out) |
| **Phase 1 — PaymentService** | Missing → Done | `Services/PaymentService.cs` |
| **Phase 1 — SearchService** | Missing → Done | `Services/SearchService.cs` |
| **Phase 1 — DashboardService** | Done | `Services/DashboardService.cs` |
| **Phase 1 — SaveResult** | Done | `Services/SaveResult.cs` |
| **Phase 2 — Dashboard real data** | Partial → Done | `Presentation/DashboardScreen.cs` (counts + recent jobs; row click + New Job wired) |
| **Phase 2 — Customers list** | Partial → Done | `Presentation/CustomerScreen.cs` (`CustomersScreen`) |
| **Phase 2 — Job Orders list + filter** | Partial → Done | `Presentation/JobOrderScreen.cs` (`JobOrdersScreen`) |
| **Phase 3 — Shared search bar** | Missing → Done | `Presentation/MainForm.cs` |
| **Phase 4 — Add/Edit Customer + devices** | Partial → Done | `Presentation/CustomerEditScreen.cs` |
| **Phase 4 — Customer Detail** | Missing → Done | `Presentation/CustomerDetailScreen.cs` |
| **Phase 4 — DeviceEditScreen** | Missing → Done | `Presentation/DeviceEditScreen.cs` |
| **Phase 5 — Device Detail + password button** | Missing → Done | `Presentation/DeviceDetailScreen.cs` |
| **Phase 5 — New Job Order** | Missing → Done | `Presentation/JobOrderNewScreen.cs` |
| **Phase 6 — Job Order Detail** | Missing → Done | `Presentation/JobOrderDetailScreen.cs` |
| **Phase 7 — Palette / fonts / icons** | Partial → Done | `Presentation/Ui.cs` plus screens |
| **Docs** | Missing → Done | `README.md`, `TEST_CHECKLIST.md`, `App.config.example` |

## Data layer (already Done, reused)

- `Data/RepairTrackContext.cs`
- `Data/DatabaseChecker.cs`
- `Data/Entities/*`
- `Data/Repositories/CustomerRepository.cs` (already had Search)
- `Data/Repositories/DeviceRepository.cs` (already had SearchBySerial)
- `Data/Repositories/JobOrderRepository.cs` (`GetByCustomer` added)
- `Data/Repositories/PaymentRepository.cs`

## Next

All phases implemented in this pass. Use `TEST_CHECKLIST.md` to click through the app.
