using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ImageServer.Tests
{
    public class FileAccessorTests
    {
        [Fact]
        public void TestQueue()
        {
            int hit = 0;

            Task task1 = FileAccessor.RunAction("samefile", () => LongRunningTask(() => 
            {
                Assert.True(hit == 0);
                hit++;
            }, 100));
            

            Task task2 = FileAccessor.RunAction("samefile", () => LongRunningTask(() => 
            {
                Assert.True(hit == 1);
                hit++;
                
            }, 10));

            Task.WaitAll(task1, task2);

            Assert.True(hit == 2);
        }

        [Fact]
        public void TestParallel()
        {
            int hit = 0;

            Task task1 = FileAccessor.RunAction("onefile", () => LongRunningTask(() =>
            {
                Assert.True(hit == 1);
                hit++;
            }, 100));


            Task task2 = FileAccessor.RunAction("anotherfile", () => LongRunningTask(() =>
            {
                Assert.True(hit == 0);
                hit++;

            }, 10));

            Task.WaitAll(task1, task2);

            Assert.True(hit == 2);
        }

        [Fact]
        public async void TestException()
        {
            await Assert.ThrowsAsync<NotImplementedException>(async () => 
            {
                await FileAccessor.RunAction("file", () => LongRunningTask(() =>
                {
                    throw new NotImplementedException();
                }, 10));
            });
            
        }

        private async Task LongRunningTask(Action action, int milliseconds)
        {
            await Task.Delay(milliseconds);
            action();
        }
    }
}
