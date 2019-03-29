using HILibrary;

namespace HI3
{
    internal class BinaryNode : Node
    {
        public Node left;
        public Token op;
        public Node right;

        public BinaryNode(Node left, Token op, Node right)
        {
            this.left = left;
            this.op = op;
            this.right = right;
        }

        public override long Visit()
        {
            long leftValue = left.Visit();
            long rightValue = right.Visit();

            switch (op.type)
            {
                case TT.Plus: return leftValue + rightValue;
                case TT.Minus: return leftValue - rightValue;
                case TT.Mul: return leftValue * rightValue;
                case TT.Div: return leftValue / rightValue;
            }
            HIDebug.Fail(op.type, Id.InvalidOperator.Str());
            return 0;
        }
    }

    internal class IntegerNode : Node
    {
        public Token integer;

        public IntegerNode(Token integerToken)
        {
            HIDebug.AssertEqual(integerToken.type, TT.Integer);
            integer = integerToken;
        }

        public override long Visit() => integer.integer;
    }

    internal abstract class Node
    {
        public abstract long Visit();
    }
}