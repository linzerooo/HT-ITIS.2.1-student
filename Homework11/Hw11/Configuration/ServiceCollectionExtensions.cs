using Hw11.Services.ExpressionParser;
using Hw11.Services.MathCalculator;
using Hw11.Services.TokenParser;
using Hw11.Services.Validator;

namespace Hw11.Configuration;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMathCalculator(this IServiceCollection services)
    {
        return services.AddTransient<IMathCalculatorService, MathCalculatorService>()
            .AddTransient<IValidator, MyValidator>()
            .AddTransient<ITokenParser, MyTokenParser>()
            .AddTransient<IExpressionParser, MyExpressionParser>();;
    }
}