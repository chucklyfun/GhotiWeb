﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ninject.MockingKernel.Moq;

namespace Utilities
{
    public class TestBase
    {
        public MoqMockingKernel testKernel = new MoqMockingKernel();
    }
}
