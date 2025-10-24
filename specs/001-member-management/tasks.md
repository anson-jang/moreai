# Tasks: 會員管理系統

**Input**: Design documents from `/specs/001-member-management/`
**Prerequisites**: plan.md (required), spec.md (required for user stories), research.md, data-model.md, contracts/

**Tests**: Tests are NOT explicitly requested in the feature specification, so test tasks are omitted for faster delivery.

**Organization**: Tasks are grouped by user story to enable independent implementation and testing of each story.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no dependencies)
- **[Story]**: Which user story this task belongs to (e.g., US1, US2, US3)
- Include exact file paths in descriptions

## Path Conventions

Paths are based on existing SmartAdmin ASP.NET Core project structure:
- **Controllers**: `src/Smartadmin/Controllers/`
- **Models**: `src/Smartadmin/Models/`
- **Services**: `src/Smartadmin/Services/`
- **Repositories**: `src/Smartadmin/Repositories/`
- **Views**: `src/Smartadmin/Views/`
- **Database**: `src/Smartadmin/Data/Scripts/`

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Project initialization and basic structure

- [x] T001 Install required NuGet packages in src/Smartadmin/Smartadmin.csproj (Microsoft.AspNetCore.Identity, Dapper, MySql.Data, SendGrid, JWT Bearer)
- [x] T002 [P] Create database connection configuration in src/Smartadmin/appsettings.json
- [x] T003 [P] Create email service configuration in src/Smartadmin/appsettings.json
- [x] T004 [P] Create JWT authentication configuration in src/Smartadmin/appsettings.json
- [x] T005 Create dependency injection configuration in src/Smartadmin/Configuration/DependencyInjection.cs

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Core infrastructure that MUST be complete before ANY user story can be implemented

**⚠️ CRITICAL**: No user story work can begin until this phase is complete

- [ ] T006 Create database schema script in src/Smartadmin/Data/Scripts/001_create_members_table.sql
- [ ] T007 [P] Create groups table schema script in src/Smartadmin/Data/Scripts/002_create_groups_table.sql
- [ ] T008 [P] Create supporting tables schema script in src/Smartadmin/Data/Scripts/003_create_sessions_and_logs_tables.sql
- [ ] T009 Create seed data script in src/Smartadmin/Data/Scripts/004_seed_default_groups.sql
- [ ] T010 [P] Create Member entity model in src/Smartadmin/Models/Member.cs
- [ ] T011 [P] Create MemberGroup entity model in src/Smartadmin/Models/MemberGroup.cs
- [ ] T012 [P] Create MemberGroupMapping entity model in src/Smartadmin/Models/MemberGroupMapping.cs
- [ ] T013 [P] Create LoginSession entity model in src/Smartadmin/Models/LoginSession.cs
- [ ] T014 [P] Create PasswordResetRequest entity model in src/Smartadmin/Models/PasswordResetRequest.cs
- [ ] T015 [P] Create OperationLog entity model in src/Smartadmin/Models/OperationLog.cs
- [ ] T016 [P] Create SecurityEvent entity model in src/Smartadmin/Models/SecurityEvent.cs
- [ ] T017 [P] Create base repository interface in src/Smartadmin/Repositories/IBaseRepository.cs
- [ ] T018 [P] Create base repository implementation in src/Smartadmin/Repositories/BaseRepository.cs
- [ ] T019 [P] Create IEmailService interface in src/Smartadmin/Services/IEmailService.cs
- [ ] T020 Create EmailService implementation in src/Smartadmin/Services/EmailService.cs
- [ ] T021 Configure ASP.NET Core Identity and JWT in src/Smartadmin/Program.cs
- [ ] T022 Create authentication middleware configuration in src/Smartadmin/Program.cs
- [ ] T023 Update navigation menu to include member management in src/Smartadmin/Views/Shared/_Layout.cshtml

**Checkpoint**: Foundation ready - user story implementation can now begin in parallel

---

## Phase 3: User Story 4 - 會員登入驗證 (Priority: P1) 🎯 MVP Foundation

**Goal**: 會員可以使用電子郵件和密碼登入系統，系統驗證身份並建立安全的登入會話

**Independent Test**: 建立測試會員並驗證完整的登入流程，包括成功登入、錯誤密碼處理和帳號鎖定機制

### Implementation for User Story 4

