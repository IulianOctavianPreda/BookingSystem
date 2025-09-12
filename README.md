# Single Barber Booking System - Weekend Project

## Simple Business Goals

-   **Online Booking**: Customers can book appointments 24/7
-   **Avoid Double Bookings**: Simple conflict prevention
-   **Basic Management**: View and manage appointments
-   **Customer Contact**: Store customer details for follow-up

## Weekend Development Plan

### Day 1 - Backend Setup

#### Morning (4 hours)

-   [ ] **Project Setup** (30 min)

    -   Create .NET Web API project
    -   Add Entity Framework SQLite package
    -   Configure connection string

-   [ ] **Database & Models** (1.5 hours)

    -   Create Customer and Appointment entities
    -   Set up DbContext
    -   Create and run initial migration

-   [ ] **Basic Controllers** (2 hours)
    -   `CustomersController`: Basic CRUD
    -   `AppointmentsController`: Basic CRUD with availability check

#### Afternoon (4 hours)

-   [ ] **Business Logic** (2 hours)

-   [ ] **API Testing** (1 hour)

    -   Create HTTP test file
    -   Test all endpoints

-   [ ] **CORS Setup** (30 min)

    -   Configure CORS for Angular frontend

-   [ ] **Basic Validation** (30 min)
    -   Ensure no double bookings
    -   Validate appointment times

### Day 2 (Sunday) - Frontend Development

#### Morning (4 hours)

-   [ ] **Angular Setup** (1 hour)

    -   Create Angular project
    -   Install Spartan
    -   Set up basic routing

-   [ ] **Services** (1 hour)

-   [ ] **Booking Component** (2 hours)
    -   Date picker
    -   Time slot selection
    -   Customer info form
    -   Service selection (Haircut/Beard/Both)

#### Afternoon (4 hours)

-   [ ] **Admin Panel** (2 hours)

    -   Simple appointment list
    -   Mark as completed/cancelled
    -   Customer contact info

-   [ ] **Styling & UX** (1.5 hours)

    -   Make it look professional with Angular Material
    -   Mobile responsive
    -   Basic loading states

-   [ ] **Integration & Testing** (30 min)
    -   Connect frontend to backend
    -   End-to-end testing

## Key Features

### Customer-Facing

1. **Simple Booking Form**

    - Select date from calendar
    - Choose available time slot
    - Enter name, email, phone
    - Select service type
    - Confirm booking

2. **Booking Confirmation**
    - Show appointment details
    - Basic "Add to Calendar" link

### Admin Panel

1. **Today's Appointments**

    - List of today's bookings
    - Customer contact info
    - Mark as completed/cancelled

2. **All Appointments**
    - Weekly calendar view
    - Basic filtering by date

## Business Rules (Hardcoded)

-   **Hours**: Monday-Saturday 9 AM to 5 PM
-   **Slots**: 30-minute appointments
-   **Services**:
    -   Haircut (30 min)
    -   Beard (30 min)
    -   Haircut + Beard (60 min - takes 2 slots)
-   **Advance Booking**: Max 4 weeks ahead
-   **Same Day**: Allow bookings until 1 hour before

## Quick Implementation Tips

## Success Criteria

-   [ ] Customer can book an appointment online
-   [ ] System prevents double bookings
-   [ ] Admin can see daily appointments
-   [ ] Works on mobile phones
-   [ ] No crashes or major bugs

## Optional Enhancements (If Time Allows)

-   Email confirmation (use a service like EmailJS)
-   Basic calendar export
-   Simple appointment modification
-   Customer appointment history
