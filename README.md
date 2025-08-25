# 🏥 HEISCO Medical System
A lightweight, modern, and responsive ASP.NET Web Forms application for managing employee medical records at HEISCO. Designed with usability and scalability in mind, this system streamlines medical visit tracking, approval workflows, external document uploads, and dynamic reporting — all within a clean, role-based access framework.

## ✨ Features
### 🔒 Role-Based Access Control (RBAC)
Manage users and assign screen-level permissions dynamically through roles.

### 🎨 Modern UI with Consistent Theme
Clean, intuitive interface with a professional color palette ensuring great usability.

### 🩺 Employee Medical Visit Management
Record, track, and review detailed employee medical history and visits.

### 📂 External Visit Uploads
Upload and reference external medical documents directly linked to employee profiles.

### 👤 User and Company Management
Fully functional management screens for creating companies, users, and assigning them appropriate access.

### 🛡️ Dynamic Navigation Menu
Navigation links generated dynamically based on user's role permissions — no hardcoded links.

### 📋 Validation and Error Handling
Strong client-side and server-side validation using ASP.NET Web Forms validators.

### 📊 Reports Screen
Generate detailed reports filtered by employee ID, date, cost center, and visit count, with clean formatting and export in PDF format for printing.

### 🧾 Medical History Screen
Displays a comprehensive view of each employee’s medical visits and past history in collapsible sections for easy navigation.

## 🧠 Tech Stack
Layer	Technology
Frontend	ASP.NET Web Forms (.aspx)
Backend	C# (.NET Framework)
Database	Microsoft SQL Server

## 📸 Screenshots
![photo-collage png](https://github.com/user-attachments/assets/b1d6a009-1793-4aa0-8baa-271a0b4e5ccb)


## ⚙️ Setup Instructions

### 1. Clone the Repository
```bash
git clone https://github.com/Navaneethp360/heisco-medical-system.git
```
### 2. Open the Solution
- Open the `.sln` file using **Visual Studio 2022** (recommended).

### 3. Database Setup
- Import the provided `MedicalSystemDB` file or use the provided SQL Script `MedicalDBscript.sql` into your SQL Server instance to create the required tables:

- Update the `DefaultConnection` string in the `Web.config` file with your database credentials.

### 4. Run the Application
- Press **F5** in Visual Studio to build and run the application locally.

## 🧩 Future Enhancements

📥 Bulk import of employee medical records via Excel/CSV.

🔔 Email notifications for follow-ups and recheck reminders.

## 🙌 Contributing
Contributions are welcome!
If you find a bug or want to propose a feature:

Fork the repo

Create a new branch

Submit a pull request

Note: Please maintain the code formatting and naming conventions used in the project.

## 📜 License
This project is licensed under the MIT License.

## 🤝 Acknowledgements
Special thanks to all contributors and the staff at HEISCO who helped define the workflows and processes integrated into this system.

