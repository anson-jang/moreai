# 會員管理模組實作計畫

## 技術堆疊
1. 後端框架：ASP.NET Core 8.0
2. 資料庫：
   - MySQL
   - Dapper 作為 ORM
3. 身份驗證：
   - ASP.NET Core Identity
   - JWT Token 處理
4. 前端技術：
   - Bootstrap 5
   - jQuery
   - AJAX
   - axios
5. 郵件服務：
   - SMTP 或 SendGrid

## 實作階段

### 階段 1：基礎設施建置
1. 資料庫設計與實作
   - 建立 Entity Framework Core DbContext
   - 實作資料模型（Member, Group, MemberGroup）
   - 設定資料庫索引和關聯
   - 建立初始遷移

2. Identity 框架整合
   - 設定 ASP.NET Core Identity
   - 自定義 User 和 Role 模型
   - 配置密碼政策
   - 設定 JWT 認證

3. 郵件服務設定
   - 實作郵件發送服務介面
   - 配置郵件範本
   - 設定 SMTP 或 SendGrid

### 階段 2：核心功能實作
1. 會員註冊功能
   - 實作註冊 API
   - 電子郵件驗證機制
   - 密碼加密處理
   - 註冊表單驗證

2. 會員登入功能
   - 實作登入 API
   - JWT Token 生成與驗證
   - 登入狀態管理
   - 登入表單驗證

3. 會員資料管理
   - CRUD 操作實作
   - 資料驗證邏輯
   - 權限檢查機制

### 階段 3：進階功能實作
1. 密碼重設功能
   - 重設請求處理
   - 安全連結生成
   - 密碼更新機制

2. 群組管理功能
   - 群組 CRUD 操作
   - 權限檢查機制
   - 預設群組設定

3. 會員群組指派
   - 群組指派介面
   - 多重群組關聯處理
   - 權限驗證

## 資料模型詳細設計

### Members
```csharp
public class Member
{
    public Guid Id { get; set; }
    public string Nickname { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string? CompanyName { get; set; }
    public bool IsActivated { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public ICollection<MemberGroup> MemberGroups { get; set; }
}
```

### Groups
```csharp
public class Group
{
    public Guid Id { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public ICollection<MemberGroup> MemberGroups { get; set; }
}
```

### MemberGroups
```csharp
public class MemberGroup
{
    public Guid MemberId { get; set; }
    public Member Member { get; set; }
    public Guid GroupId { get; set; }
    public Group Group { get; set; }
    public DateTime CreatedAt { get; set; }
}
```

## API 規格詳細設計

### 1. 會員註冊 API
```
POST /api/members/register
Content-Type: application/json

Request:
{
    "nickname": "string",
    "email": "string",
    "password": "string"
}

Response:
{
    "success": boolean,
    "message": "string",
    "memberId": "guid"
}
```

### 2. 會員登入 API
```
POST /api/members/login
Content-Type: application/json

Request:
{
    "email": "string",
    "password": "string"
}

Response:
{
    "success": boolean,
    "token": "string",
    "expiration": "datetime"
}
```

## 安全性考量
1. 密碼安全
   - 使用 ASP.NET Core Identity 的密碼雜湊
   - 密碼政策強制執行
   - 密碼重設限制與冷卻時間

2. 授權控制
   - JWT Token 基於角色的存取控制
   - API 端點授權過濾器
   - XSS 防護措施

3. 資料保護
   - HTTPS 強制執行
   - CSRF 防護
   - 資料驗證與清理

## 測試計畫
1. 單元測試
   - 服務層邏輯測試
   - 資料存取層測試
   - 密碼雜湊與驗證測試

2. 整合測試
   - API 端點測試
   - 資料庫操作測試
   - 身份驗證流程測試

3. 效能測試
   - 資料庫查詢效能
   - API 響應時間
   - 並發處理能力

## 部署計畫
1. 資料庫遷移腳本準備
2. 環境配置檔案設定
3. CI/CD 流程設定
4. 監控與日誌配置

## 時程規劃
1. 基礎設施建置：5 個工作天
2. 核心功能實作：7 個工作天
3. 進階功能實作：5 個工作天
4. 測試與調整：3 個工作天
總計：20 個工作天

## 風險評估
1. 技術風險
   - Entity Framework Core 效能優化
   - 郵件服務可靠性
   - 並發處理問題

2. 安全風險
   - 密碼政策適當性
   - 授權機制完整性
   - 資料保護措施

## 驗收標準
1. 功能性測試通過率 100%
2. 單元測試覆蓋率 > 80%
3. API 響應時間 < 500ms
4. 無高風險安全漏洞
5. 使用者操作文件完整