using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using moreai.web.Services;
using moreai.web.Configurations;
using moreai.web.Data;
using moreai.web.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();

// 添加身份驗證
builder.Services.AddAuthentication("CookieAuth")
    .AddCookie("CookieAuth", options =>
    {
        options.Cookie.Name = "UserLoginCookie";
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromDays(30);
    });

// 添加 DbContext
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? throw new InvalidOperationException("找不到 DefaultConnection 連接字串。");
var serverVersion = ServerVersion.AutoDetect(connectionString);
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, serverVersion));

// 添加記憶體快取
builder.Services.AddMemoryCache();
builder.Services.AddScoped<ICacheService, MemoryCacheService>();

// 添加郵件服務
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("Email"));
builder.Services.AddScoped<IEmailService, EmailService>();

// 配置回應快取
builder.Services.AddResponseCaching(options =>
{
    options.MaximumBodySize = 1024;
    options.UseCaseSensitivePaths = true;
});

// 配置 CORS
var allowedOrigins = builder.Configuration["AllowedOrigins"]?.Split(',') ?? new[] { "*" };
builder.Services.AddCors(options =>
{
    options.AddPolicy("DefaultPolicy",
        policyBuilder => policyBuilder
            .WithOrigins(allowedOrigins)
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// 更新管理員角色和創建Admin群組
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var adminEmail = "ansonjang@gmail.com";
    
    // 確保Admin群組存在
    var adminGroup = context.Groups.FirstOrDefault(g => g.Name == "Admin");
    if (adminGroup == null)
    {
        adminGroup = new Group
        {
            Name = "Admin",
            Description = "系統管理員群組",
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };
        context.Groups.Add(adminGroup);
        context.SaveChanges();
    }

    // 更新管理員用戶
    var admin = context.Members
        .Include(m => m.Groups)
        .FirstOrDefault(m => m.Email == adminEmail);
    
    if (admin != null)
    {
        // 設置Admin角色
        if (string.IsNullOrEmpty(admin.Role))
        {
            admin.Role = "Admin";
        }

        // 確保管理員在Admin群組中
        if (!admin.Groups.Any(g => g.GroupId == adminGroup.Id))
        {
            admin.Groups.Add(new MemberGroup
            {
                GroupId = adminGroup.Id,
                MemberId = admin.Id,
                JoinedAt = DateTime.UtcNow
            });
        }
        
        context.SaveChanges();
    }
}

app.Run();
