# Mini School Library System - GitHub Copilot Agent Spec

## Goal
Build a school book reservation web application using **ASP.NET Core MVC**.

The application must support two roles:
- **Student**
- **Librarian** (also acts as Admin)

This is a small but complete MVC application for managing books, borrow requests, and loans in a school library.

## Confirmed Technical Decisions
Use these decisions as fixed requirements during code generation:

- **Framework:** ASP.NET Core MVC on **.NET 10**
- **ORM:** Entity Framework Core
- **Auth:** ASP.NET Core Identity
- **Database:** PostgreSQL
- **EF Core provider:** `Npgsql.EntityFrameworkCore.PostgreSQL`
- **UI:** Razor Views + Bootstrap
- **Database approach:** Code-first migrations
- **Student onboarding:** Self-registration is enabled

### Development Connection String
Use this exact development connection string unless explicitly changed later:

```json
Host=localhost;Port=5432;Database=ECommerceDB;Username=postgres;Password=password
```

Note: the database name is `ECommerceDB` because it was explicitly provided. Do not rename it unless asked.

## Primary Objective
Create a clean, maintainable, role-based MVP that follows standard ASP.NET Core MVC practices and enforces business rules on the server side.

## Functional Scope

### Student Features
Students must be able to:
- See a list of all books
- See book details
- Filter and search books
- Submit a borrow request for a book
- See the borrow button disabled when a book is not available
- See their own requests with statuses:
  - Pending
  - Accepted
  - Denied
  - Cancelled
- See their active loans
- See each loan due date (7 days from issue date)
- See overdue status when a loan is late
- See returned loan history
- Self-register and sign in

### Librarian / Admin Features
Librarians must be able to:
- See all books
- Add a new book
- Edit book information
- Delete a book
- Manage book categories (add, edit, delete)
- See all students
- See which student currently holds which book
- See all borrow requests from students
- Accept a request
- Deny a request
- Manage all loans (student, book, due date)
- Mark a loan as returned
- See overdue loans

## Core Business Rules
These rules are mandatory and must be enforced in both UI and server-side logic.

1. **Book stock decreases only when a librarian accepts a request.**
2. **Book stock increases only when a librarian marks a loan as returned.**
3. **Each accepted request creates exactly one Loan record.**
4. **Loan due date = IssuedDate + 7 days.**
5. **If current date is greater than DueDate and the loan is not returned, the loan is overdue.**
6. **Students cannot access librarian routes.**
7. **Librarians can access all areas.**
8. **A student cannot submit a borrow request when AvailableCount is 0.**
9. **A request can only be accepted or denied once.**
10. **Returning a loan must set ReturnedDate and restore stock exactly once.**
11. **A student can have at most one pending borrow request at a time across the entire system.**
12. **A student cannot create a new request if they already have any pending request.**
13. **A student cannot request the same book if they already have an active loan for that book.**
14. **A student cannot create duplicate pending requests for the same book.**
15. **Category deletion must not be blocked just because books exist in that category.**
16. **When deleting a category that still has books, reassign those books to an `Uncategorized` fallback category, then delete the original category.**

## Suggested Technical Design

### Authentication and Authorization
Use ASP.NET Core Identity.

Required roles:
- `Student`
- `Librarian`

Requirements:
- Apply `[Authorize]` attributes by role
- Student pages must be protected from anonymous access where needed
- Librarian pages must be restricted to the `Librarian` role only
- Allow public access to student registration and login
- Seed at least one librarian account for development
- Optionally seed one student account for test data, but self-registration must also work

### Recommended Project Structure
Use a clean MVC structure, for example:

- `Controllers/`
- `Models/`
- `Views/`
- `Data/`
- `Services/`
- `Areas/Librarian/Controllers/`
- `Areas/Librarian/Views/`
- `ViewModels/`
- `Infrastructure/` (optional for seeding, time abstraction, or helpers)

Use an **Area** for librarian/admin pages to clearly separate privileged functionality.

## Domain Models

### ApplicationUser
Extend Identity user.

