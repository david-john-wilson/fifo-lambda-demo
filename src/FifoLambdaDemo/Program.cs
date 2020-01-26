using Amazon.CDK;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FifoLambdaDemo
{
    sealed class Program
    {
        public static void Main(string[] args)
        {
            var app = new App();
            new FifoLambdaDemoStack(app, "FifoLambdaDemoStack");
            app.Synth();
        }
    }
}
