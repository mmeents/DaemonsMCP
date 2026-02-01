using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace DaemonsMCP.Application.Utilities.Commands {

  public class GetSystemInformationCommand : IRequest<SystemInformationResult> {
  } 
  public class GetSystemInformationCommandHandler : IRequestHandler<GetSystemInformationCommand, SystemInformationResult> {
    
    public Task<SystemInformationResult> Handle(GetSystemInformationCommand request, CancellationToken cancellationToken) {
      var results = new SystemInformationResult();
      results.OSDescription = System.Runtime.InteropServices.RuntimeInformation.OSDescription;
      results.OSArchitecture = System.Runtime.InteropServices.RuntimeInformation.OSArchitecture.ToString();
      results.ProcessorCount = Environment.ProcessorCount;
      results.TotalMemoryMB = GC.GetGCMemoryInfo().TotalAvailableMemoryBytes / (1024 * 1024);
      results.FrameworkDescription = System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription;
      results.Hostname = System.Net.Dns.GetHostName();
      results.HostIPAddresses = new List<string>();
      var myIpAddresses = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName()).AddressList;
      results.HostIPAddresses.AddRange(myIpAddresses.Select(ip => ip.ToString()));      
      return Task.FromResult(results);
    }

  }

  public class SystemInformationResult {
    public string OSDescription { get; set; }
    public string OSArchitecture { get; set; }
    public int ProcessorCount { get; set; }
    public long TotalMemoryMB { get; set; }
    public string FrameworkDescription { get; set; }
    public string Hostname { get; set; } = string.Empty;
    public List<string> HostIPAddresses { get; set; } = new List<string>();
  }
}
