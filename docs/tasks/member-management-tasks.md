# 會員管理模組實作任務清單

## 設定任務 (Setup Tasks)

### 基礎建設 [P]
- **T001**: 建立 MySQL 資料庫環境 [完成]
  - 已建立資料庫 moreai_db (utf8mb4, unicode_ci)
  - 連接字串已設定在 appsettings.Development.json:
    ```json
    "DefaultConnection": "Server=localhost;Port=3306;Database=moreai_db;User=root;Password=esun@1313;CharSet=utf8mb4;"
    ```
  - 使用 root 帳號（生產環境建議使用限制權限的專用帳號）
  - 檔案: `/src/moreai.web/appsettings.Development.json`

- **T002**: 安裝必要的 NuGet 套件 [P] [完成]
  ```
  Dapper
  MySql.Data
  Microsoft.AspNetCore.Authentication.JwtBearer
  Microsoft.AspNetCore.Identity
  SendGrid
  ```
  - 檔案: `/src/moreai.web/moreai.csproj`

- **T003**: 設定專案結構 [完成]
  ```
  /src/moreai.web/
    /Models
    /Services
    /Repositories
    /Controllers
    /Views
    /Configurations
  ```

## 資料模型任務 (Model Tasks) [P]

### 實體類別
- **T004**: 建立會員相關模型 [完成]
  - 檔案: `/src/moreai.web/Models/Member.cs`
  - 檔案: `/src/moreai.web/Models/MemberDTO.cs`

- **T005**: 建立群組相關模型 [完成]
  - 檔案: `/src/moreai.web/Models/Group.cs`
  - 檔案: `/src/moreai.web/Models/GroupDTO.cs`

- **T006**: 建立會員群組關聯模型 [完成]
  - 檔案: `/src/moreai.web/Models/MemberGroup.cs`

### 資料庫存取層
- **T007**: 實作 Repository 介面 [完成]
  - 檔案: `/src/moreai.web/Repositories/IMemberRepository.cs`
  - 檔案: `/src/moreai.web/Repositories/IGroupRepository.cs`

- **T008**: 實作 Repository 類別 [完成]
  - 檔案: `/src/moreai.web/Repositories/MemberRepository.cs`
  - 檔案: `/src/moreai.web/Repositories/GroupRepository.cs`

## 服務層任務 (Service Tasks)

### 身份驗證服務
- **T009**: 實作 JWT Token 服務 [完成]
  - 檔案: `/src/moreai.web/Services/ITokenService.cs`
  - 檔案: `/src/moreai.web/Services/JwtTokenService.cs`

- **T010**: 實作密碼雜湊服務 [完成]
  - 檔案: `/src/moreai.web/Services/IPasswordService.cs`
  - 檔案: `/src/moreai.web/Services/PasswordService.cs`

### 郵件服務
- **T011**: 實作郵件服務 [P] [完成]
  - 檔案: `/src/moreai.web/Services/IEmailService.cs`
  - 檔案: `/src/moreai.web/Services/EmailService.cs`

## API 端點任務 (Controller Tasks)

### 會員控制器
- **T012**: 實作會員註冊 [完成]
  - 檔案: `/src/moreai.web/Controllers/MembersController.cs`
  - 端點: POST /api/members/register

- **T013**: 實作會員登入 [完成]
  - 檔案: `/src/moreai.web/Controllers/MembersController.cs`
  - 端點: POST /api/members/login

- **T014**: 實作會員資料維護 [完成]
  - 檔案: `/src/moreai.web/Controllers/MembersController.cs`
  - 端點: GET/PUT /api/members/{id}

### 群組控制器
- **T015**: 實作群組管理 [P] [完成]
  - 檔案: `/src/moreai.web/Controllers/GroupsController.cs`
  - 端點: GET/POST/PUT/DELETE /api/groups

- **T016**: 實作會員群組指派 [完成]
  - 檔案: `/src/moreai.web/Controllers/GroupsController.cs`
  - 端點: POST /api/members/{id}/groups

## 前端任務 (Frontend Tasks) [P]

### 會員相關頁面
- **T017**: 實作註冊頁面 [完成]
  - 檔案: `/src/moreai.web/Views/Members/Register.cshtml`
  - 檔案: `/src/moreai.web/wwwroot/js/members/register.js`

- **T018**: 實作登入頁面 [完成]
  - 檔案: `/src/moreai.web/Views/Members/Login.cshtml`
  - 檔案: `/src/moreai.web/wwwroot/js/members/login.js`

- **T019**: 實作會員資料維護頁面 [完成]
  - 檔案: `/src/moreai.web/Views/Members/Profile.cshtml`
  - 檔案: `/src/moreai.web/wwwroot/js/members/profile.js`

### 群組管理頁面
- **T020**: 實作群組管理頁面 [完成]
  - 檔案: `/src/moreai.web/Views/Groups/Index.cshtml`
  - 檔案: `/src/moreai.web/wwwroot/js/groups/index.js`

- **T021**: 實作會員群組指派頁面 [完成]
  - 檔案: `/src/moreai.web/Views/Groups/AssignMembers.cshtml`
  - 檔案: `/src/moreai.web/wwwroot/js/groups/assign.js`

## 測試任務 (Test Tasks) [P]

### 單元測試
- **T022**: Repository 測試 [完成]
  - 檔案: `/tests/moreai.Tests/Repositories/MemberRepositoryTests.cs`
  - 檔案: `/tests/moreai.Tests/Repositories/GroupRepositoryTests.cs`

- **T023**: 服務層測試 [完成]
  - 檔案: `/tests/moreai.Tests/Services/TokenServiceTests.cs`
  - 檔案: `/tests/moreai.Tests/Services/PasswordServiceTests.cs`
  - 檔案: `/tests/moreai.Tests/Services/EmailServiceTests.cs`

### 整合測試
- **T024**: API 端點測試 [完成]
  - 檔案: `/tests/moreai.Tests/Controllers/MembersControllerTests.cs`
  - 檔案: `/tests/moreai.Tests/Controllers/GroupsControllerTests.cs`

## 最終任務 (Polish Tasks)

- **T025**: 性能優化 [完成]
  - 實作資料庫索引
  - 優化 Dapper 查詢
  - 實作快取機制

- **T026**: 安全性強化 [完成]
  - HTTPS 設定
  - CSRF 防護
  - XSS 防護

- **T027**: 文件撰寫 [完成]
  - API 文件
  - 部署文件
  - 使用者手冊

## 執行順序和相依性

### 第一階段（並行執行）
- T001: 資料庫環境 [P]
- T002: NuGet 套件 [P]
- T003: 專案結構

### 第二階段（並行執行）
- T004-T006: 模型建立 [P]
- T007-T008: Repository 介面和實作
- T009-T011: 服務層實作 [P]

### 第三階段
- T012-T016: API 端點實作
- T017-T021: 前端頁面實作 [P]

### 第四階段（並行執行）
- T022-T024: 測試實作 [P]
- T025-T027: 優化和文件

## 並行執行指南

可以同時執行的任務組合：
1. 基礎建設組（T001 + T002）
2. 模型和介面組（T004-T008）
3. 服務層組（T009-T011）
4. 前端開發組（T017-T021）
5. 測試組（T022-T024）

注意事項：
- 標記 [P] 的任務可以並行執行
- 相同檔案的修改需要順序執行
- 確保完成必要的相依任務後才開始下一階段