using AutoMapper;
using Guoc.BigMall.Infrastructure.AutoMapper;
using Guoc.BigMall.Infrastructure.Extension;
using Guoc.BigMall.Application.DTO;
using Guoc.BigMall.Application.ViewObject;
using Guoc.BigMall.Domain.Entity;
using System;

namespace Guoc.BigMall.Application.Facade.MapperConfig
{
    public class ObjectMapperConfig : IAutoMapperRegistrar
    {
        public void Register(IMapperConfigurationExpression cfg)
        {
            cfg.CreateMap<RoleDTO, Role>()
               .Ignore(m => m.Id)
               .ReverseMap();

            cfg.CreateMap<AccountEditModel, Account>()
               .Ignore(m => m.Id)
               .ReverseMap();

            cfg.CreateMap<TransferOrderDto, TransferOrder>()
               .Ignore(m => m.Id)
               .ReverseMap();

            cfg.CreateMap<TransferOrderItemDto, TransferOrderItem>()
               .Ignore(m => m.Id)
               .ReverseMap();

            cfg.CreateMap<TransferCreateModel, TransferOrder>()
               .Ignore(m => m.Id)
               .Ignore(m => m.Code)
               .ReverseMap();

            cfg.CreateMap<TransferItemCreateModel, TransferOrderItem>()
               .Ignore(m => m.Id)
               .ReverseMap();

            cfg.CreateMap<TransferEditModel, TransferOrder>()
               .Ignore(m => m.Id)
               .Ignore(m => m.Code)
               .ReverseMap();

            cfg.CreateMap<TransferItemEditModel, TransferOrderItem>()
               .Ignore(m => m.Id)
                //.Ignore(m => m.TransferOrderId)
               .ReverseMap();

            cfg.CreateMap<PurchaseOrderModel, PurchaseOrder>()
               .Ignore(m => m.Id)
               .Ignore(m => m.Code)
               .ReverseMap();

            cfg.CreateMap<SaleOrderModel, SaleOrder>()
               .Ignore(m => m.Id)
               .Ignore(m => m.Code)
               .Ignore(m => m.Items)
               .Ignore(m => m.CreatedOn)
               .Ignore(m => m.CreatedBy)
               .ReverseMap();

            cfg.CreateMap<SaleOrder, SaleOrder>()
               .ReverseMap();

            cfg.CreateMap<SaleOrder, SaleOrderDto>()
               .ReverseMap();

            cfg.CreateMap<SaleOrderItem, SaleOrderItemDto>()
               .ReverseMap();

            cfg.CreateMap<SaleOrderItem, SaleOrderItemModel>()
               .ReverseMap();

            cfg.CreateMap<SaleOrderItem, SaleOrderGiftItem>()
               .Mapping(x => x.ProductId, y => y.GiftProductId)
               .Mapping(x => x.ProductName, y => y.GiftProductName)
               .Mapping(x => x.Quantity, y => y.GiftQuantity)
               .ReverseMap();

            cfg.CreateMap<SaleOrderListDto, SaleOrder>()
               .Mapping(x => x.MasterId, y => y.Id)
               .ReverseMap();

            cfg.CreateMap<SaleOrderListDto, SaleOrderItem>()
               .Mapping(x => x.MasterId, y => y.SaleOrderId)
               .ReverseMap();

            cfg.CreateMap<SaleOrderModel, SaleOrderDto>()
               .Ignore(m => m.Id)
               .Ignore(m => m.Code)
               .Ignore(m => m.Items)
               .ReverseMap();

            cfg.CreateMap<SaleOrderItemDto, SaleOrderItemModel>()
               .ReverseMap();

            cfg.CreateMap<SaleOrderItemDto, SaleOrderGiftItem>()
               .Mapping(x => x.ProductId, y => y.GiftProductId)
               .Mapping(x => x.ProductName, y => y.GiftProductName)
               .Mapping(x => x.Quantity, y => y.GiftQuantity)
               .ReverseMap();

            cfg.CreateMap<SaleOrderItem, SaleOrderItem>()
               .ReverseMap();

            cfg.CreateMap<StoreDto, Store>()
               .Ignore(x => x.Id)
               .ReverseMap();
            cfg.CreateMap<StoreModel, Store>()
            //  .Ignore(x => x.Id)
              .ReverseMap();

            cfg.CreateMap<CreateCategoryVoucherModel, CategoryRechargeVoucher>()
               .Ignore(x => x.Id)
               .Mapping(x => DateTime.Parse(x.DateRange.Split(',')[0]), y => y.StartDate)
               .Mapping(x => DateTime.Parse(x.DateRange.Split(',')[1] + " 23:59:59"), y => y.EndDate)
               .ReverseMap();

            cfg.CreateMap<CreateBrandVoucherModel, BrandRechargeVoucher>()
               .Ignore(x => x.Id)
               .Mapping(x => DateTime.Parse(x.DateRange.Split(',')[0]), y => y.StartDate)
               .Mapping(x => DateTime.Parse(x.DateRange.Split(',')[1] + " 23:59:59"), y => y.EndDate)
               .ReverseMap();
        }
    }
}