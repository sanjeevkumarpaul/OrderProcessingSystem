// Test SignalR availability and connection
console.log("=== SignalR Test Script ===");

// Check if SignalR is loaded
console.log("1. Checking SignalR availability...");
console.log("typeof signalR:", typeof signalR);

if (typeof signalR !== 'undefined') {
    console.log("✅ SignalR library is loaded successfully!");
    
    // Test connection
    console.log("2. Testing SignalR connection...");
    
    const connection = new signalR.HubConnectionBuilder()
        .withUrl("http://localhost:5269/userLogHub")
        .build();
    
    connection.start()
        .then(() => {
            console.log("✅ SignalR connection established!");
            
            // Join the UserActivityMonitors group
            return connection.invoke("JoinUserActivityGroup");
        })
        .then(() => {
            console.log("✅ Joined UserActivityMonitors group!");
            
            // Listen for user login notifications
            connection.on("ReceiveUserLogin", (message) => {
                console.log("🔔 Login notification received:", message);
                if (typeof addLoginNotification === 'function') {
                    addLoginNotification(message);
                } else {
                    console.log("⚠️ addLoginNotification function not found");
                }
            });
            
            console.log("✅ SignalR is fully configured and ready!");
        })
        .catch(err => {
            console.error("❌ SignalR connection error:", err);
        });
        
} else {
    console.error("❌ SignalR library is not loaded!");
    console.log("Check if the script tag is correct in _Host.cshtml");
    console.log("Current script tags should include: <script src=\"/js/signalr.min.js\"></script>");
}
