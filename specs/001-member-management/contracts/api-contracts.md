# API Contracts: 會員管理系統

**Feature**: 會員管理系統 | **API Version**: v1.0 | **Date**: 2024年10月25日

## Authentication & Authorization

### Authentication Schemes
- **Cookies**: Web UI 認證 (ASP.NET Core Identity)
- **JWT Bearer**: API 認證 (機器對機器)

### Authorization Policies
- **AdminOnly**: 只有管理者群組可存取
- **MemberOrAdmin**: 會員可存取自己的資料，管理者可存取所有資料
- **AuthenticatedUser**: 任何已認證使用者

## REST API Endpoints

### Authentication Endpoints

#### POST /api/auth/login
登入會員並建立認證會議。

**Request Body**:
```json
{
  "email": "user@example.com",
  "password": "SecurePass123!",
  "rememberMe": false
}
```

**Response (200 OK)**:
```json
{
  "success": true,
  "message": "登入成功",
  "data": {
    "memberId": 123,
    "email": "user@example.com",
    "nickName": "使用者名稱",
    "sessionToken": "uuid-session-token",
    "expiresAt": "2024-10-26T12:00:00Z",
    "groups": ["MEMBER"]
  }
}
```

**Response (401 Unauthorized)**:
```json
{
  "success": false,
  "message": "電子郵件或密碼錯誤",
  "errorCode": "INVALID_CREDENTIALS"
}
```

**Response (423 Locked)**:
```json
{
  "success": false,
  "message": "帳號已鎖定至 2024-10-25T13:30:00Z",
  "errorCode": "ACCOUNT_LOCKED",
  "data": {
    "lockedUntil": "2024-10-25T13:30:00Z"
  }
}
```

#### POST /api/auth/register
會員自助註冊新帳號。

**Request Body**:
```json
{
  "nickName": "新會員",
  "email": "newuser@example.com",
  "password": "SecurePass123!",
  "phoneNumber": "0912345678"
}
```

**Response (201 Created)**:
```json
{
  "success": true,
  "message": "註冊成功，請檢查電子郵件進行驗證",
  "data": {
    "memberId": 124,
    "email": "newuser@example.com",
    "emailVerificationRequired": true
  }
}
```

**Response (400 Bad Request)**:
```json
{
  "success": false,
  "message": "電子郵件已存在",
  "errorCode": "EMAIL_EXISTS",
  "errors": {
    "email": ["電子郵件地址已被使用"]
  }
}
```

#### POST /api/auth/logout
登出目前會議。

**Authorization**: Bearer Token 或 Cookie

**Response (200 OK)**:
```json
{
  "success": true,
  "message": "已成功登出"
}
```

#### POST /api/auth/forgot-password
請求密碼重設。

**Request Body**:
```json
{
  "email": "user@example.com"
}
```

**Response (200 OK)**:
```json
{
  "success": true,
  "message": "如果電子郵件存在，重設連結已發送"
}
```

#### POST /api/auth/reset-password
使用重設令牌重設密碼。

**Request Body**:
```json
{
  "resetToken": "uuid-reset-token",
  "newPassword": "NewSecurePass123!"
}
```

**Response (200 OK)**:
```json
{
  "success": true,
  "message": "密碼重設成功"
}
```

**Response (400 Bad Request)**:
```json
{
  "success": false,
  "message": "重設令牌無效或已過期",
  "errorCode": "INVALID_RESET_TOKEN"
}
```

#### GET /api/auth/verify-email/{token}
驗證電子郵件地址。

**Response (200 OK)**:
```json
{
  "success": true,
  "message": "電子郵件驗證成功"
}
```

### Member Management Endpoints

#### GET /api/members
取得會員清單 (分頁、搜尋、篩選)。

**Authorization**: AdminOnly

**Query Parameters**:
- `page` (int): 頁數，預設 1
- `pageSize` (int): 每頁筆數，預設 20，最大 100
- `search` (string): 搜尋關鍵字 (暱稱或電子郵件)
- `groupId` (int): 群組篩選
- `isActive` (bool): 狀態篩選
- `sortBy` (string): 排序欄位 (id, nickName, email, createdAt)
- `sortDesc` (bool): 降序排列，預設 false