- [ ] T024 [P] [US4] Create LoginViewModel in src/Smartadmin/Models/ViewModels/LoginViewModel.cs
- [ ] T025 [P] [US4] Create IAuthService interface in src/Smartadmin/Services/IAuthService.cs
- [ ] T026 [US4] Create AuthService implementation in src/Smartadmin/Services/AuthService.cs
- [ ] T027 [P] [US4] Create IMemberRepository interface in src/Smartadmin/Repositories/IMemberRepository.cs
- [ ] T028 [US4] Create MemberRepository implementation in src/Smartadmin/Repositories/MemberRepository.cs
- [ ] T029 [US4] Update existing AuthController to integrate new authentication logic in src/Smartadmin/Controllers/AuthController.cs
- [ ] T030 [US4] Update Login view to handle validation and error messages in src/Smartadmin/Views/Auth/Login.cshtml
- [ ] T031 [US4] Create session management endpoints in AuthController in src/Smartadmin/Controllers/AuthController.cs
- [ ] T032 [US4] Add account lockout logic and security event logging in AuthService
- [ ] T033 [US4] Create member dashboard redirect logic after successful login

**Checkpoint**: At this point, User Story 4 should be fully functional - members can login with email/password and system handles security correctly

---

## Phase 4: User Story 1 - 會員註冊 (Priority: P1) 🎯 MVP Core

**Goal**: 訪客可以申請加入會員，填入基本資料並建立會員帳號，包含電子郵件驗證流程

**Independent Test**: 完整的會員註冊流程測試，從註冊表單提交到電子郵件驗證並成功登入

### Implementation for User Story 1

- [ ] T034 [P] [US1] Create RegisterViewModel in src/Smartadmin/Models/ViewModels/RegisterViewModel.cs
- [ ] T035 [P] [US1] Create IMemberService interface in src/Smartadmin/Services/IMemberService.cs
- [ ] T036 [US1] Create MemberService implementation in src/Smartadmin/Services/MemberService.cs
- [ ] T037 [US1] Add member registration endpoints to AuthController in src/Smartadmin/Controllers/AuthController.cs
- [ ] T038 [US1] Update Register view with validation and error handling in src/Smartadmin/Views/Auth/Register.cshtml
- [ ] T039 [US1] Create email verification endpoints in AuthController in src/Smartadmin/Controllers/AuthController.cs
- [ ] T040 [US1] Create email verification view in src/Smartadmin/Views/Auth/VerifyEmail.cshtml
- [ ] T041 [US1] Create email templates for verification in src/Smartadmin/Services/EmailService.cs
- [ ] T042 [US1] Add member creation with default group assignment logic in MemberService
- [ ] T043 [US1] Add password complexity validation in RegisterViewModel and MemberService
- [ ] T044 [US1] Add operation logging for member registration in MemberService

**Checkpoint**: At this point, User Stories 1 AND 4 should both work independently - visitors can register and login

---

## Phase 5: User Story 2 - 會員資料查詢與管理 (Priority: P1) 🎯 MVP Admin

**Goal**: 管理員可以搜尋、查看和編輯現有會員的詳細資料，包括個人資訊、聯絡方式和會員狀態

**Independent Test**: 建立幾個測試會員，然後執行搜尋、查看和編輯操作來獨立測試管理功能

### Implementation for User Story 2

- [ ] T045 [P] [US2] Create MemberViewModel in src/Smartadmin/Models/ViewModels/MemberViewModel.cs
- [ ] T046 [P] [US2] Create MemberController for admin functions in src/Smartadmin/Controllers/MemberController.cs
- [ ] T047 [US2] Add member search and listing methods to MemberService in src/Smartadmin/Services/MemberService.cs
- [ ] T048 [US2] Add member CRUD operations to MemberRepository in src/Smartadmin/Repositories/MemberRepository.cs
- [ ] T049 [US2] Create member list view in src/Smartadmin/Views/Member/Index.cshtml
- [ ] T050 [US2] Create member details view in src/Smartadmin/Views/Member/Details.cshtml
- [ ] T051 [US2] Create member edit view in src/Smartadmin/Views/Member/Edit.cshtml
- [ ] T052 [US2] Create member creation view for admin in src/Smartadmin/Views/Member/Create.cshtml
- [ ] T053 [US2] Add search functionality with pagination in MemberController and views
- [ ] T054 [US2] Add authorization policies for admin-only access in MemberController
- [ ] T055 [US2] Add operation logging for all member data changes in MemberService

**Checkpoint**: At this point, core member management (registration, login, admin management) should be fully functional

---

## Phase 6: User Story 3 - 會員狀態管理 (Priority: P2)

**Goal**: 管理員可以啟用、停用或刪除會員帳號，控制會員的系統存取權限

**Independent Test**: 建立測試會員並執行狀態變更操作來獨立測試狀態管理功能

### Implementation for User Story 3

- [ ] T056 [US3] Add member status management methods to MemberService in src/Smartadmin/Services/MemberService.cs
- [ ] T057 [US3] Add status management endpoints to MemberController in src/Smartadmin/Controllers/MemberController.cs
- [ ] T058 [US3] Update member list view to show status actions in src/Smartadmin/Views/Member/Index.cshtml
- [ ] T059 [US3] Create status confirmation dialogs and JavaScript in src/Smartadmin/wwwroot/js/member-management.js
- [ ] T060 [US3] Add status change validation in AuthService to prevent disabled member login
- [ ] T061 [US3] Add security event logging for status changes in MemberService
- [ ] T062 [US3] Create member deletion with cascading cleanup logic in MemberService

