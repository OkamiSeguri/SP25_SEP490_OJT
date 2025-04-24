using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject
{
    public class OJTCondition
    {
        [Key]
        public int ConditionId {  get; set; }
        public string ConditionKey { get; set; }
        public string ConditionValue { get; set; }
    }
}
