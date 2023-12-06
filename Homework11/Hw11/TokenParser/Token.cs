namespace Hw11.Services.TokenParser;

public class Token
{
    public Token(TokenType type, string value)
    {
        Type = type;
        Value = value;
    }

    public TokenType Type { get; }
    public string Value { get; }

    public bool IsOperation => Type is 
        TokenType.Plus or
        TokenType.Minus or 
        TokenType.Multiply or 
        TokenType.Divide or
        TokenType.Negate;

    public int Priority => Type switch
    {
        TokenType.Negate => 2,
        TokenType.Multiply => 1,
        TokenType.Divide => 1,
        TokenType.Plus => 0,
        TokenType.Minus => 0,
        _ => int.MinValue
    };
}