**Response (200 OK)**:
```json
{
  "success": true,
  "data": {
    "items": [
      {
        "id": 123,
        "nickName": "會員名稱",
        "email": "user@example.com",
        "phoneNumber": "0912345678",
        "isActive": true,
        "isEmailVerified": true,
        "isLocked": false,
        "createdAt": "2024-10-01T00:00:00Z",
        "lastLoginAt": "2024-10-25T10:30:00Z",
        "groups": ["MEMBER"]
      }
    ],
    "pagination": {
      "currentPage": 1,
      "pageSize": 20,
      "totalItems": 150,
      "totalPages": 8,
      "hasNextPage": true,
      "hasPreviousPage": false
    }
  }
}
```

#### GET /api/members/{id}
取得特定會員詳細資料。

**Authorization**: AdminOnly 或 MemberOrAdmin (只能存取自己的資料)

**Response (200 OK)**:
```json
{
  "success": true,
  "data": {
    "id": 123,
    "nickName": "會員名稱",
    "email": "user@example.com",
    "phoneNumber": "0912345678",
    "isActive": true,
    "isEmailVerified": true,
    "isLocked": false,
    "lockedUntil": null,
    "failedLoginAttempts": 0,
    "createdAt": "2024-10-01T00:00:00Z",
    "lastLoginAt": "2024-10-25T10:30:00Z",
    "updatedAt": "2024-10-25T10:30:00Z",
    "groups": [
      {
        "id": 2,
        "groupCode": "MEMBER",
        "groupName": "一般會員",
        "assignedAt": "2024-10-01T00:00:00Z"
      }
    ]
  }
}
```

#### POST /api/members
建立新會員 (管理員功能)。

**Authorization**: AdminOnly

**Request Body**:
```json
{
  "nickName": "新會員",
  "email": "newmember@example.com",
  "password": "TempPassword123!",
  "phoneNumber": "0987654321",
  "groupIds": [2, 3],
  "isActive": true,
  "sendWelcomeEmail": true
}
```

**Response (201 Created)**:
```json
{
  "success": true,
  "message": "會員建立成功",
  "data": {
    "id": 125,
    "email": "newmember@example.com",
    "tempPassword": "TempPassword123!"
  }
}
```

#### PUT /api/members/{id}
更新會員資料。

**Authorization**: AdminOnly 或 MemberOrAdmin (只能更新自己的資料)

**Request Body**:
```json
{
  "nickName": "更新的名稱",
  "phoneNumber": "0912345678",
  "isActive": true
}
```

**Response (200 OK)**:
```json
{
  "success": true,
  "message": "會員資料更新成功"
}
```

#### DELETE /api/members/{id}
刪除會員 (軟刪除)。

**Authorization**: AdminOnly

**Response (200 OK)**:
```json
{
  "success": true,
  "message": "會員已刪除"
}
```

#### POST /api/members/{id}/lock
鎖定會員帳號。

**Authorization**: AdminOnly

**Request Body**:
```json
{
  "reason": "可疑活動",
  "lockDurationMinutes": 60
}
```

**Response (200 OK)**:
```json
{
  "success": true,
  "message": "會員帳號已鎖定",
  "data": {
    "lockedUntil": "2024-10-25T14:30:00Z"
  }
}
```

#### POST /api/members/{id}/unlock
解鎖會員帳號。

**Authorization**: AdminOnly

**Response (200 OK)**:
```json
{
  "success": true,
  "message": "會員帳號已解鎖"
}
```

#### POST /api/members/bulk-operation
批量操作會員。

**Authorization**: AdminOnly

**Request Body**:
```json
{
  "memberIds": [123, 124, 125],
  "operation": "ACTIVATE", // ACTIVATE, DEACTIVATE, DELETE, ASSIGN_GROUP
  "groupId": 2 // 當操作為 ASSIGN_GROUP 時必要
}
```

**Response (200 OK)**:
```json
{
  "success": true,
  "message": "批量操作完成",
  "data": {
    "processedCount": 3,
    "successCount": 2,
    "failedCount": 1,
    "errors": [
      {
        "memberId": 125,
        "error": "會員不存在"
      }
    ]
  }
}
```

### Group Management Endpoints

#### GET /api/groups
取得所有會員群組。

**Authorization**: AdminOnly

