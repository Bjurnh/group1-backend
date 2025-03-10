using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Group1.BackEnd.Application.DTOs
{
    public class ChangeRoleDto
    {
        public string UserEmail { get; set; }
        public string NewRole { get; set; }
    }
}
