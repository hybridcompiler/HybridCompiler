using System.Collections.Generic;

namespace HI6
{
    internal class Parser
    {
        public const string EntryFunctionName = "main";
        public Symbol EntryFunction { get; private set; }

        public Parser(List<Token> tokens) => this.tokens = tokens;

        public Node ParseLine()
        {
            var statement = ParseStatement();
            Match(TT.EOF);
            return statement;
        }

        public void Run()
        {
            SymbolTable.Init();
            ParseProgram();
            Match(TT.EOF);

            if (!SymbolTable.Find(EntryFunctionName, out var symbol))
            {
                throw new HIException(ET.NameError, Id.NameNotDefined);
            }
            if (symbol.Function == null) { throw new HIException(ET.TypeError, Id.NotFunctionIdentifier); }
            EntryFunction = symbol;
        }

        private readonly List<Token> tokens;
        private int currentPos;

        private Token CurrentToken => tokens[currentPos];

        private Symbol AddSymbol()
        {
            var symbol = SymbolTable.AddSymbol(CurrentToken);
            MoveNext();
            return symbol;
        }

        private void Match(TT type)
        {
            if (CurrentToken.Type != type)
            {
                throw new HIException(ET.SyntaxError, Id.InvalidSyntax, CurrentToken);
            }
            MoveNext();
        }

        private void Match(KT type)
        {
            if (!IsKeyword(type))
            {
                throw new HIException(ET.SyntaxError, Id.InvalidSyntax, CurrentToken);
            }
            MoveNext();
        }

        private bool IsKeyword(KT type) =>
            (CurrentToken.Type == TT.Keyword && CurrentToken.KeywordType == type);

        private void MoveNext() => currentPos++;

        // EBNF : program = function_statement, { function_statement } ;
        private void ParseProgram()
        {
            while (CurrentToken.Type != TT.EOF) { ParseFunction(); }
        }

        // EBNF : function_statement = "def", identifier, "(", [ identifier, { ",", identifier } ],
        //                             ")", block ;
        private void ParseFunction()
        {
            Match(KT.Def);
            var symbol = AddSymbol();
            var node = SymbolTable.Open();
            var function = new FunctionNode(node);
            symbol.Function = function;

            Match(TT.LeftParens);
            if (CurrentToken.Type != TT.RightParens)
            {
                function.AddSymbol(AddSymbol());
                while (CurrentToken.Type != TT.RightParens)
                {
                    Match(TT.Comma);
                    function.AddSymbol(AddSymbol());
                }
            }
            MoveNext();

            ParseBlockWithoutCreatingSymbolTable(node);
            SymbolTable.Close();
        }

        // EBNF : block = "{", { statement }, "}"
        private Node ParseBlock()
        {
            Match(TT.BeginBlock);
            var node = SymbolTable.Open();
            while (CurrentToken.Type != TT.EndBlock) { node.AddStatement(ParseStatement()); }
            SymbolTable.Close();
            MoveNext();
            return node;
        }

        private void ParseBlockWithoutCreatingSymbolTable(BlockNode node)
        {
            Match(TT.BeginBlock);
            while (CurrentToken.Type != TT.EndBlock) { node.AddStatement(ParseStatement()); }
            MoveNext();
        }

        // EBNF : statement = keyword_statement | expression ;
        private Node ParseStatement()
        {
            var token = CurrentToken;
            switch (token.Type)
            {
                case TT.Keyword: return ParseKeywordStatement();
            }
            return ParseExpression();
        }

        // EBNF : keyword_statement = variable_declaration | if_statement |
        //                            while_statement | for_statement | print;
        private Node ParseKeywordStatement()
        {
            var token = CurrentToken;
            MoveNext();

            switch (token.KeywordType)
            {
                case KT.Dim: return ParseVariableDeclaration();
                case KT.If: return ParseIfStatement();
                case KT.While: return ParseWhileStatement();
                case KT.For: return ParseForStatement();
                case KT.Print: return ParsePrint();
            }
            throw new HIException(ET.SyntaxError, Id.InvalidSyntax, token);
        }

        // EBNF : variable_declaration = "dim", identifier, [ "=", expression ] ;
        private Node ParseVariableDeclaration()
        {
            var symbol = AddSymbol();
            if (CurrentToken.Type == TT.Assign)
            {
                var token = CurrentToken;
                MoveNext();
                return new AssignNode(symbol, token, ParseExpression());
            }
            return new Node();
        }

