using System.Text.RegularExpressions;
using Hw9.ErrorMessages;

namespace Hw9.Validator;

    public class Validator
    {
        private static readonly Regex Delimiters = new("(?<=[-+*/()])|(?=[-+*/()])");
        private static readonly Regex Numbers = new(@"^\d+"); 
        
        public void Validate(string input)
        {
            if (string.IsNullOrEmpty(input))
                throw new Exception(MathErrorMessager.EmptyString);

            if (CheckCorrectParenthesis(input))
                throw new Exception(MathErrorMessager.IncorrectBracketsNumber);

            ValidateCharacters(input);

            var inputSplited = Delimiters.Split(input.Replace(" ", "")).Where(c => c is not "").ToArray();

            ValidateStartingToken(inputSplited);
            ValidateEndingToken(inputSplited);

            ValidateTokenSequence(inputSplited);
        }

    private void ValidateCharacters(string input)
    {
        var unknownCharacter = input.Where(c => 
            !Numbers.IsMatch(c.ToString()) && !new[] { '+', '-', '*', '/', '(', ')', '.', ' ' }.Contains(c));

        foreach (var c in unknownCharacter)
            throw new Exception(MathErrorMessager.UnknownCharacterMessage(c));
    }

    private void ValidateStartingToken(string[] inputSplited)
    {
        if (!Numbers.IsMatch(inputSplited[0]) && !new[] { "-", "(" }.Contains(inputSplited[0]))
            throw new Exception(MathErrorMessager.StartingWithOperation);
    }

    private void ValidateEndingToken(string[] inputSplited)
    {
        if (!Numbers.IsMatch(inputSplited[^1]) && inputSplited[^1] != ")")
            throw new Exception(MathErrorMessager.EndingWithOperation);
    }

    private void ValidateTokenSequence(string[] inputSplited)
    {
        var lastToken = "";
        var lastTokenIsOp = true;

        foreach (var token in inputSplited)
        {
            if (ValidateNumberToken(token, ref lastToken, ref lastTokenIsOp))
                continue;

            if (ValidateMinusToken(token, ref lastToken, ref lastTokenIsOp))
                continue;

            switch (token)
            {
                case "(":
                    ValidateOpenParenthesisToken(ref lastToken, ref lastTokenIsOp);
                    break;
                case ")":
                    ValidateCloseParenthesisToken(lastToken, ref lastTokenIsOp);
                    break;
                default:
                    ValidateOtherToken(token, lastToken, ref lastTokenIsOp);
                    break;
            }
        }
    }

    private bool ValidateNumberToken(string token, ref string lastToken, ref bool lastTokenIsOp)
    {
        if (Numbers.IsMatch(token))
        {
            lastToken = token;
            lastTokenIsOp = false;
            if (!double.TryParse(token, out _))
                throw new Exception(MathErrorMessager.NotNumberMessage(token));
            return true;
        }
        return false;
    }

    private bool ValidateMinusToken(string token, ref string lastToken, ref bool lastTokenIsOp)
    {
        if (token == "-" && lastTokenIsOp)
        {
            lastToken = token;
            lastTokenIsOp = false;
            return true;
        }
        return false;
    }

    private void ValidateOpenParenthesisToken(ref string lastToken, ref bool lastTokenIsOp)
    {
        lastToken = "(";
        lastTokenIsOp = true;
    }

    private void ValidateCloseParenthesisToken(string lastToken, ref bool lastTokenIsOp)
    {
        if (lastTokenIsOp)
            throw new Exception(MathErrorMessager.OperationBeforeParenthesisMessage(lastToken));
        lastTokenIsOp = false;
    }

    private void ValidateOtherToken(string token, string lastToken, ref bool lastTokenIsOp)
    {
        if (lastTokenIsOp)
        {
            if (lastToken == "(")
                throw new Exception(MathErrorMessager.InvalidOperatorAfterParenthesisMessage(token));
            throw new Exception(MathErrorMessager.TwoOperationInRowMessage(lastToken, token));
        }

        lastToken = token;
        lastTokenIsOp = true;
    }

    private bool CheckCorrectParenthesis(string input)
    {
        var openedParenthesis = 0;
        foreach (var c in input)
        {
            switch (c)
            {
                case '(':
                    openedParenthesis++;
                    break;
                case ')' when openedParenthesis == 0:
                    return false;
                case ')':
                    openedParenthesis--;
                    break;
            }
        }

        return openedParenthesis == 0;
    }
}