### 📄 **README.md (Secret Setup Section)**

````markdown
# 🔐 Email Configuration Setup (Secure)

This project uses **.NET User Secrets** to safely store sensitive data like email credentials during local development.  
User secrets are **stored outside the project folder** and are **not shared in GitHub** — so each developer must set them up locally.

---

## 🧠 Step 1: Initialize User Secrets

Run this command **inside the main Web project folder** (where your `.csproj` file is located):

```bash
dotnet user-secrets init
````

This adds a unique `UserSecretsId` to your project file and creates a secure storage location on your machine.

---

## ✉️ Step 2: Add Your Email Credentials

Run these commands to store your Gmail credentials (or any SMTP account):

```bash
dotnet user-secrets set "EmailSettings:Email" "youraddress@gmail.com"
dotnet user-secrets set "EmailSettings:Password" "your-app-password"
```

> ⚠️ **Important:**
> Do **not** use your regular Gmail password.
> You must create an **App Password** in your Google Account (under
> `Manage Account → Security → 2-Step Verification → App Passwords`).

Example:

```
App name: "MVCApp"
App password: "abcd efgh ijkl mnop"
```

Then use that 16-character code as your `"EmailSettings:Password"` value.

---

## ⚙️ Step 3: Verify It Works

You can verify your secret values (safe to check locally only):

```bash
dotnet user-secrets list
```

Expected output (your actual password will be masked):

```
EmailSettings:Email = youraddress@gmail.com
EmailSettings:Password = ************
```

---

## 🚀 Step 4: Run the App

Once configured, the app will automatically read these secrets from your local store at runtime — no need to modify `appsettings.json`.

```bash
dotnet run
```

When sending emails (like registration or password reset), the app will use your configured Gmail credentials.

---

## 🛡️ Notes

* User secrets are **specific to your local machine** and **not checked into GitHub**.
* Each developer must set their own secrets using the commands above.
* For production or shared environments, use **environment variables** or **a secure secrets manager** (e.g., Azure Key Vault, AWS Secrets Manager).

---

✅ That’s it! You’re all set to send emails securely without exposing credentials in source control.

