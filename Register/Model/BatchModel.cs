using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Register.Model
{
    public class BatchModel
    {
        [JsonProperty("id")]
        public int BatchId { set; get; }
        [JsonProperty("name")]
        public string BatchName { set; get; }
        [JsonProperty("status")]
        public string Status { get; set; }
    }
}
