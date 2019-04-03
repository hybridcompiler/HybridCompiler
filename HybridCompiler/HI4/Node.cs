using HILibrary;
using System.Collections.Generic;

namespace HI4
{
    internal class AssignNode : Node
    {
        public Token Assign { get; private set; }
        public Node Expression { get; private set; }
        public Symbol Identifier { get; private set; }

        public override long Visit
        {
            get
            {
                Identifier.IntegerValue = Expression.Visit;
                return Identifier.IntegerValue;
            }
        }

        public AssignNode(Symbol identifier, Token assign, Node expression)
        {
            Identifier = identifier;
            Assign = assign;
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
                        if (rightValue == 0)
                        {
                            throw new HIException(ET.ZeroDivisionError, Id.DivisionByZero, Operator);
                        }
                        return leftValue / rightValue;
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
        public List<Node> Nodes { get; } = new List<Node>();

        public override long Visit
        {
            get
            {
                long value = 0;
                foreach (var node in Nodes) { value = node.Visit; }
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
}