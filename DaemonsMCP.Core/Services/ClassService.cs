using DaemonsMCP.Core.Extensions;
using DaemonsMCP.Core.Models;
using DaemonsMCP.Core.Repositories;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace DaemonsMCP.Core.Services {
  public class ClassService : IClassService {
    private readonly IIndexRepository _indexRepository;
    private readonly ILogger<ClassService> _logger;
    private readonly IValidationService _validationService;
    public ClassService(ILoggerFactory loggerFactory, IIndexRepository indexRepository, IValidationService validationService) {
        _indexRepository = indexRepository;
        _validationService = validationService;
        _logger = loggerFactory.CreateLogger<ClassService>() ?? throw new ArgumentNullException(nameof(loggerFactory));
    }

    public async Task<OperationResult> GetClassesAsync(string projectName, int pageNo, int itemsPerPage, string? namespaceFilter = null, string? classNameFilter = null) {
      try {
        var classes = await _indexRepository.GetClassListingsAsync(projectName, pageNo, itemsPerPage, namespaceFilter, classNameFilter).ConfigureAwait(false);
        var opResult = OperationResult.CreateSuccess(Cx.ListClassesCmd, $"{Cx.ListClassesCmd} Success.", classes);
        return opResult;
      } catch (Exception ex) {
        _logger.LogError(ex, "Error retrieving classes");
        var opResult = OperationResult.CreateFailure(Cx.ListClassesCmd, $"Error: {ex.Message}",ex);
        return opResult;
      }
    }

    public async Task<OperationResult> GetClassContentAsync(string projectName, int classID) {
      try {

        var classes = await _indexRepository.GetClassContentAsync(projectName, classID).ConfigureAwait(false);

        var opResult = OperationResult.CreateSuccess(Cx.ListClassesCmd, $"{Cx.ListClassesCmd} Success.", classes);
        return opResult;


      } catch (Exception ex) {
        _logger.LogError(ex, "Error retrieving classes");
        var opResult = OperationResult.CreateFailure(Cx.ListClassesCmd, $"Error: {ex.Message}", ex);
        return opResult;
      }
    }

    public async Task<OperationResult> AddUpdateClassContentAsync(string projectName, ClassContent classContent) {
      try {

        _validationService.ValidateClassContent(classContent);        
        var result = await _indexRepository.AddUpdateClassContentAsync(projectName, classContent).ConfigureAwait(false);
        if (result != null) {
          return result;
        } else {
          return OperationResult.CreateFailure(Cx.AddUpdateClassCmd, $"{Cx.AddUpdateClassCmd} failed to add/update class.");
        }
      } catch (Exception ex) {
        _logger.LogError(ex, "Error adding/updating class");
        return OperationResult.CreateFailure(Cx.AddUpdateClassCmd, $"Error: {ex.Message}", ex);
      }
    }


    public async Task<OperationResult> GetMethodsAsync(string projectName, int pageNo, int itemsPerPage, string? namespaceFilter = null, string? classNameFilter = null, string? methodNameFilter = null) {
      try {
        var methods = await _indexRepository.GetMethodListingsAsync(projectName, pageNo, itemsPerPage, namespaceFilter, classNameFilter, methodNameFilter).ConfigureAwait(false);
        var opResult = OperationResult.CreateSuccess(Cx.ListMethodsCmd, $"{Cx.ListMethodsCmd} Success.", methods);
        return opResult;
      } catch (Exception ex) {
        _logger.LogError(ex, "Error retrieving methods");
        var opResult = OperationResult.CreateFailure(Cx.ListMethodsCmd, $"Error: {ex.Message}", ex);
        return opResult;
      }
    }

    public async Task<OperationResult> GetMethodContentAsync(string projectName, int methodID) {
      try {
        var method = await _indexRepository.GetMethodContentAsync(projectName, methodID).ConfigureAwait(false);
        var opResult = OperationResult.CreateSuccess(Cx.GetClassMethodCmd, $"{Cx.GetClassMethodCmd} Success.", method);
        return opResult;
        } catch (Exception ex) {
        _logger.LogError(ex, "Error retrieving method");
        var opResult = OperationResult.CreateFailure(Cx.GetClassMethodCmd, $"Error: {ex.Message}", ex);
        return opResult;
      }
    }

    public async Task<OperationResult> AddUpdateMethodAsync(string projectName, MethodContent methodContent) {
      try {
        //_validationService.ValidateMethodContent(methodContent);
        var result = await _indexRepository.AddUpdateMethodAsync(projectName, methodContent).ConfigureAwait(false);
        if (result != null) {
          return OperationResult.CreateSuccess(Cx.AddUpdateMethodCmd, $"{Cx.AddUpdateMethodCmd} Success.", result);
        } else {
          return OperationResult.CreateFailure(Cx.AddUpdateMethodCmd, $"{Cx.AddUpdateMethodCmd} failed to add/update method.");
        }
      } catch (Exception ex) {
        _logger.LogError(ex, "Error adding/updating method");
        return OperationResult.CreateFailure(Cx.AddUpdateMethodCmd, $"Error: {ex.Message}", ex);
      }
    }


  }

}
