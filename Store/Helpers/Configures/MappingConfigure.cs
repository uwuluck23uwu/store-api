using AutoMapper;
using ClassLibrary.Models.Data;
using ClassLibrary.Models.Dto;

namespace Store.Helpers.Configures;

public class MappingConfigure : Profile
{
    public MappingConfigure()
    {
        // User mappings
        CreateMap<User, UserDTO>()
            .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
            .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
            .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.PhoneNumber))
            .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.ImageUrl));
        CreateMap<RegisterRequestDTO, User>();
        CreateMap<UpdateProfileDTO, User>();

        // Category mappings
        CreateMap<Category, CategoryDTO>();
        CreateMap<CategoryCreateDTO, Category>();
        CreateMap<CategoryUpdateDTO, Category>();

        // Seller mappings
        CreateMap<Seller, SellerDTO>()
            .ForMember(dest => dest.OwnerName, opt => opt.MapFrom(src => $"{src.User.FirstName} {src.User.LastName}"));
        CreateMap<Seller, SellerDetailDTO>()
            .ForMember(dest => dest.OwnerName, opt => opt.MapFrom(src => $"{src.User.FirstName} {src.User.LastName}"))
            .ForMember(dest => dest.Products, opt => opt.MapFrom(src => src.Products));
        CreateMap<Seller, SellerProfileDTO>()
            .ForMember(dest => dest.Products, opt => opt.MapFrom(src => src.Products));
        CreateMap<SellerCreateDTO, Seller>();
        CreateMap<SellerUpdateDTO, Seller>();

        // Product mappings
        CreateMap<Product, ProductDTO>()
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.CategoryName))
            .ForMember(dest => dest.SellerName, opt => opt.MapFrom(src => src.Seller.ShopName));
        CreateMap<Product, ProductListDTO>()
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category != null ? src.Category.CategoryName : ""))
            .ForMember(dest => dest.ShopName, opt => opt.MapFrom(src => src.Seller != null ? src.Seller.ShopName : ""));
        CreateMap<Product, ProductDetailDTO>()
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.CategoryName))
            .ForMember(dest => dest.Seller, opt => opt.MapFrom(src => src.Seller))
            .ForMember(dest => dest.Reviews, opt => opt.MapFrom(src => src.Reviews));
        CreateMap<ProductCreateDTO, Product>();
        CreateMap<ProductUpdateDTO, Product>();

        // Address mappings
        CreateMap<Address, AddressDTO>();
        CreateMap<AddressCreateDTO, Address>();
        CreateMap<AddressUpdateDTO, Address>();

        // Cart mappings
        CreateMap<Cart, CartItemDTO>()
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.ProductName))
            .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Product.Price))
            .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.Product.ImageUrl))
            .ForMember(dest => dest.ProductImageUrl, opt => opt.MapFrom(src => src.Product.ImageUrl))
            .ForMember(dest => dest.Stock, opt => opt.MapFrom(src => src.Product.Stock))
            .ForMember(dest => dest.Unit, opt => opt.MapFrom(src => src.Product.Unit))
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.Product.IsActive))
            .ForMember(dest => dest.SellerId, opt => opt.MapFrom(src => src.Product.SellerId))
            .ForMember(dest => dest.SellerName, opt => opt.MapFrom(src => src.Product.Seller != null ? src.Product.Seller.ShopName : null))
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Product.Category != null ? src.Product.Category.CategoryName : null))
            .ForMember(dest => dest.Subtotal, opt => opt.MapFrom(src => src.Product.Price * src.Quantity))
            .ForMember(dest => dest.AddedAt, opt => opt.MapFrom(src => src.CreatedAt));
        CreateMap<AddToCartDTO, Cart>();
        CreateMap<UpdateCartItemDTO, Cart>();

        // Order mappings
        CreateMap<Order, OrderDTO>()
            .ForMember(dest => dest.ItemCount, opt => opt.MapFrom(src => src.OrderItems.Sum(oi => oi.Quantity)));
        CreateMap<Order, OrderDetailDTO>()
            .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
            .ForMember(dest => dest.OrderItems, opt => opt.MapFrom(src => src.OrderItems))
            .ForMember(dest => dest.Payment, opt => opt.MapFrom(src => src.Payment));
        CreateMap<OrderCreateDTO, Order>();

        // OrderItem mappings
        CreateMap<OrderItem, OrderItemDTO>()
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.ProductName))
            .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.Product.ImageUrl))
            .ForMember(dest => dest.Subtotal, opt => opt.MapFrom(src => src.Price * src.Quantity));
        CreateMap<OrderItem, OrderItemDetailDTO>()
            .ForMember(dest => dest.Product, opt => opt.MapFrom(src => src.Product))
            .ForMember(dest => dest.Seller, opt => opt.MapFrom(src => src.Seller))
            .ForMember(dest => dest.Order, opt => opt.MapFrom(src => src.Order))
            .ForMember(dest => dest.Subtotal, opt => opt.MapFrom(src => src.Price * src.Quantity));
        CreateMap<CreateOrderItemDTO, OrderItem>();

        // Review mappings
        CreateMap<Review, ReviewDTO>()
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => $"{src.User.FirstName} {src.User.LastName}"))
            .ForMember(dest => dest.UserImageUrl, opt => opt.MapFrom(src => src.User.ImageUrl))
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.ProductName));
        CreateMap<ReviewCreateDTO, Review>();
        CreateMap<ReviewUpdateDTO, Review>();

        // Payment mappings
        CreateMap<Payment, PaymentDTO>();
        CreateMap<PaymentCreateDTO, Payment>();

        // Image mappings
        CreateMap<Image, ImageDTO>();

        // Location mappings
        CreateMap<Location, LocationDTO>()
            .ForMember(dest => dest.SellerName, opt => opt.MapFrom(src =>
                src.Seller != null ? src.Seller.ShopName : null));
        CreateMap<Location, LocationDetailDTO>()
            .ForMember(dest => dest.SellerName, opt => opt.MapFrom(src =>
                src.Seller != null ? src.Seller.ShopName : null))
            .ForMember(dest => dest.SellerInfo, opt => opt.MapFrom(src => src.Seller))
            .ForMember(dest => dest.Products, opt => opt.MapFrom(src => src.Products));
        CreateMap<LocationCreateDTO, Location>();
        CreateMap<LocationUpdateDTO, Location>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
    }
}