Suggested fields:
- `Id`
- `FullName`
- `Email`
- `UserName`
- Identity-managed role membership (do not store a manual role string unless needed only for display)

### Category
Suggested fields:
- `Id`
- `Name`
- `Description` (optional)
- `CreatedAt`
- `UpdatedAt`

Rules:
- Category name should be unique
- Seed a default `Uncategorized` category
- `Uncategorized` must not be deletable from the UI

### Book
Suggested fields:
- `Id`
- `Title`
- `Author`
- `Description`
- `CategoryId`
- `Category`
- `ISBN` (optional)
- `TotalCount`
- `AvailableCount`
- `CreatedAt`
- `UpdatedAt`

Rules:
- `AvailableCount` must never be negative
- `AvailableCount` must not exceed `TotalCount`
- Deleting a book should be blocked if it has active loans, or handled safely with clear validation

### BorrowRequest
Suggested fields:
- `Id`
- `BookId`
- `Book`
- `StudentId`
- `Student`
- `RequestedAt`
- `Status` (`Pending`, `Accepted`, `Denied`, `Cancelled`)
- `ProcessedAt` (nullable)
- `ProcessedById` (nullable, librarian user id)
- `ProcessedBy` (optional navigation)
- `Notes` (optional)

Rules:
- A request starts as `Pending`
- Students can cancel only their own pending requests
- Accepted, denied, or cancelled requests are closed
- A student may have only one pending request total at a time

### Loan
Suggested fields:
- `Id`
- `BookId`
- `Book`
- `StudentId`
- `Student`
- `BorrowRequestId`
- `BorrowRequest`
- `IssuedDate`
- `DueDate`
- `ReturnedDate` (nullable)

Derived properties or helper logic:
- `IsReturned`
- `IsOverdue`

Rules:
- Created only when a request is accepted
- `DueDate = IssuedDate.AddDays(7)`
- Marking returned sets `ReturnedDate`

## Enums
Use enums where appropriate.

### RequestStatus
- Pending
- Accepted
- Denied
- Cancelled

## Application Flows

### Student Registration Flow
1. Visitor opens registration page.
2. Visitor creates an account.
3. System creates the Identity user.
4. System automatically assigns the `Student` role.
5. Student can sign in and access student features.

### Student Borrow Request Flow
1. Student signs in.
2. Student browses or searches books.
3. Student opens book details.
4. If `AvailableCount > 0`, the student does not already have a pending request, and the student does not already hold the same book, they can submit a borrow request.
5. Request is saved as `Pending`.
6. Librarian later accepts or denies it.

### Request Acceptance Flow
1. Librarian reviews pending requests.
2. On accept:
   - Verify the request is still pending
   - Verify the student still has this as their only pending request
   - Verify the student does not already have an active loan for the same book
   - Verify the book still has available stock
   - Change request status to `Accepted`
   - Set `ProcessedAt`
   - Set `ProcessedById`
   - Decrease `Book.AvailableCount` by 1
   - Create a `Loan`
   - Set `IssuedDate = current UTC date/time`
   - Set `DueDate = IssuedDate + 7 days`
3. Save all changes in one transaction

### Request Denial Flow
1. Librarian reviews pending requests.
2. On deny:
   - Verify the request is still pending
   - Change request status to `Denied`
   - Set `ProcessedAt`
   - Set `ProcessedById`
   - Do not change stock
   - Do not create a loan

### Loan Return Flow
1. Librarian selects an active loan.
2. Librarian clicks Mark Returned.
3. System:
   - Verifies the loan is not already returned
   - Sets `ReturnedDate`
   - Increases `Book.AvailableCount` by 1
4. Save changes in one transaction

### Category Delete Flow
1. Librarian selects a category to delete.
2. If the category is `Uncategorized`, block the action.
3. For any books assigned to the category, reassign them to `Uncategorized`.
4. Delete the selected category.
5. Save all changes in one transaction.

## Required Pages and Screens

### Public / Shared
- Home page (optional simple landing page)
- Login page
- Register page (student self-registration)
- Access denied page

