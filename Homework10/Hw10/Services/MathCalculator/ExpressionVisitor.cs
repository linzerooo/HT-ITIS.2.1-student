using System.Linq.Expressions;

namespace Hw10.Services.MathCalculator;

public class MathExpressionVisitor : ExpressionVisitor
{
    private readonly Dictionary<Expression, (Expression, Expression?)> _executeBefore = new();

    public Dictionary<Expression, (Expression, Expression?)> GetExecuteBefore(Expression expression)
    {
        Visit(expression);
        return _executeBefore;
    }

    protected override Expression VisitBinary(BinaryExpression node)
    {
        _executeBefore.Add(node, (node.Left, node.Right));
        return base.VisitBinary(node);
    }

    protected override Expression VisitUnary(UnaryExpression node)
    {
        _executeBefore.Add(node, (node.Operand, null));
        return base.VisitUnary(node);
    }
}