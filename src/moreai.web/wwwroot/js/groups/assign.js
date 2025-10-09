document.addEventListener('DOMContentLoaded', function() {
    const token = localStorage.getItem('token');
    const user = JSON.parse(localStorage.getItem('user'));
    
    if (!token || !user) {
        window.location.href = '/Members/Login';
        return;
    }

    loadGroups();
});

async function loadGroups() {
    try {
        const token = localStorage.getItem('token');
        const user = JSON.parse(localStorage.getItem('user'));

        // 獲取所有群組
        const allGroupsResponse = await fetch('/api/groups', {
            headers: {
                'Authorization': `Bearer ${token}`
            }
        });

        // 獲取會員已加入的群組
        const memberGroupsResponse = await fetch(`/api/groups/member/${user.id}`, {
            headers: {
                'Authorization': `Bearer ${token}`
            }
        });

        if (!allGroupsResponse.ok || !memberGroupsResponse.ok) {
            throw new Error('無法獲取群組資料');
        }

        const allGroups = await allGroupsResponse.json();
        const memberGroups = await memberGroupsResponse.json();

        // 找出可用的群組（所有群組中排除已加入的群組）
        const availableGroups = allGroups.filter(group => 
            !memberGroups.find(mg => mg.id === group.id));

        // 更新UI
        updateGroupsList('availableGroups', availableGroups, false);
        updateGroupsList('assignedGroups', memberGroups, true);
    } catch (error) {
        console.error('Error:', error);
        alert('載入群組資料失敗');
    }
}

function updateGroupsList(elementId, groups, isAssigned) {
    const container = document.getElementById(elementId);
    container.innerHTML = '';

    groups.forEach(group => {
        const item = document.createElement('a');
        item.href = '#';
        item.className = 'list-group-item list-group-item-action d-flex justify-content-between align-items-center';
        
        const content = document.createElement('div');
        content.innerHTML = `
            <h6 class="mb-1">${escapeHtml(group.name)}</h6>
            <small>${escapeHtml(group.description || '')}</small>
        `;

        const button = document.createElement('button');
        button.className = `btn btn-sm ${isAssigned ? 'btn-danger' : 'btn-success'}`;
        button.textContent = isAssigned ? '退出' : '加入';
        button.onclick = (e) => {
            e.preventDefault();
            if (isAssigned) {
                leaveGroup(group.id);
            } else {
                joinGroup(group.id);
            }
        };

        item.appendChild(content);
        item.appendChild(button);
        container.appendChild(item);
    });
}

async function joinGroup(groupId) {
    try {
        const token = localStorage.getItem('token');
        const user = JSON.parse(localStorage.getItem('user'));

        const response = await fetch(`/api/groups/member/${user.id}/groups/${groupId}`, {
            method: 'POST',
            headers: {
                'Authorization': `Bearer ${token}`
            }
        });

        if (!response.ok) {
            throw new Error('加入群組失敗');
        }

        // 重新載入群組列表
        loadGroups();
    } catch (error) {
        console.error('Error:', error);
        alert('加入群組失敗');
    }
}

async function leaveGroup(groupId) {
    try {
        const token = localStorage.getItem('token');
        const user = JSON.parse(localStorage.getItem('user'));

        const response = await fetch(`/api/groups/member/${user.id}/groups/${groupId}`, {
            method: 'DELETE',
            headers: {
                'Authorization': `Bearer ${token}`
            }
        });

        if (!response.ok) {
            throw new Error('退出群組失敗');
        }

        // 重新載入群組列表
        loadGroups();
    } catch (error) {
        console.error('Error:', error);
        alert('退出群組失敗');
    }
}

function escapeHtml(unsafe) {
    return unsafe
        .replace(/&/g, "&amp;")
        .replace(/</g, "&lt;")
        .replace(/>/g, "&gt;")
        .replace(/"/g, "&quot;")
        .replace(/'/g, "&#039;");
}