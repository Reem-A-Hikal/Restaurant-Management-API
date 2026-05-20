using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rest.Domain.Entities.Enums
{
    public enum CategoryStatus
    {
        Active,
        Inactive,
        Archived // Soft delete
    }
}
