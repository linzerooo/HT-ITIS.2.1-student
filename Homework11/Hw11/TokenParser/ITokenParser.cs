namespace Hw11.Services.TokenParser;

public interface ITokenParser
{
    public List<Token> Parse(string input);
}