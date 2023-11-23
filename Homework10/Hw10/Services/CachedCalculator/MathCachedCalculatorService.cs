using Hw10.DbModels;
using Hw10.Dto;
using Hw10.Services.MathCalculator;
using Microsoft.EntityFrameworkCore;

namespace Hw10.Services.CachedCalculator;

public class MathCachedCalculatorService : IMathCalculatorService
{
    private readonly ApplicationContext _dbContext;
    private readonly IMathCalculatorService _simpleCalculator;
    public MathCachedCalculatorService(ApplicationContext dbContext, IMathCalculatorService simpleCalculator)
    {
        _dbContext = dbContext;
        _simpleCalculator = simpleCalculator;
    }

    public async Task<CalculationMathExpressionResultDto> CalculateMathExpressionAsync(string? expression)
    {
        var dbSet = _dbContext.Set<SolvingExpression>();
        if (await dbSet.AnyAsync(dto => dto.Expression.Equals(expression)))
        {
            var cache = await dbSet
                .FirstAsync(dto => dto.Expression.Equals(expression));
            await Task.Delay(1000);
            return new CalculationMathExpressionResultDto(cache.Result);

        }

        var dto = await _simpleCalculator.CalculateMathExpressionAsync(expression);
        if (dto.IsSuccess)
        {
            await dbSet.AddAsync(new SolvingExpression { Expression = expression!, Result = dto.Result });
            await _dbContext.SaveChangesAsync();
        }

        return dto;
    }
}