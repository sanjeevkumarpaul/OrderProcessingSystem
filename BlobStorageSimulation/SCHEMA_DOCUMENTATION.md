# JSON Schema Documentation

## OrderTransaction.json Schema

This file represents a complete order transaction between a supplier and customer.

### Required Structure:
```json
{
    "Supplier": {
        "Name": "string (required) - Name of the supplier",
        "Quantity": "number (required) - Quantity of items",
        "Price": "number (required) - Price per item (must be between 200-1000)"
    },
    "Customer": {
        "Name": "string (required) - Name of the customer",
        "Quantity": "number (required) - Must match supplier quantity exactly",
        "Price": "number (required) - Must match supplier price exactly"
    }
}
```

### Validation Rules:
- Supplier and Customer names are required
- Supplier and Customer quantities must match exactly
- Supplier and Customer prices must match exactly
- Quantity must be greater than 0
- Price must be between 200 and 1000

### Example:
```json
{
    "Supplier": {
        "Name": "ABC Electronics Ltd",
        "Quantity": 25,
        "Price": 750.50
    },
    "Customer": {
        "Name": "Tech Solutions Inc",
        "Quantity": 25,
        "Price": 750.50
    }
}
```

## OrderCancellation.json Schema

This file represents an order cancellation request.

### Required Structure:
```json
{
    "Customer": "string (required) - Name of the customer",
    "Supplier": "string (required) - Name of the supplier", 
    "Quantity": "number (required) - Quantity to cancel (must be > 0)"
}
```

### Validation Rules:
- Customer name is required
- Supplier name is required
- Quantity must be greater than 0

### Example:
```json
{
    "Customer": "Tech Solutions Inc",
    "Supplier": "ABC Electronics Ltd",
    "Quantity": 15
}
```

## Background Service Processing

The background service monitors the BlobStorageSimulation folder for:
- OrderTransaction.json files
- OrderCancellation.json files

When files are detected, they are:
1. Parsed and validated against the schema
2. Logged with detailed information
3. Processed according to business rules
4. Errors are logged if validation fails

The service uses both file system watching and periodic polling for reliability.
