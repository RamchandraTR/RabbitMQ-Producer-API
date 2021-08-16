using Register.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Register.Database
{
    public interface IBatchDataService
    {
        List<BatchModel> Get();
        void Update(int id, string status);
    }
}