        // EBNF : if_statement = "if", "(", expression, ")", block,
        //                       { "elif", "(", expression, ")", block }, [ "else", block ] ;
        private Node ParseIfStatement()
        {
            var ifNodes = new List<Node>();
            Match(TT.LeftParens);
            ifNodes.Add(ParseExpression());
            Match(TT.RightParens);
            ifNodes.Add(ParseBlock());

            while (IsKeyword(KT.Elif))
            {
                MoveNext();
                Match(TT.LeftParens);
                ifNodes.Add(ParseExpression());
                Match(TT.RightParens);
                ifNodes.Add(ParseBlock());
            }
            Node elseNode = null;
            if (IsKeyword(KT.Else))
            {
                MoveNext();
                elseNode = ParseBlock();
            }
            return new IfNode(ifNodes, elseNode);
        }

        // EBNF : while_statement = "while", "(", expression, ")", block ;
        private Node ParseWhileStatement()
        {
            Match(TT.LeftParens);
            var expression = ParseExpression();
            Match(TT.RightParens);
            return new WhileNode(expression, ParseBlock());
        }

        // EBNF : for_statement = "for", [ [ "dim" ], identifier, "in" ], "range",
        //                        "(", expression, [ ",", expression ],
        //                        [ ",", expression ], ")", block ;
        private Node ParseForStatement()
        {
            var node = SymbolTable.Open();

            Symbol symbol = null;
            if (IsKeyword(KT.Dim))
            {
                MoveNext();
                symbol = AddSymbol();
                Match(KT.In);
            }
            else if (CurrentToken.Type == TT.Identifier)
            {
                symbol = SymbolTable.GetVariable(CurrentToken);
                MoveNext();
                Match(KT.In);
            }

            Match(KT.Range);
            Match(TT.LeftParens);

            Node end = null;
            Node step = null;
            var start = ParseExpression();
            
            if (CurrentToken.Type == TT.Comma)
            {
                MoveNext();
                end = ParseExpression();
                if (CurrentToken.Type == TT.Comma)
                {
                    MoveNext();
                    step = ParseExpression();
                }
            }
            Match(TT.RightParens);
            ParseBlockWithoutCreatingSymbolTable(node);

            SymbolTable.Close();
            return new ForNode(symbol, start, end, step, node);
        }

        // EBNF : print = "print", "(", expression | string, { ",", expression | string } ")" ;
        private Node ParsePrint()
        {
            var printNode = new PrintNode();
            Match(TT.LeftParens);
            AddExpression();
            while (CurrentToken.Type != TT.RightParens)
            {
                Match(TT.Comma);
                AddExpression();
            }
            Match(TT.RightParens);
            return printNode;

            void AddExpression()
            {
                if (CurrentToken.Type == TT.String)
                {
                    printNode.AddExpression(new StringNode(CurrentToken));
                    MoveNext();
                }
                else
                {
                    printNode.AddExpression(ParseExpression());
                }
            }
        }

        // EBNF : expression = [ identifier, ( "=" | "+=" | "-=" | "*=" | "/=" | "%=" |
        //                     "<<=" | ">>=" | "&=" | "^=" | "|=" ) ], logical_or ;
        private Node ParseExpression()
        {
            if (CurrentToken.Type == TT.Identifier && currentPos < tokens.Count - 2 &&
                (tokens[currentPos + 1].Type == TT.Assign ||
                tokens[currentPos + 2].Type == TT.Assign))
            {
                var symbol = SymbolTable.GetVariable(CurrentToken);
                MoveNext();
                var token = CurrentToken;
                if (token.Type != TT.Assign) { MoveNext(); }
                MoveNext();
                return new AssignNode(symbol, token, ParseLogicalOr());
            }

            return ParseLogicalOr();
        }

        // EBNF : logical_or = logical_and, { "||", logical_and } ;
        private Node ParseLogicalOr()
        {
            var leftNode = ParseLogicalAnd();
            var token = CurrentToken;
            while (token.Type == TT.Or)
            {
                MoveNext();
                leftNode = new BinaryNode(leftNode, token, ParseLogicalAnd());
                token = CurrentToken;
            }
            return leftNode;
        }

        // EBNF : logical_and = bitwise_or, { "&&", bitwise_or } ;
        private Node ParseLogicalAnd()
        {
            var leftNode = ParseBitwiseOr();
            var token = CurrentToken;
            while (token.Type == TT.And)
            {
                MoveNext();
                leftNode = new BinaryNode(leftNode, token, ParseBitwiseOr());
                token = CurrentToken;
            }
            return leftNode;
        }

        // EBNF : bitwise_or = bitwise_xor, { "|", bitwise_xor } ;
        private Node ParseBitwiseOr()
        {
            var leftNode = ParseBitwiseXor();
            var token = CurrentToken;
            while (token.Type == TT.BitOr)
            {
                MoveNext();
                leftNode = new BinaryNode(leftNode, token, ParseBitwiseXor());
                token = CurrentToken;
            }
            return leftNode;
        }

