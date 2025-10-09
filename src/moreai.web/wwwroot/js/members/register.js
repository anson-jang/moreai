document.getElementById('registerForm').addEventListener('submit', async function(e) {
    e.preventDefault();
    
    const username = document.getElementById('username').value;
    const email = document.getElementById('email').value;
    const firstName = document.getElementById('firstName').value;
    const lastName = document.getElementById('lastName').value;
    const password = document.getElementById('password').value;
    const confirmPassword = document.getElementById('confirmPassword').value;
    const errorMessage = document.getElementById('errorMessage');

    // 驗證密碼
    if (password !== confirmPassword) {
        errorMessage.textContent = '密碼不一致';
        errorMessage.style.display = 'block';
        return;
    }

    try {
        const response = await fetch('/api/members/register', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({
                username,
                email,
                firstName,
                lastName,
                password
            })
        });

        const data = await response.json();

        if (!response.ok) {
            throw new Error(data.message || '註冊失敗');
        }

        // 儲存 token 到 localStorage
        localStorage.setItem('token', data.token);
        localStorage.setItem('user', JSON.stringify(data.user));

        // 導向到首頁
        window.location.href = '/';
    } catch (error) {
        errorMessage.textContent = error.message;
        errorMessage.style.display = 'block';
    }
});