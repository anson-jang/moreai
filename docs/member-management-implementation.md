# 會員管理系統實作記錄

## 功能實作清單

### 1. 登入功能
- 建立 `LoginViewModel` 模型
- 實作 `AccountController` 的登入方法
- 創建登入頁面視圖 (`Login.cshtml`)
- 添加密碼驗證邏輯
- 實作 Cookie 身份驗證

### 2. 註冊功能
- 建立 `RegisterViewModel` 模型
- 實作 `AccountController` 的註冊方法
- 創建註冊頁面視圖 (`Register.cshtml`)
- 添加用戶名和電子郵件唯一性驗證
- 實作密碼雜湊功能

### 3. 安全性實作
- 使用 HMACSHA512 進行密碼雜湊
- 實作 CSRF 防護
- 添加密碼複雜度要求
- 實作記住我功能

## 主要檔案變更

### 模型（Models）
1. `LoginViewModel.cs` - 登入表單模型
2. `RegisterViewModel.cs` - 註冊表單模型
3. `Member.cs` - 會員資料模型

### 控制器（Controllers）
- `AccountController.cs` - 處理會員註冊、登入和登出

### 視圖（Views）
1. `Views/Account/Login.cshtml` - 登入頁面
2. `Views/Account/Register.cshtml` - 註冊頁面
3. `Views/Shared/_LoginPartial.cshtml` - 登入狀態局部視圖

### 輔助類別
- `Helpers/PasswordHelper.cs` - 密碼雜湊和驗證

### 資料庫相關
- `ApplicationDbContext.cs` - 資料庫上下文
- 建立初始遷移 `InitialCreate`

## 重要設定

### 身份驗證設定（Program.cs）
```csharp
builder.Services.AddAuthentication("CookieAuth")
    .AddCookie("CookieAuth", options =>
    {
        options.Cookie.Name = "UserLoginCookie";
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromDays(30);
    });
```

### 資料庫連接字串（appsettings.Development.json）
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Port=3306;Database=moreai_db;User=root;Password=esun@1313;CharSet=utf8mb4;"
  }
}
```

## 使用說明

### 登入功能
1. 訪問 `/Account/Login` 或點擊頁面右上角的「登入」按鈕
2. 輸入使用者名稱和密碼
3. 可選擇「記住我」功能
4. 點擊登入按鈕

### 註冊功能
1. 訪問 `/Account/Register` 或從登入頁面點擊「立即註冊」
2. 填寫必要資訊：
   - 名字和姓氏
   - 使用者名稱（唯一）
   - 電子郵件（唯一）
   - 密碼（至少 6 個字元）
3. 點擊註冊按鈕
4. 註冊成功後會自動登入

## 注意事項
1. 密碼要求：至少 6 個字元
2. 使用者名稱和電子郵件必須是唯一的
3. 資料庫需要先建立並執行遷移
4. 需要配置正確的資料庫連接字串

## 待優化項目
1. 添加電子郵件驗證功能
2. 實作密碼重設功能
3. 加強密碼複雜度要求
4. 添加帳號鎖定機制
5. 實作雙因素驗證
6. 優化錯誤處理和日誌記錄