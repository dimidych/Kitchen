using System;

namespace MvcStudy.Models
{
    public partial class KitchenRepository : IBaseRepository
    {
        public kitchenEntities DbCtx { get; private set; }

        public KitchenRepository()
        {
            OpenContext();
        }

        public void OpenContext()
        {
            DbCtx = new kitchenEntities();
        }

        public void CloseContext()
        {
            DbCtx.Dispose();
            DbCtx = null;
            GC.Collect();
        }

        public void Dispose()
        {
            CloseContext();
        }

        public DateTime GetStartDateOfWeek(DateTime filterDate)
        {
            var dayOfWeek = (int) (filterDate.DayOfWeek);
            return filterDate.AddDays(1 - dayOfWeek).Date;
        }
    }
}