**Checkpoint**: Member status management should work independently and integrate with existing login system

---

## Phase 7: User Story 5 - 密碼重設與安全管理 (Priority: P2)

**Goal**: 會員可以重設忘記的密碼，管理員可以協助重設會員密碼，系統確保密碼安全性

**Independent Test**: 執行完整的密碼重設流程，包括請求重設、收取郵件、設定新密碼並登入測試

### Implementation for User Story 5

- [ ] T063 [P] [US5] Create password reset ViewModels in src/Smartadmin/Models/ViewModels/
- [ ] T064 [US5] Add password reset request methods to AuthService in src/Smartadmin/Services/AuthService.cs
- [ ] T065 [US5] Add password reset endpoints to AuthController in src/Smartadmin/Controllers/AuthController.cs
- [ ] T066 [US5] Create forgot password view in src/Smartadmin/Views/Auth/ForgotPassword.cshtml
- [ ] T067 [US5] Create reset password view in src/Smartadmin/Views/Auth/ResetPassword.cshtml
- [ ] T068 [US5] Create password reset email templates in EmailService
- [ ] T069 [US5] Add admin password reset functionality to MemberController
- [ ] T070 [US5] Add password reset token management in AuthService with 5-minute expiry
- [ ] T071 [US5] Add security event logging for password reset activities
- [ ] T072 [US5] Add password strength validation and complexity requirements

**Checkpoint**: Password reset functionality should work independently for both member self-service and admin assistance

---

## Phase 8: User Story 6 - 群組管理 (Priority: P2)

**Goal**: 管理員可以建立、編輯和刪除會員群組，將會員分配到不同群組，管理群組權限和存取控制

**Independent Test**: 建立群組、分配會員和測試群組權限來獨立測試群組管理功能

### Implementation for User Story 6

- [ ] T073 [P] [US6] Create GroupViewModel in src/Smartadmin/Models/ViewModels/GroupViewModel.cs
- [ ] T074 [P] [US6] Create IGroupService interface in src/Smartadmin/Services/IGroupService.cs
- [ ] T075 [US6] Create GroupService implementation in src/Smartadmin/Services/GroupService.cs
- [ ] T076 [P] [US6] Create IGroupRepository interface in src/Smartadmin/Repositories/IGroupRepository.cs
- [ ] T077 [US6] Create GroupRepository implementation in src/Smartadmin/Repositories/GroupRepository.cs
- [ ] T078 [US6] Create GroupController for group management in src/Smartadmin/Controllers/GroupController.cs
- [ ] T079 [US6] Create group list view in src/Smartadmin/Views/Group/Index.cshtml
- [ ] T080 [US6] Create group management view in src/Smartadmin/Views/Group/Manage.cshtml
- [ ] T081 [US6] Add group assignment functionality to MemberController and views
- [ ] T082 [US6] Update member views to display group membership
- [ ] T083 [US6] Add authorization policies based on group membership
- [ ] T084 [US6] Add group operation logging and validation

**Checkpoint**: Group management should work independently and integrate with member management and authorization

---

## Phase 9: User Story 7 - 會員清單與批量操作 (Priority: P3)

**Goal**: 管理員可以查看所有會員的清單，支援分頁、排序和篩選，並能夠執行批量操作如批量停用或匯出資料

**Independent Test**: 建立多個測試會員並執行各種清單操作和批量操作來獨立測試

### Implementation for User Story 7

- [ ] T085 [US7] Add advanced search and filtering to MemberService in src/Smartadmin/Services/MemberService.cs
- [ ] T086 [US7] Add sorting and pagination enhancements to MemberController
- [ ] T087 [US7] Update member list view with advanced filters in src/Smartadmin/Views/Member/Index.cshtml
- [ ] T088 [US7] Add bulk operation methods to MemberService
- [ ] T089 [US7] Add bulk operation endpoints to MemberController
- [ ] T090 [US7] Create bulk operation JavaScript functionality in src/Smartadmin/wwwroot/js/member-management.js
- [ ] T091 [US7] Add data export functionality (CSV/Excel) to MemberService
- [ ] T092 [US7] Add export endpoints to MemberController
- [ ] T093 [US7] Create export UI components in member list view
- [ ] T094 [US7] Add bulk operation logging and error handling
- [ ] T095 [US7] Add performance optimization for large member lists

**Checkpoint**: All member management functionality should be complete with advanced list operations and bulk management

---

## Phase 10: Polish & Cross-Cutting Concerns

**Purpose**: Improvements that affect multiple user stories

