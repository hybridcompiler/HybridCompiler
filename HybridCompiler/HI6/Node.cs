using HILibrary;
using System;
using System.Collections.Generic;

namespace HI6
{
    internal class Node
    {
        public virtual long Visit => 0;

        protected void CheckDivisionByZero(long value, Token token)
        {
            if (value == 0)
            {
                throw new HIException(ET.ZeroDivisionError, Id.DivisionByZero, token);
            }
        }
    }

    internal class FunctionNode : Node
    {
        public List<Symbol> Symbols { get; } = new List<Symbol>();

        public override long Visit => block.Visit;

        public FunctionNode(Node block) => this.block = block;

        public void AddSymbol(Symbol symbol) => Symbols.Add(symbol);

        private readonly Node block;
    }

    internal class BlockNode : Node
    {
        public override long Visit
        {
            get
            {
                long value = 0;
                foreach (var node in statements) { value = node.Visit; }
                return value;
            }
        }

        public void AddStatement(Node node) => statements.Add(node);

        public void AddSymbol(Token token, Symbol symbol)
        {
            if (GetSymbol(token, out _))
            {
                throw new HIException(ET.NameError, Id.NameAlreadyDefined, token);
            }
            symbols[token.Text] = symbol;
        }

        public bool GetSymbol(Token token, out Symbol symbol) =>
            symbols.TryGetValue(token.Text, out symbol);

        public bool GetSymbol(string name, out Symbol symbol) => symbols.TryGetValue(name, out symbol);

        private readonly List<Node> statements = new List<Node>();
        private readonly Dictionary<string, Symbol> symbols = new Dictionary<string, Symbol>();
    }

    internal class IfNode : Node
    {
        public override long Visit
        {
            get
            {
                for (int i = 0; i < ifNodes.Count; i += 2)
                {
                    if (ifNodes[i].Visit != 0) { return ifNodes[i + 1].Visit; }
                }
                if (elseNode != null) { return elseNode.Visit; }
                return 0;
            }
        }

        public IfNode(List<Node> ifNodes, Node elseNode)
        {
            this.ifNodes = ifNodes;
            this.elseNode = elseNode;
        }

        private readonly List<Node> ifNodes;
        private readonly Node elseNode;
    }

    internal class WhileNode : Node
    {
        public override long Visit
        {
            get
            {
                long value = 0;
                while (expression.Visit != 0) { value = block.Visit; }
                return value;
            }
        }

        public WhileNode(Node expression, Node block)
        {
            this.expression = expression;
            this.block = block;
        }

        private readonly Node expression;
        private readonly Node block;
    }

    internal class ForNode : Node
    {
        public override long Visit
        {
            get
            {
                var start = this.end == null ? 0 : this.start.Visit;
                var end = this.end == null ? this.start.Visit : this.end.Visit;
                var step = this.step == null ? 1 : this.step.Visit;
                long value = 0;
                if (identifier == null)
                {
                    for (var i = start; i < end; i += step) { value = block.Visit; }
                }
                else
                {
                    for (identifier.IntegerValue = start;
                        identifier.IntegerValue < end; identifier.IntegerValue += step)
                    {
                        value = block.Visit;
                    }
                }
                return value;
            }
        }

        public ForNode(Symbol identifier, Node start, Node end, Node step, Node block)
        {
            this.identifier = identifier;
            this.start = start;
            this.end = end;
            this.step = step;
            this.block = block;
        }

        private readonly Symbol identifier;
        private readonly Node start;
        private readonly Node end;
        private readonly Node step;
        private readonly Node block;
    }

    internal class PrintNode : Node
    {
        public override long Visit
        {
            get
            {
                long value = 0;
                foreach (var expression in expressions)
                {
                    if (expression is StringNode stringNode)
                    {
                        Console.Write(stringNode.Visit);
                        continue;
                    }
                    value = expression.Visit;
                    Console.Write(value);
                }
                Console.WriteLine();
                return value;
            }
        }

        public void AddExpression(Node expression) => expressions.Add(expression);

        private readonly List<Node> expressions = new List<Node>();
    }

    internal class AssignNode : Node
    {
        public override long Visit
        {
            get
            {
                var value = expression.Visit;

                switch (assignOperator.Type)
                {
                    case TT.Assign: identifier.IntegerValue = value; break;
                    case TT.Plus: identifier.IntegerValue += value; break;
                    case TT.Minus: identifier.IntegerValue -= value; break;
                    case TT.Mul: identifier.IntegerValue *= value; break;

                    case TT.Div:
                        CheckDivisionByZero(value, assignOperator);
                        identifier.IntegerValue /= value;
                        break;
                    case TT.Remainder:
                        CheckDivisionByZero(value, assignOperator);
                        identifier.IntegerValue %= value;
                        break;

                    case TT.LeftShift: identifier.IntegerValue <<= (byte)value; break;
                    case TT.RightShift: identifier.IntegerValue >>= (byte)value; break;
                    case TT.BitAnd: identifier.IntegerValue &= value; break;
                    case TT.BitXor: identifier.IntegerValue ^= value; break;
                    case TT.BitOr: identifier.IntegerValue |= value; break;

                    default: throw new HIException(ET.SyntaxError, Id.InvalidSyntax, assignOperator);
                }
                return identifier.IntegerValue;
            }
        }

