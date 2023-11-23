using System.Text.RegularExpressions;
using static Hw9.ErrorMessages.MathErrorMessager;

namespace Hw9.Services;

public static class Parser
{
    private static readonly Regex Delimiters = new("(?<=[-+*/()])|(?=[-+*/()])");
    private static readonly Regex Numbers = new(@"^\d+");

    private static Dictionary<string, int> _priority = new()
    {
        { "(", 0 },
        { ")", 0 },
        { "+", 1 },
        { "-", 1 },
        { "*", 2 },
        { "/", 2 }
    };

    public static string Parse(string input) => ToReversePolishNotation(input);
    
    // Lasciate ogne speranza, voi ch'entrate
    private static string ToReversePolishNotation(string input)
    {
        var ops = new Stack<string>();
        var polish = new Stack<string>();
        var inputSplited = Delimiters.Split(input.Replace(" ", ""));
        var lastTokenIsOp = true;
        for (var i = 0; i < inputSplited.Length; i++)
        {
            var token = inputSplited[i];
            if (token.Length == 0) continue;
            if (Numbers.IsMatch(token))
            {
                polish.Push(token);
                lastTokenIsOp = false;
                continue;
            }
            if (token == "-" && lastTokenIsOp)
            {
                polish.Push(token + inputSplited[++i]);
                lastTokenIsOp = false;
                continue;
            }
            switch (token)
            {
                case "(":
                    ops.Push(token);
                    lastTokenIsOp = true;
                    continue;
                case ")":
                {
                    while (ops.Peek() != "(")
                        PushOperation(ops, polish);
                    ops.Pop();
                    lastTokenIsOp = false;
                    continue;
                }
            }

            while (ops.Count > 0 && _priority[token] <= _priority[ops.Peek()])
                PushOperation(ops, polish);
            
            ops.Push(token);
            lastTokenIsOp = true;
        }

        while (ops.Count > 0)
            PushOperation(ops, polish);

        return polish.Pop();
    }

    private static void PushOperation(Stack<string> operations, Stack<string> polish)
    {
        var op = operations.Pop();
        var val1 = polish.Pop();
        var val2 = polish.Pop();
        polish.Push(val2 + " " + val1 + " " + op);
    }

    public static void Validate(string? input)
    {
        if (string.IsNullOrEmpty(input))
            throw new Exception(EmptyString);
        if (!CheckCorrectParenthesis(input))
            throw new Exception(IncorrectBracketsNumber);
        var inputSplited = Delimiters.Split(input.Replace(" ", ""))
            .Where(c => c is not "").ToArray();
        foreach (var c in input.Where(c => 
                     !Numbers.IsMatch(c.ToString()) && !new[] { '+', '-', '*', '/', '(', ')', '.', ' ' }.Contains(c)))
            throw new Exception(UnknownCharacterMessage(c));
        if (!Numbers.IsMatch(inputSplited[0]) && !new[] { "-", "(" }.Contains(inputSplited[0]))
            throw new Exception(StartingWithOperation);
        if (!Numbers.IsMatch(inputSplited[^1]) && inputSplited[^1] != ")")
            throw new Exception(EndingWithOperation);
        var lastToken = "";
        var lastTokenIsOp = true;
        foreach (var token in inputSplited)
        {
            if (Numbers.IsMatch(token))
            {
                lastToken = token;
                lastTokenIsOp = false;
                if (!double.TryParse(token, out _))
                    throw new Exception(NotNumberMessage(token));
                continue;
            }
            if (token == "-" && lastTokenIsOp)
            {
                lastToken = token;
                lastTokenIsOp = false;
                continue;
            }
            switch (token)
            {
                case "(":
                    lastToken = token;
                    lastTokenIsOp = true;
                    continue;
                case ")":
                {
                    if (lastTokenIsOp)
                        throw new Exception(OperationBeforeParenthesisMessage(lastToken));
                    lastToken = token;
                    lastTokenIsOp = false;
                    continue;
                }
            }

            if (lastTokenIsOp)
            {
                if (lastToken == "(")
                    throw new Exception(InvalidOperatorAfterParenthesisMessage(token));
                throw new Exception(TwoOperationInRowMessage(lastToken, token));
            }

            lastToken = token;
            lastTokenIsOp = true;
        }
    }

    private static bool CheckCorrectParenthesis(string input)
    {
        var openedParenthesis = 0;
        foreach (var c in input)
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

        return openedParenthesis == 0;
    }
}