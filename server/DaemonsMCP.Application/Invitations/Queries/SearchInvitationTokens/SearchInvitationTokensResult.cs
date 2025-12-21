using DaemonsMCP.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaemonsMCP.Application.Invitations.Queries.SearchInvitationTokens {
  public class SearchInvitationTokensResult {   
    public List<InvitationTokenDto> Data { get; set; } = new();
    public int TotalCount { get; set; }
    public int PageNo { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    
  }
}
