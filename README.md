# Repair Track

A simple offline Windows desktop app for one small computer repair shop. One PC, no login. Staff track customers, devices, job orders, status, and payments.

## What you need

- Windows
- Visual Studio 2022 (or the .NET 9 SDK)
- PostgreSQL 18
- Database name: `repairtrack`
- Database user: `postgres`

## Setup

1. Create an empty database named `repairtrack` in PostgreSQL (pgAdmin is fine).
2. Open `repairTrackDb.sql` in pgAdmin and run it against `repairtrack`. That creates the tables and a few sample rows.
3. Copy `App.config.example` to `App.config` in this folder.
4. Open `App.config` and replace `YOUR_PASSWORD_HERE` with your local `postgres` password. Do not commit `App.config`.
5. Open `RepairTrack.sln` in Visual Studio and run the project (F5), or from this folder run `dotnet run`.

If the app cannot reach PostgreSQL, it shows a short message and does not start.

## How the project is folded

- `Presentation` — screens (UserControls) and shared look-and-feel in `Ui.cs`
- `Services` — validation and business rules; screens call these only
- `Data` — EF Core context, entities, repositories

## Out of scope

No delete, no login, no inventory, no receipts, no reports, no payment method, no technician notes.

See `TEST_CHECKLIST.md` for a full click-through test, and `PROGRESS.md` for what was built.
