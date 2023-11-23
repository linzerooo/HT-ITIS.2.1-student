using System.Linq.Expressions;
using System.Text;

namespace Hw10.Services.MathCalculator;

public class ExpressionBuilder
{
    private readonly Stack<Expression> _operandStack = new();
    private readonly Stack<char> _operationStack = new();

    public static string PrepareString(string expression)
        => expression.Replace(" ", string.Empty);

    public Expression Parse(string expression)
    {
        if (expression.Contains(' '))
            expression = PrepareString(expression);

        _operandStack.Clear();
        _operationStack.Clear();

        var i = 0;
        var maybeUnary = true;
        while (i < expression.Length)
        {
            var curSym = expression[i];
            if (curSym == '(')
            {
                _operationStack.Push(curSym);
                maybeUnary = true;
            }
            else if (curSym == ')')
            {
                while (_operationStack.Peek() != '(')
                    ProcessOperation();

                _operationStack.Pop();
                maybeUnary = false;
            }
            else if (IsOperation(expression[i]))
            {
                var curPriority = GetOperationPriority(curSym);
                if (maybeUnary && curSym == '-')
                    curSym = '!';

                while (_operationStack.Count > 0 && 
                       (curSym != '!' && curPriority <= GetOperationPriority(_operationStack.Peek()) ||
                        curSym == '!' && curPriority < GetOperationPriority(_operationStack.Peek())))
                    ProcessOperation();

                _operationStack.Push(curSym);
                maybeUnary = curSym != '!';
            }
            else if (char.IsDigit(expression[i]))
            {
                var builder = new StringBuilder();
                do
                {
                    builder.Append(expression[i]);
                    i++;
                } while (i < expression.Length 
                         && (char.IsDigit(expression[i]) || expression[i] == '.'));
                i--;
                var value = double.Parse(builder.ToString());

                _operandStack.Push(Expression.Constant(value));
                maybeUnary = false;
            }
            i++;
        }

        while (_operationStack.Count > 0)
            ProcessOperation();

        return _operandStack.Peek();
    }

    private void ProcessOperation()
    {
        var operation = _operationStack.Pop();

        if (operation == '!')
        {
            var el = _operandStack.Pop();
            _operandStack.Push(Expression.Negate(el));
            return;
        }

        var r = _operandStack.Pop();
        var l = _operandStack.Pop();
        switch (operation) {
            case '+':  
                _operandStack.Push(Expression.Add(l, r));  
                break;
            case '-':  
                _operandStack.Push(Expression.Subtract(l, r));  
                break;
            case '*':  
                _operandStack.Push(Expression.Multiply(l, r));  
                break;
            case '/':  
                _operandStack.Push(Expression.Divide(l, r));  
                break;
        }
    }

    private bool IsOperation(char c) => c is '+' or '-' or '!' or '*' or '/';

    private int GetOperationPriority(char operation) {
        return operation switch
        {
            '+' or '-' or '!' => 1,
            '*' or '/' => 2,
            _ => -1
        };
    }
}