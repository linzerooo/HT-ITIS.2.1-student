using System.Linq.Expressions;

namespace Hw11.Services.MathCalculator;

public sealed class MathExpressionVisitor
{
    private readonly Dictionary<Expression, Tuple<Expression, Expression?>> _executeBefore = new();

    public Dictionary<Expression, Tuple<Expression, Expression?>> GetExecuteBefore(Expression expression)
    {
        Visit(expression);
        return _executeBefore;
    }

    private void Visit(Expression node)
    {
        Visit((dynamic)node);
    }

    private void Visit(BinaryExpression node)
    {
        _executeBefore.Add(node, new Tuple<Expression, Expression?>(node.Left, node.Right));

        Visit(node.Left);
        Visit(node.Right);
    }

    private void Visit(UnaryExpression node)
    {
        _executeBefore.Add(node, new Tuple<Expression, Expression?>(node.Operand, null));

        Visit(node.Operand);
    }

    // private void Visit(ConstantExpression node)    {    }
}