# 🔔 Real-Time Notification Testing Guide

## Current Status
✅ Services Running:
- Auth Service: http://localhost:5270
- API Service: http://localhost:5269  
- UI Service: http://localhost:5253

✅ Fixed Issues:
- SignalR parameter mismatch corrected
- Authentication service now calls API on login
- Corrected API endpoint from `log-event` to `login-event`

## Testing Steps

### 1. Test SignalR Connection
1. Open Browser 1: Go to `http://localhost:5253/user-activity`
2. Check top-left corner for connection status
3. Should see: "✅ Connected & Monitoring"
4. Check browser console (F12) for SignalR logs

### 2. Test JavaScript Notification
1. On the User Activity page, wait 3 seconds
2. Should see a test notification in top-right corner
3. If this works, JavaScript and UI are functioning

### 3. Test Real Login Notifications
1. Keep Browser 1 open on User Activity page
2. Open Browser 2 (incognito): `http://localhost:5253`
3. Click "Sign in with Microsoft" or "Sign in with Google"
4. Complete login process
5. Check Browser 1 for notification

## Debug Commands

### Test API Endpoint Directly
```bash
curl -X POST "http://localhost:5269/api/UserLog/login-event" \
  -H "Content-Type: application/json" \
  -d '{"EventType":"ADMIN"}' -v
```

### Test Authentication Service
```bash
curl -s "http://localhost:5270/auth/health"
```

## Browser Console Commands

### Test JavaScript Function Manually
```javascript
// Test notification directly
addLoginNotification("TestUser", "2025-09-04 12:00:00", "192.168.1.1");

// Check if container exists
document.getElementById('login-notifications');

// Check SignalR connection
console.log("SignalR connection state:", window.hubConnection?.state);
```

## Expected Flow
1. **Login** → Auth service processes login
2. **Auth → API** → POST to `/api/UserLog/login-event`
3. **API → SignalR** → Broadcast to `UserActivityMonitors` group
4. **SignalR → Clients** → All connected browsers receive event
5. **JavaScript** → Display professional notification

## Troubleshooting
- ✅ Services running on correct ports
- ✅ API endpoint accessible 
- ✅ Authentication service fixed to call correct endpoint
- ⚠️ Need to verify SignalR hub connection and broadcasting
