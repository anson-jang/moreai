let groupsTable;
let currentGroupId;

document.addEventListener('DOMContentLoaded', function() {
    const token = localStorage.getItem('token');
    if (!token) {
        window.location.href = '/Members/Login';
        return;
    }

    loadGroups();

    document.getElementById('saveGroup').addEventListener('click', handleSaveGroup);
    document.getElementById('confirmDelete').addEventListener('click', handleDeleteGroup);
});

async function loadGroups() {
    try {
        const token = localStorage.getItem('token');
        const response = await fetch('/api/groups', {
            headers: {
                'Authorization': `Bearer ${token}`
            }
        });

        if (!response.ok) {
            throw new Error('無法獲取群組資料');
        }

        const groups = await response.json();
        const tbody = document.querySelector('#groupsTable tbody');
        tbody.innerHTML = '';

        groups.forEach(group => {
            const tr = document.createElement('tr');
            tr.innerHTML = `
                <td>${escapeHtml(group.name)}</td>
                <td>${escapeHtml(group.description || '')}</td>
                <td>${new Date(group.createdAt).toLocaleString()}</td>
                <td>${group.isActive ? '<span class="badge bg-success">啟用</span>' : '<span class="badge bg-danger">停用</span>'}</td>
                <td>
                    <button class="btn btn-sm btn-primary" onclick="editGroup(${group.id})">編輯</button>
                    <button class="btn btn-sm btn-danger" onclick="showDeleteConfirm(${group.id})">刪除</button>
                </td>
            `;
            tbody.appendChild(tr);
        });
    } catch (error) {
        console.error('Error:', error);
        alert('載入群組資料失敗');
    }
}

async function handleSaveGroup() {
    const token = localStorage.getItem('token');
    const name = document.getElementById('groupName').value;
    const description = document.getElementById('groupDescription').value;
    const isActive = document.getElementById('groupIsActive').checked;
    const errorMessage = document.getElementById('groupErrorMessage');

    try {
        const url = currentGroupId ? 
            `/api/groups/${currentGroupId}` : 
            '/api/groups';

        const method = currentGroupId ? 'PUT' : 'POST';

        const response = await fetch(url, {
            method: method,
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${token}`
            },
            body: JSON.stringify({
                name,
                description,
                isActive
            })
        });

        const data = await response.json();

        if (!response.ok) {
            throw new Error(data.message || '操作失敗');
        }

        // 關閉對話框並重新載入資料
        bootstrap.Modal.getInstance(document.getElementById('groupModal')).hide();
        loadGroups();
        resetForm();
    } catch (error) {
        errorMessage.textContent = error.message;
        errorMessage.style.display = 'block';
    }
}

async function editGroup(id) {
    try {
        const token = localStorage.getItem('token');
        const response = await fetch(`/api/groups/${id}`, {
            headers: {
                'Authorization': `Bearer ${token}`
            }
        });

        if (!response.ok) {
            throw new Error('無法獲取群組資料');
        }

        const group = await response.json();
        
        // 設定表單資料
        currentGroupId = group.id;
        document.getElementById('groupName').value = group.name;
        document.getElementById('groupDescription').value = group.description || '';
        document.getElementById('groupIsActive').checked = group.isActive;
        document.getElementById('groupModalTitle').textContent = '編輯群組';
        
        // 顯示對話框
        new bootstrap.Modal(document.getElementById('groupModal')).show();
    } catch (error) {
        console.error('Error:', error);
        alert('載入群組資料失敗');
    }
}

function showDeleteConfirm(id) {
    currentGroupId = id;
    new bootstrap.Modal(document.getElementById('deleteConfirmModal')).show();
}

async function handleDeleteGroup() {
    try {
        const token = localStorage.getItem('token');
        const response = await fetch(`/api/groups/${currentGroupId}`, {
            method: 'DELETE',
            headers: {
                'Authorization': `Bearer ${token}`
            }
        });

        if (!response.ok) {
            throw new Error('刪除失敗');
        }

        // 關閉對話框並重新載入資料
        bootstrap.Modal.getInstance(document.getElementById('deleteConfirmModal')).hide();
        loadGroups();
    } catch (error) {
        console.error('Error:', error);
        alert('刪除群組失敗');
    }
}

function resetForm() {
    currentGroupId = null;
    document.getElementById('groupForm').reset();
    document.getElementById('groupModalTitle').textContent = '新增群組';
    document.getElementById('groupErrorMessage').style.display = 'none';
}

function escapeHtml(unsafe) {
    return unsafe
        .replace(/&/g, "&amp;")
        .replace(/</g, "&lt;")
        .replace(/>/g, "&gt;")
        .replace(/"/g, "&quot;")
        .replace(/'/g, "&#039;");
}

// 當對話框關閉時重置表單
document.getElementById('groupModal').addEventListener('hidden.bs.modal', resetForm);