**Response (200 OK)**:
```json
{
  "success": true,
  "data": [
    {
      "id": 1,
      "groupCode": "ADMIN",
      "groupName": "管理者",
      "description": "系統管理員，具有完整管理權限",
      "isActive": true,
      "memberCount": 2,
      "createdAt": "2024-10-01T00:00:00Z"
    },
    {
      "id": 2,
      "groupCode": "MEMBER",
      "groupName": "一般會員",
      "description": "基本會員權限",
      "isActive": true,
      "memberCount": 150,
      "createdAt": "2024-10-01T00:00:00Z"
    }
  ]
}
```

#### GET /api/groups/{id}
取得特定群組詳細資料。

**Authorization**: AdminOnly

**Response (200 OK)**:
```json
{
  "success": true,
  "data": {
    "id": 2,
    "groupCode": "MEMBER",
    "groupName": "一般會員",
    "description": "基本會員權限",
    "isActive": true,
    "createdAt": "2024-10-01T00:00:00Z",
    "updatedAt": "2024-10-01T00:00:00Z",
    "members": [
      {
        "id": 123,
        "nickName": "會員名稱",
        "email": "user@example.com",
        "assignedAt": "2024-10-01T00:00:00Z"
      }
    ]
  }
}
```

#### POST /api/groups
建立新群組。

**Authorization**: AdminOnly

**Request Body**:
```json
{
  "groupCode": "VIP",
  "groupName": "VIP會員",
  "description": "VIP會員專屬權限",
  "isActive": true
}
```

**Response (201 Created)**:
```json
{
  "success": true,
  "message": "群組建立成功",
  "data": {
    "id": 4,
    "groupCode": "VIP"
  }
}
```

#### PUT /api/groups/{id}
更新群組資料。

**Authorization**: AdminOnly

**Request Body**:
```json
{
  "groupName": "更新的群組名稱",
  "description": "更新的描述",
  "isActive": true
}
```

#### POST /api/groups/{groupId}/members/{memberId}
將會員加入群組。

**Authorization**: AdminOnly

**Response (200 OK)**:
```json
{
  "success": true,
  "message": "會員已加入群組"
}
```

#### DELETE /api/groups/{groupId}/members/{memberId}
將會員從群組移除。

**Authorization**: AdminOnly

**Response (200 OK)**:
```json
{
  "success": true,
  "message": "會員已從群組移除"
}
```

### Reporting & Analytics Endpoints

#### GET /api/reports/member-stats
取得會員統計資料。

**Authorization**: AdminOnly

**Response (200 OK)**:
```json
{
  "success": true,
  "data": {
    "totalMembers": 152,
    "activeMembers": 140,
    "inactiveMembers": 12,
    "lockedMembers": 3,
    "unverifiedEmails": 8,
    "registrationsThisMonth": 15,
    "loginsToday": 45,
    "groupDistribution": [
      {
        "groupName": "管理者",
        "memberCount": 2
      },
      {
        "groupName": "一般會員",
        "memberCount": 130
      },
      {
        "groupName": "付費會員",
        "memberCount": 20
      }
    ]
  }
}
```

#### GET /api/reports/security-events
取得安全事件報告。

**Authorization**: AdminOnly

**Query Parameters**:
- `startDate` (date): 開始日期
- `endDate` (date): 結束日期
- `eventType` (string): 事件類型篩選
- `isResolved` (bool): 是否已處理

**Response (200 OK)**:
```json
{
  "success": true,
  "data": {
    "events": [
      {
        "id": 1,
        "eventType": "LOGIN_FAILED",
        "eventDescription": "多次登入失敗嘗試",
        "memberId": 123,
        "memberEmail": "user@example.com",
        "ipAddress": "192.168.1.100",
        "createdAt": "2024-10-25T10:30:00Z",
        "isResolved": false
      }
    ],
    "summary": {
      "totalEvents": 25,
      "resolvedEvents": 20,
      "pendingEvents": 5,
      "eventTypes": {
        "LOGIN_FAILED": 15,
        "ACCOUNT_LOCKED": 5,
        "SUSPICIOUS_LOGIN": 3,
        "PASSWORD_RESET_REQUEST": 2
      }
    }
  }
}
```

## Data Transfer Objects (DTOs)

### Request DTOs

