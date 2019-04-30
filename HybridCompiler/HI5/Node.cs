using HILibrary;
using System;
using System.Collections.Generic;

namespace HI5
{
    internal class AssignNode : Node
    {
        public Token AssignOperator { get; private set; }
        public Node Expression { get; private set; }
        public Symbol Identifier { get; private set; }

        public override long Visit
        {
            get
            {
                long value = Expression.Visit;

                switch (AssignOperator.Type)
                {
                    case TT.Assign: Identifier.IntegerValue = value; break;
                    case TT.Plus: Identifier.IntegerValue += value; break;
                    case TT.Minus: Identifier.IntegerValue -= value; break;
                    case TT.Mul: Identifier.IntegerValue *= value; break;

                    case TT.Div:
                        CheckDivisionByZero(value, AssignOperator);
                        Identifier.IntegerValue /= value;
                        break;
                    case TT.Remainder:
                        CheckDivisionByZero(value, AssignOperator);
                        Identifier.IntegerValue %= value;
                        break;

                    case TT.LeftShift: Identifier.IntegerValue <<= (byte)value; break;
                    case TT.RightShift: Identifier.IntegerValue >>= (byte)value; break;
                    case TT.BitAnd: Identifier.IntegerValue &= value; break;
                    case TT.BitXor: Identifier.IntegerValue ^= value; break;
                    case TT.BitOr: Identifier.IntegerValue |= value; break;

                    default: throw new HIException(ET.SyntaxError, Id.InvalidSyntax, AssignOperator);
                }
                return Identifier.IntegerValue;
            }
        }

        public AssignNode(Symbol identifier, Token assignOperator, Node expression)
        {
            Identifier = identifier;
            AssignOperator = assignOperator;
            Expression = expression;
        }
    }

    internal class BinaryNode : Node
    {
        public Node Left { get; private set; }
        public Token Operator { get; private set; }
        public Node Right { get; private set; }

        public override long Visit
        {
            get
            {
                long leftValue = Left.Visit;
                long rightValue = Right.Visit;

                switch (Operator.Type)
                {
                    case TT.Plus: return leftValue + rightValue;
                    case TT.Minus: return leftValue - rightValue;
                    case TT.Mul: return leftValue * rightValue;

                    case TT.Div:
                        CheckDivisionByZero(rightValue, Operator);
                        return leftValue / rightValue;
                    case TT.Remainder:
                        CheckDivisionByZero(rightValue, Operator);
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
                HIDebug.Fail(Operator.Type, Id.InvalidOperator.ToText());
                return 0;
            }
        }

        public BinaryNode(Node left, Token op, Node right)
        {
            Left = left;
            Operator = op;
            Right = right;
        }
    }

    internal class BlockNode : Node
    {
        public List<Node> Statements { get; } = new List<Node>();

        public override long Visit
        {
            get
            {
                long value = 0;
                foreach (var node in Statements) { value = node.Visit; }
                return value;
            }
        }
    }

    internal class IdentifierNode : Node
    {
        public Symbol Identifier { get; private set; }

        public override long Visit => Identifier.IntegerValue;

        public IdentifierNode(Symbol identifier) => Identifier = identifier;
    }

    internal class IntegerNode : Node
    {
        public Token Integer { get; private set; }

        public override long Visit => Integer.IntegerValue;

        public IntegerNode(Token integerToken)
        {
            HIDebug.AssertEqual(integerToken.Type, TT.Integer);
            Integer = integerToken;
        }
    }

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

    internal class UnaryNode : Node
    {
        public Node Node { get; private set; }
        public Token Sign { get; private set; }

        public override long Visit
        {
            get
            {
                switch (Sign.Type)
                {
                    case TT.Not: return Node.Visit == 0 ? 1 : 0;
                    case TT.Plus: return Node.Visit;
                    case TT.Minus: return -Node.Visit;
                }
                HIDebug.Fail(Sign.Type, Id.InvalidOperator.ToText());
                return 0;
            }
        }

        public UnaryNode(Node node, Token sign)
        {
            Node = node;
            Sign = sign;
        }
    }

    internal class PrintNode : Node
    {
        public Node Expression { get; private set; }

        public override long Visit
        {
            get
            {
                var expression = Expression.Visit;
                Console.WriteLine(expression);
                return expression;
            }
        }

        public PrintNode(Node expression) => Expression = expression;
    }

    internal class IfNode : Node
    {
        public List<Node> IfNodes { get; private set; }
        public Node ElseNode { get; private set; }

        public override long Visit
        {
            get
            {
                for (int i = 0;i < IfNodes.Count; i += 2)
                {
                    if (IfNodes[i].Visit != 0) { return IfNodes[i + 1].Visit; }
                }
                if (ElseNode != null) { return ElseNode.Visit; }
                return 0;
            }
        }

        public IfNode(List<Node> ifNodes, Node elseNode)
        {
            IfNodes = ifNodes;
            ElseNode = elseNode;
        }
    }

    internal class WhileNode : Node
    {
        public Node Expression { get; private set; }
        public Node Block { get; private set; }

        public override long Visit
        {
            get
            {
                long value = 0;
                while (Expression.Visit != 0) { value = Block.Visit; }
                return value;
            }
        }

        public WhileNode(Node expression, Node block)
        {
            Expression = expression;
            Block = block;
        }
    }

    internal class ForNode : Node
    {
        public Symbol Identifier { get; private set; }
        public long Start { get; private set; }
        public Node End { get; private set; }
        public Node Step { get; private set; }
        public Node Block { get; private set; }

        public override long Visit
        {
            get
            {
                long step = Step == null ? 1 : Step.Visit;
                long value = 0;
                if (Identifier == null)
                {
                    for (long i = Start; i < End.Visit; i += step) { value = Block.Visit; }
                }
                else
                {
                    for (Identifier.IntegerValue = Start;
                        Identifier.IntegerValue < End.Visit; Identifier.IntegerValue += step)
                    {
                        value = Block.Visit;
                    }
                }
                return value;
            }
        }

        public ForNode(Symbol identifier, Node start, Node end, Node step, Node block)
        {
            Identifier = identifier;
            if (end == null)
            {
                Start = 0;
                End = start;
            }
            else
            {
                Start = start.Visit;
                End = end;
            }
            Step = step;
            Block = block;
        }
    }
}