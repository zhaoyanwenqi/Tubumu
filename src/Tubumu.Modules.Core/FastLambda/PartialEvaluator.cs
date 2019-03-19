﻿namespace Tubumu.Modules.Core.FastLambda
{
    /// <summary>
    /// PartialEvaluator
    /// </summary>
    public class PartialEvaluator : PartialEvaluatorBase
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public PartialEvaluator()
            : base(new Evaluator())
        {
        }
    }
}
