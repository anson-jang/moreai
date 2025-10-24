using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using MySql.Data.MySqlClient;
using System.Data;

namespace Smartadmin.Configuration;

/// <summary>
/// 會員管理系統的依賴注入配置
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// 註冊會員管理系統相關服務
    /// </summary>
    /// <param name="services">服務集合</param>
    /// <param name="configuration">設定</param>
    /// <returns>服務集合</returns>
    public static IServiceCollection AddMemberManagement(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
        // 註冊資料庫連線
        services.AddScoped<IDbConnection>(provider =>
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            return new MySqlConnection(connectionString);
        });

        // 註冊 ASP.NET Core Identity
        services.AddIdentity<IdentityUser, IdentityRole>(options =>
        {
            // 密碼複雜度設定
            var passwordConfig = configuration.GetSection("MemberManagement:PasswordComplexity");
            options.Password.RequiredLength = passwordConfig.GetValue<int>("MinLength", 8);
            options.Password.RequireUppercase = passwordConfig.GetValue<bool>("RequireUppercase", true);
            options.Password.RequireLowercase = passwordConfig.GetValue<bool>("RequireLowercase", true);
            options.Password.RequireDigit = passwordConfig.GetValue<bool>("RequireDigit", true);
            options.Password.RequireNonAlphanumeric = passwordConfig.GetValue<bool>("RequireSpecialChar", true);

            // 帳號鎖定設定
            var lockoutConfig = configuration.GetSection("MemberManagement:AccountLocking");
            options.Lockout.MaxFailedAccessAttempts = lockoutConfig.GetValue<int>("MaxFailedAttempts", 5);
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(
                lockoutConfig.GetValue<int>("LockoutDurationMinutes", 30));
            options.Lockout.AllowedForNewUsers = true;

            // 使用者設定
            options.User.RequireUniqueEmail = true;
            options.SignIn.RequireConfirmedEmail = configuration
                .GetSection("MemberManagement:EmailVerification")
                .GetValue<bool>("RequireVerification", true);
        })
        .AddDefaultTokenProviders();

        // 註冊 JWT 認證
        var jwtConfig = configuration.GetSection("Authentication");
        var jwtSecret = jwtConfig.GetValue<string>("JwtSecret");
        
        if (string.IsNullOrEmpty(jwtSecret))
        {
            throw new InvalidOperationException("JWT Secret is required in Authentication:JwtSecret configuration");
        }

        var key = Encoding.UTF8.GetBytes(jwtSecret);

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = false; // 開發環境設定，生產環境應該設為 true
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = jwtConfig.GetValue<string>("JwtIssuer"),
                ValidateAudience = true,
                ValidAudience = jwtConfig.GetValue<string>("JwtAudience"),
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };
        })
        .AddCookie(options =>
        {
            options.LoginPath = "/Auth/Login";
            options.LogoutPath = "/Auth/Logout";
            options.AccessDeniedPath = "/Auth/AccessDenied";
            options.ExpireTimeSpan = TimeSpan.FromHours(
                jwtConfig.GetValue<int>("SessionExpirationHours", 24));
            options.SlidingExpiration = true;
        });

        // 註冊授權策略
        services.AddAuthorization(options =>
        {
            options.AddPolicy("AdminOnly", policy =>
                policy.RequireRole("Administrator", "Admin"));
            
            options.AddPolicy("MemberOrAdmin", policy =>
                policy.RequireAuthenticatedUser());
        });

        return services;
    }

    /// <summary>
    /// 註冊會員管理業務服務
    /// </summary>
    /// <param name="services">服務集合</param>
    /// <returns>服務集合</returns>
    public static IServiceCollection AddMemberManagementServices(this IServiceCollection services)
    {
        // Repository 層服務將在 Phase 2 中註冊
        // Service 層服務將在 Phase 2 中註冊
        // 這裡預留擴展點

        return services;
    }

    /// <summary>
    /// 註冊第三方服務
    /// </summary>
    /// <param name="services">服務集合</param>
    /// <param name="configuration">設定</param>
    /// <returns>服務集合</returns>
    public static IServiceCollection AddExternalServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // 註冊 SendGrid 電子郵件服務
        var emailConfig = configuration.GetSection("Email");
        var emailProvider = emailConfig.GetValue<string>("Provider");

        if (emailProvider?.ToLowerInvariant() == "sendgrid")
        {
            var sendGridConfig = emailConfig.GetSection("SendGrid");
            var apiKey = sendGridConfig.GetValue<string>("ApiKey");
            
            if (!string.IsNullOrEmpty(apiKey))
            {
                // SendGrid 服務註冊將在實作 EmailService 時完成
            }
        }

        return services;
    }
}