- [ ] T096 [P] Add comprehensive error handling across all controllers
- [ ] T097 [P] Add input validation and sanitization across all endpoints
- [ ] T098 [P] Add performance monitoring and logging enhancements
- [ ] T099 [P] Create member management CSS styling in src/Smartadmin/wwwroot/css/member-management.css
- [ ] T100 [P] Add security headers and CSRF protection validation
- [ ] T101 Update SmartAdmin navigation and dashboard integration
- [ ] T102 Add database indexes for performance optimization
- [ ] T103 Run quickstart.md validation and documentation updates
- [ ] T104 [P] Add API rate limiting configuration
- [ ] T105 [P] Add comprehensive audit logging for compliance

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: No dependencies - can start immediately
- **Foundational (Phase 2)**: Depends on Setup completion - BLOCKS all user stories
- **User Stories (Phase 3-9)**: All depend on Foundational phase completion
  - User stories can then proceed in parallel (if staffed)
  - Or sequentially in priority order (P1 → P2 → P3)
- **Polish (Phase 10)**: Depends on all desired user stories being complete

### User Story Dependencies

- **User Story 4 (P1) - Login**: Can start after Foundational - No dependencies on other stories (foundational for auth)
- **User Story 1 (P1) - Registration**: Can start after US4 or in parallel - Integrates with login system
- **User Story 2 (P1) - Member Management**: Can start after US1/US4 - Needs authentication system
- **User Story 3 (P2) - Status Management**: Depends on US2 - Extends member management
- **User Story 5 (P2) - Password Reset**: Can start after US4 - Extends authentication
- **User Story 6 (P2) - Group Management**: Can start after US2 - Integrates with member management
- **User Story 7 (P3) - Advanced Lists**: Depends on US2/US3 - Extends member management with bulk operations

### Within Each User Story

- Models and ViewModels can be created in parallel [P]
- Services depend on repositories and models
- Controllers depend on services
- Views depend on controllers and ViewModels
- Story integration comes after core implementation

### Parallel Opportunities

**Phase 1 (Setup)**: T002, T003, T004 can run in parallel
**Phase 2 (Foundational)**: T007, T008, and T010-T016, T017-T020 can run in parallel
**User Stories**: After Phase 2, US4 and US1 can start in parallel, then US2 can start, followed by US3/US5/US6 in parallel

---

## Parallel Example: User Story 4 (Login)

```bash
# Launch models/viewmodels together:
Task: "Create LoginViewModel in src/Smartadmin/Models/ViewModels/LoginViewModel.cs"
Task: "Create IMemberRepository interface in src/Smartadmin/Repositories/IMemberRepository.cs"
Task: "Create IAuthService interface in src/Smartadmin/Services/IAuthService.cs"

# Then implementation:
Task: "Create MemberRepository implementation in src/Smartadmin/Repositories/MemberRepository.cs"
Task: "Create AuthService implementation in src/Smartadmin/Services/AuthService.cs"
```

---

## Implementation Strategy

### MVP First (Login + Registration Only)

1. Complete Phase 1: Setup
2. Complete Phase 2: Foundational (CRITICAL - blocks all stories)
3. Complete Phase 3: User Story 4 (Login)
4. Complete Phase 4: User Story 1 (Registration)
5. **STOP and VALIDATE**: Test registration → email verification → login flow
6. Deploy/demo if ready

### Full P1 Delivery (Add Admin Management)

1. Complete MVP first (Phases 1-4)
2. Add Phase 5: User Story 2 (Member Management)
3. **STOP and VALIDATE**: Test full admin workflow
4. Deploy/demo complete P1 functionality

### Incremental Delivery

1. MVP (Login/Registration) → Foundation ready
2. Add Admin Management → Core system complete
3. Add Status Management → Member lifecycle complete
4. Add Password Reset → Security complete
5. Add Group Management → Authorization complete  
6. Add Bulk Operations → Enterprise features complete

### Parallel Team Strategy

With multiple developers after Foundational phase completes:

- **Developer A**: User Story 4 (Login) → User Story 5 (Password Reset)
- **Developer B**: User Story 1 (Registration) → User Story 6 (Groups)
- **Developer C**: User Story 2 (Admin Management) → User Story 3 (Status) → User Story 7 (Bulk)

---

## Notes

- Total tasks: 105 tasks across 10 phases
- [P] tasks = 32 parallelizable tasks for faster execution
- [Story] labels: 72 story-specific tasks across 7 user stories
- Each user story includes independent test criteria
- MVP scope: Phases 1-4 (40 tasks) for basic member registration and login
- Full P1 scope: Phases 1-5 (55 tasks) for complete core functionality
- SmartAdmin integration points identified throughout
- Database-first approach with explicit schema creation
- Security and audit logging integrated from foundation up