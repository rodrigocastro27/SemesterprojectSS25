# Flutter Setup

## System Requirements

* **Disk Space:** 2.5 GB minimum (excluding IDE/tools)

### 🪟 Windows

* **OS:** Windows 10 (v1803) or later, 64-bit
* **Tools Required:**
  * Git for Windows
  * Windows PowerShell 5.0+
  * Visual Studio Code
  * (*Optional*) Android Studio for device emulation and SDK management

### 🍎 macOS

* **OS:** macOS 12 Monterey or later (recommended: latest stable)
* **Tools Required:**
  * Xcode (install via App Store)
  * Git (pre-installed or install with `brew install git`)

⚠️ If building for iOS: you must use a macOS system with Xcode installed and configured.

### 🐧 Linux

* **OS:** Any modern 64-bit Linux distro (e.g., Ubuntu 20.04+)
* **Tools Required:**
  * Git (`sudo apt install git`)
  * bash, mkdir, rm, unzip, which (usually pre-installed)
  * curl or wget
  * libGLU, GTK, and related dependencies for desktop GUI:

    ```sh
    sudo apt install libgl1-mesa-dev xz-utils curl unzip
    ```

## 📥 Installing Flutter

1. Download the SDK from the [official site](https://docs.flutter.dev/get-started/install).

2. Extract it to a folder (for example):
   * Windows: `C:\src\flutter`
   * macOS/Linux: `~/development/flutter`

3. Add Flutter to your system `PATH`:
   * Windows:
     * Search for "Environment Variables"
     * Edit Path and add: `C:\src\flutter\bin`

   * macOS/Linux (in .bashrc, .zshrc, etc.):

        ```bash
        export PATH="$PATH:$HOME/development/flutter/bin"
        ```

## 🧪 Run Flutter Doctor

Open a terminal and run:

```bash
flutter doctor
```

This checks:
* Flutter installation
* Dart SDK
* Android SDK (if installed)
* Connected devices
* Required dependencies (like Xcode)

Follow the suggestions it gives to fix any issues.

## 🧱 Android SDK Setup

If you're not using Android Studio, you **must** install the Android SDK manually:

1. Download command line tools.
2. Set environment variables:

    ```bash
    export ANDROID_HOME=$HOME/Android/Sdk
    export PATH=$PATH:$ANDROID_HOME/emulator
    export PATH=$PATH:$ANDROID_HOME/tools
    export PATH=$PATH:$ANDROID_HOME/tools/bin
    export PATH=$PATH:$ANDROID_HOME/platform-tools
    ```

3. Accept licenses:

    ```bash
    yes | sdkmanager --licenses
    ```

📦 Or just [install Android Studio](https://developer.android.com/studio/install) and use its built-in *SDK Manager* and *AVD Manager* to set the same variables and accept the licenses.

## 👨‍💻 Using VS Code for Flutter

1. Install VS Code: https://code.visualstudio.com
2. Install Extensions:
   * Open Extensions panel (Ctrl+Shift+X)
   * Install:
     * ``Flutter``
     * ``Dart``
3. Open your project:
   * Clone the repository from Git
   * In the terminal, open the folder of the repository `SemesterProjectSS25`
   * Then execute:
        ```sh
        cd semester_project
        ```
4. Select a device:
   * Connect a real device via USB with **USB debuggin enbaled** in the *Developer Settings*.
   * Create and launch an emulator in Android Studio
   * For easy access to the devices, access the command palette (`Ctrl+Shift+P`) and type "*Flutter: Select Device*" and click on the desired one (they only appear if they are connected).

5. Run the app:
   * Press F5, or
   * Use the terminal:

        ```sh
        flutter run     # in the semester_project directory
        ```
   * Right click on the `main.dart` file and choose `Run With/Without Debugging`.
   * In the `DEBUG CONSOLE` the loggs should show the building and launching process of the app, as well as app messages.

⚠️ Make sure the [server is running](server-setup) before launching the app, otherwise the device won't establish a connection.