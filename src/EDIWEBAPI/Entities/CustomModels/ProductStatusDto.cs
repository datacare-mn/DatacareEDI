using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EDIWEBAPI.Entities.CustomModels
{
    public class ProductStatusDto
    {
        public string ID { get; set; }
        public string NAME { get; set; }
        public string COLOR { get; set; }
        public Dictionary<int, int> VALUEMAPPING { get; set; }
        public int VALUE1
        {
            get
            {
                return GetValue(1);
            }
        }
        public int VALUE2
        {
            get
            {
                return GetValue(2);
            }
        }
        public int VALUE3
        {
            get
            {
                return GetValue(3);
            }
        }
        public int VALUE4
        {
            get
            {
                return GetValue(4);
            }
        }
        public int VALUE5
        {
            get
            {
                return GetValue(5);
            }
        }
        public int VALUE6
        {
            get
            {
                return GetValue(6);
            }
        }

        private void InitMapping()
        {
            if (VALUEMAPPING == null)
                VALUEMAPPING = new Dictionary<int, int>();
        }

        public int GetValue(int index)
        {
            InitMapping();
            return VALUEMAPPING.ContainsKey(index) ? VALUEMAPPING[index] : 0;
        }

        public void SetValue(int index, int value)
        {
            InitMapping();
            if (VALUEMAPPING.ContainsKey(index))
                VALUEMAPPING[index] = value;
            else
                VALUEMAPPING.Add(index, value);
        }
    }
}
