═══════════════════════════════════════════════════════════════
                    FinalPOS - Point of Sale System
                    Simple Installation Guide
═══════════════════════════════════════════════════════════════

📦 WHAT YOU NEED:
─────────────────
✅ MySQL Server (Download link below)
✅ Windows 7 or higher

⚠️ IMPORTANT: .NET Framework is usually already installed on Windows 10/11.
   If you get an error about .NET Framework, download it from Microsoft.

═══════════════════════════════════════════════════════════════
STEP 1: INSTALL MYSQL SERVER
═══════════════════════════════════════════════════════════════

Download MySQL from one of these options:

Option A - MySQL Official (Recommended):
https://dev.mysql.com/downloads/installer/
→ Download "MySQL Installer for Windows"
→ Choose "Full" or "Server only" installation
→ During installation, remember your root password (or leave blank)

Option B - XAMPP (Easier, includes MySQL):
https://www.apachefriends.org/
→ Download XAMPP for Windows
→ Install and start MySQL from XAMPP Control Panel

═══════════════════════════════════════════════════════════════
STEP 2: RUN THE APPLICATION
═══════════════════════════════════════════════════════════════

1. Make sure MySQL is running
   - Check Windows Services (search "services" in Start menu)
   - Look for "MySQL" service - it should be "Running"

2. Double-click: FinalPOS.exe

3. First Time Setup:
   - If MySQL has NO password: App starts automatically ✅
   - If MySQL has password: Enter your MySQL root password when prompted
   - Database will be created automatically
   - Tables will be created automatically

4. Login:
   - Username: admin
   - Password: admin
   - ⚠️ Change password after first login!

═══════════════════════════════════════════════════════════════
TROUBLESHOOTING
═══════════════════════════════════════════════════════════════

❌ Error: "Database initialization failed"
   → Check if MySQL is running
   → Verify MySQL is on port 3306
   → Enter correct MySQL password if prompted

❌ Error: ".NET Framework not found"
   → Download from: https://dotnet.microsoft.com/download/dotnet-framework/net472
   → Install and restart computer

❌ Error: "Missing DLL files"
   → Make sure ALL files are in the same folder
   → Don't delete any DLL files
   → Extract all files from zip

❌ MySQL Connection Error
   → Open MySQL Workbench or command line
   → Test connection: mysql -u root -p
   → If it works there, the app should work too

═══════════════════════════════════════════════════════════════
QUICK START CHECKLIST
═══════════════════════════════════════════════════════════════

☐ MySQL Server installed
☐ MySQL service is running
☐ All files extracted to same folder
☐ Double-click FinalPOS.exe
☐ Enter MySQL password if prompted
☐ Login with: admin / admin

═══════════════════════════════════════════════════════════════
SUPPORT
═══════════════════════════════════════════════════════════════

If you have issues:
1. Check if MySQL is running
2. Verify all files are in the same folder
3. Check error message for specific issue
4. Make sure .NET Framework 4.7.2+ is installed

═══════════════════════════════════════════════════════════════

