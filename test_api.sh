#!/bin/bash

echo "Testing UserLog API Endpoints..."

# Function to test endpoint
test_endpoint() {
    echo "Testing: $1"
    response=$(curl -s -X POST "http://localhost:5269$1" -H "Content-Type: application/json")
    echo "Response: $response"
    echo "---"
}

# Test all login endpoints
test_endpoint "/api/UserLog/login-as-manager"
test_endpoint "/api/UserLog/login-as-admin"
test_endpoint "/api/UserLog/login-as-user"

# Test getting recent logs
echo "Getting recent user logs:"
curl -s "http://localhost:5269/api/UserLog?pageSize=5" | python3 -m json.tool
echo "---"

# Test statistics
echo "Getting statistics:"
curl -s "http://localhost:5269/api/UserLog/statistics" | python3 -m json.tool