        public AssignNode(Symbol identifier, Token assignOperator, Node expression)
        {
            this.identifier = identifier;
            this.assignOperator = assignOperator;
            this.expression = expression;
        }

        private readonly Token assignOperator;
        private readonly Node expression;
        private readonly Symbol identifier;
    }

    internal class BinaryNode : Node
    {
        public override long Visit
        {
            get
            {
                long leftValue = left.Visit;
                long rightValue = right.Visit;

                switch (op.Type)
                {
                    case TT.Plus: return leftValue + rightValue;
                    case TT.Minus: return leftValue - rightValue;
                    case TT.Mul: return leftValue * rightValue;

                    case TT.Div:
                        CheckDivisionByZero(rightValue, op);
                        return leftValue / rightValue;
                    case TT.Remainder:
                        CheckDivisionByZero(rightValue, op);
                        return leftValue % rightValue;

                    case TT.Equal: return leftValue == rightValue ? 1 : 0;
                    case TT.NotEqual: return leftValue != rightValue ? 1 : 0;
                    case TT.LessThan: return leftValue < rightValue ? 1 : 0;
                    case TT.LessThanEqual: return leftValue <= rightValue ? 1 : 0;
                    case TT.GreaterThan: return leftValue > rightValue ? 1 : 0;
                    case TT.GreaterThanEqual: return leftValue >= rightValue ? 1 : 0;

                    case TT.And: return (leftValue == 0 ? 0 : 1) & (rightValue == 0 ? 0 : 1);
                    case TT.Or: return (leftValue == 0 ? 0 : 1) | (rightValue == 0 ? 0 : 1);

                    case TT.BitAnd: return leftValue & rightValue;
                    case TT.BitOr: return leftValue | rightValue;
                    case TT.BitXor: return leftValue ^ rightValue;

                    case TT.LeftShift: return leftValue << (byte)rightValue;
                    case TT.RightShift: return leftValue >> (byte)rightValue;
                }
                HIDebug.Fail(op.Type, Id.InvalidOperator.ToText());
                return 0;
            }
        }

        public BinaryNode(Node left, Token op, Node right)
        {
            this.left = left;
            this.op = op;
            this.right = right;
        }

        private readonly Node left;
        private readonly Token op;
        private readonly Node right;
    }

    internal class UnaryNode : Node
    {
        public override long Visit
        {
            get
            {
                switch (sign.Type)
                {
                    case TT.Not: return node.Visit == 0 ? 1 : 0;
                    case TT.Plus: return node.Visit;
                    case TT.Minus: return -node.Visit;
                }
                HIDebug.Fail(sign.Type, Id.InvalidOperator.ToText());
                return 0;
            }
        }

        public UnaryNode(Node node, Token sign)
        {
            this.node = node;
            this.sign = sign;
        }

        private readonly Node node;
        private readonly Token sign;
    }

    internal class IdentifierNode : Node
    {
        public override long Visit => identifier.IntegerValue;

        public IdentifierNode(Symbol identifier) => this.identifier = identifier;

        private readonly Symbol identifier;
    }

    internal class StringNode : Node
    {
        public new string Visit => str.Text;

        public StringNode(Token stringToken)
        {
            HIDebug.AssertEqual(stringToken.Type, TT.String);
            str = stringToken;
        }

        private readonly Token str;
    }

    internal class IntegerNode : Node
    {
        public override long Visit => integer.IntegerValue;

        public IntegerNode(Token integerToken)
        {
            HIDebug.AssertEqual(integerToken.Type, TT.Integer);
            integer = integerToken;
        }

        private readonly Token integer;
    }

    internal class CallNode : Node
    {
        public override long Visit
        {
            get
            {
                var function = SymbolTable.GetFunction(functionToken).Function;
                if (function.Symbols.Count != expressions.Count)
                {
                    throw new HIException(ET.TypeError, Id.InvalidArgumentsNumber, functionToken);
                }
                for (int i = 0;i < expressions.Count;i++)
                {
                    function.Symbols[i].IntegerValue = expressions[i].Visit;
                }
                foreach(var expression in expressions) {  }
                return function.Visit;
            }
        }

        public void AddExpression(Node expression) => expressions.Add(expression);

        public CallNode(Token token) => functionToken = token;

        private readonly List<Node> expressions = new List<Node>();
        private readonly Token functionToken;
    }
}