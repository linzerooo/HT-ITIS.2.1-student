using System.Diagnostics.CodeAnalysis;

namespace Hw11.Services.TokenParser;

public class MyTokenParser : ITokenParser
{
    private readonly char[] _operations = { '+', '-', '*', '/' };
    private readonly char[] _brackets = { '(', ')' };
    private readonly List<Token> _tokens = new();


    public List<Token> Parse(string input)
    {
        var i = 0;
        while (i < input.Length)
        {
            if (_brackets.Contains(input[i]))
            {
                _tokens.Add(ParseBracket(input, i));
            }
            else if (_operations.Contains(input[i]))
            {
                _tokens.Add(ParseOperation(input, i));
            }
            else if (char.IsDigit(input[i]))
            {
                _tokens.Add(ParseNumber(input, ref i));
            }

            i++;
        }

        return _tokens;
    }

    private Token ParseNumber(string input, ref int position)
    {
        var startPos = position;
        while (position < input.Length && (char.IsDigit(input[position]) || input[position] == '.'))
            position++;

        return new Token(TokenType.Number, input[startPos..position--]);
    }

    [ExcludeFromCodeCoverage]
    private Token ParseOperation(string input, int pos)
    {
        return input[pos] switch
        {
            '+' => new Token(TokenType.Plus, "+"),
            '*' => new Token(TokenType.Multiply, "*"),
            '/' => new Token(TokenType.Divide, "/"),
            '-' when pos == 0 || _tokens[^1].Type == TokenType.OpenBracket 
                => new Token(TokenType.Negate, "-"),
            '-' => new Token(TokenType.Minus, "-"),
            _ => throw new ArgumentException("Cannot parse operation")
        };
    }

    [ExcludeFromCodeCoverage]
    private Token ParseBracket(string input, int pos)
    {
        return input[pos] switch
        {
            '(' => new Token(TokenType.OpenBracket, "("),
            ')' => new Token(TokenType.CloseBracket, ")"),
            _ => throw new ArgumentException("Cannot parse bracket")
        };
    }
}