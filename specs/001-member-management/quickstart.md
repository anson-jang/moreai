# Quick Start Guide: 會員管理系統

**Feature**: 會員管理系統 | **Version**: 1.0 | **Date**: 2024年10月25日

## 系統概述

會員管理系統是一個完整的會員註冊、認證和管理解決方案，整合到現有的 SmartAdmin ASP.NET Core 框架中。提供會員自助註冊、管理員會員管理、群組權限控制和安全審計功能。

## 快速設定

### 1. 環境需求

**軟體需求**:
- .NET 9.0 SDK
- MySQL 8.0 Server  
- Visual Studio 2022 或 VS Code
- Git

**硬體需求**:
- 記憶體: 最少 4GB，建議 8GB
- 硬碟: 至少 2GB 可用空間
- 處理器: 支援 x64 架構

### 2. 資料庫設定

#### 建立資料庫
```sql
-- 連接到 MySQL 並建立資料庫
CREATE DATABASE SmartAdminDb CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
USE SmartAdminDb;
```

#### 執行資料庫腳本
```bash
# 在專案根目錄執行
cd src/Smartadmin/Data/Scripts
mysql -u root -p SmartAdminDb < 001_create_members_table.sql
mysql -u root -p SmartAdminDb < 002_create_groups_table.sql  
mysql -u root -p SmartAdminDb < 003_seed_default_groups.sql
```

### 3. 專案設定

#### 更新 appsettings.json
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=SmartAdminDb;Uid=root;Pwd=your-password;charset=utf8mb4"
  },
  "Authentication": {
    "JwtSecret": "your-super-secret-jwt-key-here",
    "JwtIssuer": "SmartAdmin",
    "JwtAudience": "SmartAdmin-Users",
    "SessionExpirationHours": 24
  },
  "Email": {
    "Provider": "SendGrid", // or "SMTP"
    "SendGrid": {
      "ApiKey": "your-sendgrid-api-key",
      "FromEmail": "noreply@yourdomain.com",
      "FromName": "SmartAdmin System"
    },
    "SMTP": {
      "Host": "smtp.gmail.com",
      "Port": 587,
      "Username": "your-email@gmail.com",
      "Password": "your-app-password",
      "EnableSsl": true
    }
  },
  "MemberManagement": {
    "PasswordComplexity": {
      "MinLength": 8,
      "RequireUppercase": true,
      "RequireLowercase": true,
      "RequireDigit": true,
      "RequireSpecialChar": true
    },
    "AccountLocking": {
      "MaxFailedAttempts": 5,
      "LockoutDurationMinutes": 30
    },
    "EmailVerification": {
      "LinkExpirationMinutes": 5,
      "RequireVerification": true
    }
  }
}
```

#### 安裝 NuGet 套件
```bash
# 在 Smartadmin 專案目錄中執行
dotnet add package Microsoft.AspNetCore.Identity --version 9.0.0
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer --version 9.0.0
dotnet add package Dapper --version 2.1.35
dotnet add package MySql.Data --version 9.0.0
dotnet add package SendGrid --version 9.29.3
dotnet add package Newtonsoft.Json --version 13.0.3
```

### 4. 啟動應用程式

```bash
# 建置專案
dotnet build

# 執行專案
dotnet run --project src/Smartadmin/Smartadmin.csproj
```

預設會在 `https://localhost:5001` 啟動應用程式。

## 基本使用

### 1. 首次登入

**預設管理員帳號**:
- 電子郵件: `ansonjang@gmail.com`
- 密碼: 需要先透過資料庫直接設定或使用密碼重設功能

### 2. 導航到會員管理

1. 登入後點選左側選單的「會員管理」
2. 可以看到會員清單、新增會員、群組管理等功能

### 3. 建立第一個會員

1. 點選「新增會員」按鈕
2. 填入會員資料:
   - 暱稱: 必填，1-50字元
   - 電子郵件: 必填，格式驗證
   - 密碼: 必填，至少8字元包含大小寫字母數字特殊字符
   - 電話號碼: 選填
   - 群組: 選擇會員所屬群組
3. 點選「儲存」建立會員

### 4. 測試會員註冊流程

1. 開啟瀏覽器無痕模式
2. 前往 `/Auth/Register` 頁面
3. 填寫註冊表單
4. 檢查電子郵件收到驗證連結
5. 點選驗證連結啟用帳號
6. 使用新帳號登入測試

## 主要功能說明

### 會員管理功能

| 功能 | 路徑 | 說明 |
|------|------|------|
| 會員清單 | `/Member` | 查看、搜尋、篩選所有會員 |
| 新增會員 | `/Member/Create` | 管理員建立新會員帳號 |
| 編輯會員 | `/Member/Edit/{id}` | 修改會員基本資料 |
| 會員詳情 | `/Member/Details/{id}` | 查看會員完整資訊 |
| 鎖定會員 | `/Member/Lock/{id}` | 鎖定/解鎖會員帳號 |
| 批量操作 | `/Member/BulkOperation` | 批量啟用/停用/刪除會員 |

### 認證功能

| 功能 | 路徑 | 說明 |
|------|------|------|
| 會員登入 | `/Auth/Login` | 會員登入系統 |
| 會員註冊 | `/Auth/Register` | 會員自助註冊 |
| 密碼重設 | `/Auth/ForgotPassword` | 忘記密碼重設流程 |
| 電子郵件驗證 | `/Auth/VerifyEmail` | 驗證電子郵件地址 |
| 登出 | `/Auth/Logout` | 安全登出系統 |