```csharp
public class LoginRequestDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }
    
    [Required]
    public string Password { get; set; }
    
    public bool RememberMe { get; set; } = false;
}

public class RegisterRequestDto
{
    [Required]
    [StringLength(50, MinimumLength = 1)]
    public string NickName { get; set; }
    
    [Required]
    [EmailAddress]
    [StringLength(100)]
    public string Email { get; set; }
    
    [Required]
    [StringLength(100, MinimumLength = 8)]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]",
        ErrorMessage = "密碼必須包含大小寫字母、數字和特殊字符")]
    public string Password { get; set; }
    
    [Phone]
    public string? PhoneNumber { get; set; }
}

public class CreateMemberRequestDto
{
    [Required]
    [StringLength(50, MinimumLength = 1)]
    public string NickName { get; set; }
    
    [Required]
    [EmailAddress]
    public string Email { get; set; }
    
    [Required]
    [StringLength(100, MinimumLength = 8)]
    public string Password { get; set; }
    
    [Phone]
    public string? PhoneNumber { get; set; }
    
    public List<int> GroupIds { get; set; } = new();
    
    public bool IsActive { get; set; } = true;
    
    public bool SendWelcomeEmail { get; set; } = true;
}

public class UpdateMemberRequestDto
{
    [StringLength(50, MinimumLength = 1)]
    public string? NickName { get; set; }
    
    [Phone]
    public string? PhoneNumber { get; set; }
    
    public bool? IsActive { get; set; }
}
```

### Response DTOs

```csharp
public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; }
    public T? Data { get; set; }
    public string? ErrorCode { get; set; }
    public Dictionary<string, List<string>>? Errors { get; set; }
}

public class LoginResponseDto
{
    public int MemberId { get; set; }
    public string Email { get; set; }
    public string NickName { get; set; }
    public string SessionToken { get; set; }
    public DateTime ExpiresAt { get; set; }
    public List<string> Groups { get; set; }
}

public class MemberListItemDto
{
    public int Id { get; set; }
    public string NickName { get; set; }
    public string Email { get; set; }
    public string? PhoneNumber { get; set; }
    public bool IsActive { get; set; }
    public bool IsEmailVerified { get; set; }
    public bool IsLocked { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public List<string> Groups { get; set; }
}

public class PaginatedResponse<T>
{
    public List<T> Items { get; set; }
    public PaginationInfo Pagination { get; set; }
}

public class PaginationInfo
{
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
    public int TotalItems { get; set; }
    public int TotalPages { get; set; }
    public bool HasNextPage { get; set; }
    public bool HasPreviousPage { get; set; }
}
```

## Error Codes

### Authentication Errors
- `INVALID_CREDENTIALS`: 認證資訊錯誤
- `ACCOUNT_LOCKED`: 帳號已鎖定
- `EMAIL_NOT_VERIFIED`: 電子郵件未驗證
- `SESSION_EXPIRED`: 會議已過期
- `INVALID_RESET_TOKEN`: 重設令牌無效

### Member Management Errors
- `MEMBER_NOT_FOUND`: 會員不存在
- `EMAIL_EXISTS`: 電子郵件已存在
- `INVALID_GROUP`: 群組不存在或無效
- `PERMISSION_DENIED`: 權限不足
- `VALIDATION_ERROR`: 資料驗證錯誤

### System Errors
- `INTERNAL_ERROR`: 系統內部錯誤
- `DATABASE_ERROR`: 資料庫操作錯誤
- `EMAIL_SERVICE_ERROR`: 電子郵件服務錯誤

## Rate Limiting

### Authentication Endpoints
- 登入: 每 IP 每分鐘最多 5 次嘗試
- 註冊: 每 IP 每小時最多 3 次
- 密碼重設: 每 IP 每小時最多 5 次

### API Endpoints
- 一般 API: 每使用者每分鐘 100 次請求
- 報告 API: 每使用者每分鐘 10 次請求
- 批量操作: 每使用者每分鐘 5 次請求

## Security Headers

所有 API 回應都包含以下安全標頭：

```
X-Content-Type-Options: nosniff
X-Frame-Options: DENY
X-XSS-Protection: 1; mode=block
Content-Security-Policy: default-src 'self'
Strict-Transport-Security: max-age=31536000; includeSubDomains
```

## Versioning Strategy

- API 版本透過 URL 路徑指定: `/api/v1/members`
- 當前版本: v1.0
- 向後相容性保證: 主要版本 (v1, v2) 之間可能有破壞性變更
- 次要版本 (v1.1, v1.2) 保持向後相容