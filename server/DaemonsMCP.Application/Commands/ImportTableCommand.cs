using DaemonsMCP.Domain.Enums;
using DaemonsMCP.Domain.Entities;
using DaemonsMCP.Domain.Models;
using DaemonsMCP.Domain.Parsers;
using DaemonsMCP.Domain.Repositories;
using MediatR;

namespace DaemonsMCP.Application.Commands {

  public record ImportTableRequest(int ParentId, string SqlStatement);
  public record ImportTableCommand(int ParentId, string SqlStatement) : IRequest<ModelDto[]?>;

  public class ImportTableCommandHandler : IRequestHandler<ImportTableCommand, ModelDto[]?> {
    private readonly IModelRepository _modelRepository;
    private readonly IModelPropertyRepository _modelPropertyRepository;

    public ImportTableCommandHandler(IModelRepository modelRepository, IModelPropertyRepository modelPropertyRepository) {
      _modelRepository = modelRepository;
      _modelPropertyRepository = modelPropertyRepository;
    }

    public async Task<ModelDto[]?> Handle(ImportTableCommand request, CancellationToken cancellationToken) {
      // Parse SQL
      var parsedTables = SqlParser.ParseCreateTable(request.SqlStatement);
      if (parsedTables == null) return null;
      var resultModels = new List<ModelDto>();
      foreach (var parsedTable in parsedTables.Tables) { 
        var parent = await _modelRepository.GetByIdWithChildrenAsync(request.ParentId, 2, cancellationToken);
        if (parent == null) return null;
        int nextRank = parent.Children.Count() + 1;

        // Create Table model
        var tableModel = new Model(parent.ProjectId, parsedTable.Name, (int)Mte.TableModel, nextRank, request.ParentId, request.SqlStatement );
        var savedTable = await _modelRepository.AddAsync(tableModel, cancellationToken);      
        if (savedTable == null) return null;

        // Add default properties + schema      
        var schemaProp = savedTable.Properties.FirstOrDefault(p => p.PropertyKey == "Schema");
        if (schemaProp != null) {
          schemaProp.PropertyValue = parsedTable.Schema;
          await _modelPropertyRepository.UpdateAsync(schemaProp, cancellationToken);
        }     

        // Create columns
        int colRank = 1;
        foreach (var col in parsedTable.Columns) {

          var columnModel = new Model ( tableModel.ProjectId, col.Name, (int)Mte.TableColumnModel, colRank++, savedTable.Id, null);
          var savedColumn = await _modelRepository.AddAsync(columnModel, cancellationToken);        
          if (savedColumn == null) continue;

          var columnTypeProp = savedColumn.Properties.FirstOrDefault(p => p.PropertyKey == "ColumnType");
          if (columnTypeProp != null) {
            columnTypeProp.PropertyValue = col.ColumnTypeId.ToString();
            await _modelPropertyRepository.UpdateAsync(columnTypeProp, cancellationToken);
          }

          var isPkProp = savedColumn.Properties.FirstOrDefault(p => p.PropertyKey == "IsPrimaryKey");
          if (isPkProp != null) { 
            isPkProp.PropertyValue = col.IsPrimaryKey ? "1" : "0"; 
            await _modelPropertyRepository.UpdateAsync(isPkProp, cancellationToken);
          }

          var isNullableProp = savedColumn.Properties.FirstOrDefault(p => p.PropertyKey == "IsNullable");
          if (isNullableProp != null) {
            isNullableProp.PropertyValue = col.IsNullable ? "1" : "0";
            await _modelPropertyRepository.UpdateAsync(isNullableProp, cancellationToken);
          }

          var maxLenProp = savedColumn.Properties.FirstOrDefault(p => p.PropertyKey == "MaxLength");
          if (maxLenProp != null && !String.IsNullOrEmpty(col.MaxLength)) {
            maxLenProp.PropertyValue = col.MaxLength;
            await _modelPropertyRepository.UpdateAsync(maxLenProp, cancellationToken);
          }        
        }
        var resultModel = await _modelRepository.GetByIdWithChildrenAsync(savedTable.Id, 2, cancellationToken);
        if (resultModel != null) {
          resultModels.Add(resultModel.ToDto());
        }
      } // foreach table

      return resultModels.Select(m => m.ToDto()).ToArray();
    }
  }
}
