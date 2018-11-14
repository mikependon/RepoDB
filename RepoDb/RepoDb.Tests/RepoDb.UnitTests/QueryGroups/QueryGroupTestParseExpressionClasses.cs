using System;
using System.Text;

namespace RepoDb.UnitTests
{
    public partial class QueryGroupTest
    {
        public class QueryGroupTestExpressionClassMember
        {
            public string PropertyString { get; set; }
        }

        public class QueryGroupTestExpressionClass
        {
            public int PropertyInt { get; set; }
            public string PropertyString { get; set; }
            public double PropertyDouble { get; set; }
            public DateTime PropertyDateTime { get; set; }
            public float PropertySingle { get; set; }
            public Guid PropertyGuid { get; set; }
            public Boolean PropertyBoolean { get; set; }
            public Byte[] PropertyBytes { get; set; }
            public QueryGroupTestExpressionClassMember Member { get; set; }
        }

        private int GetIntValueForParseExpression()
        {
            return 1;
        }

        private string GetStringValueForParseExpression()
        {
            return "ABC";
        }

        private double GetDoubleValueForParseExpression()
        {
            return 1234567.12;
        }

        private DateTime GetDateTimeValueForParseExpression()
        {
            return new DateTime(2018, 01, 01, 2, 4, 11, 112);
        }

        private float GetSingleValueForParseExpression()
        {
            return 18891;
        }

        private Guid GetGuidValueForParseExpression()
        {
            return Guid.Parse("4C43B849-4FD1-4E7D-95CA-A0EE0D358DE7");
        }

        private bool GetBooleanValueForParseExpression()
        {
            return 1 != 0;
        }

        private Byte[] GetBytesValueForParseExpression()
        {
            return Encoding.UTF8.GetBytes(GetStringValueForParseExpression());
        }
    }
}
