using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UploadWebApi.Infraestructura.Web;

namespace UploadWebApi.Tests
{
    [TestClass]
    public class QueryStringUnitTest
    {

        readonly string _query_with_question = "?param1=val1&param2=val2&param3=val3";
        readonly string _query_without_question = "param1=val1&param2=val2&param3=val3";
        readonly string _query_empty_without_question = "";
        readonly string _query_empty_with_question = "?";
        readonly string _query_with_question_under = "?param1_uno=val1&param2=val2&param3=val3";


        [TestMethod]
        public void Parse_Querystring_Question_Ok()
        {
            var ret = QueryStringHelper.ParseQuery(_query_with_question);

            Assert.AreEqual(3, ret.Count);
            Assert.AreEqual("param1", ret.Keys[0]);
        }

        [TestMethod]
        public void Parse_Querystring_NoQuestion_Ok()
        {
            var ret = QueryStringHelper.ParseQuery(_query_without_question);

            Assert.AreEqual(3, ret.Count);
            Assert.AreEqual("param1", ret.Keys[0]);

        }

        [TestMethod]
        public void Parse_Empty_Querystring_NoQuestion_Ok()
        {
            var ret = QueryStringHelper.ParseQuery(_query_empty_without_question);

            Assert.AreEqual(0, ret.Count);
        }

        [TestMethod]
        public void Parse_Empty_Querystring_Question_Ok()
        {
            var ret = QueryStringHelper.ParseQuery(_query_empty_with_question);

            Assert.AreEqual(0, ret.Count);
        }

        [TestMethod]
        public void Parse_Under_Querystring_Question_Ok()
        {
            var ret = QueryStringHelper.ParseQuery(_query_with_question_under);

            Assert.AreEqual(3, ret.Count);
            Assert.AreEqual("param1_uno", ret.Keys[0]);

        }




    }
}
