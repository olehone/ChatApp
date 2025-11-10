const connection = new signalR.HubConnectionBuilder()
    .withUrl("/chathub")
    .withAutomaticReconnect()
    .build();

connection.on("ReceiveMessage", data => addMessage(data));

connection.start()
    .then(() => {
        console.log("Connected to SignalR");
        loadMessages();
    })
    .catch(err => console.error(err));

document.getElementById('sendBtn').addEventListener('click', sendMessage);
document.getElementById('messageInput').addEventListener('keypress', e => {
    if (e.key === 'Enter') sendMessage();
});

async function sendMessage() {
    const username = document.getElementById('usernameInput').value.trim();
    const message = document.getElementById('messageInput').value.trim();
    if (!username || !message) return alert('Enter name and message');

    try {
        await connection.invoke("SendMessage", username, message);
        document.getElementById('messageInput').value = '';
    } catch (err) {
        console.error(err);
        alert('Failed to send message');
    }
}

async function loadMessages() {
    try {
        const res = await fetch("/api/chat/messages");
        if (!res.ok) throw new Error("Failed to fetch messages");
        const data = await res.json();
        document.getElementById('messages').innerHTML = '';
        data.forEach(addMessage);
    } catch (err) {
        console.error("Error loading messages:", err);
    }
}

function addMessage(data) {
    const container = document.getElementById('messages');
    const div = document.createElement('div');
    div.className = 'message';

    const usernameColor = getUsernameColor(data.username);

    let sentimentBadge = '';
    if (data.sentiment) {
        sentimentBadge = `<span class="sentiment ${data.sentiment}">${data.sentiment}</span>`;
    }

    div.innerHTML = `
        <div class="message-header">
            <span class="username" style="color: ${usernameColor}">${escapeHtml(data.username)}</span>
            <span class="timestamp">${new Date(data.timestamp).toLocaleTimeString()} ${sentimentBadge}</span>
        </div>
        <div class="message-text">${escapeHtml(data.message)}</div>
    `;
    container.appendChild(div);
    container.scrollTop = container.scrollHeight;
}


function getUsernameColor(username) {
    let hash = 0;
    for (let i = 0; i < username.length; i++) {
        hash = username.charCodeAt(i) + ((hash << 5) - hash);
    }
    const h = hash % 360;
    return `hsl(${h}, 70%, 50%)`;
}


function escapeHtml(text) {
    const div = document.createElement('div');
    div.textContent = text;
    return div.innerHTML;
}
