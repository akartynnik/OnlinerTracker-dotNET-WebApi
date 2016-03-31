using OnlinerTracker.Data;
using System;
using System.Collections.Generic;

namespace OnlinerTracker.Interfaces
{
    public interface IExternalProductService
    {
        List<ExternalProduct> Get(string searchQuery, Guid userId);
    }
}
