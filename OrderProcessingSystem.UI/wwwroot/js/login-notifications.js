window.addLoginNotification = function(userName, loginTime, ipAddress) {
    console.log('addLoginNotification called with:', userName, loginTime, ipAddress);
    const container = document.getElementById('login-notifications');
    if (container) {
        console.log('Container found, adding professional notification');
        
        // Format the message
        const message = `User ${userName} logged in at ${loginTime} from ${ipAddress}`;
        
        // Create professional notification HTML with 50% transparency
        const notificationHtml = `
            <div class='login-notification' style='
                background: rgba(255, 255, 255, 0.5);
                backdrop-filter: blur(10px);
                border: 1px solid rgba(255, 255, 255, 0.2);
                border-radius: 12px;
                box-shadow: 0 8px 32px rgba(0, 0, 0, 0.1);
                padding: 16px 20px;
                margin-bottom: 12px;
                animation: slideIn 0.4s cubic-bezier(0.25, 0.46, 0.45, 0.94);
                transition: all 0.3s ease;
                cursor: pointer;
                position: relative;
                overflow: hidden;
            '>
                <div style='
                    position: absolute;
                    top: 0;
                    left: 0;
                    width: 4px;
                    height: 100%;
                    background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
                '></div>
                <div style='display: flex; align-items: center; padding-left: 12px;'>
                    <div style='
                        width: 40px;
                        height: 40px;
                        background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
                        border-radius: 50%;
                        display: flex;
                        align-items: center;
                        justify-content: center;
                        margin-right: 12px;
                        box-shadow: 0 4px 12px rgba(102, 126, 234, 0.3);
                    '>
                        <i class='fas fa-user-check' style='color: white; font-size: 18px;'></i>
                    </div>
                    <div style='flex: 1;'>
                        <div style='
                            font-weight: 600;
                            font-size: 14px;
                            color: #2d3748;
                            margin-bottom: 4px;
                            letter-spacing: 0.01em;
                        '>Login Activity</div>
                        <div style='
                            font-size: 13px;
                            color: #4a5568;
                            line-height: 1.4;
                        '>${message}</div>
                    </div>
                    <div style='
                        width: 8px;
                        height: 8px;
                        background: #48bb78;
                        border-radius: 50%;
                        margin-left: 8px;
                        animation: pulse 2s infinite;
                    '></div>
                </div>
            </div>
        `;
        
        // Add the notification
        container.insertAdjacentHTML('afterbegin', notificationHtml);
        
        // Auto-remove after 4 seconds with smooth fade out
        const notifications = container.querySelectorAll('.login-notification');
        const latestNotification = notifications[0];
        
        if (latestNotification) {
            console.log('Setting up auto-remove for professional notification');
            setTimeout(() => {
                latestNotification.style.transform = 'translateX(110%)';
                latestNotification.style.opacity = '0';
                
                setTimeout(() => {
                    if (latestNotification.parentNode) {
                        latestNotification.parentNode.removeChild(latestNotification);
                        console.log('Professional notification removed');
                    }
                }, 400);
            }, 4000);
        }
        
        // Keep only the latest 5 notifications
        const allNotifications = container.querySelectorAll('.login-notification');
        if (allNotifications.length > 5) {
            for (let i = 5; i < allNotifications.length; i++) {
                allNotifications[i].remove();
            }
        }
    } else {
        console.error('Container with id "login-notifications" not found!');
    }
};

// Add CSS animations if not already added
if (!document.getElementById('login-notification-styles')) {
    const style = document.createElement('style');
    style.id = 'login-notification-styles';
    style.textContent = `
        @keyframes slideIn {
            from {
                transform: translateX(110%);
                opacity: 0;
            }
            to {
                transform: translateX(0);
                opacity: 1;
            }
        }
        
        @keyframes pulse {
            0% {
                opacity: 1;
                transform: scale(1);
            }
            50% {
                opacity: 0.7;
                transform: scale(1.2);
            }
            100% {
                opacity: 1;
                transform: scale(1);
            }
        }
        
        .login-notification:hover {
            transform: translateY(-2px) !important;
            box-shadow: 0 12px 48px rgba(0, 0, 0, 0.15) !important;
        }
    `;
    document.head.appendChild(style);
}

