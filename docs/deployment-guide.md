# MoreAI 會員管理系統部署指南

## 系統需求

- .NET 9.0 SDK
- MySQL 8.0 或更新版本
- 支援 HTTPS 的網頁伺服器（建議使用 Nginx）

## 部署步驟

### 1. 準備資料庫
1. 建立新的 MySQL 資料庫：
```sql
CREATE DATABASE moreai_db CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
```

2. 建立資料庫用戶：
```sql
CREATE USER 'moreai_user'@'localhost' IDENTIFIED BY '您的密碼';
GRANT ALL PRIVILEGES ON moreai_db.* TO 'moreai_user'@'localhost';
FLUSH PRIVILEGES;
```

3. 執行資料庫遷移：
```bash
dotnet ef database update
```

### 2. 應用程式設定

1. 更新 appsettings.json：
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Port=3306;Database=moreai_db;User=moreai_user;Password=您的密碼;"
  },
  "Jwt": {
    "SecretKey": "您的JWT密鑰",
    "Issuer": "您的網域",
    "Audience": "您的網域"
  }
}
```

2. 設定 HTTPS：
- 安裝 SSL 憑證
- 更新 Nginx 設定檔

### 3. 發佈應用程式

1. 建立發佈檔案：
```bash
dotnet publish -c Release -o ./publish
```

2. 將發佈檔案複製到伺服器：
```bash
scp -r ./publish/* user@your-server:/var/www/moreai/
```

### 4. 設定 Nginx

建立新的 Nginx 設定檔：
```nginx
server {
    listen 443 ssl;
    server_name your-domain.com;

    ssl_certificate /path/to/your/certificate.crt;
    ssl_certificate_key /path/to/your/private.key;

    location / {
        proxy_pass http://localhost:5000;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection keep-alive;
        proxy_set_header Host $host;
        proxy_cache_bypass $http_upgrade;
        proxy_set_header X-Real-IP $remote_addr;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
    }
}
```

### 5. 啟動服務

1. 建立系統服務：
```bash
sudo nano /etc/systemd/system/moreai.service
```

2. 加入服務設定：
```ini
[Unit]
Description=MoreAI Web Application
After=network.target

[Service]
WorkingDirectory=/var/www/moreai
ExecStart=/usr/bin/dotnet /var/www/moreai/moreai.web.dll
Restart=always
RestartSec=10
SyslogIdentifier=moreai
User=www-data
Environment=ASPNETCORE_ENVIRONMENT=Production

[Install]
WantedBy=multi-user.target
```

3. 啟動服務：
```bash
sudo systemctl enable moreai
sudo systemctl start moreai
```

## 監控與維護

### 日誌查看
```bash
sudo journalctl -u moreai.service -f
```

### 效能監控
- 使用 Application Insights 或其他監控工具
- 定期檢查資料庫效能
- 監控 API 回應時間

### 備份策略
1. 定期備份資料庫：
```bash
mysqldump -u moreai_user -p moreai_db > backup.sql
```

2. 設定自動備份排程：
```bash
0 2 * * * mysqldump -u moreai_user -p moreai_db > /backup/moreai_$(date +\%Y\%m\%d).sql
```