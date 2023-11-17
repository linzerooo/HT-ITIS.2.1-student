using System.Linq.Expressions;
using Hw9.ErrorMessages;
using static Hw9.ErrorMessages.MathErrorMessager;

namespace Hw9.Services.Expressions;

public class ExpressionTreeVisitor : ExpressionVisitor
{
    protected override Expression VisitBinary(BinaryExpression root)
    {
        var result = CompileTreeAsync(root.Left, root.Right).Result;

        return root.NodeType switch
        {
            ExpressionType.Add => Expression.Add(Expression.Constant(result[0]), Expression.Constant(result[1])),
            ExpressionType.Subtract => Expression.Subtract(Expression.Constant(result[0]),
                Expression.Constant(result[1])),
            ExpressionType.Multiply => Expression.Multiply(Expression.Constant(result[0]),
                Expression.Constant(result[1])),
            ExpressionType.Divide => result[1] == 0
                ? throw new Exception(MathErrorMessager.DivisionByZero)
                : Expression.Divide(Expression.Constant(result[0]), Expression.Constant(result[1])),
            _ =>  throw new Exception()
        };
    }

    public static Task<Expression> VisitExpression(Expression expr) =>
        Task.Run(() => new ExpressionTreeVisitor().Visit(expr));

    private static async Task<double[]> CompileTreeAsync(Expression left, Expression right)
    {
        await Task.Delay(1000);
        var task1 = Task.Run(() => Expression.Lambda<Func<double>>(left).Compile().Invoke());
        var task2 = Task.Run(() => Expression.Lambda<Func<double>>(right).Compile().Invoke());
        return await Task.WhenAll(task1, task2);
    }
}