### 群組管理功能

| 功能 | 路徑 | 說明 |
|------|------|------|
| 群組清單 | `/Group` | 管理所有會員群組 |
| 建立群組 | `/Group/Create` | 建立新的會員群組 |
| 群組成員 | `/Group/Members/{id}` | 管理群組成員關係 |

### API 端點

所有功能都提供對應的 REST API 端點，基礎路徑為 `/api/v1/`：

- 認證 API: `/api/v1/auth/*`
- 會員 API: `/api/v1/members/*`  
- 群組 API: `/api/v1/groups/*`
- 報告 API: `/api/v1/reports/*`

## 權限與角色

### 預設群組

| 群組代碼 | 群組名稱 | 權限描述 |
|----------|----------|----------|
| `ADMIN` | 管理者 | 完整系統管理權限，可管理所有會員和群組 |
| `MEMBER` | 一般會員 | 基本會員權限，可查看和編輯自己的資料 |
| `PREMIUM` | 付費會員 | 進階會員權限，可使用付費功能 |

### 權限控制

- **頁面權限**: 透過 `[Authorize]` 屬性控制
- **功能權限**: 透過群組成員身份檢查
- **資料權限**: 會員只能存取自己的資料，管理員可存取所有資料

## 安全設定

### 密碼安全
- 最小長度: 8 字元
- 必須包含: 大寫字母、小寫字母、數字、特殊字符
- 使用 ASP.NET Core Identity 標準雜湊演算法

### 帳號安全
- 登入失敗 5 次後鎖定 30 分鐘
- 電子郵件驗證連結 5 分鐘有效
- 密碼重設連結 5 分鐘有效
- 登入會議預設 24 小時有效

### 通訊安全
- 強制 HTTPS 連線
- JWT 令牌用於 API 認證
- Cookie 用於 Web 界面認證
- CSRF 防護已啟用

## 疑難排解

### 常見問題

**Q: 無法連接到資料庫**
```
A: 檢查以下項目:
1. MySQL 服務是否啟動
2. 連接字符串是否正確
3. 資料庫是否已建立
4. 使用者權限是否足夠
```

**Q: 電子郵件無法發送**
```
A: 檢查以下項目:
1. SendGrid API 金鑰是否正確
2. SMTP 設定是否正確
3. 防火牆是否阻擋郵件服務埠
4. 電子郵件服務商是否有使用限制
```

**Q: 會員無法登入**
```
A: 檢查以下項目:
1. 帳號是否已啟用 (IsActive = true)
2. 電子郵件是否已驗證 (IsEmailVerified = true)
3. 帳號是否被鎖定 (IsLocked = false)
4. 密碼是否正確
```

**Q: JWT 令牌驗證失敗**
```
A: 檢查以下項目:
1. JWT 密鑰設定是否正確
2. 令牌是否已過期
3. 令牌格式是否正確
4. HTTP Header 是否包含 Authorization
```

### 日誌檢查

查看應用程式日誌以診斷問題：

```bash
# 檢查應用程式日誌
tail -f logs/SmartAdmin-*.log

# 檢查系統日誌 (Windows)
Get-EventLog -LogName Application -Source "SmartAdmin"

# 檢查系統日誌 (Linux)
tail -f /var/log/syslog | grep SmartAdmin
```

### 開發工具

**API 測試**:
- 使用內建 Swagger UI: `https://localhost:5001/swagger`
- 或使用 Postman 匯入 API 規格

**資料庫管理**:
- MySQL Workbench
- phpMyAdmin
- DBeaver

## 效能調校

### 資料庫優化
```sql
-- 建立索引提升查詢效能
CREATE INDEX idx_member_email_active ON Member(Email, IsActive);
CREATE INDEX idx_member_search ON Member(NickName, Email);
CREATE INDEX idx_session_token_expires ON LoginSession(SessionToken, ExpiresAt);
```

### 應用程式快取
```csharp
// 在 Startup.cs 中啟用記憶體快取
services.AddMemoryCache();

// 快取群組資訊
services.Configure<CacheOptions>(options => {
    options.GroupCacheDurationMinutes = 30;
    options.MemberCacheDurationMinutes = 10;
});
```

### 監控設定
```json
// appsettings.json 中啟用效能監控
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "MemberManagement": "Debug"
    }
  },
  "Performance": {
    "EnableMetrics": true,
    "SlowQueryThresholdMs": 1000,
    "EnableQueryLogging": true
  }
}
```

## 下一步

1. **客製化界面**: 根據需求調整 UI/UX 設計
2. **整合外部系統**: 如單一登入 (SSO)、第三方認證
3. **進階權限**: 實作更細緻的權限控制
4. **報告分析**: 建立會員行為分析和報告
5. **行動應用**: 開發行動 App 支援
6. **效能監控**: 建立完整的監控和告警系統

## 支援資源

- **技術文件**: `/specs/001-member-management/`
- **API 文件**: `/specs/001-member-management/contracts/`
- **資料模型**: `/specs/001-member-management/data-model.md`
- **問題回報**: 透過 Git Issues 回報問題
- **功能建議**: 透過 Git Discussions 提出建議

---

**注意**: 這是快速啟動指南，詳細的技術實作請參考相關的技術文件和 API 規格。