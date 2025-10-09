# MoreAI 會員管理系統 API 文件

## 身份驗證

所有需要身份驗證的 API 都需要在 Header 中加入 JWT Token：
```
Authorization: Bearer {token}
```

## API 端點

### 會員管理

#### 註冊會員
- **URL**: `/api/members/register`
- **方法**: `POST`
- **請求內容**:
  ```json
  {
    "username": "string",
    "email": "string",
    "password": "string"
  }
  ```
- **回應**:
  ```json
  {
    "success": true,
    "data": {
      "id": "number"
    },
    "message": "string"
  }
  ```

#### 會員登入
- **URL**: `/api/members/login`
- **方法**: `POST`
- **請求內容**:
  ```json
  {
    "email": "string",
    "password": "string"
  }
  ```
- **回應**:
  ```json
  {
    "success": true,
    "data": {
      "token": "string",
      "username": "string"
    },
    "message": "string"
  }
  ```

#### 取得會員資料
- **URL**: `/api/members/{id}`
- **方法**: `GET`
- **需要授權**: 是
- **回應**:
  ```json
  {
    "success": true,
    "data": {
      "id": "number",
      "username": "string",
      "email": "string"
    },
    "message": "string"
  }
  ```

### 群組管理

#### 建立群組
- **URL**: `/api/groups`
- **方法**: `POST`
- **需要授權**: 是
- **請求內容**:
  ```json
  {
    "name": "string",
    "description": "string"
  }
  ```
- **回應**:
  ```json
  {
    "success": true,
    "data": {
      "id": "number"
    },
    "message": "string"
  }
  ```

#### 指派會員到群組
- **URL**: `/api/groups/{groupId}/members`
- **方法**: `POST`
- **需要授權**: 是
- **請求內容**:
  ```json
  {
    "memberIds": ["number"]
  }
  ```
- **回應**:
  ```json
  {
    "success": true,
    "message": "string"
  }
  ```

## 錯誤處理

所有 API 在發生錯誤時都會返回統一的錯誤格式：
```json
{
  "success": false,
  "message": "錯誤訊息",
  "errors": ["詳細錯誤訊息列表"]
}
```

## 狀態碼

- 200: 成功
- 400: 請求格式錯誤
- 401: 未授權
- 403: 權限不足
- 404: 資源不存在
- 500: 伺服器錯誤