// using System.Linq.Expressions;
// using Hw9.Dto;
// using Hw9.Expressions;
// using Hw9.Services.Expressions;
//
// namespace Hw9.Services.MathCalculator;
//
// public class MathCalculatorService : IMathCalculatorService
// {
//     public async Task<CalculationMathExpressionResultDto> CalculateMathExpressionAsync(string? expression)
//     {
//         try
//         {
//             new Validator.Validator().Validate(expression);
//             var polishString = new Parser.Parser().Parse(expression!);
//             var expr = ExpressionTree.ParseToExpression(polishString);
//             var result = Expression.Lambda<Func<double>>(
//                 await ExpressionTreeVisitor.VisitExpression(expr)).Compile().Invoke();
//             return new CalculationMathExpressionResultDto(result);
//         }
//         catch (Exception ex)
//         {
//             return new CalculationMathExpressionResultDto(ex.Message);
//         }
//     }
// }

using System.Linq.Expressions;
using Hw9.Dto;
using Hw9.Services.Expressions;

namespace Hw9.Services.MathCalculator;

public class MathCalculatorService : IMathCalculatorService
{
    public async Task<CalculationMathExpressionResultDto> CalculateMathExpressionAsync(string? expression)
    {
        try
        {
            Parser.Validate(expression);
            var polishString = Parser.Parse(expression!);
            var expr = ExpressionTree.ConvertToExpression(polishString);
            var result = Expression.Lambda<Func<double>>(
                await ExpressionTreeVisitor.VisitExpression(expr)).Compile().Invoke();
            return new CalculationMathExpressionResultDto(result);
        }
        catch (Exception ex)
        {
            return new CalculationMathExpressionResultDto(ex.Message);
        }
    }
}