﻿using Shashlik.Kernel.Dependency.Conditions;

namespace Shashlik.Kernel.Test.TestClasses.DependencyCondition
{
    [ConditionDependsOn(typeof(NeedTestOption2Miss))]
    public class DependsOnNeedTestOption1True : Shashlik.Kernel.Dependency.ISingleton
    {
        // should be not null
    }
}