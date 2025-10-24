# Implementation Plan: 會員管理系統

**Branch**: `001-member-management` | **Date**: 2024年10月25日 | **Spec**: [spec.md](./spec.md)
**Input**: Feature specification from `/specs/001-member-management/spec.md`

**Note**: This template is filled in by the `/speckit.plan` command. See `.specify/templates/commands/plan.md` for the execution workflow.

## Summary

實作完整的會員管理系統，包含會員自助註冊、管理員管理會員、登入驗證、密碼重設、群組管理等功能。系統將整合到現有的 SmartAdmin 框架中，使用 ASP.NET Core 9.0、MySQL 8.0、Dapper ORM，並提供 RESTful API 和現代化的 Web 界面。

## Technical Context

**Language/Version**: C# with .NET 9.0 (ASP.NET Core 9.0)  
**Primary Dependencies**: ASP.NET Core Identity, Dapper ORM, MySQL.Data, JWT Bearer Authentication, SendGrid/SMTP  
**Storage**: MySQL 8.0 database with Dapper ORM for data access  
**Testing**: xUnit testing framework with ASP.NET Core TestHost  
**Target Platform**: Windows/Linux web server, modern browsers (Chrome, Firefox, Safari, Edge)  
**Project Type**: Web application with MVC pattern integration into existing SmartAdmin framework  
**Performance Goals**: 
- 登入驗證 < 3 秒
- 會員搜尋 < 2 秒  
- 支援 1,000 同時線上會員
- 批量操作支援 100+ 會員記錄  

**Constraints**: 
- 整合現有 SmartAdmin 框架不破壞既有功能
- 遵循個人資料保護法規
- 密碼重設連結 5 分鐘有效期限制
- 登入失敗 5 次鎖定 30 分鐘安全限制  

**Scale/Scope**: 
- 支援 10,000+ 會員資料
- 7 個主要 User Stories
- 21 個功能需求
- 3 個預設會員群組 (管理者、一般會員、付費會員)

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

**Status**: ✅ PASS - No constitution violations detected

**Assessment**: 
- Constitution file is in template state with no defined constraints
- Implementation follows ASP.NET Core best practices
- Uses established patterns (MVC, Repository, Dependency Injection)
- Integrates with existing SmartAdmin framework without breaking changes
- No additional complexity beyond standard web application architecture

## Project Structure

### Documentation (this feature)

```text
specs/[###-feature]/
├── plan.md              # This file (/speckit.plan command output)
├── research.md          # Phase 0 output (/speckit.plan command)
├── data-model.md        # Phase 1 output (/speckit.plan command)
├── quickstart.md        # Phase 1 output (/speckit.plan command)
├── contracts/           # Phase 1 output (/speckit.plan command)
└── tasks.md             # Phase 2 output (/speckit.tasks command - NOT created by /speckit.plan)
```

### Source Code (repository root)

```text
src/Smartadmin/                    # Existing ASP.NET Core project
├── Controllers/                   # MVC Controllers
│   ├── MemberController.cs        # 會員管理 API 控制器
│   ├── AuthController.cs          # 認證相關控制器 (existing, extend)
│   └── GroupController.cs         # 群組管理控制器
├── Models/                        # Data Models & ViewModels
│   ├── Member.cs                  # 會員實體模型
│   ├── MemberGroup.cs             # 會員群組模型
│   ├── LoginSession.cs            # 登入會議模型
│   ├── PasswordResetRequest.cs    # 密碼重設請求模型
│   ├── SecurityEvent.cs           # 安全事件模型
│   ├── ViewModels/                # ViewModels for views
│   │   ├── MemberViewModel.cs
│   │   ├── LoginViewModel.cs
│   │   └── RegisterViewModel.cs
├── Services/                      # Business Logic Services
│   ├── IMemberService.cs          # 會員服務介面
│   ├── MemberService.cs           # 會員服務實作
│   ├── IAuthService.cs            # 認證服務介面
│   ├── AuthService.cs             # 認證服務實作
│   ├── IGroupService.cs           # 群組服務介面
│   ├── GroupService.cs            # 群組服務實作
│   └── IEmailService.cs           # 電子郵件服務介面
├── Repositories/                  # Data Access Layer
│   ├── IMemberRepository.cs       # 會員資料存取介面
│   ├── MemberRepository.cs        # 會員資料存取實作 (Dapper)
│   ├── IGroupRepository.cs        # 群組資料存取介面
│   └── GroupRepository.cs         # 群組資料存取實作
├── Views/                         # Razor Views (existing structure)
│   ├── Member/                    # 會員管理視圖
│   │   ├── Index.cshtml           # 會員清單
│   │   ├── Create.cshtml          # 新增會員
│   │   ├── Edit.cshtml            # 編輯會員
│   │   └── Details.cshtml         # 會員詳細資料
│   ├── Auth/                      # 認證相關視圖 (existing, extend)
│   │   ├── Login.cshtml           # 登入頁面 (existing)
│   │   ├── Register.cshtml        # 註冊頁面 (existing, modify)
│   │   └── ResetPassword.cshtml   # 密碼重設頁面
│   └── Group/                     # 群組管理視圖
│       ├── Index.cshtml
│       └── Manage.cshtml
├── wwwroot/                       # Static files (existing)
│   ├── js/
│   │   └── member-management.js   # 會員管理 JavaScript
│   └── css/
│       └── member-management.css  # 會員管理樣式
├── Data/                          # Database context & migrations
│   ├── Scripts/                   # SQL scripts
│   │   ├── 001_create_members_table.sql
│   │   ├── 002_create_groups_table.sql
│   │   └── 003_seed_default_groups.sql
└── Configuration/                 # Configuration extensions
    └── DependencyInjection.cs     # Service registration
```

**Structure Decision**: Web application pattern integrated into existing SmartAdmin ASP.NET Core project. The structure extends the current MVC architecture with new controllers, services, and views while maintaining consistency with the existing codebase. Data access uses Dapper ORM with repository pattern for clean separation of concerns.

## Complexity Tracking

**Status**: ✅ No violations detected - standard web application architecture used.

實作使用標準的 ASP.NET Core MVC 模式和成熟的設計模式 (Repository, Service Layer, Dependency Injection)，沒有不必要的複雜性。
