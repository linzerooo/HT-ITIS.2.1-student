using System.Globalization;
using Hw11.ErrorMessages;
using Hw11.Exceptions;

namespace Hw11.Services.Validator;

public class MyValidator : IValidator
{
    private readonly char[] _operations = { '+', '-', '*', '/' };
    private readonly char[] _brackets = { '(', ')' };

    public void ValidateMathExpression(string? input)
    {
        ValidateNotEmptiness(input);
        ValidateBrackets(input!);
        ValidateLackOfUnknownCharacters(input!);
        ValidateOperationPlace(input!);
        ValidateNumbersCorrectness(input!);
    }

    private void ValidateNotEmptiness(string? input)
    {
        if (!string.IsNullOrEmpty(input)) return;

        throw new InvalidSyntaxException(MathErrorMessager.EmptyString);
    }

    private void ValidateBrackets(string input)
    {
        var stack = new Stack<char>();
        foreach (var ch in input)
        {
            switch (ch)
            {
                case '(':
                    stack.Push(ch);
                    break;
                case ')' when stack.TryPeek(out var tail) && tail == '(':
                    stack.Pop();
                    break;
                case ')':
                    throw new InvalidSyntaxException(MathErrorMessager.IncorrectBracketsNumber);
            }
        }

        if (stack.Count <= 0) return;

        throw new InvalidSyntaxException(MathErrorMessager.IncorrectBracketsNumber);
    }

    private void ValidateLackOfUnknownCharacters(string input)
    {
        foreach (var ch in input.Where(ch => 
                     !char.IsDigit(ch) && 
                     !char.IsWhiteSpace(ch) && 
                     !_operations.Contains(ch) &&
                     !_brackets.Contains(ch) &&
                     ch != '.'))
        {
            throw new InvalidSymbolException(MathErrorMessager.UnknownCharacterMessage(ch));
        }
    }

    private void ValidateOperationPlace(string input)
    {
        var stack = new Stack<char>();
        foreach (var ch in input)
        {
            if (_operations.Contains(ch))
            {
                switch (stack.TryPeek(out var tail))
                {
                    case true when _operations.Contains(tail):
                        throw new InvalidSyntaxException(MathErrorMessager.TwoOperationInRowMessage(tail.ToString(), ch.ToString()));
                    case true when tail == '(' && ch != '-':
                        throw new InvalidSyntaxException(MathErrorMessager.InvalidOperatorAfterParenthesisMessage(ch.ToString()));
                    case false when ch != '-':
                        throw new InvalidSyntaxException(MathErrorMessager.StartingWithOperation);
                    default:
                        stack.Push(ch);
                        break;
                }
            }
            else if (_brackets.Contains(ch))
            {
                switch (stack.TryPeek(out var tail))
                {
                    case true when ch == ')' && _operations.Contains(tail):
                        throw new InvalidSyntaxException(MathErrorMessager.OperationBeforeParenthesisMessage(tail.ToString()));
                    default:
                        stack.Push(ch);
                        break;
                }
            }
            else if (!char.IsWhiteSpace(ch))
            {
                stack.Push(ch);
            }
        }

        if (_operations.Contains(stack.Pop()))
        {
            throw new InvalidSyntaxException(MathErrorMessager.EndingWithOperation);
        }
    }

    private void ValidateNumbersCorrectness(string input)
    {
        var numberStartPos = 0;
        var isPreviousDigit = false;

        for (var i = 0; i < input.Length; i++)
        {
            if (!char.IsDigit(input[i]) && !isPreviousDigit) continue;

            if (char.IsDigit(input[i]) && !isPreviousDigit)
            {
                numberStartPos = i;
                isPreviousDigit = true;
            }
            else if (char.IsDigit(input[i]) && isPreviousDigit || input[i] == '.')
            {
                isPreviousDigit = true;
            }
            else if (!char.IsDigit(input[i]) && isPreviousDigit)
            {
                var maybeNumber = input[numberStartPos..i];
                if (!double.TryParse(maybeNumber, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out _))
                { 
                    throw new InvalidNumberException(MathErrorMessager.NotNumberMessage(maybeNumber));
                }

                isPreviousDigit = false;
            }
        }

        if (isPreviousDigit)
        {
            var maybeNumber = input[numberStartPos..input.Length];
            if (!double.TryParse(maybeNumber, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out _))
            {
                throw new InvalidNumberException(MathErrorMessager.NotNumberMessage(maybeNumber));
            }
        }
    }
}