
namespace Guoc.BigMall.Domain.ValueObject
{
    public enum InvokeSapServiceType
    {
        OrderCreated = 1,
        OrderUpdated = 2,
        OrderAudited = 3,
        OrderAbandoned = 4,
        OrderRejected = 5,
        OrderOutStock = 6,
        ReturnOrderCreated = 7,
        ReturnOrderInStock = 8,
        ConvertToSaleOrder = 9
    }
}