// Test function to verify the notification system works
window.testNotification = function() {
    console.log('Testing notification function...');
    const container = document.getElementById('login-notifications');
    if (container) {
        console.log('✅ Container found, calling addLoginNotification');
        addLoginNotification('testUser', new Date().toLocaleString(), '192.168.1.100');
    } else {
        console.error('❌ Container with id "login-notifications" not found!');
        console.log('Available elements:', document.querySelectorAll('[id]'));
    }
};

// Script loaded successfully
console.log('login-notifications.js loaded successfully');

// Manual SignalR test function
// Test SignalR function with improved debugging
function testSignalR() {
    console.log("=== Testing SignalR ===");
    console.log("typeof signalR:", typeof signalR);
    
    if (typeof signalR === 'undefined') {
        console.error("❌ SignalR is not defined. Library not loaded!");
        
        // Try to wait and check again
        setTimeout(() => {
            console.log("Checking again after 1 second...");
            console.log("typeof signalR:", typeof signalR);
            
            if (typeof signalR !== 'undefined') {
                console.log("✅ SignalR loaded after delay!");
                performSignalRTest();
            } else {
                console.error("❌ SignalR still not loaded after delay");
            }
        }, 1000);
        
        return;
    }
    
    performSignalRTest();
}

function performSignalRTest() {
    console.log("✅ SignalR is available!");
    console.log("SignalR VERSION:", signalR.VERSION);
    
    try {
        // Create connection
        const connection = new signalR.HubConnectionBuilder()
            .withUrl("http://localhost:5269/userLogHub")
            .configureLogging(signalR.LogLevel.Information)
            .build();
        
        console.log("✅ HubConnection created successfully");
        
        // Start connection
            connection.start()
                .then(() => {
                    console.log("✅ Connected to SignalR Hub!");
                    
                    // Add visible debug for connection
                    let debugDiv = document.getElementById('signalr-debug');
                    if (!debugDiv) {
                        debugDiv = document.createElement('div');
                        debugDiv.id = 'signalr-debug';
                        debugDiv.style.position = 'fixed';
                        debugDiv.style.bottom = '10px';
                        debugDiv.style.right = '10px';
                        debugDiv.style.background = '#222';
                        debugDiv.style.color = '#0f0';
                        debugDiv.style.padding = '8px 16px';
                        debugDiv.style.zIndex = 9999;
                        debugDiv.style.fontSize = '16px';
                        document.body.appendChild(debugDiv);
                    }
                    debugDiv.innerText = 'SignalR: Connected to hub successfully!';
                    
                    // Join the UserActivityMonitors group (though it should be automatic)
                    return connection.invoke("JoinUserActivityGroup");
                })
                .then(() => {
                    console.log("✅ Joined UserActivityMonitors group!");
                    
                    // Update debug message
                    let debugDiv = document.getElementById('signalr-debug');
                    if (debugDiv) {
                        debugDiv.innerText = 'SignalR: Connected and joined group! Waiting for notifications...';
                    }
                    
                    // Set up message handler
                    connection.on("UserLoggedIn", (message) => {
                        console.log("🔔 Received login notification:", message);
                        // Add a visible debug message to the page
                        let debugDiv = document.getElementById('signalr-debug');
                        if (debugDiv) {
                            debugDiv.innerText = 'SignalR: Received login notification! ' + (message && message.Message ? message.Message : JSON.stringify(message));
                        }
                        if (typeof addLoginNotification === 'function') {
                            addLoginNotification(message);
                        }
                    });
                    
                    console.log("✅ SignalR is fully configured and working!");
                })
                .catch(err => {
                    console.error("❌ SignalR connection failed:", err);
                    
                    // Add visible error message
                    let debugDiv = document.getElementById('signalr-debug');
                    if (!debugDiv) {
                        debugDiv = document.createElement('div');
                        debugDiv.id = 'signalr-debug';
                        debugDiv.style.position = 'fixed';
                        debugDiv.style.bottom = '10px';
                        debugDiv.style.right = '10px';
                        debugDiv.style.background = '#822';
                        debugDiv.style.color = '#f00';
                        debugDiv.style.padding = '8px 16px';
                        debugDiv.style.zIndex = 9999;
                        debugDiv.style.fontSize = '16px';
                        document.body.appendChild(debugDiv);
                    }
                    debugDiv.innerText = 'SignalR: Connection failed! ' + err.toString();
                });    } catch (ex) {
        console.error("❌ Exception in SignalR test:", ex);
    }
}
