using Guoc.BigMall.Application.Search;
using Guoc.BigMall.Application.ViewObject;
using Guoc.BigMall.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guoc.BigMall.Application
{
    public interface IStocktakingPlannFacade
    {
        IEnumerable<StocktakingPlanDto> GetPageList(Pager page, SearchInventoryPlan condition);
        IEnumerable<StocktakingPlanProductDto> GetInventoryPlanProduct(int planId);
        int CreatePlan(int storeId, int userId, string userName);
        void InsertPlanItem(List<PlanProductDto> planProductItems);
        void Confirm(int storeId);
    }
}
