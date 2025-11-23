# Item/Node System Architecture - D3Core v3

## What We've Built

### 1. Repository Layer (Domain/Infrastructure)
**Location:** `server/DaemonsMCP.Infrastructure/Repositories/`

- **IItemRepository & ItemRepository**
  - CRUD operations for Items
  - Hierarchical queries (GetByIdWithChildren, GetByParentId, GetRootItems)
  - Search with multiple filters (name, details, type, status, parent)
  - Cascade delete support

- **IItemTypeRepository & ItemTypeRepository**
  - CRUD operations for ItemTypes (used for both item types and status types)
  - Hierarchical support
  - IsInUse check to prevent deleting types in use

### 2. Domain Models (camelCase DTOs)
**Location:** `server/DaemonsMCP.Domain/Models/`

- **ItemDto** - DTO for item responses with hierarchical children
- **ItemTypeDto** - DTO for item type responses
- **GetItemByIdQuery** - Query to get item by id with maxDepth
- **SearchItemsQuery** - Query to search items with filters
- **AddUpdateItemCommand** - Command to add or update items
- **DeleteItemCommand** - Command to delete with strategies
- **GetAllItemTypesQuery** - Query to get all item types
- **AddUpdateItemTypeCommand** - Command to add/update item types

### 3. Application Layer (MediatR Handlers)
**Location:** `server/DaemonsMCP.Application/Items/` & `/ItemTypes/`

**Items:**
- **GetItemByIdQueryHandler** - Retrieves item with children recursively
- **SearchItemsQueryHandler** - Searches items with filters
- **AddUpdateItemCommandHandler** - Creates or updates items

**ItemTypes:**
- **GetAllItemTypesQueryHandler** - Returns all item types
- **AddUpdateItemTypeCommandHandler** - Creates or updates item types

### 4. Tools Handler (MCP Interface)
**Location:** `server/DaemonsMCP.Infrastructure/Tools/ItemToolsHandler.cs`

**Methods:**
- `GetItemById(itemId, maxDepth)` - camelCase parameters
- `SearchItems(parentId, nameContains, detailsContains, typeId, statusId, maxDepth)`
- `AddUpdateItem(id, parentId, itemTypeId, statusTypeId, rank, name, details, ...)`
- `GetAllItemTypes()`
- `AddUpdateItemType(id, name, description, rank, parentId)`

### 5. Registration
- All repositories registered in `DependencyInjection.cs`
- ItemToolsHandler registered as singleton

---

## What Still Needs to Be Done

### 1. Delete Operations
- Create `DeleteItemCommandHandler` in `Application/Items/Commands/DeleteItem/`
- Implement the delete strategies from old system
- Add `DeleteItem()` method to ItemToolsHandler

### 2. Todo-Specific Operations (Optional - based on old system)
These were special convenience methods in the old system:
- `MakeTodoList(listName, items[])` - Creates todo list with items
- `GetNextTodoItem(listName, maxDepth)` - Gets next todo and marks in-progress
- `MarkTodoDone(itemId)` - Marks todo complete
- `RestoreAsTodo(itemId)` - Restores to "Not Started"
- `MarkTodoCancel(itemId)` - Cancels a todo

Could create these as:
- `Application/Items/Commands/TodoOperations/` folder with specific handlers
- Or just use the existing AddUpdateItem with specific status types

### 3. MCP Server Integration
The ItemToolsHandler methods need to be wired up to the MCP server endpoints.
This typically happens in `McpServerHostedService` or similar.

### 4. API Controllers (If needed for REST API)
Create controllers in `DaemonsMCP.Api/Controllers/` if you want REST endpoints:
- `ItemsController` with GET/POST/PUT/DELETE
- `ItemTypesController` with GET/POST/PUT

### 5. Seed Data for ItemTypes
Might want to create default item types and status types on first run:
- Create migration or seed method
- Default types like: "Task", "Note", "Bug", "Feature", etc.
- Default statuses: "Not Started", "In Progress", "Complete", "On Hold", "Cancelled"

### 6. Validation
Consider adding FluentValidation:
- `AddUpdateItemCommandValidator`
- Ensure required fields, max lengths, etc.

---

## Architecture Benefits

1. **Clean Separation**: Domain → Application → Infrastructure → Tools
2. **MediatR CQRS**: Clear separation of reads (Queries) and writes (Commands)
3. **Testability**: Each handler can be unit tested independently
4. **API Ready**: Same MediatR handlers can be called from both MCP tools and REST API
5. **camelCase**: All external-facing parameters use camelCase for consistency

---

## Quick Test Checklist

1. ✅ Repositories created and registered
2. ✅ Domain models (DTOs, Queries, Commands) created
3. ✅ Query handlers created for Items and ItemTypes
4. ✅ Command handler created for AddUpdateItem
5. ✅ ItemToolsHandler created and registered
6. ⏳ Delete command handler needed
7. ⏳ MCP server endpoint wiring needed
8. ⏳ Todo operations (optional)
9. ⏳ API controllers (optional)

---

## Next Steps

1. **Compile and test** - Make sure everything builds
2. **Add DeleteItem** - Create the delete command handler
3. **Wire up MCP endpoints** - Connect ItemToolsHandler methods to MCP server
4. **Test with Claude** - Try calling the methods through MCP
5. **Consider Todo methods** - Decide if you want the special todo helpers
