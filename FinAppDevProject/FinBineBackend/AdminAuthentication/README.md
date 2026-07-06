# AdminAuthentication Module

This folder contains all code related to **admin authentication** in the FinBine backend.  
It handles verifying Firebase ID tokens and exposing a secure login endpoint for the frontend.

---

## 📂 Folder Structure

- **Controllers/**
  - `AdminAuthController.cs`  
    - Exposes the `POST /api/admin/login` endpoint.  
    - Accepts a Firebase ID token from the frontend.  
    - Calls the service to verify the token.  
    - Returns `200 OK` with user info if valid, or `401 Unauthorized` if invalid.

- **Services/**
  - `AdminAuthService.cs`  
    - Initializes Firebase Admin SDK (using service account credentials).  
    - Verifies ID tokens with Firebase.  
    - Extracts UID and email from the token.  
    - Returns a structured `AdminLoginResponse`.

- **Models/**
  - `AdminLoginRequest.cs`  
    - DTO for incoming login requests.  
    - Contains the Firebase ID token (`Token`).
  - `AdminLoginResponse.cs`  
    - DTO for responses back to the frontend.  
    - Contains `Success`, `Message`, `AdminId`, and `Email`.

---

## 🔑 Authentication Flow

1. **Frontend (Vue)**  
   - User logs in with Firebase (email/password or SSO).  
   - Firebase returns an **ID token**.  
   - Frontend sends `{ token }` to `POST /api/admin/login`.

2. **Backend (ASP.NET Core)**  
   - `AdminAuthController` receives the request.  
   - Passes the token to `AdminAuthService`.  
   - Service verifies the token with Firebase Admin SDK.  
   - If valid → returns `Success = true` with UID + email.  
   - If invalid → returns `Success = false` with error message.

3. **Frontend (Vue)**  
   - On success → redirects to `/admin/dashboard`.  
   - On failure → shows error message.

---

## ⚖️ Notes
- No `"admin": true` claim check is required because this Firebase project is **only for admin users**.  
- Service account JSON (`finbineadmin-firebase-adminsdk-fbsvc-ed0487c71b.json`) must be kept **secure** and never committed to GitHub.  
- All exceptions are handled gracefully to avoid backend crashes.

---

## 🎯 Summary
This module provides a clean separation of concerns:
- **Controller** → API endpoint.  
- **Service** → Firebase verification logic.  
- **Models** → Request/response contracts.  

Together, they ensure only authenticated Firebase users can access the admin dashboard.
