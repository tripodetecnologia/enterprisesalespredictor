namespace EnterpriseSalesPredictor.Infrastructure.FileProcessing;

public static class UploadHeaders
{
    public const string InvoiceNumber = "Numero de factura";
    public const string CustomerName = "Nombre Cliente";
    public const string CustomerIdentification = "Identificación Cliente";
    public const string CustomerAddress = "Dirección Cliente";
    public const string CustomerCity = "Ciudad Cliente";
    public const string CustomerPhone = "Telefono Cliente";
    public const string CustomerZone = "Zona Cliente";
    public const string ProductType = "TipoProducto";
    public const string Product = "Producto";
    public const string ProductReference = "Referencia Producto";
    public const string ProductBrand = "Marca Producto";
    public const string ProductPurchasePrice = "Precio Compra Producto";
    public const string ProductSalePrice = "Precio Venta Producto";
    public const string ProductAvailableUnits = "Unidades Disponibles Producto";
    public const string QuantitySold = "Cantidad Vendida";
    public const string SaleAmount = "Valor Venta";
    public const string SaleDate = "Fecha Venta";
    public const string SellerName = "Nombre Vendedor";
    public const string SellerIdentification = "Identificación Vendedor";
    public const string SupplierName = "Nombre Proveedor";
    public const string SupplierIdentification = "Identificación Proveedor";
    public const string SupplierAddress = "Dirección Proveedor";
    public const string SupplierPhone = "Telefono Proveedor";
    public const string SupplierCity = "Ciudad Proveedor";
    public const string InvoicePaymentMethod = "Medio de pago factura";

    public static readonly string[] Required =
    {
        InvoiceNumber,
        CustomerName,
        CustomerIdentification,
        CustomerAddress,
        CustomerCity,
        CustomerPhone,
        CustomerZone,
        ProductType,
        Product,
        ProductReference,
        ProductBrand,
        ProductPurchasePrice,
        ProductSalePrice,
        ProductAvailableUnits,
        QuantitySold,
        SaleAmount,
        SaleDate,
        SellerName,
        SellerIdentification,
        SupplierName,
        SupplierIdentification,
        SupplierAddress,
        SupplierPhone,
        SupplierCity,
        InvoicePaymentMethod
    };
}
