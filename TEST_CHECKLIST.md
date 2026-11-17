# Repair Track — manual test checklist

Use the sample rows from `repairTrackDb.sql` unless a step says to add new data. Money should look like ₱1,500.00. There is no delete anywhere.

## Start

- [ ] Copy `App.config.example` to `App.config`, set the password, run the app.
- [ ] App opens the main window with sidebar: Dashboard, Customers, Job Orders.
- [ ] Sidebar shows an Offline badge.
- [ ] Wrong password or PostgreSQL stopped shows a friendly message and the app does not crash.

## Dashboard

- [ ] Four cards show counts: Received, In Repair, Ready for Pickup, Completed (this month only).
- [ ] Recent active jobs list is not empty if sample data is loaded. Completed jobs are not in this list.
- [ ] Status cells use status colors. Balance above 0 is the due color. Zero balance is the positive color.
- [ ] Click a row. Job Order Detail opens for that job.
- [ ] Click New Job Order. The new job form opens.

## Customers list

- [ ] Sidebar Customers shows real customers (id like C-00001, name, phone, address, device count).
- [ ] Click a row. Customer Detail opens.
- [ ] Click Add Customer. Add Customer form opens.
- [ ] Click Edit Selected with no row chosen. Message asks you to select a customer.
- [ ] Click Edit Selected with a row chosen. Edit Customer opens.

## Job Orders list

- [ ] List shows jobs with filter default All.
- [ ] Change filter to Received, In Repair, Ready for Pickup, Completed. Rows match.
- [ ] Click a row. Job Order Detail opens.
- [ ] Click New Job Order. The new job form opens.

## Shared search (top of every screen)

- [ ] Type a customer name (for example `juan`). Results appear under the box in under 2 seconds.
- [ ] Type a phone number fragment. Matching customers appear.
- [ ] Type a serial fragment (for example `SN-`). Matching devices appear.
- [ ] Click a customer result. Customer Detail opens.
- [ ] Click a device result. Device Detail opens.
- [ ] Type nonsense. You see “No matching customers or devices.”
- [ ] Press Escape. Results hide.
- [ ] Search still works after you switch sidebar pages.

## Customer add / edit / detail

- [ ] Add Customer: leave name empty, Save. Friendly message, no crash.
- [ ] Enter a name, Save. You stay on Edit Customer so you can add devices.
- [ ] Add Device from that screen. Device form opens. Serial empty → friendly message.
- [ ] Save a device (type, serial, optional brand/password). You return to Edit Customer and the device is in the list.
- [ ] Click the device row. Device Detail opens.
- [ ] From Customer Detail, Edit works. Phone and address can be blank.
- [ ] Customer Detail shows devices and job history. Click a job. Job Order Detail opens.
- [ ] Required name shows a red asterisk.

## Device detail and new job

- [ ] Device Detail shows customer, type, brand, serial. Password is not visible.
- [ ] Click Password. Stored password appears in a small window. If none, it says so.
- [ ] Click the customer name. Customer Detail opens.
- [ ] Click Edit. Change a field, Save. Device Detail shows the change.
- [ ] Click New Job Order from the device. Device is pre-selected.
- [ ] New job with empty issue → friendly message.
- [ ] New job with issue and blank cost. Job is created as Received. Detail opens. History has one Received row. Cost is ₱0.00.
- [ ] New job with cost 1500. Total cost shows ₱1,500.00.

## Job Order Detail

- [ ] Header shows JO-year-id, customer, device, serial, received date, Password button.
- [ ] Progress tracker shows Received → In Repair → Ready for Pickup → Completed. Current and past steps are colored.
- [ ] Step forward one status. History gets a new row with date and time. You cannot jump two steps.
- [ ] Step back one status. If you leave Completed, date released is cleared (job is active again).
- [ ] Move to Completed. Date released is set (job leaves the dashboard active list).
- [ ] Edit total cost to a valid number. Save cost. Summary updates.
- [ ] Set cost lower than already paid. Friendly message, cost unchanged.
- [ ] Edit issue description. Save issue. Reload still shows the new text.
- [ ] Financial summary: total cost, total paid, balance. Unpaid balance uses due color. Fully paid uses positive color.
- [ ] Record payment of 0 or blank. Friendly message.
- [ ] Record payment larger than remaining balance. Friendly message naming the remaining balance.
- [ ] Record a valid payment. It appears in the list. Balance drops. There is no edit or delete for payments.
- [ ] No technician note, no payment method, no “by name”.

## Look and feel

- [ ] Screens use the same fonts (Segoe UI) and the shared colors. No random leftover colors.
- [ ] Icons show on sidebar and main buttons (or a simple fallback if the icon font is missing).
- [ ] Window background is light gray; cards and tables are white.

## Rules that must stay true

- [ ] No Delete button on any screen.
- [ ] Screens still work if you open search, open a result, then use Back.
- [ ] Closing the app and opening it again shows the data you saved.
