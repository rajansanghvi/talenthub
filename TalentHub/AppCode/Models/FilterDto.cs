using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TalentHub.AppCode.Models {
  public class FilterDto {
    public string ColumnName { get; set; }
    public string Operator { get; set; }
    public string ColumnValue { get; set; }
  }
}