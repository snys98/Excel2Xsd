using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Excel2Xsd
{
    public class Node
    {
        public static List<string> DefinedTypes = new List<string>(){ "mobileCommon:MobileRequestHead", "common:ResponseStatusType" };

        public const string ElementTemplate =
            "<xs:element name=\"{Name}\" type=\"{Type}\"><xs:annotation><xs:documentation>{Remark}</xs:documentation></xs:annotation></xs:element>";

        public const string ComplexTypeTemplate =
            "<xs:complexType name=\"{Name}\">" +
            "<xs:annotation><xs:documentation>{Remark}</xs:documentation></xs:annotation>" +
            "<xs:sequence>" +
            "{Elements}" +
            "</xs:sequence>" +
            "</xs:complexType>";

        public const string ListElementTemplate =
            "<xs:element name=\"{Name}\" type=\"{Type}\" minOccurs=\"0\" maxOccurs=\"unbounded\"><xs:annotation><xs:documentation>{Remark}</xs:documentation></xs:annotation></xs:element>";

        public const string ElementWithComplexTypeTemplate = "<xs:element name=\"{Name}\">" +
                                                             "<xs:complexType>" +
                                                             "<xs:sequence>" +
                                                             "{Elements}"+
                                                             "</xs:sequence></xs:complexType>" +
                                                             "</xs:element>";

        public const string EnumTypeTemplate = @"<xs:simpleType name=""{Name}"">
                                                    <xs:restriction base=""xs:string"">
                                                      {Elements}
                                                    </xs:restriction>
                                                  </xs:simpleType>";

        public const string EnumElementTemplate = @"<xs:enumeration value=""{Name}"">
                                                        <xs:annotation>
                                                            <xs:documentation>{Remark}</xs:documentation>
                                                        </xs:annotation>
                                                    </xs:enumeration>";

        public string Name { get; set; }
        public Node Parent { get; set; }
        public List<Node> Children { get; set; }
        public bool IsList { get; set; }
        public int Level { get; set; }
        public string Type { get; set; }
        public string Metadata { get; set; }
        public string Remark { get; set; }
        public int RowNum { get; set; }
        public string Range { get; set; }

        public override string ToString()
        {
            return String.Format("{0}:{{{1},{2},{3},{4},{5}}}", RowNum, Level, Name, Type, Metadata, Remark);
        }

        public string BuildComplexElement()
        {
            return ElementWithComplexTypeTemplate.Replace("{Name}", this.Name).Replace("{Elements}",
                this.Children.Aggregate(string.Empty,
                    (current, child) =>
                        current + child.BuildElement()));
        }

        public string BuildComplexType(bool isCoupeWithList = false)
        {
            string typeName = string.Empty;

            typeName = this.Type;

            if (DefinedTypes.Any(x => x == typeName))
            {
                return string.Empty;
            }
            DefinedTypes.Add(typeName);
            return ComplexTypeTemplate.Replace("{Name}", typeName)
                                    .Replace("{Remark}", this.Remark)
                                    .Replace("{Elements}",
                                        this.Children.Aggregate(string.Empty,
                                            (current, child) =>
                                                current + child.BuildElement()));
        }

        public string BuildElement()
        {
            if (IsList)
            {
                if (IsBaseType)
                {
                    var type = "xs:" + this.Metadata;
                    return ListElementTemplate.Replace("{Name}", this.Name)
                    .Replace("{Type}", type)
                    .Replace("{Remark}", this.Remark);
                }
                if (IsEnum)
                {
                    return ElementTemplate.Replace("{Name}", this.Name)
                    .Replace("{Type}", this.Name)
                    .Replace("{Remark}", this.Remark);
                }
                return ListElementTemplate.Replace("{Name}", this.Name)
                    .Replace("{Type}", this.Type)
                    .Replace("{Remark}", this.Remark);
            }
            else
            {
                if (IsBaseType)
                {
                    var type = "xs:" + this.Metadata;
                    return ElementTemplate.Replace("{Name}", this.Name)
                    .Replace("{Type}", type)
                    .Replace("{Remark}", this.Remark);
                }
                if (IsEnum)
                {
                    return ElementTemplate.Replace("{Name}", this.Name).Replace("{Type}", this.Type).Replace("{Remark}", this.Remark);
                }
                return ElementTemplate.Replace("{Name}", this.Name).Replace("{Type}", this.Type).Replace("{Remark}", this.Remark);
            }
        }

        private bool IsEnum => this.Metadata.ToUpper() == "ENUM";

        private bool IsBaseType => this.Type == String.Empty && this.Metadata.ToUpper() != "ENUM";

        public string BuildEnumType(bool isCoupeWithList = false)
        {
            string typeName = string.Empty;
            typeName = this.Type;
            if (DefinedTypes.Any(x => x == typeName))
            {
                return string.Empty;
            }
            DefinedTypes.Add(typeName);
            return EnumTypeTemplate.Replace("{Name}", typeName).Replace("{Remark}", this.Remark).Replace("{Elements}",this.BuildEnumElement(this.Range));
        }

        private string BuildEnumElement(string range)
        {
            var result = string.Empty;
            var enums = range.Split('\n').ToList();

            foreach (var @enum in enums)
            {
                var splits = @enum.Split('=');
                result += EnumElementTemplate.Replace("{Name}", splits[1])
                    .Replace("{Remark}", splits.Count() > 2 ? splits[2] : "");
            }
            
            return result;
        }
    }
    public static partial class Extensions
    {
        private static List<Node> list; 
        public static void FormStruct(this List<Node> nodes)
        {
            list = nodes;
            foreach (var node in nodes)
            {
                node.Parent = node.GetParent(node.Level);
                node.IsList = node.Metadata == "List";
            }
            foreach (var node in nodes)
            {
                node.Children = nodes.Where(x => x.Parent == node).ToList();
            }
        }

        public static Node GetParent(this Node node,int originalLevel)
        {
            if (node.Level < 1)
            {
                return null;
            }
            var above = list.Find(x => x.RowNum == node.RowNum - 1);
            if (above.Level != originalLevel - 1)
            {
                above = above.GetParent(originalLevel);
            }
            return above;
        }

    }
}
