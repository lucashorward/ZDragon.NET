﻿using System;
using System.Linq;
using System.Xml.Schema;
using Compiler.AST;

namespace Mapper.XSD
{
	public partial class Mapper
	{
		public static XmlSchemaType MapList<T>(T e) where T : IElement, IRestrictable {
            XmlSchemaComplexType complexType = new XmlSchemaComplexType();
            complexType.Name = e.Name;

            XmlSchemaSequence sequence = new XmlSchemaSequence();
            var min = e.Restrictions.FirstOrDefault(r => r.Key == "min");
            var max = e.Restrictions.FirstOrDefault(r => r.Key == "max");
            sequence.MinOccurs = min is null ? 0 : int.Parse(min.Value);
            sequence.MaxOccurs = max is null ? 10 : int.Parse(max.Value);


            string _type = e.Type.Last().Value;

            XmlSchemaElement element = new XmlSchemaElement();
            switch (_type)
            {
                case "String":
                    element.SchemaType = Mapper.MapString(e);
                    break;
                case "Number":
                    element.SchemaType = Mapper.MapNumber(e);
                    element.Name = null;
                    break;
                case "Boolean":
                    element.SchemaType = Mapper.MapBoolean(e);
                    break;
                case "DateTime":
                    element.SchemaType = Mapper.MapDateTime(e);
                    break;
                case "Date":
                    element.SchemaType = Mapper.MapDate(e);
                    break;
                case "Time":
                    element.SchemaType = Mapper.MapTime(e);
                    break;
                default:
                    element.RefName = new System.Xml.XmlQualifiedName("self:" + _type);
                    break;
            }

            element.Name = null;
            sequence.Items.Add(element);
            complexType.Particle = sequence;
            return complexType;
        }
	}
}
