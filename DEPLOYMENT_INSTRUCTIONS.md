# FinalPOS Deployment Instructions

## 📦 How to Deploy FinalPOS to Another PC

### Step 1: Build the Project in Release Mode

1. Open the project in Visual Studio
2. Go to **Build** → **Configuration Manager**
3. Set **Active solution configuration** to **Release**
4. Click **Build** → **Build Solution** (or press `F6`)
5. Wait for the build to complete successfully

### Step 2: Locate the Release Folder

The compiled files are in:
```
FinalPOS\bin\Release\
```

### Step 3: Copy All Files from Release Folder

**IMPORTANT:** Copy the **ENTIRE** `Release` folder contents, not just the EXE!

You need to copy:
- ✅ `FinalPOS.exe` (main executable)
- ✅ `FinalPOS.exe.config` (configuration file - REQUIRED!)
- ✅ **ALL DLL files** (26+ files including):
  - `MySql.Data.dll`
  - `MetroFramework.dll`, `MetroFramework.Design.dll`, `MetroFramework.Fonts.dll`
  - `Microsoft.ReportViewer.*.dll` (multiple files)
  - `BouncyCastle.Cryptography.dll`
  - `Google.Protobuf.dll`
  - And 20+ more DLL files
- ✅ **All localization folders** (optional but recommended):
  - `de\`, `es\`, `fr\`, `it\`, `ja\`, `ko\`, `pt\`, `ru\`, `zh-CHS\`, `zh-CHT\`

### Step 4: Create Deployment Package

**Option A: Zip File (Recommended)**
1. Select all files in the `Release` folder
2. Right-click → **Send to** → **Compressed (zipped) folder**
3. Name it: `FinalPOS_Deployment.zip`

**Option B: USB Drive**
1. Copy the entire `Release` folder to a USB drive
2. Or create a folder on USB and copy all files

### Step 5: Prerequisites on Friend's PC

Before running the application, ensure these are installed:

#### 1. .NET Framework 4.7.2 or Higher
- **Check if installed:**
  - Press `Win + R`
  - Type: `appwiz.cpl` and press Enter
  - Look for ".NET Framework 4.7.2" or higher in the list
  
- **If not installed, download:**
  - Download from: https://dotnet.microsoft.com/download/dotnet-framework/net472
  - Install and restart if required

#### 2. MySQL Server
- **Must be installed and running**
- **Port:** 3306 (default MySQL port)
- **Root user:** Can have password or no password
  - If password exists, the app will prompt for it on first run
  - If no password, it will work automatically

**MySQL Installation:**
- Download MySQL from: https://dev.mysql.com/downloads/installer/
- Or use XAMPP (includes MySQL): https://www.apachefriends.org/
- Make sure MySQL service is **running**

### Step 6: Deploy to Friend's PC

1. **Extract/Copy files:**
   - Extract the zip file OR copy the Release folder
   - Place in a folder like: `C:\FinalPOS\` or `C:\Program Files\FinalPOS\`

2. **Verify files:**
   - Make sure `FinalPOS.exe` and `FinalPOS.exe.config` are in the same folder
   - Make sure all DLL files are present

### Step 7: Run the Application

1. **Double-click** `FinalPOS.exe`
2. **First run behavior:**
   - If MySQL has **no password**: App starts automatically
   - If MySQL has **password**: A password prompt will appear
     - Enter the MySQL root password
     - Click OK
     - Password will be saved for future use

3. **Database setup (automatic):**
   - Database `POS_NEXA_ERP` will be created automatically
   - All tables will be created automatically
   - Default admin user will be created:
     - **Username:** `admin`
     - **Password:** `admin`

4. **Login:**
   - Use the default credentials to log in
   - Change password after first login (recommended)

### Step 8: Troubleshooting

#### Error: "Database initialization failed"
**Solutions:**
- ✅ Check if MySQL Server is running
- ✅ Verify MySQL is on port 3306
- ✅ Check if root user has permission to create databases
- ✅ If password prompt appears, enter correct MySQL root password

#### Error: "Missing DLL" or "Could not load file"
**Solutions:**
- ✅ Make sure ALL files from Release folder are copied
- ✅ All DLL files must be in the same folder as FinalPOS.exe
- ✅ Check if .NET Framework 4.7.2+ is installed

#### Error: "MySQL connection failed"
**Solutions:**
- ✅ Verify MySQL service is running (check Windows Services)
- ✅ Test MySQL connection using MySQL Workbench or command line
- ✅ Check firewall settings (port 3306 should be accessible)

### Quick Checklist

Before sending to friend:
- [ ] Project built in Release mode
- [ ] All files from `bin\Release\` folder copied
- [ ] `FinalPOS.exe.config` included
- [ ] All DLL files included
- [ ] Files packaged (zip or folder)

Friend's PC requirements:
- [ ] .NET Framework 4.7.2+ installed
- [ ] MySQL Server installed
- [ ] MySQL service running
- [ ] Port 3306 available

### Default Login Credentials

After first run, use these to log in:
- **Username:** `admin`
- **Password:** `admin`

**⚠️ IMPORTANT:** Change the password after first login!

---

## 📝 Summary

1. **Build** project in Release mode
2. **Copy** entire `bin\Release\` folder
3. **Send** to friend (zip or USB)
4. **Install** prerequisites (.NET Framework, MySQL)
5. **Run** `FinalPOS.exe`
6. **Enter** MySQL password if prompted
7. **Login** with admin/admin

The application will automatically:
- ✅ Create the database
- ✅ Create all tables
- ✅ Set up initial data
- ✅ Save MySQL password for future use

