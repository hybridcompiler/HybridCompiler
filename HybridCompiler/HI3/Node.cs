using HILibrary;

namespace HI3
{
    internal class BinaryNode : Node
    {
        public Node Left { get; private set; }
        public Token Operator { get; private set; }
        public Node Right { get; private set; }

        public BinaryNode(Node left, Token op, Node right)
        {
            Left = left;
            Operator = op;
            Right = right;
        }

        public override long Visit()
        {
            long leftValue = Left.Visit();
            long rightValue = Right.Visit();

            switch (Operator.Type)
            {
                case TT.Plus: return leftValue + rightValue;
                case TT.Minus: return leftValue - rightValue;
                case TT.Mul: return leftValue * rightValue;
                case TT.Div: return leftValue / rightValue;
            }
            HIDebug.Fail(Operator.Type, Id.InvalidOperator.Str());
            return 0;
        }
    }

    internal class IntegerNode : Node
    {
        public Token Integer { get; private set; }

        public IntegerNode(Token integerToken)
        {
            HIDebug.AssertEqual(integerToken.Type, TT.Integer);
            Integer = integerToken;
        }

        public override long Visit() => Integer.Integer;
    }

    internal abstract class Node
    {
        public abstract long Visit();
    }
}