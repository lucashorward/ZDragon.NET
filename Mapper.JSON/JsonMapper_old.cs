﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Compiler.AST;
using Newtonsoft.Json.Schema;

namespace Mapper.JSON
{
    public class JsonMapper_old
    {
        public JsonMapper_old(List<IASTNode> parseTree)
        {

            var schemas = new List<JSchema>();

            foreach (var node in parseTree)
            {
                if (node is ASTType t)
                {
                    var schema = new JSchema
                    {
                        Title = t.Name,
                        Description = Annotate(t.Annotations),
                        Type = JSchemaType.Object
                    };
                    var properties = new JSchema { };

                    var requiredFields = new List<string>();
                    foreach (var field in t.Fields) {
                        var _mode = field.Type.First().Value;
                        var _type = field.Type.Last().Value;

                        if (_mode != "List")
                        {
                            JSchemaType fieldType = ConvertToJsonType(_type);
                            schema.Properties.Add(
                                field.Name,
                                new JSchema
                                {
                                    Description = Annotate(field.Annotations),
                                    Type = fieldType
                                });
                        }
                        else
                        {
                            JSchemaType fieldType = JSchemaType.Array;
                            var list = new JSchema
                            {
                                Description = Annotate(field.Annotations),
                                Type = fieldType
                            };
                            list.Items.Add(new JSchema
                            {
                                Type = ConvertToJsonType(_type)
                            });
                            schema.Properties.Add(field.Name,list);

                        }

                        var nullable = field.Type.First().Value == "Maybe";
                        if (!nullable)
                        {
                            schema.Required.Add(field.Name);
                        }
                    }

                    schemas.Add(schema);                    
                }
            }

        }

        private JSchemaType ConvertToJsonType(string sourceType)
        {
            return sourceType switch
            {
                "String" => JSchemaType.String,
                "Number" => JSchemaType.Number,
                _ => JSchemaType.String
            };
        }

        private string Annotate(IEnumerable<ASTAnnotation> annotations)
        {
            return string.Join("\n", annotations.Select(a => a.Value).ToList());
        }

    }
}

/*
$schema": "http://json-schema.org/draft-04/schema#",
   "title": "Product",
   "description": "A product from Acme's catalog",
   "type": "object",
	
   "properties": {
	
      "id": {
         "description": "The unique identifier for a product",
         "type": "integer"
      },
		
      "name": {
         "description": "Name of the product",
         "type": "string"
      },
		
      "price": {
         "type": "number",
         "minimum": 0,
         "exclusiveMinimum": true
      }
   },
	
   "required": ["id", "name", "price"]
*/
