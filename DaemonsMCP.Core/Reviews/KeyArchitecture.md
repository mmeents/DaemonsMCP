
CORE ARCHITECTURE:
- Built on PackedTables.NET for file-based database storage
- Microsoft.CodeAnalysis for C# parsing and indexing
- Dependency injection with singleton IndexService
- MCPSharp for MCP protocol implementation
- 27 tools total (26 working + remove node to add)

DATA FLOW:
1. IndexService scans .cs files using Microsoft.CodeAnalysis
2. ProcessFileAsync extracts classes/methods with line boundaries
3. FindClassStartCutLine method backs up to find XML docs
4. Data stored in .daemons/*.pktbs files
5. ProjectIndexModel manages queries and updates

KEY FILES TO UNDERSTAND:
- IndexService.cs: Core indexing logic
- ProjectIndexModel.cs: Database operations and queries  
- ProjectItemRepo.cs: Nodes tree management
- DaemonsTools.cs: 26 MCP tool implementations

FILTER FIXES:
- Changed IsNullOrWhiteSpace to IsNullOrEmpty throughout
- Fixed namespace filtering completely
- Method and Class filtering now works