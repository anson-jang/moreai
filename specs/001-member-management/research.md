# Research: 會員管理系統

**Feature**: 會員管理系統 | **Phase**: 0 - Research | **Date**: 2024年10月25日

## Current State Analysis

### Existing SmartAdmin Framework Inventory

#### Controllers Analysis
- **AuthController.cs**: 現有認證控制器，需要擴展以支援新的會員註冊和密碼重設功能
- **DashboardController.cs**: 儀表板控制器，可能需要整合會員統計資訊
- **Other Controllers**: Apps, Error, Forms, Forum, Icons, Tables, Ui, Utilities - 不需要修改

#### Models Analysis  
- **ErrorViewModel.cs**: 現有錯誤模型，可重用於會員操作錯誤處理
- **缺少**: 會員相關的實體模型和 ViewModels 需要全新建立

#### Views Analysis
- **Auth Views**: 現有登入、註冊、密碼重設視圖需要修改以符合新需求
  - `Login.cshtml` - 需要整合新的登入驗證機制
  - `Register.cshtml` - 需要修改以支援電子郵件驗證流程
  - `Forgetpassword.cshtml` - 需要更新密碼重設流程
- **Layout**: `_BaseLayout.cshtml`, `_Layout.cshtml` 可重用，需要添加會員管理導航

#### Database Context
- **缺少**: 目前沒有實體框架或資料存取層，需要建立 Dapper-based 資料存取

### Technology Stack Validation

#### ASP.NET Core 9.0 Integration
- ✅ **已確認**: 專案使用 .NET 9.0 (`<TargetFramework>net9.0</TargetFramework>`)
- ✅ **相容性**: ASP.NET Core Identity 支援 .NET 9.0
- ✅ **JWT 支援**: Microsoft.AspNetCore.Authentication.JwtBearer 可用

#### Database Strategy
- **MySQL 8.0**: 需要配置連接字符串和 MySQL.Data NuGet 套件
- **Dapper ORM**: 輕量級 ORM，適合與現有架構整合
- **Migration Strategy**: 使用 SQL scripts 而非 EF migrations 保持簡潔

#### Email Service Integration
- **SendGrid**: 雲端電子郵件服務，適合密碼重設和驗證郵件
- **SMTP Fallback**: 本地 SMTP 設定作為備選方案
- **Template System**: 需要建立郵件模板系統

## Architectural Decisions

### Service Layer Architecture
```
Controllers → Services → Repositories → Database
    ↓           ↓           ↓
ViewModels   DTOs      Entities
```

**Rationale**: 
- 遵循現有 MVC 模式
- 清楚的關注點分離
- 可測試性和可維護性

### Authentication & Authorization Strategy
- **ASP.NET Core Identity**: 使用內建身份系統
- **JWT Tokens**: API 存取的無狀態令牌
- **Cookie Authentication**: Web 界面的有狀態認證
- **Role-Based Access**: 基於群組的權限控制

### Data Access Pattern
- **Repository Pattern**: 抽象化資料存取邏輯
- **Dapper**: 高效能的微型 ORM
- **Connection Management**: 使用 DI 容器管理資料庫連接
- **Transaction Support**: 複雜操作的事務支援

## Integration Points

### SmartAdmin Framework Integration
1. **Navigation**: 在現有選單中添加會員管理選項
2. **Styling**: 使用現有 CSS 框架和主題
3. **JavaScript**: 整合現有 jQuery 和 Bootstrap 組件
4. **Layout**: 維持一致的頁面布局和用戶體驗

### Security Considerations
1. **Password Hashing**: 使用 ASP.NET Core Identity 的密碼雜湊
2. **CSRF Protection**: 利用現有的 CSRF 令牌
3. **XSS Prevention**: 使用 Razor 視圖的自動編碼
4. **SQL Injection**: Dapper 參數化查詢防護

### Performance Optimizations
1. **Database Indexes**: 會員搜尋的索引策略
2. **Caching**: 群組資訊和權限的記憶體快取
3. **Pagination**: 大量會員清單的分頁處理
4. **Async Operations**: 非同步資料庫操作

## Risk Assessment

### Technical Risks
1. **Database Migration**: 現有系統可能沒有資料庫，需要建立完整的資料庫架構
   - **Mitigation**: 提供完整的 SQL 建立腳本和範例資料
   
2. **Email Service Dependency**: 電子郵件服務故障影響註冊和密碼重設
   - **Mitigation**: 提供 SMTP 備選方案和離線模式

3. **Performance Under Load**: 大量會員資料的效能影響
   - **Mitigation**: 資料庫索引優化和查詢效能監控

### Integration Risks  
1. **SmartAdmin 框架相容性**: 新功能可能與現有樣式衝突
   - **Mitigation**: 遵循現有設計模式和 CSS 類別命名

2. **Authentication 衝突**: 新認證系統與現有登入機制的衝突
   - **Mitigation**: 逐步遷移和向後相容性支援

## Development Roadmap

### Phase 1: Core Infrastructure (Week 1-2)
- 建立資料模型和資料庫架構
- 實作基本的資料存取層 (Repository + Dapper)
- 設定依賴注入和服務註冊

### Phase 2: Authentication System (Week 2-3)  
- 整合 ASP.NET Core Identity
- 實作登入、註冊、密碼重設功能
- 建立電子郵件服務

### Phase 3: Member Management (Week 3-4)
- 會員 CRUD 操作
- 搜尋和篩選功能
- 批量操作

### Phase 4: Group Management (Week 4-5)
- 群組管理功能
- 權限控制系統
- 群組與會員關聯

### Phase 5: UI/UX Integration (Week 5-6)
- SmartAdmin 整合
- 響應式界面
- JavaScript 互動功能

### Phase 6: Testing & Optimization (Week 6-7)
- 單元測試和整合測試
- 效能優化
- 安全性測試

## Dependencies & Prerequisites

### NuGet Packages Required
```xml
<PackageReference Include="Microsoft.AspNetCore.Identity" Version="9.0.0" />
<PackageReference Include="Microsoft.AspNetCore.Authentication.JwtBearer" Version="9.0.0" />
<PackageReference Include="Dapper" Version="2.1.35" />
<PackageReference Include="MySql.Data" Version="9.0.0" />
<PackageReference Include="SendGrid" Version="9.29.3" />
<PackageReference Include="Newtonsoft.Json" Version="13.0.3" />
```

### Configuration Requirements
- MySQL 資料庫連接字符串
- SendGrid API 金鑰或 SMTP 設定
- JWT 簽章金鑰
- 應用程式設定更新

### Development Environment
- Visual Studio 2022 或 VS Code
- MySQL 8.0 Server
- .NET 9.0 SDK
- Git 版本控制

## Conclusion

研究結果顯示會員管理系統可以成功整合到現有的 SmartAdmin 框架中。主要挑戰在於建立完整的資料庫架構和電子郵件服務整合。建議的架構使用成熟的 ASP.NET Core 模式，確保可維護性和擴展性。

**下一步**: 進入 Phase 1 設計階段，建立詳細的資料模型和 API 合約規格。