﻿using Couchbase.Core.Serialization;
using Couchbase.Core.Transcoders;
using Couchbase.IO.Converters;
using Couchbase.IO.Operations;
using NUnit.Framework;

namespace Couchbase.Tests.IO.Operations
{
    [TestFixture]
    public class GetKOperationTests : OperationTestBase
    {
        [Test]
        public void When_Key_Exists_GetK_Returns_Value()
        {
            var key = "When_Key_Exists_GetK_Returns_Value";

            //delete the value if it exists
            var delete = new Delete(key, GetVBucket(), new DefaultTranscoder(), OperationLifespanTimeout);
            IOService.Execute(delete);

            //Add the key
            var add = new Add<dynamic>(key, new { foo = "foo" }, GetVBucket(), Transcoder, OperationLifespanTimeout);
            Assert.IsTrue(IOService.Execute(add).Success);

            var getK = new GetK<dynamic>(key, GetVBucket(),  Transcoder, OperationLifespanTimeout);

            var result = IOService.Execute(getK);
            Assert.IsTrue(result.Success);

            var expected = new {foo = "foo"};
            Assert.AreEqual(result.Value.foo.Value, expected.foo);
        }

        [Test]
        public void Test_OperationResult_Returns_Defaults()
        {
            var op = new GetK<string>("Key", GetVBucket(),  Transcoder, OperationLifespanTimeout);

            var result = op.GetResultWithValue();
            Assert.IsNull(result.Value);
            Assert.IsEmpty(result.Message);
        }

        [Test]
        public void When_Type_Is_String_DataFormat_String_Is_Used()
        {
            var key = "When_Type_Is_String_DataFormat_String_Is_Used";

            //delete the value if it exists
            var delete = new Delete(key, GetVBucket(),  Transcoder, OperationLifespanTimeout);
            IOService.Execute(delete);

            //Add the key
            var add = new Add<string>(key, "foo", GetVBucket(),  Transcoder, OperationLifespanTimeout);
            Assert.IsTrue(IOService.Execute(add).Success);

            var getK = new GetK<string>(key, GetVBucket(),  Transcoder, OperationLifespanTimeout);

            getK.CreateExtras();
            Assert.AreEqual(DataFormat.String, getK.Format);

            var result = IOService.Execute(getK);
            Assert.IsTrue(result.Success);

            Assert.AreEqual(DataFormat.String, getK.Format);
        }

        [Test]
        public void When_Type_Is_Object_DataFormat_Json_Is_Used()
        {
            var key = "When_Type_Is_Object_GetK_Uses_DataFormat_Json";

            //delete the value if it exists
            var delete = new Delete(key, GetVBucket(), Transcoder, OperationLifespanTimeout);
            IOService.Execute(delete);

            //Add the key
            var add = new Add<dynamic>(key, new { foo = "foo" }, GetVBucket(), Transcoder, OperationLifespanTimeout);
            Assert.IsTrue(IOService.Execute(add).Success);

            var getK = new GetK<dynamic>(key, GetVBucket(), Transcoder, OperationLifespanTimeout);

            getK.CreateExtras();
            Assert.AreEqual(DataFormat.Json, getK.Format);

            var result = IOService.Execute(getK);
            Assert.IsTrue(result.Success);

            Assert.AreEqual(DataFormat.Json, getK.Format);
        }

        [Test]
        public void When_Type_Is_ByteArray_DataFormat_Binary_Is_Used()
        {
            var key = "When_Type_Is_Object_GetK_Uses_DataFormat_Json";

            //delete the value if it exists
            var delete = new Delete(key, GetVBucket(), Transcoder, OperationLifespanTimeout);
            IOService.Execute(delete);

            //Add the key
            var add = new Add<byte[]>(key, new byte[] { 0x0 }, GetVBucket(), Transcoder, OperationLifespanTimeout);
            Assert.IsTrue(IOService.Execute(add).Success);

            var getK = new GetK<byte[]>(key, GetVBucket(), Transcoder, OperationLifespanTimeout);

            getK.CreateExtras();
            Assert.AreEqual(DataFormat.Binary, getK.Format);

            var result = IOService.Execute(getK);
            Assert.IsTrue(result.Success);

            Assert.AreEqual(DataFormat.Binary, getK.Format);
        }


        [Test]
        public void Test_Clone()
        {
            var operation = new GetK<string>("key", GetVBucket(), Transcoder, OperationLifespanTimeout)
            {
                Cas = 1123
            };
            var cloned = operation.Clone();
            Assert.AreEqual(operation.CreationTime, cloned.CreationTime);
            Assert.AreEqual(operation.Cas, cloned.Cas);
            Assert.AreEqual(operation.VBucket.Index, cloned.VBucket.Index);
            Assert.AreEqual(operation.Key, cloned.Key);
            Assert.AreEqual(operation.Opaque, cloned.Opaque);
        }
    }
}
