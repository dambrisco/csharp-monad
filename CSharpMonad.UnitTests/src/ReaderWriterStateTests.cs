////////////////////////////////////////////////////////////////////////////////////////
// The MIT License (MIT)
// 
// Copyright (c) 2014 Paul Louth
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
// 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Monad;

namespace Monad.UnitTests
{
    [TestFixture]
    public class ReaderWriterStateTests
    {
        [Test]
        public void ReaderWriterStateTest1()
        {
            var world = RWS.Return<Env,string,App,int>(0);

            var rws = (from _   in world
                       from app in RWS.Get<Env,string,App>()
                       from env in RWS.Ask<Env,string,App>()
                       from x   in Value(app.UsersLoggedIn, "Users logged in: " + app.UsersLoggedIn)
                       from y   in Value(100, "System folder: " + env.SystemFolder)
                       from s   in RWS.Put<Env,string,App>(new App { UsersLoggedIn = 35 })
                       from t   in RWS.Tell<Env,string,App>("Process complete")
                       select x * y)
                      .Memo(new Env(), new App());

            var res = rws(); 

            Assert.IsTrue(res.Value == 3400); 
            Assert.IsTrue(res.State.UsersLoggedIn == 35); 
            Assert.IsTrue(res.Output.Count() == 3); 
            Assert.IsTrue(res.Output.First() == "Users logged in: 34"); 
            Assert.IsTrue(res.Output.Skip(1).First() == "System folder: C:/Temp"); 
            Assert.IsTrue(res.Output.Skip(2).First() == "Process complete"); 
        }

        public static RWS<Env,string,App,int> Value(int val, string log)
        {
            return (Env r, App s) => RWS.Tell<string,App,int>(val, log);
        }
    }


    public class App
    {
        public int UsersLoggedIn = 34;
    }

    public class Env
    {
        public string SystemFolder = "C:/Temp";
    }
}

