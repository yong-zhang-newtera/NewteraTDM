using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Http;

namespace Ebaas.WebApi.Infrastructure
{
    public abstract class CriteriaOperator
    {
    }

    public class GroupOperator : CriteriaOperator
    {
        public GroupOperator(GroupOperatorType opType)
        {
            this.OperatorType = opType;
            this.Operands = new List<CriteriaOperator>();
        }
        public GroupOperatorType OperatorType { get; set;}

        public List<CriteriaOperator> Operands { get; set; }
    }

    public class BinaryOperator : CriteriaOperator
    {
        public BinaryOperator(BinaryOperatorType opType)
        {
            this.OperatorType = opType;
        }
        public BinaryOperatorType OperatorType { get; set; }

        public CriteriaOperator LeftOperand { get; set; }

        public CriteriaOperator RightOperand { get; set; }
    }

    public class OperandProperty : CriteriaOperator
    {
        public OperandProperty(string name)
        {
            this.PropertyName = name;
        }
        public string PropertyName { get; set; }
    }

    public class OperandValue : CriteriaOperator
    {
        public OperandValue(string value)
        {
            this.Value = value;
        }
        public string Value { get; set; }
    }

    public enum GroupOperatorType
    {
        And,
        Or
    }

    public enum BinaryOperatorType
    {
        Equal,
        NotEqual,
        Greater,
        GreaterOrEqual,
        Less,
        LessOrEqual,
        Like,
        In
    }
}