### Student Pages
- Book list page
- Book details page
- My requests page
- My active loans page
- My loan history page

### Librarian Pages
Inside `Areas/Librarian`:
- Dashboard
- Books list
- Create book
- Edit book
- Delete book
- Categories list
- Create category
- Edit category
- Delete category
- Students list
- Student details (optional but recommended)
- Requests list
- Loan list
- Overdue loans list

## Controllers (Suggested)

### Student Side
- `HomeController`
- `BooksController`
  - `Index`
  - `Details`
- `RequestsController`
  - `Create`
  - `MyRequests`
  - `Cancel`
- `LoansController`
  - `MyActiveLoans`
  - `MyHistory`
- `AccountController` only if not using default Identity UI scaffolding

### Librarian Area
- `DashboardController`
- `BooksController`
- `CategoriesController`
- `StudentsController`
- `RequestsController`
- `LoansController`

## UI and UX Expectations
- Use a simple Bootstrap-based responsive layout
- Show clear badges for statuses:
  - Pending
  - Accepted
  - Denied
  - Cancelled
  - Overdue
  - Returned
- Disable the borrow button when a book is unavailable
- Disable or hide the borrow button when the student already has a pending request
- Show a clear explanation message when request creation is blocked by a business rule
- Confirm destructive actions (delete, deny, mark returned)
- Show validation messages clearly
- Use tables for librarian management pages
- Use search and filter controls on the books list page

## Search and Filter Requirements
At minimum, support searching books by:
- Title
- Author

Optional but recommended:
- Category
- Availability

## Validation Rules
- Book title is required
- Author is required
- Category is required
- `TotalCount` must be greater than or equal to 0
- `AvailableCount` must be between 0 and `TotalCount`
- Category name is required
- A student must not be able to cancel another student's request
- A student must not be able to see another student's private requests or loans
- A student must not be able to create a request while another of their requests is pending
- A student must not be able to request a book they currently hold in an active loan

## Data Access and Consistency
- Use EF Core with async methods
- Use eager loading where needed for book/category/student display
- Protect stock updates with server-side validation
- Prefer transactions when accepting requests, returning loans, and deleting categories with reassignment
- Prevent duplicate state changes on already processed requests or already returned loans
- Add unique indexes or defensive checks where useful to reduce invalid duplicates

## Recommended Services
Create service classes to keep controllers thin.

Suggested services:
- `IBookService`
- `IRequestService`
- `ILoanService`
- `ICategoryService`
- `IIdentitySeedService` (optional)

At minimum, move business-critical logic into services, especially:
- Accept request
- Deny request
- Create loan from request
- Mark loan as returned
- Category reassignment on delete
- Overdue calculations
- Student request eligibility checks

## Overdue Logic
Implement overdue status based on current server time.

A loan is overdue when:
- `ReturnedDate == null`
- and current UTC time is greater than `DueDate`

Use UTC consistently for `RequestedAt`, `ProcessedAt`, `IssuedDate`, `DueDate`, and `ReturnedDate`.

## Database and EF Core Configuration
Use code-first migrations.

### Required Packages
At minimum:
- `Microsoft.AspNetCore.Identity.EntityFrameworkCore`
- `Microsoft.AspNetCore.Identity.UI` (if using Identity UI)
- `Microsoft.EntityFrameworkCore`
- `Microsoft.EntityFrameworkCore.Design`
- `Npgsql.EntityFrameworkCore.PostgreSQL`

### DbContext
Create an application DbContext that inherits from `IdentityDbContext<ApplicationUser>` and contains:
- `DbSet<Book>`
- `DbSet<Category>`
- `DbSet<BorrowRequest>`
- `DbSet<Loan>`

### Migrations Workflow
1. Create initial Identity + domain models
2. Configure PostgreSQL provider
3. Add EF Core migrations
4. Update database
5. Seed roles and sample users
6. Seed `Uncategorized`, sample categories, and sample books

### Program.cs Requirements
Configure:
- PostgreSQL with the provided connection string
- Identity
- Role management
- Authentication and authorization
- Default route + area route support
- Optional automatic seeding on startup for development

