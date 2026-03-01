# School Library System

A complete ASP.NET Core MVC application for managing a school library's book reservation system.

## Features

### Student Features
- Self-registration and authentication
- Browse and search books by title, author, or category
- View book details and availability
- Submit borrow requests for available books
- View personal request history with status tracking
- View active loans and due dates
- View loan history
- Automatic overdue status detection

### Librarian Features
- Dashboard with statistics
- Complete book management (CRUD operations)
- Category management with automatic reassignment
- View and manage all students
- Review and process borrow requests (accept/deny)
- Manage all loans
- Mark loans as returned
- View overdue loans

## Technology Stack

- **Framework**: ASP.NET Core MVC (.NET 10)
- **ORM**: Entity Framework Core
- **Database**: PostgreSQL (Npgsql provider)
- **Authentication**: ASP.NET Core Identity
- **UI**: Bootstrap 5 + Razor Views

## Business Rules

1. Book stock decreases only when a librarian accepts a request
2. Book stock increases only when a librarian marks a loan as returned
3. Each accepted request creates exactly one Loan record
4. Loan due date = IssuedDate + 7 days
5. Students cannot have multiple pending requests at the same time
6. Students cannot request a book they already have on loan
7. Categories can be deleted - books are reassigned to "Uncategorized"
8. "Uncategorized" category cannot be deleted
9. Overdue status is automatic based on current date vs due date

## Database Configuration

The application uses PostgreSQL with the following connection string (configured in `Program.cs`):

```
Host=localhost;Port=5432;Database=ECommerceDB;Username=postgres;Password=password
```

## Getting Started

### Prerequisites

1. .NET 10 SDK
2. PostgreSQL server running on localhost:5432
3. PostgreSQL database credentials (default: postgres/password)

### Installation

1. Clone the repository
2. Ensure PostgreSQL is running
3. Run the application:
   ```bash
   dotnet run
   ```
4. The database will be automatically created and seeded on first run

### Default Credentials

**Librarian Account:**
- Email: librarian@school.com
- Password: Librarian123!

**Student Account:**
- Email: student@school.com
- Password: Student123!

Students can also self-register from the registration page.

## Project Structure

```
BookReservation/
├── Controllers/           # Student-facing controllers
├── Areas/
│   └── Librarian/        # Librarian-specific controllers and views
├── Models/               # Domain models
├── ViewModels/           # View models for forms
├── Services/             # Business logic services
├── Data/                 # DbContext and seeding
├── Views/                # Razor views for students
└── Areas/Librarian/Views/ # Razor views for librarians
```

## Key Models

- **ApplicationUser**: Extended Identity user with FullName
- **Book**: Book entity with availability tracking
- **Category**: Book categories with relationship management
- **BorrowRequest**: Student requests with status tracking
- **Loan**: Active and historical loan records

## Services

- **IBookService**: Book management operations
- **ICategoryService**: Category management with reassignment logic
- **IRequestService**: Request processing and eligibility checks
- **ILoanService**: Loan management and overdue tracking

## Routes

### Public Routes
- `/` - Home page
- `/Account/Login` - Login page
- `/Account/Register` - Student registration
- `/Books` - Browse books
- `/Books/Details/{id}` - Book details

### Student Routes (Requires Student Role)
- `/Requests/MyRequests` - My borrow requests
- `/Loans/MyActiveLoans` - My active loans
- `/Loans/MyHistory` - My loan history

### Librarian Routes (Requires Librarian Role)
- `/Librarian/Dashboard` - Dashboard
- `/Librarian/Books` - Manage books
- `/Librarian/Categories` - Manage categories
- `/Librarian/Requests/Pending` - Pending requests
- `/Librarian/Loans/Active` - Active loans
- `/Librarian/Loans/Overdue` - Overdue loans
- `/Librarian/Students` - View all students

## Database Migrations

The project uses EF Core migrations. The initial migration is automatically applied on startup.

To create additional migrations:
```bash
dotnet ef migrations add MigrationName
```

To update the database manually:
```bash
dotnet ef database update
```

## Seed Data

The application automatically seeds:
- Student and Librarian roles
- Default librarian account
- Sample student account
- "Uncategorized" category (required)
- Sample categories (Fiction, Science, History, Technology)
- Sample books with varying availability

## Security Features

- Password requirements enforced
- Role-based authorization
- Anti-forgery token validation
- Secure authentication cookies
- Server-side validation for all operations

## License

This is a sample educational project for demonstration purposes.
