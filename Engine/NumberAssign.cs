using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public static class NumberAssign
    {
        private static int _nextNumber = 0;

        public static int GenNextNum()
        {
            _nextNumber = (_nextNumber + 1);
            return _nextNumber;
        }
    }
}
