using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace WebReport.Models {
    public class LoginScreenModel {
        public string EmployeeId { get; set; }
        public IList<SelectListItem> Employees { get; set; }
    }
}
