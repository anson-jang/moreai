document.addEventListener('DOMContentLoaded', async function() {
    const user = JSON.parse(localStorage.getItem('user'));
    const token = localStorage.getItem('token');

    if (!user || !token) {
        window.location.href = '/Members/Login';
        return;
    }

    try {
        const response = await fetch(`/api/members/${user.id}`, {
            headers: {
                'Authorization': `Bearer ${token}`
            }
        });

        if (!response.ok) {
            throw new Error('無法獲取會員資料');
        }

        const data = await response.json();
        document.getElementById('username').value = data.username;
        document.getElementById('email').value = data.email;
        document.getElementById('firstName').value = data.firstName;
        document.getElementById('lastName').value = data.lastName;
    } catch (error) {
        showError(error.message);
    }
});

document.getElementById('profileForm').addEventListener('submit', async function(e) {
    e.preventDefault();
    
    const user = JSON.parse(localStorage.getItem('user'));
    const token = localStorage.getItem('token');
    const errorMessage = document.getElementById('errorMessage');
    const successMessage = document.getElementById('successMessage');

    const email = document.getElementById('email').value;
    const firstName = document.getElementById('firstName').value;
    const lastName = document.getElementById('lastName').value;
    const currentPassword = document.getElementById('currentPassword').value;
    const newPassword = document.getElementById('newPassword').value;
    const confirmNewPassword = document.getElementById('confirmNewPassword').value;

    // 檢查新密碼是否一致
    if (newPassword && newPassword !== confirmNewPassword) {
        showError('新密碼不一致');
        return;
    }

    const updateData = {
        email,
        firstName,
        lastName
    };

    // 如果有輸入新密碼，加入密碼相關欄位
    if (newPassword) {
        if (!currentPassword) {
            showError('請輸入目前的密碼');
            return;
        }
        updateData.currentPassword = currentPassword;
        updateData.newPassword = newPassword;
    }

    try {
        const response = await fetch(`/api/members/${user.id}`, {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${token}`
            },
            body: JSON.stringify(updateData)
        });

        const data = await response.json();

        if (!response.ok) {
            throw new Error(data.message || '更新失敗');
        }

        // 更新 localStorage 中的使用者資料
        const updatedUser = {
            ...user,
            email: data.email,
            firstName: data.firstName,
            lastName: data.lastName
        };
        localStorage.setItem('user', JSON.stringify(updatedUser));

        // 清除密碼欄位
        document.getElementById('currentPassword').value = '';
        document.getElementById('newPassword').value = '';
        document.getElementById('confirmNewPassword').value = '';

        // 顯示成功訊息
        showSuccess('資料更新成功');
    } catch (error) {
        showError(error.message);
    }
});

function showError(message) {
    const errorMessage = document.getElementById('errorMessage');
    const successMessage = document.getElementById('successMessage');
    errorMessage.textContent = message;
    errorMessage.style.display = 'block';
    successMessage.style.display = 'none';
}

function showSuccess(message) {
    const errorMessage = document.getElementById('errorMessage');
    const successMessage = document.getElementById('successMessage');
    successMessage.textContent = message;
    successMessage.style.display = 'block';
    errorMessage.style.display = 'none';
}