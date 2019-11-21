using System;
using System.Collections.Generic;
using System.Linq;

namespace Compiler.AST
{
    public class ASTChoice : IASTNode, INamable, ICloneable
    {
        public string Name { get; set; } = "";
        public List<ASTTypeDefinition> Type { get; set; } = new List<ASTTypeDefinition>();
        public List<ASTOption> Options { get; } = new List<ASTOption>();

        public ASTChoice() { }

        public ASTChoice(string name, List<ASTTypeDefinition> type, List<ASTOption> options)
        {
            this.Name = name;
            this.Type = type;
            this.Options = options;
        }

        public static (List<ASTError>, ASTChoice) Parse(IParser parser)
        {
            if (parser.HasNext()) parser.Next();
            var nameId = parser.Consume(TokenType.Identifier);
            parser.Consume(TokenType.Equal);
            parser.TryConsume(TokenType.EndStatement);
            parser.Consume(TokenType.ContextEnded);
            var result = new ASTChoice(nameId.Value, ASTTypeDefinition.ParseType(parser).ToList(), ASTOption.Parse(parser).ToList());
            return (new List<ASTError>(), result);
        }

        public object Clone()
        {
            return new ASTChoice
            (
                (string)this.Name.Clone(),
                ObjectCloner.CloneList<ASTTypeDefinition>(this.Type),
                ObjectCloner.CloneList<ASTOption>(this.Options)
            );
        }
    }
}