        // EBNF : bitwise_xor = bitwise_and, { "^", bitwise_and } ;
        private Node ParseBitwiseXor()
        {
            var leftNode = ParseBitwiseAnd();
            var token = CurrentToken;
            while (token.Type == TT.BitXor)
            {
                MoveNext();
                leftNode = new BinaryNode(leftNode, token, ParseBitwiseAnd());
                token = CurrentToken;
            }
            return leftNode;
        }

        // EBNF : bitwise_and = equality, { "&", equality } ;
        private Node ParseBitwiseAnd()
        {
            var leftNode = ParseEuality();
            var token = CurrentToken;
            while (token.Type == TT.BitAnd)
            {
                MoveNext();
                leftNode = new BinaryNode(leftNode, token, ParseEuality());
                token = CurrentToken;
            }
            return leftNode;
        }

        // EBNF : equality = relational, { "==" | "!=", relational } ;
        private Node ParseEuality()
        {
            var leftNode = ParseRelational();
            var token = CurrentToken;
            while (token.Type == TT.Equal || token.Type == TT.NotEqual)
            {
                MoveNext();
                leftNode = new BinaryNode(leftNode, token, ParseRelational());
                token = CurrentToken;
            }
            return leftNode;
        }

        // EBNF : relational = shift, { "<" | ">" | "<=" | ">=", shift } ;
        private Node ParseRelational()
        {
            var leftNode = ParseShift();
            var token = CurrentToken;
            while (token.Type == TT.LessThan || token.Type == TT.LessThanEqual ||
                token.Type == TT.GreaterThan || token.Type == TT.GreaterThanEqual)
            {
                MoveNext();
                leftNode = new BinaryNode(leftNode, token, ParseShift());
                token = CurrentToken;
            }
            return leftNode;
        }

        // EBNF : shift = additive, { "<<" | ">>", additive } ;
        private Node ParseShift()
        {
            var leftNode = ParseAdditive();
            var token = CurrentToken;
            while (token.Type == TT.LeftShift || token.Type == TT.RightShift)
            {
                MoveNext();
                leftNode = new BinaryNode(leftNode, token, ParseAdditive());
                token = CurrentToken;
            }
            return leftNode;
        }

        // EBNF : additive = multiplicative, { "+" | "-", multiplicative } ;
        private Node ParseAdditive()
        {
            var leftNode = ParseMultiplicative();
            var token = CurrentToken;
            while (token.Type == TT.Plus || token.Type == TT.Minus)
            {
                MoveNext();
                leftNode = new BinaryNode(leftNode, token, ParseMultiplicative());
                token = CurrentToken;
            }
            return leftNode;
        }

        // EBNF : multiplicative = unary, { "*" | "/" | "%", unary } ;
        private Node ParseMultiplicative()
        {
            var leftNode = ParseUnary();
            var token = CurrentToken;
            while (token.Type == TT.Mul || token.Type == TT.Div || token.Type == TT.Remainder)
            {
                MoveNext();
                leftNode = new BinaryNode(leftNode, token, ParseUnary());
                token = CurrentToken;
            }
            return leftNode;
        }

        // EBNF : unary = [ "+" | "-" | "!" ], factor ;
        private Node ParseUnary()
        {
            var token = CurrentToken;
            if (token.Type == TT.Not || token.Type == TT.Plus || token.Type == TT.Minus)
            {
                MoveNext();
                return new UnaryNode(ParseFactor(), token);
            }
            return ParseFactor();
        }

        // EBNF : factor = integer | identifier | call_function | ( "(", expression, ")" ) ;
        private Node ParseFactor()
        {
            var token = CurrentToken;
            MoveNext();

            switch (token.Type)
            {
                case TT.Integer: return new IntegerNode(token);
                case TT.Identifier:
                    if (CurrentToken.Type == TT.LeftParens) { return ParseCallfunciton(token); }
                    return new IdentifierNode(SymbolTable.GetVariable(token));

                case TT.LeftParens:
                    var node = ParseAdditive();
                    Match(TT.RightParens);
                    return node;

                default: throw new HIException(ET.SyntaxError, Id.InvalidSyntax, token);
            }
        }

        // EBNF : call_function = identifier, "(", [ expression, { ",", expression } ], ")" ;
        private Node ParseCallfunciton(Token token)
        {
            var callFunction = new CallNode(token);

            Match(TT.LeftParens);
            if (CurrentToken.Type != TT.RightParens)
            {
                callFunction.AddExpression(ParseExpression());
                while (CurrentToken.Type != TT.RightParens)
                {
                    Match(TT.Comma);
                    callFunction.AddExpression(ParseExpression());
                }
            }
            Match(TT.RightParens);
            return callFunction;
        }
    }
}