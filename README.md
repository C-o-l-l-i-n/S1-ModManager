# Geek Pack Manager for Schedule 1

A lightweight Windows application built with VB.NET for automating the installation, updating, and uninstallation of the Geek Pack mod collection for the game **Schedule I**.

---

## 🚀 Features

- **🔍 Modpack Version Check**  
  Checks for the latest modpack version online and alerts the user when an update is available.

- **🔄 One-Click Install/Update**  
  Downloads and installs the latest version of MelonLoader and mod files. Automatically removes outdated files.

- **🧹 Complete Uninstallation**  
  Deletes all related mod files, folders, and components with one click.

- **📂 Smart Game Path Detection**  
  Automatically locates the Schedule I install directory via Steam registry, or prompts user to select it.

- **📋 Version & Log Management**  
  Displays current installed versions and logs operations to help with troubleshooting.

---

## 🛠 Tech Stack

- **Language:** VB.NET (.NET Framework)
- **UI:** WinForms
- **Utilities:**
  - Steam registry and folder parsing
  - WebClient for cloud file transfer
  - Zip extraction for mod file delivery

---

## 🌐 Cloud Dependencies

The application fetches modpack files and update instructions from the following sources:

- Modpack Version
- Application Version
- Update Executable URL
- Mod Files ZIP URL

---

## 📝 How to Use

1. **Download & Run** the Geek Pack Manager executable.
2. The app will:
    - Attempt to auto-locate the game via Steam
    - Check for modpack updates
    - Let you install or uninstall mods with a button click
3. Follow on-screen instructions for installation or uninstallation.

---

## 📄 License

This project is licensed under the **Apache License 2.0**.

---

## 👤 Author

**Collin Frame**  
[https://collinframe.com](https://collinframe.com)

---

## 💬 Contributing / Feedback

Suggestions, bug reports, or feature requests are welcome via GitHub issues or direct contact. Contributions not currently accepted without prior discussion.