## Seed Data (Required for Development)
Seed:
- Roles: `Librarian`, `Student`
- 1 librarian user
- `Uncategorized` category
- A few normal categories
- A few books with mixed availability

Recommended seeded librarian credentials can be simple development-only values.

## Error Handling
- Show friendly messages for invalid operations
- Prevent crashes on invalid ids
- Return `NotFound` when entities do not exist
- Return `Forbid` or redirect to access denied when role rules are violated
- Return validation errors for business-rule failures instead of silently ignoring them

## Non-Functional Expectations
- Clean, readable, maintainable code
- Use view models where appropriate instead of exposing entities directly in all views
- Follow standard naming conventions
- Use async/await consistently
- Keep controllers focused on HTTP concerns
- Keep business rules out of views
- Prefer small, testable service methods

## Suggested Implementation Order for Copilot Agent
1. Create ASP.NET Core MVC project targeting .NET 10 with Individual Accounts or Identity scaffolding
2. Configure PostgreSQL and the provided connection string
3. Configure roles and seed a librarian account
4. Create domain models and DbContext relationships
5. Add migrations and database initialization
6. Build student registration flow and automatic `Student` role assignment
7. Build student book browsing pages
8. Build student request submission and personal request tracking
9. Enforce single pending request and same-book active-loan restrictions
10. Build librarian book and category management
11. Build librarian request review (accept / deny)
12. Build loan management and return flow
13. Add overdue views and status badges
14. Refactor business rules into services
15. Polish validation, authorization, and UI

## Definition of Done
The implementation is complete when:
- Students can self-register and sign in
- Students can browse books and request available books
- A student cannot create a second pending request while one is still pending
- A student cannot request the same book while already holding it
- Librarians can approve or deny requests
- Accepting a request decreases available stock and creates a loan
- Returning a loan increases available stock
- Due date is always 7 days after issue date
- Overdue loans are clearly visible
- Students cannot access librarian routes
- Core CRUD operations for books and categories work
- Category deletion reassigns books to `Uncategorized` instead of blocking
- The application runs after migrations with seeded sample data

## Important Coding Constraints for GitHub Copilot Agent
- Do not implement business rules only in the UI; always enforce them server-side
- Do not reduce stock when a request is merely created
- Do not increase stock when a request is denied or cancelled
- Do not create a loan unless a librarian accepts a request
- Do not allow negative inventory
- Do not allow repeated return actions for the same loan
- Do not allow a student to have multiple pending requests at the same time
- Do not allow a student to request a book they already hold
- Keep privileged routes inside the librarian area and protect them with role-based authorization
- Use PostgreSQL, not SQL Server or SQLite, unless explicitly changed later
- Use the provided connection string as the default development configuration

## Nice-to-Have Enhancements (Optional)
These are optional and should only be added after the MVP works:
- Pagination on book list and admin tables
- Toast notifications for success and error states
- Audit fields on all entities
- Soft delete for books
- Dashboard cards showing totals (books, pending requests, active loans, overdue loans)
- Concurrency protection using row version fields or optimistic concurrency patterns

## Deliverable Expectation
Generate a fully working ASP.NET Core MVC project that includes:
- Models
- DbContext
- Migrations
- Seed logic
- Controllers
- Services
- Razor views
- Role-based authorization
- Bootstrap UI
- PostgreSQL configuration
- Self-registration flow for students

The result should be runnable locally with PostgreSQL and demonstrate all required features in the MVP.

## Assumptions Resolved From Clarifications
These decisions are no longer open and should be treated as final:

1. Target the latest stable ASP.NET Core version, implemented here as **.NET 10**.
2. Use **PostgreSQL**.
3. Use the provided connection string exactly as given.
4. **Students can self-register.**
5. **Students cannot have multiple pending requests at the same time**, even for different books.
6. **Students cannot request the same book again while they already have it on loan.**
7. **Category deletion is allowed** even when books exist, by reassigning linked books to `Uncategorized` before delete.
