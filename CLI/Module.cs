﻿using System;
using System.Collections.Generic;
using System.Linq;
using Compiler;
using Compiler.AST;

namespace CLI
{
    public class Module
    {
        public string Name { get; }
        public string Path { get; }
        public string BasePath { get; }
        public string OutPath { get; }
        public Project Project { get; }
        public List<string> References { get; private set; } = new List<string>();
        public Transpiler Transpiler { get; private set; }
        public ASTGenerator Generator { get; private set; }
        public DateTime LastParsed { get; private set; }

        public Module(string path, string basePath, Project project)
        {
            this.Path = path;
            this.BasePath = basePath;
            this.Name = CreateModuleName();
            this.OutPath = System.IO.Path.GetFullPath($"out/{Name}", basePath);
            this.Project = project;
        }

        public void Parse()
        {
            var code = ReadModuleText();
            this.Generator = new ASTGenerator(code);
            this.LastParsed = DateTime.Now;
            this.Transpiler = new Transpiler(this.Generator, this.Project);

            References = Generator.AST.FindAll(n => n is ASTImport).Select(i => ((ASTImport)i).Name).ToList(); 
        }

        public void SaveModuleOutput()
        {
            this.Transpiler.StartMappings();
            Console.WriteLine($"Perfectly parsed: {Name}");
            SaveResult("Model.xsd", Transpiler.XsdToString());
            SaveResult("index.html", Transpiler.HtmlToString());
            foreach (var (key, value) in Transpiler.JsonToString())
            {
                SaveResult(key, value);
            }

            // We would now also want to resolve the other modules
            // which depend upon this module so that they are automatically
            // regenerated and their output changed.
            this.Project
                .Modules
                .FindAll(m => m.References.FirstOrDefault(r => r == this.Name) != null)
                .ForEach(m =>
                    {
                        m.Parse();
                        m.SaveModuleOutput();
                    });
        }

        private string ReadModuleText()
        {
            return System.IO.File.ReadAllText(Path) + "\n";
        }

        private void SaveResult(string fileName, string source)
        {
            string filePath = System.IO.Path.GetFullPath(fileName, OutPath);
            System.IO.Directory.CreateDirectory(OutPath);
            System.IO.File.WriteAllText(filePath, source);
        }

        private string CreateModuleName()
        {
            var p = Path.Replace(BasePath, "").Replace("/", ".").Replace(".car", "");
            if (p.StartsWith(".", StringComparison.Ordinal))
            {
                p = p.Substring(1);
            }
            return p;
        }

        public override string ToString()
        {
            return $@"
Module:     {Name}
Path:       {Path}
Dir:        {BasePath}
LastParsed: {LastParsed}
";
        }

    }

    public static class DictionaryHelpers
    {
        public static void Deconstruct<T1, T2>(this KeyValuePair<T1, T2> tuple, out T1 key, out T2 value)
        {
            key = tuple.Key;
            value = tuple.Value;
        }

    }
}
