using System.Globalization;
using System.Linq.Expressions;
using Hw11.Services.TokenParser;

namespace Hw11.Services.ExpressionParser;

public class MyExpressionParser : IExpressionParser
{
    public Expression ParseExpressionFromTokens(IEnumerable<Token> tokens)
    {
        var expStack = new Stack<Expression>();
        var stack = new Stack<Token>();

        foreach (var token in tokens)
        {
            if (token.Type == TokenType.Number)
            {
                expStack.Push(Expression.Constant(
                    double.Parse(token.Value, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture)));
                continue;
            }

            if (token.IsOperation)
            {
                while (stack.TryPeek(out var last) && last.Priority >= token.Priority)
                {
                    if (last.Type == TokenType.Negate)
                    {
                        PushOperation(stack.Pop(), expStack);
                    }
                    else if (expStack.Count > 1)
                    {
                        PushOperation(stack.Pop(), expStack);
                    }
                }
                stack.Push(token);
                continue;
            }

            if (token.Type == TokenType.OpenBracket)
            {
                stack.Push(token);
                continue;
            }

            if (token.Type == TokenType.CloseBracket)
            {
                while (stack.TryPeek(out var last))
                {
                    if (last.Type != TokenType.OpenBracket)
                    {
                        PushOperation(stack.Pop(), expStack);
                    }
                    else
                    {
                        break;
                    }
                }

                stack.Pop();
            }
        }

        while (stack.TryPeek(out var last))
        {
            PushOperation(stack.Pop(), expStack);
        }

        return expStack.Pop();
    }

    private void PushOperation(Token token, Stack<Expression> stack)
    {
        switch (token.Type)
        {
            case TokenType.Plus:
                stack.Push(Expression.Add(stack.Pop(), stack.Pop()));
                break;
            case TokenType.Minus:
                var second = stack.Pop();
                var first = stack.Pop();
                stack.Push(Expression.Subtract(first, second));
                break;
            case TokenType.Multiply:
                stack.Push(Expression.Multiply(stack.Pop(), stack.Pop()));
                break;
            case TokenType.Divide:
                second = stack.Pop();
                first = stack.Pop();
                stack.Push(Expression.Divide(first, second));
                break;
            case TokenType.Negate:
                stack.Push(Expression.Negate(stack.Pop()));
                break;
        }
    }
}
