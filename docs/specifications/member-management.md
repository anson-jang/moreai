# 會員管理模組規格說明

## 功能概述
實作完整的會員管理系統，包含會員註冊、登入、資料維護、密碼重設、群組管理等功能。

## 詳細功能規格

### 1. 會員註冊
- **輸入欄位**
  - 暱稱
  - 電子郵件（唯一識別碼）
  - 密碼（8-16字元，需包含英數字及特殊符號）
- **流程**
  1. 使用者填寫註冊表單
  2. 系統驗證電子郵件是否重複
  3. 密碼加密儲存
  4. 發送驗證信件（含啟用連結）
  5. 使用者點擊啟用連結完成註冊

### 2. 會員登入
- **輸入欄位**
  - 電子郵件
  - 密碼
- **驗證規則**
  - 僅允許已啟用的帳號登入
  - 密碼驗證通過後導向主頁面

### 3. 會員資料維護
- **可維護欄位**
  - 電話
  - 地址
  - 公司名稱
- **功能**
  - 新增
  - 修改
  - 刪除

### 4. 密碼重設
- **功能流程**
  1. 登入頁面提供「啟動密碼重設」連結
  2. 輸入電子郵件後發送重設信件
  3. 點擊信件中的重設連結
  4. 輸入新密碼（需符合密碼規則）
  5. 加密儲存新密碼

### 5. 群組維護（管理者專用）
- **欄位**
  - 群組名稱
  - 群組代碼（唯一識別碼）
- **預設群組**
  - 管理者
  - 一般會員
  - 付費會員
- **功能**
  - 新增群組
  - 修改群組名稱（不可修改群組代碼）
  - 刪除群組

### 6. 會員群組指派（管理者專用）
- 允許管理者指派會員至多個群組
- 預設管理者帳號：ansonjang@gmail.com

## 技術規格

### 資料庫設計
1. Members 表
   - Id (PK)
   - Nickname
   - Email (Unique)
   - PasswordHash
   - Phone
   - Address
   - CompanyName
   - IsActivated
   - CreatedAt
   - UpdatedAt

2. Groups 表
   - Id (PK)
   - Code (Unique)
   - Name
   - CreatedAt
   - UpdatedAt

3. MemberGroups 表
   - MemberId (FK)
   - GroupId (FK)
   - CreatedAt

### 安全考量
1. 密碼加密使用 ASP.NET Core Identity 的密碼雜湊機制
2. 電子郵件驗證連結需包含時效性
3. 實作 CSRF 防護
4. 密碼重設連結需有時效性限制

### API 端點
1. 會員註冊: POST /api/members/register
2. 會員登入: POST /api/members/login
3. 會員資料: 
   - GET /api/members/{id}
   - PUT /api/members/{id}
4. 密碼重設:
   - POST /api/members/reset-password/initiate
   - POST /api/members/reset-password/complete
5. 群組管理:
   - GET /api/groups
   - POST /api/groups
   - PUT /api/groups/{id}
   - DELETE /api/groups/{id}
6. 會員群組:
   - GET /api/members/{id}/groups
   - POST /api/members/{id}/groups
   - DELETE /api/members/{id}/groups/{groupId}

## 使用者介面
- 使用 Bootstrap 5 框架
- 實作響應式設計
- 表單驗證使用 client-side 和 server-side 雙重驗證
- 密碼強度即時檢查
- AJAX 非同步處理所有表單提交