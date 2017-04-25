using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Excel2Xsd
{
    public class InterfaceObj
    {
        private List<Node> Nodes { get; }

        public InterfaceObj(DataTable table)
        {
            var allNodes = new List<Node>();
            var validNodes = table.GetRowNumValuePairsOfColumn().Select(validRowPair =>
            {
                var node = new Node();
                node.RowNum = validRowPair.rowNum;
                node.Level = validRowPair.level;
                node.Name = validRowPair.name;
                node.Type = validRowPair.type;
                node.Metadata = validRowPair.metadata;
                node.Range = validRowPair.range;
                node.Remark = validRowPair.range;
                return node;
            });
            allNodes.AddRange(validNodes);
            allNodes.FormStruct();
            Nodes = allNodes;
        }
        private InterfaceObj()
        {
            
        }

        public string BuildDto()
        {
            //todo:将dto和message的xsd区分开来
            var result = string.Empty;
            foreach (var node in this.Nodes)
            {
                if (node.Children.Any())
                {
                    if (!node.IsList)
                    {
                        //empty:ComplexElement
                        if (node.Type == string.Empty)
                        {
                            var complexElementStr = node.BuildComplexElement();
                            result += complexElementStr;
                        }
                        //type:ListElement+TypeDefine
                        else
                        {
                            var typeDefineStr = node.BuildComplexType();
                            result += typeDefineStr;
                            //var elementStr = node.BuildElement();
                            //requestContent += elementStr;
                        }
                    }
                    //type:Element+TypeDefine
                    else
                    {
                        //var listElementStr = node.BuildElement();
                        //requestContent += listElementStr;
                        var typeDefineStr = node.BuildComplexType(true);
                        result += typeDefineStr;
                    }
                }
                else
                {
                    if (node.Metadata.ToUpper() == "ENUM")
                    {
                        var typeDefineStr = node.BuildEnumType(true);
                        result += typeDefineStr;
                    }
                    //var simpleElementStr = node.BuildElement();
                    //result += simpleElementStr;
                }
            }
            return result;
        }
    }
}
