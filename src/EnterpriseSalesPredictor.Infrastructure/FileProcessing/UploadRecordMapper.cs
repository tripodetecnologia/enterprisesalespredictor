using EnterpriseSalesPredictor.Application.Interfaces.Uploads;

namespace EnterpriseSalesPredictor.Infrastructure.FileProcessing;

public static class UploadRecordMapper
{
    public static UploadRecordData? Map(Dictionary<string, string> row, int rowNumber, List<UploadParseError> errors)
    {
        var record = new UploadRecordData
        {
            InvoiceNumber = ReadText(row, UploadHeaders.InvoiceNumber, rowNumber, errors),
            CustomerName = ReadText(row, UploadHeaders.CustomerName, rowNumber, errors),
            CustomerIdentification = ReadText(row, UploadHeaders.CustomerIdentification, rowNumber, errors),
            CustomerAddress = ReadText(row, UploadHeaders.CustomerAddress, rowNumber, errors),
            CustomerCity = ReadText(row, UploadHeaders.CustomerCity, rowNumber, errors),
            CustomerPhone = ReadText(row, UploadHeaders.CustomerPhone, rowNumber, errors),
            CustomerZone = ReadText(row, UploadHeaders.CustomerZone, rowNumber, errors),
            ProductType = ReadText(row, UploadHeaders.ProductType, rowNumber, errors),
            ProductName = ReadText(row, UploadHeaders.Product, rowNumber, errors),
            ProductReference = ReadText(row, UploadHeaders.ProductReference, rowNumber, errors),
            ProductBrand = ReadText(row, UploadHeaders.ProductBrand, rowNumber, errors),
            ProductPurchasePrice = ReadDecimal(row, UploadHeaders.ProductPurchasePrice, rowNumber, errors),
            ProductSalePrice = ReadDecimal(row, UploadHeaders.ProductSalePrice, rowNumber, errors),
            ProductAvailableUnits = ReadInt(row, UploadHeaders.ProductAvailableUnits, rowNumber, errors),
            QuantitySold = ReadDecimal(row, UploadHeaders.QuantitySold, rowNumber, errors),
            SaleAmount = ReadDecimal(row, UploadHeaders.SaleAmount, rowNumber, errors),
            SaleDate = ReadDate(row, UploadHeaders.SaleDate, rowNumber, errors),
            SellerName = ReadText(row, UploadHeaders.SellerName, rowNumber, errors),
            SellerIdentification = ReadText(row, UploadHeaders.SellerIdentification, rowNumber, errors),
            SupplierName = ReadText(row, UploadHeaders.SupplierName, rowNumber, errors),
            SupplierIdentification = ReadText(row, UploadHeaders.SupplierIdentification, rowNumber, errors),
            SupplierAddress = ReadText(row, UploadHeaders.SupplierAddress, rowNumber, errors),
            SupplierPhone = ReadText(row, UploadHeaders.SupplierPhone, rowNumber, errors),
            SupplierCity = ReadText(row, UploadHeaders.SupplierCity, rowNumber, errors),
            InvoicePaymentMethod = ReadText(row, UploadHeaders.InvoicePaymentMethod, rowNumber, errors)
        };

        var hasErrorsForRow = errors.Any(error => error.RowNumber == rowNumber);
        return hasErrorsForRow ? null : record;
    }

    private static string ReadText(Dictionary<string, string> row, string key, int rowNumber, List<UploadParseError> errors)
    {
        if (!row.TryGetValue(key, out var value) || string.IsNullOrWhiteSpace(value))
        {
            errors.Add(new UploadParseError
            {
                RowNumber = rowNumber,
                FieldName = key,
                ErrorMessage = "Value is required."
            });
            return string.Empty;
        }

        return value.Trim();
    }

    private static decimal ReadDecimal(Dictionary<string, string> row, string key, int rowNumber, List<UploadParseError> errors)
    {
        var value = ReadText(row, key, rowNumber, errors);
        if (string.IsNullOrEmpty(value))
        {
            return 0;
        }

        if (!decimal.TryParse(value.Replace(',', '.'), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var parsed))
        {
            errors.Add(new UploadParseError
            {
                RowNumber = rowNumber,
                FieldName = key,
                ErrorMessage = "Decimal value is invalid."
            });
            return 0;
        }

        return parsed;
    }

    private static int ReadInt(Dictionary<string, string> row, string key, int rowNumber, List<UploadParseError> errors)
    {
        var value = ReadText(row, key, rowNumber, errors);
        if (string.IsNullOrEmpty(value))
        {
            return 0;
        }

        if (!int.TryParse(value, out var parsed))
        {
            errors.Add(new UploadParseError
            {
                RowNumber = rowNumber,
                FieldName = key,
                ErrorMessage = "Integer value is invalid."
            });
            return 0;
        }

        return parsed;
    }

    private static DateTime ReadDate(Dictionary<string, string> row, string key, int rowNumber, List<UploadParseError> errors)
    {
        var value = ReadText(row, key, rowNumber, errors);
        if (string.IsNullOrEmpty(value))
        {
            return DateTime.MinValue;
        }

        if (!DateTime.TryParse(value, out var parsed))
        {
            errors.Add(new UploadParseError
            {
                RowNumber = rowNumber,
                FieldName = key,
                ErrorMessage = "Date value is invalid."
            });
            return DateTime.MinValue;
        }

        return parsed;
    }
}
