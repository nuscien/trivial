using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Trivial.Reflection;
using Trivial.Tasks;

namespace Trivial.UnitTest.Tasks
{
    [TestClass]
    public class RetryTest
    {
        [TestMethod]
        public async Task TestLinearAsync()
        {
            var count = 0;
            var now = DateTime.Now;
            var retry = new LinearRetryPolicy(3, TimeSpan.FromMilliseconds(20), TimeSpan.FromMilliseconds(10));
            await retry.ProcessAsync(() =>
            {
                count++;
                throw new ApplicationException();
            }, typeof(ApplicationException));
            Assert.AreEqual(4, count);
            Assert.IsTrue(DateTime.Now - now >= TimeSpan.FromMilliseconds(90));

            var isSucc = false;
            try
            {
                await retry.ProcessAsync(() =>
                {
                    count++;
                    throw new InvalidOperationException();
                }, typeof(ApplicationException));
            }
            catch (InvalidOperationException)
            {
                isSucc = true;
            }

            Assert.IsTrue(isSucc);
            Assert.AreEqual(5, count);

            var h = new ExceptionHandler();
            h.Add<ApplicationException>(ex =>
            {
                return null;
            });
            h.Add<ArgumentException>(ex =>
            {
                if (ex.Message == "name") return null;
                return new ApplicationException();
            });

            await retry.ProcessAsync(() =>
            {
                count++;
                throw new ApplicationException();
            }, h.GetException);
            await retry.ProcessAsync(() =>
            {
                count++;
                throw new ArgumentException("name");
            }, h.GetException);
            Assert.AreEqual(13, count);

            isSucc = false;
            try
            {
                await retry.ProcessAsync(() =>
                {
                    count++;
                    throw new ArgumentException("other");
                }, h.GetException);
            }
            catch (ApplicationException)
            {
                isSucc = true;
            }

            Assert.AreEqual(14, count);
            Assert.IsTrue(isSucc);
        }
    }
}
