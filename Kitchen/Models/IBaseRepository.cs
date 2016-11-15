using System;

namespace MvcStudy.Models
{
    public interface IBaseRepository : IDisposable
    {
        kitchenEntities DbCtx { get; } 
        void OpenContext();
        void CloseContext();
        DateTime GetStartDateOfWeek(DateTime filterDate);
    }
}