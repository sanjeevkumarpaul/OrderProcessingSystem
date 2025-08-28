# JSON File Validation Unit Tests

## Overview

This test suite provides comprehensive validation for JSON files used in the OrderProcessing background service. The tests ensure that JSON files conform to the expected schemas and that validation logic works correctly.

## Test Coverage

### 1. BlobStorageMonitorServiceTests.cs
**35 Tests Total** - Validates the background service's JSON processing logic

#### OrderTransaction Validation Tests (10 tests)
- ✅ `ValidateOrderTransaction_ValidData_ReturnsTrue` - Tests valid order transaction
- ✅ `ValidateOrderTransaction_EmptySupplierName_ReturnsFalse` - Tests empty supplier name validation
- ✅ `ValidateOrderTransaction_EmptyCustomerName_ReturnsFalse` - Tests empty customer name validation
- ✅ `ValidateOrderTransaction_MismatchedQuantities_ReturnsFalse` - Tests quantity matching validation
- ✅ `ValidateOrderTransaction_MismatchedPrices_ReturnsFalse` - Tests price matching validation
- ✅ `ValidateOrderTransaction_ZeroQuantity_ReturnsFalse` - Tests zero quantity validation
- ✅ `ValidateOrderTransaction_NegativeQuantity_ReturnsFalse` - Tests negative quantity validation
- ✅ `ValidateOrderTransaction_PriceBelowMinimum_ReturnsFalse` - Tests price minimum (200) validation
- ✅ `ValidateOrderTransaction_PriceAboveMaximum_ReturnsFalse` - Tests price maximum (1000) validation
- ✅ `ValidateOrderTransaction_PriceAtMinimum_ReturnsTrue` - Tests boundary condition (price = 200)
- ✅ `ValidateOrderTransaction_PriceAtMaximum_ReturnsTrue` - Tests boundary condition (price = 1000)

#### OrderCancellation Validation Tests (6 tests)
- ✅ `ValidateOrderCancellation_ValidData_ReturnsTrue` - Tests valid cancellation data
- ✅ `ValidateOrderCancellation_EmptyCustomerName_ReturnsFalse` - Tests customer name requirement
- ✅ `ValidateOrderCancellation_EmptySupplierName_ReturnsFalse` - Tests supplier name requirement
- ✅ `ValidateOrderCancellation_ZeroQuantity_ReturnsFalse` - Tests positive quantity requirement
- ✅ `ValidateOrderCancellation_NegativeQuantity_ReturnsFalse` - Tests negative quantity validation

#### JSON Serialization Tests (4 tests)
- ✅ `OrderTransactionModel_SerializeDeserialize_MaintainsData` - Tests round-trip serialization
- ✅ `OrderCancellationModel_SerializeDeserialize_MaintainsData` - Tests round-trip serialization
- ✅ `OrderTransactionModel_DeserializeInvalidJson_ThrowsJsonException` - Tests invalid JSON handling
- ✅ `OrderCancellationModel_DeserializeInvalidJson_ThrowsJsonException` - Tests invalid JSON handling

#### Schema Compliance Tests (15 tests using Theory/InlineData)
- ✅ Multiple scenarios testing various combinations of valid/invalid data
- Tests empty strings, null values, boundary conditions, and edge cases
- Validates JSON string parsing with different data combinations

### 2. JsonSchemaValidationTests.cs
**Additional schema-focused tests**

#### JSON Structure Tests
- ✅ `OrderTransactionModel_ValidJsonStructure_DeserializesCorrectly` - Tests complete JSON structure
- ✅ `OrderCancellationModel_ValidJsonStructure_DeserializesCorrectly` - Tests complete JSON structure
- ✅ `OrderTransactionModel_WrongFieldTypes_ThrowsJsonException` - Tests type validation
- ✅ `OrderCancellationModel_WrongFieldTypes_ThrowsJsonException` - Tests type validation
- ✅ `OrderTransactionModel_EmptyJson_CreatesDefaultObject` - Tests empty JSON handling
- ✅ `OrderCancellationModel_EmptyJson_CreatesDefaultObject` - Tests empty JSON handling

#### Theory-based Validation Tests
- ✅ `OrderTransactionValidation_VariousScenarios_ReturnsExpectedResult` - Multiple parameter combinations
- ✅ `OrderCancellationValidation_VariousScenarios_ReturnsExpectedResult` - Multiple parameter combinations

## Validation Rules Tested

### OrderTransaction.json Schema
```json
{
    "Supplier": {
        "Name": "string (required, non-empty)",
        "Quantity": "number (required, > 0)",
        "Price": "number (required, 200-1000)"
    },
    "Customer": {
        "Name": "string (required, non-empty)", 
        "Quantity": "number (must match supplier quantity exactly)",
        "Price": "number (must match supplier price exactly)"
    }
}
```

**Validation Rules:**
- ✅ Supplier name is required and non-empty
- ✅ Customer name is required and non-empty
- ✅ Quantities must match between supplier and customer
- ✅ Prices must match between supplier and customer
- ✅ Quantity must be greater than 0
- ✅ Price must be between 200 and 1000 (inclusive)

### OrderCancellation.json Schema
```json
{
    "Customer": "string (required, non-empty)",
    "Supplier": "string (required, non-empty)",
    "Quantity": "number (required, > 0)"
}
```

**Validation Rules:**
- ✅ Customer name is required and non-empty
- ✅ Supplier name is required and non-empty  
- ✅ Quantity must be greater than 0

## Test Features

### Comprehensive Coverage
- **Positive Tests** - Valid data scenarios
- **Negative Tests** - Invalid data scenarios
- **Boundary Tests** - Edge cases and limits
- **Error Handling** - JSON parsing exceptions
- **Type Safety** - Wrong data types
- **Schema Compliance** - Structure validation

### Advanced Testing Techniques
- **Theory/InlineData** - Parameterized tests for multiple scenarios
- **Reflection** - Testing private methods through reflection
- **Mocking** - Using Moq for dependency injection
- **JSON Serialization** - Round-trip testing
- **Exception Testing** - Validating error conditions

### Mock Setup
```csharp
var mockLogger = new Mock<ILogger<BlobStorageMonitorService>>();
var mockServiceProvider = new Mock<IServiceProvider>();
var mockOptions = new Mock<IOptions<BlobStorageSimulationOptions>>();
```

## Running the Tests

```bash
# Build the test project
cd OrderProcessingSystem.Tests
dotnet build

# Run all tests
dotnet test

# Run with detailed output
dotnet test --verbosity normal

# Run specific test class
dotnet test --filter "JsonSchemaValidationTests"
dotnet test --filter "BlobStorageMonitorServiceTests"
```

## Integration with Background Service

These tests validate the core logic that runs in the `BlobStorageMonitorService`:

1. **File Detection** - Service monitors for JSON files
2. **JSON Parsing** - Files are deserialized to models
3. **Schema Validation** - Models are validated using tested methods
4. **Error Logging** - Invalid files are logged and rejected
5. **Processing** - Valid files are processed for business logic

The unit tests ensure that each step of this pipeline works correctly with various input scenarios.
