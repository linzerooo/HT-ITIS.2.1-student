using System.Linq.Expressions;
using Hw11.Services.TokenParser;

namespace Hw11.Services.ExpressionParser;

public interface IExpressionParser
{
    public Expression ParseExpressionFromTokens(IEnumerable<Token> tokens);
}