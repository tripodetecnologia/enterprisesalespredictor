using EnterpriseSalesPredictor.Application.DTOs.Sales;

namespace EnterpriseSalesPredictor.Application.Interfaces.Sales;

public interface ISalesWriteService
{
    Task<SaleDto> CreateSaleAsync(CreateSaleCommand command, CancellationToken cancellationToken = default);
}
