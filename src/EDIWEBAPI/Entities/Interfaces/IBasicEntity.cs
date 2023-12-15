using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EDIWEBAPI.Entities.Interfaces
{
    public interface IBasicEntity
    {
        int ID { get; set; }
        string NAME { get; set; }
        int ENABLED { get; set; }
        int VIEWORDER { get; set; }
    }
}
