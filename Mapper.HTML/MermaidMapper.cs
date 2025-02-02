﻿using System;
using System.Collections.Generic;
using System.Linq;
using Compiler;
using Compiler.AST;

namespace Mapper.HTML
{
    public class MermaidMapper : VisitorBase<string>
    {
        private List<string> Parts = new List<string>();
        public MermaidMapper(IEnumerable<IASTNode> nodeTree) : base(nodeTree) { }

        public override string VisitASTAlias(ASTAlias astAlias)
        {
            var template = $@"
class {astAlias.Name} {{
{string.Join("\n", astAlias.Type.Select(t => t.Value).ToList())}
}}
";
            this.Parts.Add(template);

            return template;

        }

        public override string VisitASTAnnotation(ASTAnnotation astAnnotation)
        {
            return "";
        }

        public override string VisitASTChapter(ASTChapter astChapter)
        {
            return "";
        }

        public override string VisitASTChoice(ASTChoice astChoice)
        {
            string template = $@"
class {astChoice.Name} {{
{string.Join("\n", astChoice.Options.Select(o => o.Value).ToList())}
}}

";
            this.Parts.Add(template);
            return template;
        }

        public override string VisitASTData(ASTData astData)
        {
            var typeReferences = astData.Options.Select(f =>
            {
                string _type = f.Name;
                if (_type != "String"
                    && _type != "Number"
                    && _type != "Boolean"
                    && _type != "Date"
                    && _type != "DateTime"
                    && _type != "Time")
                {
                    return $@"{astData.Name} --* {_type}";
                }
                else
                {
                    return "";
                }
            });
            var template = $@"
class {astData.Name} {{
{string.Join("\n", astData.Options.Select(o => o.ToMermaidString()).ToList())}
}}
{string.Join("\n", typeReferences)}
";
            this.Parts.Add(template);
            return template;
        }

        public override string VisitASTDirective(ASTDirective astDirective)
        {
            return "";
        }

        public override string VisitASTOption(ASTOption astOption)
        {
            return "";
        }

        public override string VisitASTParagraph(ASTParagraph astParagraph)
        {
            return "";
        }

        public override string VisitASTRestriction(ASTRestriction astRestriction)
        {
            return "";
        }

        public override string VisitASTType(ASTType astType)
        {
            var extensions = astType.Extensions.Select(e => $"{astType.Name} --|> {e}");
            var fields = astType.Fields.Select(f => $@"{f.Name}: {string.Join(" ", f.Type.Select(t => t.Value))}").ToList();
            var typeReferences = astType.Fields.Select(f =>
            {
                var _mod = f.Type.First().Value;
                var _type = f.Type.Last().Value;
                if (_type != "String"
                    && _type != "Number"
                    && _type != "Boolean"
                    && _type != "Date"
                    && _type != "DateTime"
                    && _type != "Time")
                {
                    if (_mod == "List")
                    {
                        var min = f.Restrictions.FirstOrDefault(r => r.Key == "min")?.Value ?? "0";
                        var max = f.Restrictions.FirstOrDefault(r => r.Key == "max")?.Value ?? "*";

                        return $@"{astType.Name} --o ""{min}..{max}"" {_type}";
                    } else
                    {
                        return $@"{astType.Name} --o {_type}";
                    }
                }
                else
                {
                    return "";
                }
            });

            if (astType.Fields.Any())
            {
                var template = $@"
class {astType.Name} {{
{string.Join("\n", fields)}
}}
{string.Join("\n", extensions)}
{string.Join("\n", typeReferences)}
";
                this.Parts.Add(template);
                return template;
            }
            else
            {
                var template = $@"
class {astType.Name}
{string.Join("\n", extensions)}
";
                Parts.Add(template);
                return template;
            }

        }

        public override string VisitASTTypeDefinition(ASTTypeDefinition astTypeDefinition)
        {
            return "";
        }

        public override string VisitASTTypeField(ASTTypeField astTypeField)
        {
            return "";
        }

        public override string VisitDefault(IASTNode node)
        {
            return "";
        }

        public override string ToString()
        {

            return $@"
classDiagram
{string.Join("", Parts.ToList())}
".Trim();
        }

        public override string VisitASTFlow(ASTFlow astFlow)
        {
            return "";
        }
    }
}
