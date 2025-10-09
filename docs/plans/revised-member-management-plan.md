# 會員管理模組實作計畫（修訂版）

## 技術堆疊
1. 後端框架：ASP.NET Core 8.0
2. 資料庫：
   - MySQL 8.0
   - Dapper 作為 ORM
3. 身份驗證：
   - ASP.NET Core Identity
   - JWT Token 處理
4. 前端技術：
   - Bootstrap 5
   - axios 用於 API 調用
5. 郵件服務：
   - SMTP 或 SendGrid

## 實作階段

### 階段 1：基礎設施建置
1. 資料庫設計與實作
   - 建立 MySQL 資料庫
   - 使用 Dapper 實作資料訪問層
   - 設定資料庫索引和關聯
   - 準備資料庫遷移腳本

2. Identity 框架整合
   - 設定 ASP.NET Core Identity with MySQL
   - 自定義 User 和 Role 存儲
   - 配置密碼政策
   - 設定 JWT 認證

3. 郵件服務設定
   - 實作郵件發送服務介面
   - 配置郵件範本
   - 設定 SMTP 或 SendGrid

### 階段 2：核心功能實作
1. 資料訪問層
   - 實作 Repository 模式
   - 使用 Dapper 建立高效能查詢
   - 實作交易管理

2. 會員功能實作
   - 註冊與驗證流程
   - 登入與 JWT 處理
   - 資料維護 CRUD 操作

3. 前端開發
   - 使用 axios 實作 API 調用
   - 表單驗證與錯誤處理
   - JWT token 管理

### 階段 3：進階功能實作
1. 密碼重設功能
   - 重設流程實作
   - 安全連結生成
   - 前端重設介面

2. 群組管理功能
   - 群組 CRUD 操作
   - 權限檢查機制
   - 管理介面實作

3. 會員群組指派
   - 多重群組關聯處理
   - 批量指派功能
   - 權限驗證

## 資料訪問層設計

### Repository 介面
```csharp
public interface IMemberRepository
{
    Task<Member> GetByIdAsync(Guid id);
    Task<Member> GetByEmailAsync(string email);
    Task<bool> CreateAsync(Member member);
    Task<bool> UpdateAsync(Member member);
    Task<IEnumerable<Member>> GetAllAsync();
}
```

### Dapper 實作
```csharp
public class MemberRepository : IMemberRepository
{
    private readonly IDbConnection _db;
    
    public MemberRepository(IDbConnection db)
    {
        _db = db;
    }
    
    public async Task<Member> GetByIdAsync(Guid id)
    {
        const string sql = @"
            SELECT m.*, g.* 
            FROM Members m 
            LEFT JOIN MemberGroups mg ON m.Id = mg.MemberId
            LEFT JOIN Groups g ON mg.GroupId = g.Id
            WHERE m.Id = @Id";
            
        var memberDict = new Dictionary<Guid, Member>();
        await _db.QueryAsync<Member, Group, Member>(
            sql,
            (member, group) =>
            {
                if (!memberDict.TryGetValue(member.Id, out var memberEntry))
                {
                    memberEntry = member;
                    memberEntry.MemberGroups = new List<MemberGroup>();
                    memberDict.Add(member.Id, memberEntry);
                }
                
                if (group != null)
                {
                    memberEntry.MemberGroups.Add(new MemberGroup
                    {
                        MemberId = member.Id,
                        GroupId = group.Id,
                        Group = group
                    });
                }
                
                return memberEntry;
            },
            new { Id = id },
            splitOn: "Id"
        );
        
        return memberDict.Values.FirstOrDefault();
    }
}
```

## API 端點實作

### Controller 範例
```csharp
[ApiController]
[Route("api/[controller]")]
public class MembersController : ControllerBase
{
    private readonly IMemberRepository _memberRepository;
    private readonly ITokenService _tokenService;
    
    public MembersController(
        IMemberRepository memberRepository,
        ITokenService tokenService)
    {
        _memberRepository = memberRepository;
        _tokenService = tokenService;
    }
    
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        // 實作註冊邏輯
    }
}
```

## 前端實作

### API 服務
```javascript
const api = axios.create({
    baseURL: '/api',
    timeout: 5000,
    headers: {
        'Content-Type': 'application/json'
    }
});

api.interceptors.request.use(config => {
    const token = localStorage.getItem('token');
    if (token) {
        config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
});
```

## 部署考量

### MySQL 部署
1. 使用 Docker 容器化
2. 設定主從複製
3. 備份策略

### 效能優化
1. MySQL 查詢優化
   - 索引設計
   - 查詢計畫分析
2. Dapper 效能調整
   - 批量操作優化
   - 連接池配置

## 測試策略

### 單元測試
```csharp
public class MemberRepositoryTests
{
    private readonly IMemberRepository _repository;
    private readonly IDbConnection _db;
    
    public MemberRepositoryTests()
    {
        _db = new MySqlConnection("connection_string");
        _repository = new MemberRepository(_db);
    }
    
    [Fact]
    public async Task GetById_ShouldReturnMember_WhenExists()
    {
        // 準備測試資料
        // 執行測試
        // 驗證結果
    }
}
```

## 時程規劃（修訂版）
1. 基礎設施建置：6 個工作天
   - MySQL 環境設定：1天
   - Dapper 整合：2天
   - Identity 設定：2天
   - 郵件服務：1天

2. 核心功能實作：8 個工作天
   - 資料訪問層：2天
   - 會員功能：3天
   - 前端開發：3天

3. 進階功能實作：4 個工作天
   - 密碼重設：1天
   - 群組管理：2天
   - 會員群組：1天

4. 測試與調整：2 個工作天
   - 單元測試：1天
   - 整合測試：1天

總計：20 個工作天

## 風險評估（更新）
1. 技術風險
   - MySQL 效能調優
   - Dapper 查詢優化
   - 高併發處理

2. 安全風險
   - SQL 注入防護
   - 密碼存儲安全
   - JWT Token 保護

## 驗收標準（更新）
1. 所有 API 端點回應時間 < 200ms
2. 資料庫查詢效能監控達標
3. 安全性測試通過
4. 使用者介面響應時間 < 1s
5. 代碼覆蓋率 > 85%