using System.Collections.Generic;
using NUnit.Framework;

namespace Tests.EditMode
{
    public class RandomGeneratorTest
    {
        private List<Result> m_testResults;
        private int[] m_pickedAmounts;
        private RandomResultGenerator m_generator;
        
        [Test]
        public void RandomGeneratorTestSimplePasses()
        {
            for (int i = 0; i < 100; i++)
            {
                int pickedIndex = m_generator.PickRandomResult();
                m_pickedAmounts[pickedIndex]++;
            }
            
            for (int i = 0; i < m_pickedAmounts.Length; i++)
            {
                Assert.AreEqual(m_testResults[i]._chance, m_pickedAmounts[i]);
            }
            
        }
        
        [SetUp]
        public void Setup()
        {
            m_testResults = new List<Result>()
            {
                new Result { _chance = 13 },
                new Result { _chance = 13 },
                new Result { _chance = 13 },
                new Result { _chance = 13 },
                new Result { _chance = 13 },
                new Result { _chance = 9 },
                new Result { _chance = 8 },
                new Result { _chance = 7 },
                new Result { _chance = 6 },
                new Result { _chance = 5 },
            };
            
            m_pickedAmounts = new int[m_testResults.Count];
            m_generator = new RandomResultGenerator(m_testResults);
        }
    
    }
}
